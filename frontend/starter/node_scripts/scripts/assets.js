const fs = require('fs-extra');
const path = require('path');
const { assets, dist } = require('../helpers/paths');
const { errorLog, finishLog, startLog } = require('../helpers/logger');
const globalVars = require('../helpers/globalVars');
const { setWebFolder } = require('../helpers/setWebFolder');

/*----------------------------------------------------------------------------------------------
	Assets Files
 ----------------------------------------------------------------------------------------------*/
const destination = path.join(dist, 'assets');

const getAllFiles = function(dirPath, arrayOfFiles) {
	const files = fs.readdirSync(dirPath);

	arrayOfFiles = arrayOfFiles || [];

	files.forEach((file) => {
		if (fs.statSync(dirPath + '/' + file).isDirectory()) {
			arrayOfFiles = getAllFiles(dirPath + '/' + file, arrayOfFiles);
		} else {
			arrayOfFiles.push(path.join(dirPath, file));
		}
	});

	return arrayOfFiles;
};

const copyAssets = () => {
	startLog('assets', 'assets');

	const allImages = getAllFiles(assets + '/images');

	allImages.forEach((y) => {
		const stats = fs.statSync(y);
		const fileSizeInBytes = stats.size;
		// Convert the file size to megabytes (optional)
		if (fileSizeInBytes > 1_000_000) {
			const image = path.parse(y);

			console.warn(`${image.base} is larger than 1MB, it's ${Math.round(((fileSizeInBytes / 1024 / 1024) + Number.EPSILON) * 100) / 100}MB`);
		}
	});

	return new Promise((res) => {
		fs.copy(assets, destination, async(err) => {
			if (err) {
				errorLog('assets', 'An error occured while copying the folder.');
			} else {
				finishLog('assets');
			}

			await setWebFolder();

			if (globalVars.webFolder) {
				const webFolder = path.join(globalVars.webFolder, 'assets');
				startLog('w-assets', 'webFolder assets');

				fs.copy(destination, webFolder, async(err) => {
					if (err) {
						errorLog('w-assets', 'An error occured while copying the folder.');
					} else {
						finishLog('w-assets');
					}
					res();
				});
			} else {
				res();
			}

		});
	});
};

module.exports = {
	copyAssets
};
