const path = require('path');
const fs = require('fs-extra');

// This was implemented since the original file had console.log that didn't look nice with our output 😢
const separator = path.sep;
const modulePath = `node_modules${separator}svgicons2svgfont${separator}src${separator}index.js`;
const originalModule = fs.readFileSync(modulePath, { encoding: 'utf-8' });
if (originalModule) {
	const keyWord = 'this.log(\'Font created\');';
	const modifiedModule = originalModule.replace(keyWord, '');
	fs.writeFileSync(modulePath, modifiedModule);
}

const { createSVG } = require('./helpers/createSVG');
const { createTTF } = require('./helpers/createTTF');
const { createWOFF } = require('./helpers/createWOFF');
const { createWOFF2 } = require('./helpers/createWOFF2');
const { copyTemplate } = require('./helpers/copyTemplate');
const { finishLog, startLog, errorLog } = require('../../helpers/logger');
const { dist, root } = require('../../helpers/paths');
const { makeFolderIfMissing } = require('../../helpers/makeFolderIfMissing');
const globalVars = require('../../helpers/globalVars');
const { optimizeSVGs } = require('./optimizeSvgs');
const { setWebFolder } = require('../../helpers/setWebFolder');

const SVG_PATH = path.resolve(root, 'src/assets/svg');
const FONT_OUTPUT_PATH = path.join(dist, 'assets/fonts');
const SCSS_OUTPUT_PATH = path.resolve(root, 'src/scss/layout');

const item = (name, code) => {
	return `	@if $filename == ${name} {
		$char: '\\${code}';
	}\n`;
};

const item2 = (prefix, name) => {
	return `.${prefix}-${name} {
	@include font(${name});
}\n`;
};

const options = {
	src: SVG_PATH, // svg path
	dist: FONT_OUTPUT_PATH, // output path
	cssDist: SCSS_OUTPUT_PATH,
	fontName: 'svgicons', // font name
	css: true, // Create CSS files.
	svgicons2svgfont: {
		fontHeight: 1000,
		normalize: true
	},
	classNamePrefix: 'font'
};

const cssString = [];
const cssString2 = [];

const buildIcons = async() => {
	makeFolderIfMissing(FONT_OUTPUT_PATH);
	const unicodeObject = await createSVG(options);

	Object.keys(unicodeObject).forEach((name) => {
		const _code = unicodeObject[name];

		cssString.push(item(name, _code.charCodeAt(0).toString(16).toUpperCase()));
		cssString2.push(item2(options.classNamePrefix, name));
	});

	const ttf = await createTTF(options); // SVG Font => TTF
	await createWOFF(options, ttf); // TTF => WOFF
	await createWOFF2(options, ttf); // TTF => WOFF2
	// delete .svg file after everything is generated
	await fs.promises.unlink(path.join(FONT_OUTPUT_PATH, `${options.fontName}.svg`));
	await fs.promises.unlink(path.join(FONT_OUTPUT_PATH, `${options.fontName}.ttf`));

	if (options.css) {
		return copyTemplate(options.cssDist, {
			fontname: options.fontName,
			cssString: cssString.join(''),
			cssString2: cssString2.join(''),
			timestamp: new Date().getTime(),
			output: '../assets/fonts'
		});
	}

	return;
};

const copyFontsToWebFolder = () => {
	return new Promise((res) => {
		if (!globalVars.webFolder) res();

		startLog('iconfont-copy', 'fonts-to-web');
		const fonts = path.join(dist, 'assets/fonts');
		const destination = path.join(globalVars.webFolder, 'assets/fonts');

		fs.copy(fonts, destination, async(err) => {
			if (err) {
				errorLog('iconfont-copy', 'An error occured while copying the folder.');
			} else {
				finishLog('iconfont-copy');
			}
			res();
		});
	});
};

const iconfont = async() => {
	startLog('iconfont', 'iconfont');
	await setWebFolder();
	await optimizeSVGs();
	await buildIcons();
	await copyFontsToWebFolder();
	finishLog('iconfont');
};

module.exports = {
	iconfont
};
