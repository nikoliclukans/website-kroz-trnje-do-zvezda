
const accordions = {
	init: function() {
		this.openAccordions();
	},

	openAccordions: function() {
		$('.js-acc-btn').click(function(e) {
			e.preventDefault();
			const $this = $(this);
			if ($this.next().hasClass('show')) {
				$this.next().removeClass('show');
				$this.next().slideUp(350);
			} else {
				$this.parent().parent().find('li .accordions-content').removeClass('show');
				$this.parent().parent().find('li .accordions-content').slideUp(350);
				$this.next().toggleClass('show');
				$this.next().slideToggle(350);
			}
		});
	}
};

export default accordions;
