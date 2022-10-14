const path = require('path');
const ttf2woff = require('ttf2woff');
const fs = require('fs-extra');
const { startLog, finishLog } = require('../../../helpers/logger');

function createWOFF(options = {}, ttf) {
	startLog('createWOFF', 'create woff');
	return new Promise((resolve, reject) => {
		const DIST_PATH = path.join(options.dist, options.fontName + '.woff');
		const woff = Buffer.from(ttf2woff(ttf).buffer);
		fs.writeFile(DIST_PATH, woff, (err) => {
			if (err) {
				return reject(err);
			}
			finishLog('createWOFF');
			resolve();
		});
	});
}

module.exports = { createWOFF };
