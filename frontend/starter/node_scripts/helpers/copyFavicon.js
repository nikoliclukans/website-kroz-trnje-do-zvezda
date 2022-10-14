const fs = require('fs');
const globalVars = require('./globalVars');
const path = require('path');
const { root, dist } = require('./paths');
const { startLog, finishLog } = require('./logger');

const copyFavicon = () => {
	startLog('favicon', 'favicon');
	return new Promise((res) => {
		fs.readdirSync(path.join(root, 'src/favicon')).forEach((file) => {
			fs.copyFileSync(path.join(root, `src/favicon/${file}`), path.join(dist, file));

			if (!globalVars.webFolder) return;
			fs.copyFileSync(`./src/favicon/${file}`, `${globalVars.webFolder}/${file}`);
		});

		finishLog('favicon');
		res();
	});
};

module.exports = {
	copyFavicon
};
