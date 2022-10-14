const polyfills = {
	isNaN: function() {
		if (Number.isNaN) return;

		Object.defineProperty(Number, 'isNaN', {
			value: function(value) {
				return value !== value;
			}
		});
	},
	entries: function() {
		if (Object.entries) return;

		Object.entries = function(obj) {
			const ownProps = Object.keys(obj);
			let i = ownProps.length;
			const resArray = new Array(i);

			while (i--) {
				resArray[i] = [ownProps[i], obj[ownProps[i]]];
			}

			return resArray;
		};
	},
	objectFit: function() {
		if ('objectFit' in document.documentElement.style === false) {
			document.addEventListener('DOMContentLoaded', () => {
				Array.prototype.forEach.call(document.querySelectorAll('img[data-object-fit]'), (image) => {
					(image.runtimeStyle || image.style).background = `url("${image.src}") no-repeat 50%/${image.currentStyle ? image.currentStyle['object-fit'] : image.getAttribute('data-object-fit')}`;
					image.src = `data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='${image.width}' height='${image.height}'%3E%3C/svg%3E`;
				});
			});
		}
	}
};

export default polyfills;
