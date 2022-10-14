const fs = require('fs-extra');
const { dist } = require('../helpers/paths');
const path = require('path');
const globalVars = require('../helpers/globalVars');
const { startLog, finishLog, errorLog } = require('../helpers/logger');

const resetDev = async() => {
	startLog('dist', 'Remove dist');

	if (!fs.existsSync(dist)) return;

	const itemsToDelete = await fs.readdir(dist).then((p) => p.filter((x) => !globalVars.foldersToIgnoreInDist.includes(x)).map((x) => path.join(dist, x)));

	const promises = itemsToDelete.map((f) => new Promise((res, rej) => fs.remove(f).then(res).catch(rej)));

	return Promise.all(promises).then(() => {
		finishLog('dist');
	}).catch(() => errorLog('dist', 'Something went wrong!'));
};

module.exports = { resetDev };
