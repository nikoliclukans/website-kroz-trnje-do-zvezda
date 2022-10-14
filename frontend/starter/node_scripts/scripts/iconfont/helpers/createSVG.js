const path = require('path');
const SVGIcons2SVGFont = require('svgicons2svgfont');
const fs = require('fs-extra');

const UnicodeObj = {};
let startUnicode = 0xea01;

function getIconUnicode(name) {
	const unicode = String.fromCharCode(startUnicode++);
	UnicodeObj[name] = unicode;
	return [unicode];
}

function filterSvgFiles(svgFolderPath) {
	const files = fs.readdirSync(svgFolderPath, 'utf-8');
	const svgArr = [];
	if (!files) {
		throw new Error(`Error! Svg folder is empty.${svgFolderPath}`);
	}

	for (const i in files) {
		if (typeof files[i] !== 'string' || path.extname(files[i]) !== '.svg') continue;
		if (svgArr.indexOf(files[i]) >= -1) {
			svgArr.push(path.join(svgFolderPath, files[i]));
		}
	}
	return svgArr;
}

function createSVG(options = {}) {
	return new Promise((resolve, reject) => {
		// init
		const fontStream = new SVGIcons2SVGFont({
			...options.svgicons2svgfont,
		});

		function writeFontStream(svgPath) {
			// file name
			const _name = path.basename(svgPath, '.svg');
			const glyph = fs.createReadStream(svgPath);
			glyph.metadata = { unicode: getIconUnicode(_name), name: _name };
			fontStream.write(glyph);
		}

		const DIST_PATH = path.join(options.dist, options.fontName + '.svg');
		// Setting the font destination
		fontStream.pipe(fs.createWriteStream(DIST_PATH))
			.on('finish', () => {
				resolve(UnicodeObj);
			})
			.on('error', (err) => {
				if (err) {
					reject(err);
				}
			});
		filterSvgFiles(options.src).forEach((svg) => {
			if (typeof svg !== 'string') return false;
			writeFontStream(svg);
		});

		// Do not forget to end the stream
		fontStream.end();
	});
}

module.exports = { createSVG };
