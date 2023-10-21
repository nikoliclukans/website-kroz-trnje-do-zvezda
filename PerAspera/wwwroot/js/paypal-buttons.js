document.addEventListener("DOMContentLoaded", function () {
	var personalInfoElems = document.querySelectorAll('.personal-info');
	var paypalInfoElems = document.querySelectorAll('.paypal-info');
	var radios = document.querySelectorAll('input[name="SelectedPaymentOption"]');

	updateDisplayBasedOnRadio();

	function updateDisplayBasedOnRadio() {
		var personalRadio = document.querySelector('input[name="SelectedPaymentOption"][value="CashOnDelivery"]');
		var orderedItemsDiv = document.getElementById('ordered-items');
		var shopCards = window.parent.document.querySelector('.shop-cards__wrap');
		var platiButton = document.getElementById('shop-form-submit-btn');


		if (personalRadio.checked) {
			personalInfoElems.forEach(function (e) {
				e.style.display = 'block';
			});
			paypalInfoElems.forEach(function (e) {
				e.style.display = 'none';
			});
			orderedItemsDiv.classList.remove('disabled-cart');
			if (shopCards) {
				shopCards.classList.remove('disabled-cart');
			}
			$(platiButton).show();
		} else {
			$(platiButton).hide();

			paypalInfoElems.forEach(function (e) {
				e.style.display = 'block';
			});
			orderedItemsDiv.classList.add('disabled-cart');

			if (shopCards) {
				shopCards.classList.add('disabled-cart');
			}

			document.getElementById('paypal-button-container').innerHTML = '';

			renderPayPalButton();
		}
	}


	radios.forEach(function (radio) {
		radio.addEventListener('change', function () {
			updateDisplayBasedOnRadio();
		});
	});
});