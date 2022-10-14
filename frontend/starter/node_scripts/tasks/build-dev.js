const globalVars = require('../helpers/globalVars');
const handleOldNodeVersion = require('../helpers/handleOldNodeVersion');

const { setWebFolder } = require('../helpers/setWebFolder');
const { hbs } = require('../scripts/handlebars');
const { webpack: js } = require('../scripts/javascript');
const { finishLog, startLog, errorLog } = require('../helpers/logger');
const { iconfont } = require('../scripts/iconfont');
const { runSCSS } = require('../scripts/scss');
const { copyAssets } = require('../scripts/assets');
const { copyFavicon } = require('../helpers/copyFavicon');
const { resetDev } = require('../scripts/resetDev');

// build all files for DEVELOPMENT
(async() => {
	if (!globalVars.isCorrectNodeVersion) return handleOldNodeVersion();

	const prepareDev = () => {
		startLog('prep-dev', 'preparation');
		return new Promise((res) => {
			globalVars.mode = 'development';
			finishLog('prep-dev');
			res();
		});
	};

	startLog('dev', 'build-dev');

	await resetDev();
	await prepareDev();
	await setWebFolder();
	await iconfont();
	await runSCSS();
	await js();
	await copyAssets();
	await hbs();
	await copyFavicon();

	if (globalVars.buildHasError) {
		errorLog('dev', '?');
	} else {
		finishLog('dev');
	}
})();
