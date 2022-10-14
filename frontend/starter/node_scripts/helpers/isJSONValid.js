/**
 *
 * @param {JSON} json
 */
const isJSONValid = (json) => {
	try {
		JSON.parse(json);
	} catch (e) {
		return false;
	}
	return true;
};

module.exports = { isJSONValid };
