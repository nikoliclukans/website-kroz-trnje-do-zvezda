const fs = require('fs');
const globalVars = require('./globalVars');
const path = require('path');
const { root } = require('./paths');

const getDirectories = (source) => fs.readdirSync(source).filter((name) => name.indexOf('.Web') > -1)[0];

function setWebFolder() {
	if (globalVars.webFolder) return true;
	if (globalVars.isNewVersion) return true;

	return new Promise((res) => {
		const result = getDirectories(path.join(root, '../'));

		if (result) globalVars.webFolder = `../${result}`;

		res();
	});
}

module.exports = { setWebFolder };
