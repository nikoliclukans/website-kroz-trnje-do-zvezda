function paymentPage() {
    var paymentContainer = "#paypal-tab";
    return {
        init: function () {
            PaymentService.init({
                renderSectionId: paymentContainer,
                priceCalculatorEndpoint: priceCalculatorUrl,
                bookingId: $("#bookingId").val(),
                completeBookingUrl: completeBookingUrl,
                carId: $("#carId").val()
            });

            $(document).on('click', '.js-popup-button', function () {
                $('body').addClass('scroll-disabled');
                var $this = $(this),
                    $popup = $("#" + $this.data("popup-id"));

                if ($popup.hasClass('open')) {
                    $popup.removeClass('open');
                    $popup.addClass('open');
                } else {
                    $('.card-content__popup').removeClass('open');
                    $popup.addClass('open');
                }
            });

            $("#cachePaymentButton").on("click", function () {
                var $carId = $("#carId");
                var bookingId = $("#bookingId").val();
                var $button = $(this);
                Loader.show();
                fetch(cachePaymentUrl, {
                    method: 'post',
                    headers: {
                        'content-type': 'application/json'
                    },
                    body: JSON.stringify({
                        RentACarBooking: $carId.length ? bookingId : null,
                        ServiceBookingId: $carId.length ? null : bookingId,
                        CarId: $carId.val(),
                        Culture: $("#culture").val()
                    })
                }).then(function (res) {
                    var $messageOverlay = $("#message-overlay");
                    $messageOverlay.removeClass("error-popup");
                    $messageOverlay.addClass("successe-popup");
                    $messageOverlay.addClass("open");
                    $messageOverlay.show();
                    Loader.hide();
                    $button.closest(".card-content__popup").hide()
                    return;
                })
            });
        }
    }
}
PaymentPage = new paymentPage();
$(document).ready(PaymentPage.init)