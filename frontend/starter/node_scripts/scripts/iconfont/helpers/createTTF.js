const path = require('path');
const svg2ttf = require('svg2ttf');
const fs = require('fs-extra');

function createTTF(options = {}) {
	return new Promise((resolve, reject) => {
		options.svg2ttf = options.svg2ttf || {};
		const DIST_PATH = path.join(options.dist, options.fontName + '.ttf');
		const ttf = svg2ttf(fs.readFileSync(path.join(options.dist, options.fontName + '.svg'), 'utf8'), options.svg2ttf);
		const ttfBuf = Buffer.from(ttf.buffer);
		fs.writeFile(DIST_PATH, ttfBuf, (err) => {
			if (err) {
				return reject(err);
			}
			resolve(ttfBuf);
		});
	});
}

module.exports = { createTTF };
