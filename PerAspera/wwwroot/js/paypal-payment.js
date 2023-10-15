function renderPayPalButton() {

    var totalAmountRSD = document.getElementById('paypal-button-container').getAttribute('data-total-amount');

    var exchangeRate = document.getElementById('paypal-button-container').getAttribute('exchange-rate');

    var currency = document.getElementById('paypal-button-container').getAttribute('currency');

    var totalAmountEUR = totalAmountRSD / exchangeRate;


    paypal.Buttons({
        createOrder: function (data, actions) {

            return actions.order.create({
                purchase_units: [
                    {
                        amount: {
                            currency_code: currency,
                            value: totalAmountEUR.toFixed(2)
                        }
                    }
                ]
            });
        },
        onApprove: function (data, actions) {

            return actions.order.capture().then(function (details) {
                $('.order-form__content').append($("#shopFormMessage"));
                $('.order-form__content').find("#shopFormMessage").addClass("show");
                $('#shopForm').remove();
                setTimeout(function () {
                    location.reload();
                }, 3000); 
            });
        }
    }).render('#paypal-button-container');
}