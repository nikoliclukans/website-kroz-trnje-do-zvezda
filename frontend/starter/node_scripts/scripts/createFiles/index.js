const fs = require('fs');
const globalVars = require('../../helpers/globalVars');
const path = require('path');
const { html, scss } = require('../../helpers/paths');
const os = require('os');
/*----------------------------------------------------------------------------------------------
	Create/Read/Update Files
 ----------------------------------------------------------------------------------------------*/

function getEOL(text) {
	const m = text.match(/\r\n|\n/g);
	const u = m && m.filter((a) => a === '\n').length;
	const w = m && m.length - u;
	if (u === w) {
		return os.EOL; // use the OS default
	}
	return u > w ? '\n' : '\r\n';
}

Array.prototype.insert = function(index, item) {
	this.splice(index, 0, item);
};

const appendCssImport = (data, name) => {
	const eol = getEOL(data);
	const dataArray = data.split(eol);
	const index = dataArray.indexOf('# Print Styles');
	dataArray.insert(index - 2, `@import 'modules/${name}';`);

	return dataArray.join('\n');
};

const createFiles = (arg, type) => {
	const directory = path.join(html, `/${type}s/${arg}`);

	function create(file, lang) {
		let temp = `${type}-hbs-temp.txt`;
		let filename;

		// detect which file to create
		if (file === 'scss') {
			if (type === 'partial') {
				temp = 'partial-scss-temp.txt';
			} else {
				temp = 'module-scss-temp.txt';
			}
			filename = `_${arg}.scss`;
		} else if (file === 'json') {
			temp = `${type}-json-temp.txt`;
			filename = lang ? lang + '.data.json' : 'data.json';
		} else {
			filename = `${arg}.hbs`;
		}

		const readDir = file === 'style' ? path.join(scss, 'style.scss') : path.join(__dirname, `cf-templates/${temp}`);
		let writeDir;

		if (file === 'style') {
			writeDir = path.join(scss, 'style.scss');
		} else if (file === 'scss') {
			writeDir = path.join(scss, `modules/${filename}`);
		} else {
			writeDir = `${directory}/${filename}`;
		}

		globalVars.rf(readDir, (data) => {
			const output = file === 'style' ? appendCssImport(data, arg) : data.replace(new RegExp(`@{${type}}`, 'g'), arg);
			fs.writeFileSync(writeDir, output);
		});
	}

	// create if template or module doesn't exists
	if (!fs.existsSync(directory)) {
		fs.mkdirSync(directory);

		create('hbs');
		if (globalVars.isMultilanguage) {
			globalVars.languages.map((l) => {
				create('json', l);
			});
			create('json');
		} else {
			create('json');
		}

		if (type === 'module' || type === 'partial') {
			create('scss');
			create('style');
		}

		globalVars.logMSG(`node_scripts/scripts/createFiles/cf-templates/${type}-log-temp.txt`, arg, 'green');
	} else {
		globalVars.logMSG(globalVars.warningTemp, `ERROR: ${type} '${arg}' already exists`);
	}
};

module.exports = {
	createFiles
};
