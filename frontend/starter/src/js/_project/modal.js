
const modalHandler = {
	modalOverlay: document.querySelector('.js-modal-overlay'),
	modalOpenBtn: document.querySelectorAll('.js-modal-open'),
	modalCloseBtn: document.querySelectorAll('.js-modal-close'),

	init: function() {
		if (this.modalCloseBtn) {
			this.closeModal();
		}

		if (this.modalOpenBtn) {
			this.openModal();
		}
	},

	openModal: function() {
		this.modalOpenBtn.forEach((modalOpenBtnElement) => {
			modalOpenBtnElement.addEventListener('click', () => {
				this.modalOverlay.classList.add('modal-overlay--opened');
			});
		});
	},

	closeModal: function() {
		this.modalCloseBtn.forEach((modalCloseBtnElement) => {
			modalCloseBtnElement.addEventListener('click', () => {
				this.modalOverlay.classList.remove('modal-overlay--opened');
			});
		});
	}
};

export default modalHandler;
