const chokidar = require('chokidar');
const path = require('path');
const { setWebFolder } = require('../helpers/setWebFolder');
const { webpack } = require('../scripts/javascript');
const { hbs, registerHbs } = require('../scripts/handlebars');
const { runSCSS } = require('../scripts/scss');
const { startLog } = require('../helpers/logger');
const globalVars = require('../helpers/globalVars');
const fs = require('fs');
const { html, root } = require('../helpers/paths');

const state = {
	js: {
		isActive: false,
		nextTask: null
	},
	hbs: {
		isActive: false,
		nextTask: null
	},
	scss: {
		isActive: false,
		nextTask: null
	}
};

let data = [];

const getAllHbsFilesFromDirectory = function(dir, filesList) {
	const files = fs.readdirSync(dir);
	filesList = filesList || [];
	files.forEach((file) => {
		if (fs.statSync(dir + '/' + file).isDirectory()) {
			filesList = getAllHbsFilesFromDirectory(dir + '/' + file, filesList);
		} else {
			const f = path.parse(file);

			if (f.ext === '.hbs') {
				filesList.push(path.join(dir, file));
			}
		}
	});
	return filesList;
};

const watchFunction = async(state, fn, data) => {
	if (!state.isActive) {
		state.isActive = true;
		await fn(data);
		state.isActive = false;
		if (state.nextTask) {
			await state.nextTask(data);
			state.nextTask = null;
		}
	} else {
		state.nextTask = fn;
	}
};

const getFolderObject = (fileName) => {
	const isModules = path.parse(fileName).dir.includes(`${path.sep}html${path.sep}modules${path.sep}`);
	const isPartials = path.parse(fileName).dir.includes(`${path.sep}html${path.sep}partials${path.sep}`);
	const isShared = path.parse(fileName).dir.includes(`${path.sep}html${path.sep}shared${path.sep}`);
	const isTemplates = path.parse(fileName).dir.includes(`${path.sep}html${path.sep}templates${path.sep}`);

	return { isModules, isPartials, isShared, isTemplates };
};

const getHbsName = (fileName) => {
	const { isModules, isPartials, isShared, isTemplates } = getFolderObject(fileName);

	let folderName = '';
	if (isModules) folderName = 'modules';
	if (isTemplates) folderName = 'templates';
	if (isShared) folderName = 'shared';
	if (isPartials) folderName = 'partials';

	const split = fileName.split(path.sep);
	const templateIndex = split.findIndex((x) => x === folderName);
	return split[templateIndex + 1];
};

const initialMapping = async() => {
	data = [];
	const allHbsFiles = getAllHbsFilesFromDirectory(html);

	allHbsFiles.forEach((file) => {
		const moduleName = getHbsName(file);
		let moduleType = '';
		const regex = /({{> .*?}})/g;
		const lookupRegex = /({{> \(.*\))/g;
		const { isModules, isPartials, isShared, isTemplates } = getFolderObject(file);

		if (isTemplates) {moduleType = 'template';}
		if (isPartials) {moduleType = 'partial';}
		if (isModules) {moduleType = 'module';}
		if (isShared) {moduleType = 'shared';}

		const fileContent = fs.readFileSync(file, { encoding: 'utf-8' });
		const initialData = {
			moduleType,
			moduleName,
			path: file,
			partials: (fileContent.match(regex) || []).map((x) => x.split(' ')[1])?.filter((x) => !x.startsWith('(') && !x.startsWith('@')),
			lookup: !!fileContent.match(lookupRegex)?.length
		};

		if (initialData.lookup) {
			const json = fs.readFileSync(initialData.path.replace(`${initialData.moduleName}.hbs`, 'data.json'), { encoding: 'utf-8' });
			const data = JSON.parse(json).data;
			if (!data) return;

			const newModules = [...new Set(Object.values(data)?.filter((d) => typeof d === 'object' && d.length)?.flat()?.map((x) => x.moduleAlias))];
			initialData.partials = [...new Set([...initialData.partials, ...newModules])];

			delete initialData.lookup;
		}

		initialData.path = path.parse(file).dir.replace(root + path.sep, '') + path.sep + 'data.json';

		data.push(initialData);
	});

	data.filter((d) => d.moduleType !== 'template').forEach((d) => {
		d.newDeps = [...new Set([...d.partials, ...d.partials.map((p) => data.find((x) => x.moduleName === p).partials).flat()])];

		while (d.partials.length !== d.newDeps.length) {
			d.partials = JSON.parse(JSON.stringify(d.newDeps));

			d.newDeps = [...new Set([...d.partials, ...d.partials.map((p) => data.find((x) => x.moduleName === p).partials).flat()])];
		}

		delete d.newDeps;
	});

	data.filter((d) => d.moduleType === 'template').forEach((d) => {
		d.newDeps = [...new Set([...d.partials, ...d.partials.map((p) => data.find((x) => x.moduleName === p).partials).flat()])];

		d.partials = d.newDeps;

		delete d.newDeps;
	});
};
registerHbs();
initialMapping();

globalVars.mode = 'development';

startLog('scss_watch', 'started scss watch');
chokidar.watch(path.join(root, 'src/scss')).on('change', async() => {
	await setWebFolder();
	await watchFunction(state.scss, runSCSS);
});

startLog('js_watch', 'started js watch');
chokidar.watch(path.join(root, 'src/js')).on('change', async() => {
	await setWebFolder();
	await watchFunction(state.js, webpack);
});

startLog('html_watch', 'started html watch');
chokidar.watch(path.join(root, 'src/html'), {
	awaitWriteFinish: {
		stabilityThreshold: 100,
	}
}).on('change', async(file) => {
	console.log('hbs changed');
	await initialMapping();

	const { isModules, isPartials, isShared, isTemplates } = getFolderObject(file);

	const name = getHbsName(file);

	if (name === 'master') {
		const files = data.filter((d) => d.moduleType === 'template').map((d) => d.path);
		watchFunction(state.hbs, hbs, files);
	} else if (isTemplates) {
		const files = [data.find((d) => d.moduleName === name).path];
		watchFunction(state.hbs, hbs, files);
	} else if (isModules || isPartials || isShared) {
		const files = data.filter((d) => d.moduleType === 'template' && d.partials.includes(name))?.map((x) => x.path);
		watchFunction(state.hbs, hbs, files);
	} else {
		console.log('No module or template are calling this module!');
	}
});
