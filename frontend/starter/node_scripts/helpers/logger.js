require('colors');
const ids = {};

/**
 *
 * @param {string} id unique id that will connect startLog with finishLog
 * @param {string} input label of the log that will be visible in the console
 */
const startLog = (id, input) => {
	const date = new Date();
	ids[id] = { time: date.getTime(), label: input };

	console.log(`[${date.toLocaleTimeString().grey}] Starting '${input.cyan}'...`);
};

/**
 *
 * @param {string} id use the already declared id in the startLog, to connect with this function
 */
const finishLog = (id) => {
	const date = new Date();
	const now = date.getTime();
	const duration = `${(now - ids[id].time) / 1000}s`;

	console.log(`[${date.toLocaleTimeString().grey}] Finished '${ids[id].label.cyan}'${duration ? ` after ${duration.magenta}` : '.'}`);
};

/**
 *
 * @param {string} id unique id that will connect startLog with finishLog
 * @param {string} errorMessage label of the error log that will be visible in the console
 */
const errorLog = (id, errorMessage) => {
	const date = new Date();
	const now = date.getTime();
	const duration = `${(now - ids[id].time) / 1000}s`;

	console.log(`[${date.toLocaleTimeString().grey}] '${ids[id].label.cyan}' ${'errored'.red}${duration ? ` ${'after'.red} ${duration.magenta}` : '.'}`);
	if (errorMessage) console.log(`ERROR!: ${errorMessage.red}`);
};

module.exports = {
	startLog,
	finishLog,
	errorLog
};
