const fs = require('fs');
const globalVars = require('../../helpers/globalVars');
const hbsHelpers = require('./hbsHelpers');
const beautify = require('js-beautify').html;
const handlebars = require('handlebars');
const path = require('path');
const { finishLog, startLog } = require('../../helpers/logger');
const { html, dist } = require('../../helpers/paths');
const { isJSONValid } = require('../../helpers/isJSONValid');

const invalidJSONSet = new Set();
let stop = false;
const message = {
	badJSON: () => {
		console.log('\x1b[41m%s\x1b[30m', 'This .json file is not valid:', '\x1b[0m');
		console.log('\x1b[7m', [...invalidJSONSet][0], '\x1b[0m');
	},
	badHBS: (item) => {
		console.log('\x1b[41m%s\x1b[30m', 'This .hbs file is not valid:', '\x1b[0m');
		console.log('\x1b[7m', item, '\x1b[0m');
	},
	missingJSON: (item) => {
		console.log('\x1b[41m%s\x1b[30m', 'This .json file is missing:', '\x1b[0m');
		console.log('\x1b[7m', item, '\x1b[0m');
	},
	missingHTML: (item) => {
		console.log('\x1b[41m%s\x1b[30m', 'This .html file is missing:', '\x1b[0m');
		console.log('\x1b[7m', item, '\x1b[0m');
	}
};

const hbsConfig = {
	'indent_size': 4,
	'preserve_newlines': false,
	'indent_char': '	'
};

/*----------------------------------------------------------------------------------------------
	HBS
 ----------------------------------------------------------------------------------------------*/

/**
 *
 * Delete all files with passed filter from dist
 *
 * @param {string} startPath
 * @param {string} filter
 * @param {Function} resolve
 */
const deleteFiles = (startPath, filter, resolve) => {
	if (!fs.existsSync(startPath)) {
		console.log('No dir ', startPath);
		return;
	}

	const files = fs.readdirSync(startPath);
	for (let i = 0; i < files.length; i++) {
		const filename = path.join(startPath, files[i]);
		const stat = fs.lstatSync(filename);
		if (stat.isDirectory()) {
			deleteFiles(filename, filter); //recurse
		} else if (filename.indexOf(filter) >= 0) {
			fs.unlinkSync(filename);
			resolve();
		}
	}
};

const cleanHTML = () => {
	return new Promise((res) => {
		const startPath = dist;
		const filter = '.html';

		deleteFiles(startPath, filter, res);
	});
};

/**
 *
 * Register all HBS helpers
 *
 * @returns Promise<void>
 */
const registerHelpers = () => {
	return new Promise((res) => {
		const keys = Object.keys(hbsHelpers);

		keys.forEach((helper) => handlebars.registerHelper(helper, hbsHelpers[helper]));

		res();
	});
};

/**
 *
 * @param {string} path
 */
const registerPartial = (path) => {
	if (!fs.existsSync(path)) {
		return;
	}

	fs.readdirSync(path).forEach((file) => {
		const pathToHbs = `${path + file}/${file}.hbs`;
		if (!fs.existsSync(pathToHbs)) return;

		const html = fs.readFileSync(pathToHbs, 'utf8');
		handlebars.registerPartial(file, html);
	});
};

// register all the partials from modules and shared folders
const registerPartials = () => {
	const modules = './src/html/modules/';
	const shared = './src/html/shared/';
	const partials = './src/html/partials/';

	return new Promise((res) => {
		registerPartial(modules);
		registerPartial(shared);
		registerPartial(partials);

		res();
	});
};

/**
 *
 * @param {string[]} inputFiles
 */
const singleLanguageHBS = (inputFiles = []) => {
	stop = false;
	const templates = './src/html/templates';
	const paths = inputFiles;
	const { defaultLanguage } = globalVars;

	return new Promise((res) => {
		if (!inputFiles.length) {
			findFilesInDirectory(templates, 'data.json', (filename) => {
				const filePath = path.parse(filename);
				if (filePath.base === 'data.json') {
					paths.push(filename);
				}
			});
		}

		// check if all the paths exist
		paths.forEach((p) => {
			if (stop) return;

			const jsonPathObject = path.parse(p);
			const filePath = jsonPathObject.dir;
			const htmlPath = `${filePath }/${filePath.split(path.sep).at(-1)}.hbs`;

			if (!fs.existsSync(htmlPath)) {
				console.log(`${htmlPath} does not have a .hbs file`);
				stop = true;
				return;
			}

			const html = fs.readFileSync(htmlPath, 'utf8');
			const json = fs.readFileSync(p, 'utf8');

			if (!isJSONValid(json)) {
				invalidJSONSet.add(p);
				message.badJSON();

				stop = true;
				return;
			}

			const data = extendObject(JSON.parse(json));

			if ([...invalidJSONSet].length) {
				message.badJSON();

				stop = true;
				return;
			}

			if (stop) return;

			const h = handlebars.compile(html);

			// set language in head
			data.data.language = defaultLanguage;

			if (globalVars.mode === 'production') {
				data.cssVersion = `?v=${new Date().getTime()}`;
				data.globalScriptVersion = `?v=${new Date().getTime()}`;
			}

			if (globalVars.mode !== 'production') {
				data.data.seo.hideFromSearchEngines = true;
			}

			// set filename
			const fileName = `${data.template}.html`;

			try {
				const rawHTML = h(data);
				const parsedHTML = beautify(rawHTML, hbsConfig);

				writeFile(path.join(dist, fileName), parsedHTML);
			} catch (e) {
				const { dir, base } = path.parse(htmlPath);
				message.badHBS(`File: ${dir}${path.sep}${base} \n Message:${e.message}`);
				stop = true;
				return;
			}
		});

		res();

	});
};

const multiLanguageHBS = () => {
	stop = false;
	const templates = path.join(html, 'templates');
	const paths = [];
	const { languages } = globalVars;

	return new Promise((res) => {
		languages.forEach((language) => {
			findFilesInDirectory(templates, '.hbs', (filename) => {
				const hbsPath = path.parse(path.resolve(filename));
				const dir = hbsPath.dir;
				const pathToJson = `${dir}/${language}.data.json`;

				if (fs.existsSync(pathToJson)) {
					paths.push({
						path: pathToJson,
						language: language
					});
				} else {
					message.missingJSON(pathToJson);
				}
			});
		});

		paths.forEach((p) => {
			if (stop) return;

			const json = fs.readFileSync(p.path, 'utf8');
			const data = extendObject(JSON.parse(json));

			const jsonPathObject = path.parse(p.path);
			const filePath = jsonPathObject.dir;
			const htmlPath = `${filePath}/${data.template}.hbs`;

			if (!fs.existsSync(htmlPath)) {
				console.log(`${htmlPath} does not have a .hbs file`);
				stop = true;
				return;
			}

			const html = fs.readFileSync(htmlPath, 'utf8');

			if (!isJSONValid(json)) {
				invalidJSONSet.add(p.path);
				message.badJSON();

				stop = true;
				return;
			}

			if ([...invalidJSONSet].length) {
				message.badJSON();

				stop = true;
				return;
			}

			if (stop) return;

			const h = handlebars.compile(html);

			// set language in head
			data.data.language = p.language;

			if (globalVars.mode === 'production') {
				data.cssVersion = `?v=${new Date().getTime()}`;
				data.globalScriptVersion = `?v=${new Date().getTime()}`;
			}

			if (globalVars.mode !== 'production') {
				data.data.seo.hideFromSearchEngines = true;
			}

			// set filename
			const hbsPath = path.parse(path.resolve(p.path));
			const dir = hbsPath.dir;

			let folders = dir.split('templates')[1].split(path.sep);
			folders.pop();
			folders = folders.join(path.sep) + path.sep;

			const language = p.language === globalVars.defaultLanguage ? '' : p.language;

			const fileName = `${language}${folders}${data.template}.html`;

			try {
				const rawHTML = h(data);
				const parsedHTML = beautify(rawHTML, hbsConfig);

				writeFile(path.join(dist, fileName), parsedHTML);
			} catch (e) {
				const { dir, base } = path.parse(htmlPath);
				message.badHBS(`File: ${dir}${path.sep}${base} \n Message:${e.message}`);
				stop = true;
				return;
			}
		});

		res();
	});
};

const extendObject = (object) => {
	const htmlRoot = './src/html/';

	Object.keys(object).forEach((prop) => {
		if (stop) return object;

		if (typeof object[prop] === 'object') {
		// extend marker
			const extendKey = '>>';

			if (Object.keys(object[prop]).includes(extendKey)) {
				const url = htmlRoot + object[prop][extendKey];

				// catch if the file is not present
				if (!fs.existsSync(url)) {
					message.missingJSON(object[prop][extendKey]);
					stop = true;
					return;
				} else {
					const extendedObjectData = fs.readFileSync(url, 'utf-8');

					// check if read json is valid
					const jsonIsInvalid = !isJSONValid(extendedObjectData);
					if (jsonIsInvalid) {
						invalidJSONSet.add(url);
						stop = true;
						return true;
					}

					const parsedExtendedObject = JSON.parse(extendedObjectData);

					delete object[prop][extendKey];

					object[prop] = {
						...parsedExtendedObject,
						...object[prop],
					};
				}
			}

			if (!stop) {
				extendObject(object[prop]);
			}
		}

		if (typeof object[prop] === 'string') {
			// check if inner prop is a string that contains ${url}
			// for multilanguage links
			if (object[prop].indexOf('${url}') > -1) {
				object[prop] = object[prop].replace('${url}', globalVars.path);
			} else if (object[prop].endsWith('.content.html')) {
				const htmlPath = htmlRoot + object[prop];
				if (fs.existsSync(htmlPath)) {
					const html = fs.readFileSync(htmlPath, 'utf8');
					object[prop] = html;
				} else {
					message.missingHTML(htmlPath);
					object[prop] = '';
				}
			}
		}

	});

	return object;
};

/**
 *
 * @param {string} startPath Path to read the data from
 * @param {string} filter Filter to match before callback is called
 * @param {function} callback Callback to be call on all the filtered results
 * @returns void
 */
const findFilesInDirectory = (startPath, filter, callback) => {
	if (!fs.existsSync(startPath)) {
		console.log('No dir ', startPath);
		return;
	}

	const files = fs.readdirSync(startPath);
	for (let i = 0; i < files.length; i++) {
		const filename = path.join(startPath, files[i]);
		const stat = fs.lstatSync(filename);
		if (stat.isDirectory()) {
			findFilesInDirectory(filename, filter, callback); //recurse
		} else if (filename.indexOf(filter) >= 0) callback(filename);
	}
};

/**
 *
 * @param {string} path1
 * @param {string} contents
 * @param {Function} cb
 */
const writeFile = (path1, contents, cb = () => {}) => {
	fs.mkdir(path.dirname(path1), { recursive: true }, (err) => {
		if (err) return cb(err);

		fs.writeFile(path1, contents, cb);
	});
};

const registerHbs = () => {
	startLog('hbs-r', 'register handlebars partials', 1);

	return new Promise((res) => {
		registerHelpers();
		registerPartials();

		finishLog('hbs-r', 1);
		res();
	});
};

const compileHbs = (inputFiles) => {
	startLog('hbs-c', 'compile handlebars templates', 1);
	return new Promise((res) => {

		invalidJSONSet.clear();
		if (globalVars.isMultilanguage) {
			multiLanguageHBS();
		} else {
			singleLanguageHBS(inputFiles);
		}

		finishLog('hbs-c', 1);
		res();
	});
};

/**
 *
 * @param {string[]} inputFiles
 */
const hbs = (inputFiles) => {
	startLog('hbs', 'hbs');
	return new Promise((res) => {
		registerHbs();
		compileHbs(inputFiles);

		finishLog('hbs');
		res();
	});
};

const addLanguage = () => {
	if (!globalVars.isMultilanguage) {
		console.log('Multilanguage is off');
		return;
	}

	findFilesInDirectory('./src/html/', 'data.json', (file) => {
		globalVars.languages.forEach((lng) => {
			const filePath = path.parse(file);
			if (!fs.existsSync(`${filePath.dir}/${lng}.data.json`)) {
				fs.copyFile(`${filePath.dir}/data.json`, `${filePath.dir}/${lng}.data.json`, (err) => {
					if (err) throw err;
				});
			}
		});
	});
};

module.exports = {
	hbs, addLanguage, cleanHTML, compileHbs, registerHbs
};
