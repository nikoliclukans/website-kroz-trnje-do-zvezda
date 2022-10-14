const finder = require('find-package-json');
const path = require('path');
const globalVars = require('./globalVars');

const packageJsonPath = finder(__dirname).next().filename;
const root = path.parse(packageJsonPath).dir;

module.exports = {
	root,
	assets: path.join(root, 'src/assets'),
	dist: path.join(root, globalVars.isNewVersion ? '../wwwroot' : 'dist'),
	scss: path.join(root, 'src/scss'),
	js: path.join(root, 'src/js'),
	html: path.join(root, 'src/html')
};
