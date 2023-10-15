$(document).ready(function () {

    $(document).on('click', "[id^='remove-item-']", function () {
        var $this = $(this);

        var removedItemId = $this.attr("id");
        var removedItemName = removedItemId.substring(removedItemId.indexOf("remove-item-") + "remove-item-".length);

        $("#ordereditem-" + removedItemName).remove();
        updateTotalPrice();
        if ($('#ordered-items div[id^="ordereditem-"]').length === 0) {
            $("#order-message").css("display", "inline-block");
            $("#payment-type").css("display", "none")
            $("#total-price-label").css("display", "none");
            $("#total-price-value").css("display", "none");
        }
    });

    $(document).on('blur', "[id^='ordered-item-quantity-']", function () {
        var $this = $(this);

        if ($this.val().trim() === '' || $this.val().trim() === "0") {
            $this.val('1');
        };
        var updateItemId = $this.attr("id");
        var updatedItemName = updateItemId.substring(updateItemId.indexOf("ordered-item-quantity-") + "ordered-item-quantity-".length);
        var priceLabel = $("#ordered-item-price-" + updatedItemName);
        priceLabel.text("");
        var price;
        $(".shop-card__content").find('h3').filter(function () {
            if ($.trim($(this).text()).replace(/\s+/g, '') === updatedItemName) {
                price = $(this).closest('.shop-card__content').find('.shop-card__price').text().replace(/\D/g, '')
            }
        });
        var currentPrice = parseInt($this.val()) * parseFloat(price);
        priceLabel.text(currentPrice.toString() + " RSD");

        updateTotalPrice();
    });

    $(document).on('keypress', "[id^='ordered-item-quantity-']", function (event) {
        var keyCode = event.which;

        if (keyCode < 48 || keyCode > 57) {
            event.preventDefault();
        }
    });

    $(document).on('input', "[id^='ordered-item-quantity-']", function () {
        var $this = $(this);

        if ($this.val() === '') {
            return;
        };

        var updateItemId = $this.attr("id");
        var updatedItemName = updateItemId.substring(updateItemId.indexOf("ordered-item-quantity-") + "ordered-item-quantity-".length);

        var currentQuantity = $this[0].value;
        var priceLabel = $("#ordered-item-price-" + updatedItemName);
        priceLabel.text("");
        var price;
        $(".shop-card__content").find('h3').filter(function () {
            if ($.trim($(this).text()).replace(/\s+/g, '') === updatedItemName) {
                price = $(this).closest('.shop-card__content').find('.shop-card__price').text().replace(/\D/g, '')
            }
        });
        var currentPrice = parseInt(currentQuantity) * parseFloat(price);
        priceLabel.text(currentPrice.toString() + " RSD");

        updateTotalPrice();
    });

    $('.js-order').click(function () {
        $("#order-message").css("display", "none");
        $("#payment-type").css("display", "inline-block")

        var $this = $(this);
        var price = $this.closest('.shop-card__content').find('.shop-card__price').text().replace(/\D/g, '');

        var nameValue = $this.attr('data-shop-item-name');
        var trimmedName = $.trim(nameValue).replace(/\s+/g, '');

        if ($("#ordered-items").find("#ordered-item-name-" + trimmedName).length > 0) {
            var currentQuantity = parseInt($("#ordered-item-quantity-" + trimmedName).val()) + 1;
            $("#ordered-item-quantity-" + trimmedName).val(currentQuantity);

            var currentPrice = parseFloat($("#ordered-item-price-" + trimmedName).text().replace(/\D/g, '')) + parseFloat(price);
            $("#ordered-item-price-" + trimmedName).text(currentPrice + " RSD");
        } else {

            var labelName = $("<label>").attr("id", "ordered-item-name-" + trimmedName).text(nameValue);
            labelName.attr("class", "ordered-items-format");
            labelName.css({
                'font-weight': 'bold'
            });

            var inputQuantity = $("<input>").attr("id", "ordered-item-quantity-" + trimmedName).val(1);
            inputQuantity.attr("class", "ordered-items-format")
            inputQuantity.attr("class", "ordered-items-format-input")
            var labelQuantity = $("<label>").text("kom.");
            labelQuantity.attr("class", "ordered-items-format");

            var labelPrice = $("<label>").attr("id", "ordered-item-price-" + trimmedName).text(price + " RSD");
            labelPrice.attr("class", "ordered-items-format");

            var buttonClose = $("<button>").attr("id", "remove-item-" + trimmedName).text("Ukloni iz korpe");
            buttonClose.attr("type", "button");
            buttonClose.attr("class", "ordered-items-format");
            buttonClose.css({
                'color': 'blue',
                'text-decoration': 'underline'
            });

            var divOrderedItem = $("<div>").addClass("form-row");
            divOrderedItem.attr("id", "ordereditem-" + trimmedName);
            divOrderedItem.append(labelName, inputQuantity, labelQuantity, labelPrice, buttonClose);

            $("#ordered-items").append(divOrderedItem);
        }

        $("#total-price-label").css("display", "inline-block");
        $("#total-price-value").css("display", "inline-block");
        updateTotalPrice();
    });

    $("#paypal, #cashondelivery").on("change", function () {
        if ($(this).is(":checked")) {
            $("#payment-type-error").css("display", "none");
        }
    })


    $("#shop-form-submit-btn").click(function () {
        if ($('#ordered-items div[id^="ordereditem-"]').length === 0) {
            $("#order-message").css("display", "inline-block");
            return;
        }
        if (!$("#paypal").is(":checked") && !$("#cashondelivery").is(":checked")) {
            $("#payment-type-error").css("display", "inline-block")
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

            if ($('#paypal').is(':checked')) {

            }
        });
    });

    function updateTotalPrice() {
        var totalPrice = 0;

        $("#ordered-items").find('div[id^="ordereditem-"]').each(function () {
            var $div = $(this);
            var name = $div.attr('id').substring($div.attr('id').indexOf("ordereditem-") + "ordereditem-".length);

            var price;
            $(".shop-card__content").find('h3').filter(function () {
                if ($.trim($(this).text()).replace(/\s+/g, '') === name) {
                    price = $(this).closest('.shop-card__content').find('.shop-card__price').text().replace(/\D/g, '')
                }
            });

            var quantity = $div.find('input[id^="ordered-item-quantity-"]').val();
            totalPrice += parseFloat(price) * parseInt(quantity);
        });


        $("#total-price-value").text(totalPrice.toString() + " RSD");
        $('#paypal-button-container').attr('data-total-amount', totalPrice);
    };

    var statusParam = getUrlParameter('status');

    function getUrlParameter(name) {
        name = name.replace(/[[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        var results = regex.exec(location.search);
        return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    }


    if (statusParam === "COMPLETED") {
        $('.order-form__content').append($("#shopFormMessage"));
        $('.order-form__content').find("#shopFormMessage").addClass("show");
        $('#shopForm').remove();


        var targetDiv = $('#shopFormMessage');
        if (targetDiv.length) {
            $('html, body').animate({
                scrollTop: targetDiv.offset().top
            }, 1000);
        }

        
    } else if (statusParam === "FAILED") {
        alert("Vaše plaćanje nije uspešno izvršeno.")
    }
});