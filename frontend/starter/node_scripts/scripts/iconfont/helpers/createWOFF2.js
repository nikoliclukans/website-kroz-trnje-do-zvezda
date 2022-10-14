const path = require('path');
const ttf2woff2 = require('ttf2woff2');
const fs = require('fs-extra');
const { finishLog, startLog } = require('../../../helpers/logger');

function createWOFF2(options = {}, ttf) {
	startLog('createWOFF2', 'create woff2');
	return new Promise((resolve, reject) => {
		const DIST_PATH = path.join(options.dist, options.fontName + '.woff2');
		const woff2 = Buffer.from(ttf2woff2(ttf).buffer);
		fs.writeFile(DIST_PATH, woff2, (err) => {
			if (err) {
				return reject(err);
			}
			finishLog('createWOFF2');
			resolve();
		});
	});
}

module.exports = { createWOFF2 };
