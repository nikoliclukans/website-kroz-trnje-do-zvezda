const postcss = require('postcss');
const sass = require('sass');
const stylelint = require('stylelint');
const flexBugsFix = require('postcss-flexbugs-fixes');
const autoprefixer = require('autoprefixer');
const globalVars = require('../../helpers/globalVars');
const fs = require('fs-extra');
const path = require('path');
const { dist, scss, root } = require('../../helpers/paths');
const { makeFolderIfMissing } = require('../../helpers/makeFolderIfMissing');
const { startLog, finishLog, errorLog } = require('../../helpers/logger');

// /*----------------------------------------------------------------------------------------------
// SCSS
//  ----------------------------------------------------------------------------------------------*/

const colors = {
	red: (input) => {
		return `\x1b[31m${input}\x1b[0m`;
	},
	orange: (input) => {
		return `\x1b[33m${input}\x1b[0m`;
	}
};

const compileSCSS = () => {
	startLog('css-c', 'css-compile');

	const filePath = path.join(scss, '/layout/_icon-font.scss');
	const outputFolder = path.join(dist, 'css');
	const output = path.join(outputFolder, 'style.min.css');

	return new Promise((res) => {
		makeFolderIfMissing(outputFolder);
		fs.closeSync(fs.openSync(filePath, 'a'));

		try {
			const result = sass.compile(path.join(scss, 'style.scss'), {
				sourceMap: true,
				style: globalVars.mode !== 'development' ? 'compressed' : 'expanded',
			});

			const sourceMapString = JSON.stringify(result.sourceMap);
			const sourceMapBase64 = (Buffer.from(sourceMapString, 'utf8') || '').toString('base64');
			const sourceMapComment = '/*# sourceMappingURL=data:application/json;charset=utf-8;base64,' + sourceMapBase64 + ' */';
			const css = result.css.toString() + '\n'.repeat(2) + sourceMapComment;

			postcss([autoprefixer, flexBugsFix]).process(css, { from: undefined }).then((result) => {
				fs.writeFile(output, result.css, (err) => {
					if (err) {
						console.log(err);
					}
					if (!globalVars.webFolder) {
						res();
						finishLog('css-c');
					}
				});
				if (globalVars.webFolder) {
					const webFolder = path.join(root, globalVars.webFolder, 'css');
					makeFolderIfMissing(webFolder);
					fs.writeFile(path.join(webFolder, 'style.min.css'), result.css, (err) => {
						if (err) {
							console.log(err);
						}
						finishLog('css-c');
						res();
					});
				}
			});
		} catch (err) {
			errorLog('css-c', 'scss complile failed');
			const filePath = path.resolve(err.file);
			const trimmedPath = filePath.replace(path.resolve(scss), 'scss');

			console.log(`\nmessage: ${colors.red(err.message)} \nfile: ${trimmedPath} \nline: ${err.line} \ncolumn: ${err.column}\n`);
			res();
		}
	});

};

const compileRTE = () => {
	startLog('css-rte', 'css-rte-compile');

	const outputFolder = path.join(dist, 'css');
	const output = path.join(outputFolder, 'rte-styles.css');

	if (!fs.existsSync(path.join(scss, 'rte-styles.scss'))) {
		finishLog('css-rte');
		return;
	}

	return new Promise((res) => {
		makeFolderIfMissing(outputFolder);

		try {
			const result = sass.compile(path.join(scss, 'rte-styles.scss'), {
				style: globalVars.mode !== 'development' ? 'compressed' : 'expanded',
			});

			const css = result.css.toString() + '\n'.repeat(2);

			postcss().process(css, { from: undefined }).then((result) => {
				fs.writeFile(output, result.css, (err) => {
					if (err) {
						console.log(err);
					}
					if (!globalVars.webFolder) {
						res();
						finishLog('css-rte');
					}
				});
				if (globalVars.webFolder) {
					const webFolder = path.join(root, globalVars.webFolder, 'css');
					makeFolderIfMissing(webFolder);
					fs.writeFile(path.join(webFolder, 'rte-styles.css'), result.css, (err) => {
						if (err) {
							console.log(err);
						}
						finishLog('css-rte');
						res();
					});
				}
			});
		} catch (err) {
			errorLog('css-rte', 'rte complile failed');
			const filePath = path.resolve(err.file);
			const trimmedPath = filePath.replace(path.resolve(scss), 'scss');

			console.log(`\nmessage: ${colors.red(err.message)} \nfile: ${trimmedPath} \nline: ${err.line} \ncolumn: ${err.column}\n`);
			res();
		}
	});

};

const lintSCSS = () => {
	startLog('css-l', 'css-lint');
	return new Promise((res) => {

		stylelint.lint({
			configFile: '.stylelintrc.json',
			files: 'src/scss/**/*.scss',
			fix: true
		})
			.then((data) => {
				const output = stylelint.formatters.string(data.results);

				if (output) console.log(output);
				finishLog('css-l');
				res(!!output);
			})
			.catch((err) => {
			// do things with err e.g.
				console.error(err.stack);
				finishLog('css-l');

				res(err.stack);
			});
	});
};

const runSCSS = async() => {
	startLog('css', 'css');
	const hasLintErrorsOrWarnings = await lintSCSS();

	if (hasLintErrorsOrWarnings) {
		errorLog('css', 'css not compiled, fix lint warnings/errors');
	} else {
		await compileSCSS();
		await compileRTE();
		finishLog('css');
	}
};

module.exports = {
	compileSCSS, lintSCSS, runSCSS
};
