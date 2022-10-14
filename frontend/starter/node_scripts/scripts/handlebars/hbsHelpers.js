const globalVars = require('../../helpers/globalVars');

module.exports = {
	ifEquals: function(arg1, arg2, options) {
		return (arg1 === arg2) ? options.fn(this) : options.inverse(this);
	},

	compare: function(v1, operator, v2) {
		switch (operator) {
			case '==': return v1 === v2;
			case '===': return v1 === v2;
			case '!=': return v1 !== v2;
			case '!==': return v1 !== v2;
			case '<': return v1 < v2;
			case '<=': return v1 <= v2;
			case '>': return v1 > v2;
			case '>=': return v1 >= v2;
			case '&&': return v1 && v2;
			case '||': return v1 || v2;
			default: throw new Error('helper {{compare}}: invalid operator ' + operator);
		}
	},

	isProduction: function() {
		return globalVars.mode === 'production';
	},

	getPath: function() {
		return globalVars.isMultilanguage ? globalVars.path : '';
	},

	isMultilanguage: function() {
		return globalVars.isMultilanguage;
	},

	increment: function(index) {
		return (parseInt(index) || 0) + 1;
	}
};
