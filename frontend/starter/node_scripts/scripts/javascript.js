const webpack = require('webpack');
const fs = require('fs-extra');
const webpackConfigDev = require('../../webpack.dev');
const webpackConfigProd = require('../../webpack.prod');
const globalVars = require('../helpers/globalVars');
const { finishLog, startLog, errorLog } = require('../helpers/logger');
const path = require('path');
const { dist } = require('../helpers/paths');

const runWebpack = () => {
	startLog('js', 'js');
	return new Promise((res) => {

		const isNotDev = globalVars.mode !== 'development';
		const webpackConfig = isNotDev ? webpackConfigProd : webpackConfigDev;

		/* TEMPORARY */
		webpackConfig.output.path = path.join(dist, 'js');

		const compiler = webpack(webpackConfig);

		const config = {
			colors: true,
			performance: false,
			timings: false,
			excludeAssets: true,
			assets: false,
			entrypoints: false,
			modules: false,
			hash: false,
			version: false,
			builtAt: false,
		};

		compiler.run((_, stats) => {
			if (stats.hasErrors()) {
				console.log(stats.toString(config));
				globalVars.buildHasError = true;
				errorLog('js');
			} else if (stats.hasWarnings()) {
				console.log(stats.toString(config));
				finishLog('js');
			} else {
				finishLog('js');
			}

			if (globalVars.webFolder) {
				fs.copy(path.join(dist, 'js'), path.join(globalVars.webFolder, 'js'), (err) => {
					if (err) return errorLog('err');
				});
			}

			res();
		});
	});
};

module.exports = {
	webpack: runWebpack
};
