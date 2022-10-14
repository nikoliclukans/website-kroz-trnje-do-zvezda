const globalVars = require('../helpers/globalVars');
const { setWebFolder } = require('../helpers/setWebFolder');
const { hbs } = require('../scripts/handlebars');
const { webpack: js } = require('../scripts/javascript');
const { finishLog, startLog, errorLog } = require('../helpers/logger');
const { iconfont } = require('../scripts/iconfont');
const { runSCSS } = require('../scripts/scss');
const { copyAssets } = require('../scripts/assets');
const { copyFavicon } = require('../helpers/copyFavicon');
const { resetDev } = require('../scripts/resetDev');
const handleOldNodeVersion = require('../helpers/handleOldNodeVersion');

// build all files for PRODUCTION
(async() => {
	if (!globalVars.isCorrectNodeVersion) return handleOldNodeVersion();

	const prepareProd = () => {
		return new Promise((res) => {

			globalVars.cookieCivic = false;
			globalVars.mode = 'production';
			res();
		});
	};

	startLog('build-prod', 'prod');

	await resetDev();
	await prepareProd();
	await setWebFolder();
	await iconfont();
	await runSCSS();
	await js();
	await copyAssets();
	await hbs();
	await copyFavicon();

	if (globalVars.buildHasError) {
		errorLog('build-prod', '?');
	} else {
		finishLog('build-prod');
	}
})();
