const fs = require('fs');
const packageJson = require('../../package.json');
const userConfig = require('../../config.json');

const [nodeMinimumMainVersion, nodeMinimumSubVersion] = packageJson.engines.node.split('.').map((x) => parseInt(x.replace(/\D/g, '')));

const nodeVersion = process.versions.node.split('.');
const nodeMainVersion = parseInt(nodeVersion[0]);
const nodeSubVersion = parseInt(nodeVersion[1]);

module.exports = {
	...userConfig,
	warningTemp: 'node_scripts/scripts/createFiles/cf-templates/warning-log-temp.txt',
	webFolder: null,
	isCorrectNodeVersion: nodeMainVersion > nodeMinimumMainVersion || (nodeMainVersion === nodeMinimumMainVersion && nodeSubVersion >= nodeMinimumSubVersion),
	buildHasError: false,
	rf(src, callback) {
		fs.readFile(src, 'utf8', (err, data) => {
			if (!err) {
				callback(data);
			} else {
				console.log('ERROR: ', err);
			}
		});
	},
	logMSG(template, str, color = 'yellow') {
		module.exports.rf(template, (data) => {
			data = data.replace(new RegExp('@{str}', 'g'), str);
			color = color === 'yellow' ? '\x1b[33m' : '\x1b[32m';

			console.log(color);
			console.log(data);
			console.log('\x1b[37m');
		});
	},
};
