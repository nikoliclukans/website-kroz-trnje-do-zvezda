const commonConfig = require('./webpack.common');

module.exports = {
	...commonConfig,
	mode: 'development',
	module: {
		rules: [
			{
				test: require.resolve('jquery'),
				loader: 'expose-loader',
				options: {
					exposes: ['$', 'jQuery'],
				},
			},
			...commonConfig.module.rules,
		]
	}
};
