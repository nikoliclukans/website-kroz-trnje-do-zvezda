const globalVars = require('../helpers/globalVars');
const { setWebFolder } = require('../helpers/setWebFolder');
const { hbs } = require('../scripts/handlebars');
const { webpack: js } = require('../scripts/javascript');
const { finishLog, startLog, errorLog } = require('../helpers/logger');
const { iconfont } = require('../scripts/iconfont');
const { runSCSS } = require('../scripts/scss');
const { copyAssets } = require('../scripts/assets');
const fs = require('fs');
const path = require('path');
const { root, dist } = require('../helpers/paths');
const { copyFavicon } = require('../helpers/copyFavicon');
const { resetDev } = require('../scripts/resetDev');
const handleOldNodeVersion = require('../helpers/handleOldNodeVersion');

// build all files for STAGING
(async() => {
	if (!globalVars.isCorrectNodeVersion) return handleOldNodeVersion();

	const prepareStage = () => {
		return new Promise((res) => {
			globalVars.mode = 'stage';
			fs.copyFileSync(path.join(root, 'robots.txt'), path.join(dist, 'robots.txt'));

			res();
		});
	};

	startLog('build-stage', 'stage');

	await resetDev();
	await prepareStage();
	await setWebFolder();
	await iconfont();
	await runSCSS();
	await js();
	await copyAssets();
	await hbs();
	await copyFavicon();

	if (globalVars.buildHasError) {
		errorLog('build-stage', '?');
	} else {
		finishLog('build-stage');
	}
})();
