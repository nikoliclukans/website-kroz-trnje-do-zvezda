
const modalHandler = {
	init: function() {
		this.openAccordions();
	},

	openAccordions: function() {
		$('.js-modal-open').click(function(e) {
			e.preventDefault();
			const $this = $(this);
			if ($this.next().hasClass('show')) {
				$this.next().removeClass('show');
			} else {
				$this.parent().parent().find('.js-modal-overlay').removeClass('show');
				$this.next().toggleClass('show');
			}
		});
	}
};

export default modalHandler;
