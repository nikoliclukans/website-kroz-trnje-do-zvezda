
const modalHandler = {
	init: function() {
		this.openAccordions();
		this.scroll();
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
		$('.shop-card').on('click', '.js-shop-modal-open', function() {
			$('.js-shop-modal-overlay').removeClass('open');
			if ($(this).next().is(':hidden')) {
				$(this).next().addClass('open');
			} else {
				$(this).removeClass('open');
			}
		});
		$('.js-modal-close').on('click', () => {
			$('.js-shop-modal-overlay').removeClass('open');
			$('.js-modal-overlay').removeClass('modal-overlay--opened');
		});
	},
	scroll: function() {

		$('.js-order').on('click', () => {
			$('html,body').animate({
				scrollTop: $('.order-form').offset().top },
			'slow');
			$('.js-shop-modal-overlay').removeClass('open');
		});
	}
};

export default modalHandler;
