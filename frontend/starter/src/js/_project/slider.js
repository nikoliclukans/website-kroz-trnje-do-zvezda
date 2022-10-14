import Swiper from 'swiper';
import { Navigation, Pagination, Scrollbar } from 'swiper';

const slider = {
	selector: '.js-slider',

	init: function() {
		this.swiper();
	},

	swiper: function() {
		// eslint-disable-next-line no-unused-vars
		const swiper = new Swiper('.js-swiper', {
			loop: false,
			slidesPerView: 'auto',
			spaceBetween: 30,
			scrollbar: {
				el: '.swiper-scrollbar',
				hide: false,
				draggable: true
			},

			modules: [Navigation, Pagination, Scrollbar]
		});
	}
};

export default slider;
