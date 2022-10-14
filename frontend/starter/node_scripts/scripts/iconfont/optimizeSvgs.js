const fs = require('fs');
const path = require('path');
const { optimize } = require('svgo');
const globalVars = require('../../helpers/globalVars');
const { startLog, errorLog, finishLog } = require('../../helpers/logger');

/*----------------------------------------------------------------------------------------------
	SVGs / IconFont
----------------------------------------------------------------------------------------------*/
// SVG optimization
const svgPath = 'src/assets/svg';
const plugins = [
	'cleanupAttrs',
	'removeDoctype',
	'removeXMLProcInst',
	'removeComments',
	'removeMetadata',
	'removeTitle',
	'removeDesc',
	'removeUselessDefs',
	'removeEditorsNSData',
	'removeEmptyAttrs',
	'removeHiddenElems',
	'removeEmptyText',
	'removeEmptyContainers',
	{
		name: 'removeViewBox',
		active: false,
	},
	'cleanupEnableBackground',
	'convertStyleToAttrs',
	'convertColors',
	{
		name: 'convertPathData',
		params: {
			noSpaceAfterFlags: false
		}
	},
	'convertTransform',
	'removeUnknownsAndDefaults',
	'removeNonInheritableGroupAttrs',
	'removeUselessStrokeAndFill',
	'removeUnusedNS',
	'cleanupIDs',
	'cleanupNumericValues',
	'moveElemsAttrsToGroup',
	'moveGroupAttrsToElems',
	'collapseGroups',
	'removeRasterImages',
	'mergePaths',
	'convertShapeToPath',
	'sortAttrs',
	'removeDimensions',
	{
		name: 'removeAttrs',
		params: {
			attrs: ['(stroke|fill)'],
		},
	},
	{
		name: 'addAttributesToSVGElement',
		params: {
			attributes: [{ 'fill': 'currentColor' }]
		}
	},
];
const optimizeSVGsArray = [];

const renameSvgFiles = (file, filePath, allFiles) => {
	return new Promise((resolve) => {
		globalVars.rf(filePath, () => {
			let newFile;

			if (file.substring(0, 4) !== 'ico-') {
				newFile = file.toLowerCase();
				newFile = newFile.toLowerCase();
				newFile = newFile.replace('.svg', '').replace(/-/g, ' ').replace(/[^\w\s]/gi, '').replace(/ /g, '-');
				newFile = `ico-${newFile}.svg`;

				if (allFiles.includes(newFile)) {
					// delete file if already exists
					try {
						fs.unlinkSync(filePath);
						globalVars.logMSG(globalVars.warningTemp, `deleted '${filePath}' as file with same name already exists`);
					} catch (err) {
						console.log(err);
					}
				} else {
					// rename file
					fs.renameSync(filePath, path.join(svgPath, newFile));
				}
			} else {
				newFile = file;
			}

			resolve(newFile);
		});
	});
};

const optimizeSvgFiles = (filePath) => {
	return new Promise((resolve) => {
		globalVars.rf(filePath, (data) => {
			resolve(optimize(data, { path: filePath, plugins }));
		});
	});
};

const saveSvgFiles = (filepath, data) => {
	return new Promise((resolve) => {
		fs.writeFile(filepath, data, (err) => {
			if (err) throw err;
			resolve(filepath);
		});
	});
};

const optimizeSVGs = () => {
	startLog('optimize-svg', 'optimize-svgs');
	return new Promise((res) => {

		fs.readdirSync(svgPath).forEach((file, i, allFiles) => {
			const filePath = path.join(svgPath, file);

			// remove non svg files from folder
			if (path.extname(file) !== '.svg') {
				try {
					fs.unlinkSync(filePath);
					globalVars.logMSG(globalVars.warningTemp, `deleted '${filePath}' as it is not an SVG file`);
				} catch (err) {
					console.log(err);
				}
			} else {
			// svgo optimization
				const optimizeSVG = optimizeSvgFiles(filePath)
					.then((result) => {
					// save optimized file
						return saveSvgFiles(filePath, result.data);
					})
					.then(() => {
					// clean file name and prefix it
						return renameSvgFiles(file, filePath, allFiles);
					});

				// store all optimization Promise functions
				optimizeSVGsArray.push(optimizeSVG);
			}
		});

		// build font icons when all optimization functions are done
		Promise.all(optimizeSVGsArray).then(() => {
			finishLog('optimize-svg');
			res();
		}).catch((error) => {
			errorLog('optimize-svg', error);
		});
	});
};

module.exports = {
	optimizeSVGs
};
