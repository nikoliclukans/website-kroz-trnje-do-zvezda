const packageJson = require('../../package.json');
const [nodeMinimumMainVersion, nodeMinimumSubVersion] = packageJson.engines.node.split('.').map((x) => parseInt(x.replace(/\D/g, '')));

const handleOldNodeVersion = () => {
	let mostChactersInLine = 0;
	const textArray = [
		'Bad node version!',
		`You have: ${process.version}`,
		`You need at least: v${nodeMinimumMainVersion}.${nodeMinimumSubVersion}.0`,
		`use NVM to switch to v${nodeMinimumMainVersion}`
	];

	textArray.forEach((textLine) => { if (textLine.length > mostChactersInLine) mostChactersInLine = textLine.length; });

	console.log('\x1b[41m%s\x1b[30m', ' '.repeat(mostChactersInLine + 5), '\x1b[0m');
	console.log('\x1b[41m%s\x1b[30m', ` #${'#'.repeat(mostChactersInLine + 2)}#`, '\x1b[0m');
	console.log('\x1b[41m%s\x1b[30m', ` #${' '.repeat(mostChactersInLine + 2)}#`, '\x1b[0m');
	textArray.forEach((t) => {
		console.log('\x1b[41m%s\x1b[30m', ` # ${t}${' '.repeat(mostChactersInLine - t.length)} #`, '\x1b[0m');
	});
	console.log('\x1b[41m%s\x1b[30m', ` #${' '.repeat(mostChactersInLine + 2)}#`, '\x1b[0m');
	console.log('\x1b[41m%s\x1b[30m', ` #${'#'.repeat(mostChactersInLine + 2)}#`, '\x1b[0m');
	console.log('\x1b[41m%s\x1b[30m', ' '.repeat(mostChactersInLine + 5), '\x1b[0m');
};

module.exports = handleOldNodeVersion;
