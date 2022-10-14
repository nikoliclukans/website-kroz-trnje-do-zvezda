const cookieModal = {
	cookieBtn: document.querySelector('.js-cookie-opener'),
	confirmText: 'Confirm Settings',

	init: function() {
		this.cookieModal();
	},

	cookieModal: function() {
		if (!this.cookieBtn) return;
		this.cookieBtn.addEventListener('click', (e) => {
			e.preventDefault();
			CookieControl.open();
			this.cookieAppendInfoButton();
		});
		document.addEventListener('click', (e) => {
			if (e.currentTarget.id === '#confirm-btn') {
				document.getElementById('ccc').innerHTML = '';
			}
		});
	},

	cookieAppendInfoButton: function() {
		const $cookiesContainer = $('#ccc');
		const cookieInfoBtn = `<button class="btn-info" id="confirm-btn">${this.confirmText}</button>`;
		setTimeout(() => {
			const confirmBtnHolder = $cookiesContainer.find('#ccc-info');
			if (!$('#confirm-btn').length) {
				confirmBtnHolder.append(cookieInfoBtn);
			}
		}, 200);
	}
};

export default cookieModal;
