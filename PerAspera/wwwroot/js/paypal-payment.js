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
                if ($('#ordered-items div[id^="ordereditem-"]').length === 0) {
                    $("#order-message").css("display", "inline-block");
                    return;
                }

                $('#ordered-items-inputs').empty();

                $("#ordered-items").find('div[id^="ordereditem-"]').each(function (index) {
                    var $div = $(this);
                    var name = $div.attr('id').substring($div.attr('id').indexOf("ordereditem-") + "ordereditem-".length);

                    var inputName = $('<input>').attr({
                        type: 'hidden',
                        id: 'orderedItemName-' + name,
                        name: 'OrderItems[' + index + '].Name',
                        value: $("#ordered-item-name-" + name).text()
                    });

                    var inputQuantity = $('<input>').attr({
                        type: 'hidden',
                        id: 'orderedItemQuantity-' + name,
                        name: 'OrderItems[' + index + '].Quantity',
                        value: $("#ordered-item-quantity-" + name).val()
                    });

                    var inputPrice = $('<input>').attr({
                        type: 'hidden',
                        id: 'orderedItemPrice-' + name,
                        name: 'OrderItems[' + index + '].Price',
                        value: parseFloat($("#ordered-item-price-" + name).text().replace(/\D + /g, ''))
                    });

                    var inputTotalPrice = $('<input>').attr({
                        type: 'hidden',
                        id: 'orderedItemsTotalPrice',
                        name: "TotalPrice",
                        value: parseFloat($("#total-price-value").text().replace(/\D + /g, ''))
                    })

                    $('#ordered-items-inputs').append(inputName, inputQuantity, inputPrice, inputTotalPrice);

                });

                var platiButton = document.getElementById('shop-form-submit-btn');

                $(platiButton).click();

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