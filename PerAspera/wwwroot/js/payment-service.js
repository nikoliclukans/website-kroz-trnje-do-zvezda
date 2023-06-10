function paymentService() {
    var configuration = null;
    return {
        init(config) {
            configuration = config;

            paypal.Buttons({
                style: {
                    color: 'blue'
                },
                createOrder: function (data, actions) {
                    var url = configuration.priceCalculatorEndpoint + "?slotId=" + configuration.bookingId + "&carId=" + configuration.carId;
                    return fetch(url, {
                        method: 'get',
                        headers: {
                            'content-type': 'application/json'
                        }
                    }).then(function (res) {
                        //check is successfull
                        return res.json();
                    }).then(function (data) {
                        //compare prices if not matching raise an issue.
                        return actions.order.create({
                            purchase_units: [{
                                amount: {
                                    value: data.Price
                                }
                            }]
                        });
                    });
                },

                // Finalize the transaction after payer approval
                onApprove: function (data, actions) {
                    return actions.order.capture().then(function (orderData) {
                        var transaction = orderData.purchase_units[0].payments.captures[0];
                        var payer = orderData.payer;
                        var url = configuration.completeBookingUrl;
                        return fetch(url, {
                            method: 'post',
                            headers: {
                                'content-type': 'application/json'
                            },
                            body: JSON.stringify({
                                BookingId: configuration.bookingId,
                                TransactionId: transaction.id,
                                Status: transaction.status,
                                Name: payer.name.given_name,
                                LastName: payer.name.surname,
                                EmailAddress: payer.email_address,
                                Culture: $("#culture").val(),
                                CarId: $("#carId").val(),
                                Amount: transaction.amount.value
                            })
                        }).then(function (res) {
                            var $messageOverlay = $("#message-overlay");
                            $messageOverlay.removeClass("error-popup");
                            $messageOverlay.addClass("successe-popup");
                            $messageOverlay.addClass("open");
                            $messageOverlay.show();
                            Loader.hide();
                            return;
                        })
                    });
                },
                onCancel: function () {
                    Loader.hide();
                },
                onError: function (err) {
                    console.log(error);
                    var $messageOverlay = $("#message-overlay");
                    $messageOverlay.addClass("error-popup");
                    $messageOverlay.addClass("open");
                    $messageOverlay.removeClass("successe-popup");
                    $messageOverlay.show();
                    Loader.hide();
                }
            }).render(config.renderSectionId);
        }
    }
}

PaymentService = new paymentService();