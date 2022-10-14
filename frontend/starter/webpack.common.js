const path = require('path');
const webpack = require('webpack');
const ESLintPlugin = require('eslint-webpack-plugin');

const eslintOptions = {
	fix: true,
	files: './src/js/**/*.js'
};

module.exports = {
	entry: {
		global: './src/js/global.js'
	},
	output: {
		path: path.join(__dirname, './dist/js/'),
		filename: '[name].min.js'
	},
	resolve: {
		alias: {
			core: path.join(__dirname, 'core'),
		},
	},
	module: {
		rules: [
			{
				test: path.join(__dirname),
				loader: 'babel-loader',
			},
		],
	},
	optimization: {
		usedExports: true
	},
	plugins: [
		new webpack.ProvidePlugin({
			$: 'jquery',
			jQuery: 'jquery',
			'window.jQuery': 'jquery'
		}),
		new ESLintPlugin(eslintOptions),
	]
};
