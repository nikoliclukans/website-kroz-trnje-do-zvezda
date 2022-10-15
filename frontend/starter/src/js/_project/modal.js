
const modalHandler = {
	init: function() {
		this.openAccordions();
	},

	openAccordions: function() {
		$('.js-modal-open').on('click', function() {
			const $this = $(this);
			const thisIndex = $this.index();
			const slide = $('.js-modal-overlay');
			$('.js-modal-open').removeClass('active');
			$this.addClass('active');
			slide.eq(thisIndex).addClass('modal-overlay--opened');
		});
		$('.js-modal-close').on('click', () => {

			$('.js-modal-overlay').removeClass('modal-overlay--opened');
		});
	}
};

export default modalHandler;
