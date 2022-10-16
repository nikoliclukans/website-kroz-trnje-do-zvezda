
const search = {
	searchToggleBtn: document.querySelector('.js-search-open'),
	search: document.querySelector('.header-search'),
	$body: $('body'),
	$win: $(window),
	topScroll: 0,
	disabledScrollClass: 'disabled',
	disableScroll: function() {
		if (!this.isScrollDisabled) {
			this.topScroll = this.$win.scrollTop();
			this.$body.css('top', -this.topScroll + 'px').addClass(this.disabledScrollClass);
			this.isScrollDisabled = true;
		}
	},
	enableScroll: function() {
		this.$body.removeAttr('style').removeClass(this.disabledScrollClass);
		this.$win.scrollTop(this.topScroll);
		this.isScrollDisabled = false;
	},
	init: function() {
		this.openSearch();
		this.openNav();
	},

	openSearch: function() {
		this.searchToggleBtn.addEventListener('click', () => {
			this.search.classList.toggle('header-search--open');
		});
	},

	openNav: function() {
		const _this = this;
		$('.js-menu').click(function() {
			const $this = $(this);
			$this.toggleClass('open');
			$('.nav').toggleClass('open');
			if ($('.nav').hasClass('open')) {
				_this.disableScroll();
			} else {
				_this.enableScroll();
			}
		  });
	}
};

export default search;
