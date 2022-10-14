
const search = {
	searchToggleBtn: document.querySelector('.js-search-open'),
	search: document.querySelector('.header-search'),
	init: function() {
		this.openSearch();
	},

	openSearch: function() {
		this.searchToggleBtn.addEventListener('click', () => {
			this.search.classList.toggle('header-search--open');
		});
	}
};

export default search;
