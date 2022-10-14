const handlebars = require('handlebars');
const path = require('path');
const fs = require('fs');
const { startLog, finishLog, errorLog } = require('../../../helpers/logger');

const copyTemplate = (cssFileOuputPath, templateOptions) => {
	startLog('save-scss', 'create scss');
	return new Promise((resolve, reject) => {

		const hbsPath = path.resolve(__dirname, '../template/iconfont.hbs');
		const hbs = fs.readFileSync(hbsPath, 'utf8');

		const handlebarsCompiler = handlebars.compile(hbs);
		const parsedHtml = handlebarsCompiler(templateOptions);

		try {
			fs.writeFileSync(cssFileOuputPath + '/_icon-font.scss', parsedHtml);
			finishLog('save-scss');
			resolve();
		} catch (err) {
			errorLog('save-scss');
			reject('failed');
		}

	});
};

module.exports = { copyTemplate };
