// Loading screen fadeout
$(window).on('load', function () {
    $('#loading-screen').fadeOut(1000, function () {
        $(this).remove();
    });
});

// Form validation, adding items to the invoice
$(document).ready(function () {
    // Initialize item list
    let items = [];

    $('.add-price-btn').on('click', function () {
        //event.preventDefault();
        const item = $('#ProductSelection').val();
        let quantity = parseFloat($('#quantity').val());
        let salePrice = parseFloat($('#SalePrice').val());
        let purchasePrice = parseFloat($('#PurchasePrice').val());
        const store = $('#InventorySelection').val();

        // Validation for required fields
        //if (item === '' || isNaN(quantity) || isNaN(salePrice) || isNaN(purchasePrice) || store === '') {
        //    Swal.fire({
        //        text: 'يرجى ملء جميع الحقول المطلوبة.',
        //        icon: 'warning',
        //        showConfirmButton: false,
        //        timer: 2000,
        //        toast: true,
        //        position: 'top-end',
        //        background: '#ffffff',
        //        iconColor: '#d9534f',
        //        customClass: {
        //            popup: 'small-toast',
        //            title: 'small-toast-title',
        //            content: 'small-toast-content'
        //        }
        //    });
        //    return;
        //}

        // Validation for positive values of quantity and prices
        if (quantity <= 0 || salePrice <= 0 || purchasePrice <= 0) {
            Swal.fire({
                text: 'القيم المدخلة للكمية أو الأسعار يجب أن تكون أكبر من الصفر.',
                icon: 'error',
                showConfirmButton: false,
                timer: 3000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#d9534f',
                customClass: {
                    popup: 'small-toast',
                    title: 'small-toast-title',
                    content: 'small-toast-content'
                }
            });
            return;
        }

        // Add item to the items list
        const totalSalePrice = quantity * salePrice;
        const totalPurchasePrice = quantity * purchasePrice;

        items.push({
            item: item,
            quantity: quantity,
            salePrice: salePrice,
            purchasePrice: purchasePrice,
            store: store,
            totalSalePrice: totalSalePrice,
            totalPurchasePrice: totalPurchasePrice
        });

        // Add item to invoice table
        $('#invoice-body').append(`
            <tr>
                <td>${item}</td>
                <td>${quantity}</td>
                <td>${salePrice.toFixed(2)}</td>
                <td>${purchasePrice.toFixed(2)}</td>
                <td>${totalSalePrice.toFixed(2)}</td>
                <td>${totalPurchasePrice.toFixed(2)}</td>
                <td>${store}</td>
                <td><i class='bx bxs-edit edit-btn'></i></td>
                <td><i class='bx bxs-trash delete-btn'></i></td>
            </tr>
        `);

        updateTotal();
        resetForm();
    });

    // Delete item from invoice table
    $('#invoice-body').on('click', '.delete-btn', function () {
        const index = $(this).closest('tr').index();
        items.splice(index, 1); // Remove the item from the list
        $(this).closest('tr').remove();
        updateTotal();
    });

    // Edit item in invoice table
    $('#invoice-body').on('click', '.edit-btn', function () {
        const $row = $(this).closest('tr');
        const index = $row.index();

        const item = $row.find('td').eq(0).text();
        const quantity = $row.find('td').eq(1).text();
        const salePrice = $row.find('td').eq(2).text();
        const purchasePrice = $row.find('td').eq(3).text();
        const store = $row.find('td').eq(6).text();

        $('#item').val(item);
        $('#quantity').val(quantity);
        $('#SalePrice').val(salePrice);
        $('#PurchasePrice').val(purchasePrice);
        $('#store').val(store);

        items.splice(index, 1); // Remove the item from the list for editing
        $row.remove();
        updateTotal();
    });

    // Update total price with discount and payment details
    function updateTotal() {
        let total = 0;
        let totalPurchase = 0;
        $('#invoice-body tr').each(function () {
            const rowTotalSale = parseFloat($(this).find('td').eq(4).text());
            const rowTotalPurchase = parseFloat($(this).find('td').eq(5).text());
            total += rowTotalSale;
            totalPurchase += rowTotalPurchase;
        });

        const discount = parseFloat($('#discount').val()) || 0;
        const discountType = $('#discount-type').val();
        const totalDiscount = discountType === 'نسبة' ? (total * discount / 100) : discount;
        const finalTotal = total - totalDiscount;

        $('#total-amount-text').text(finalTotal.toFixed(2));

        const paymentMethod = $('#payment-method').val();
        const cashAmount = parseFloat($('#cash-amount').val()) || 0;
        let remaining = 0;

        if (paymentMethod === 'كاش') {
            remaining = 0;
        } else if (paymentMethod === 'آجل') {
            if (cashAmount < 0 || cashAmount > finalTotal) {
                Swal.fire({
                    text: 'المبلغ النقدي يجب أن يكون ضمن حدود الفاتورة.',
                    icon: 'error',
                    showConfirmButton: false,
                    timer: 3000,
                    toast: true,
                    position: 'top-end',
                    background: '#ffffff',
                    iconColor: '#d9534f',
                    customClass: {
                        popup: 'small-toast',
                        title: 'small-toast-title',
                        content: 'small-toast-content'
                    }
                });
                $('#cash-amount').val('');
                return;
            }
            remaining = finalTotal - cashAmount;
        }

        $('#remaining-amount-text').text(remaining.toFixed(2));
    }

    // Reset form after adding an item
    function resetForm() {
        $('#item').val('');
        $('#quantity').val('');
        $('#SalePrice').val('');
        $('#PurchasePrice').val('');
        $('#store').val('');
    }

    // Event listeners for changes in payment method, discount, and cash amount
    $('#payment-method').on('change', updateTotal);
    $('#cash-amount').on('input', updateTotal);
    $('#discount, #discount-type').on('input change', updateTotal);

    // Save button (not implemented)
    



    // Dynamic updates for card views when entering item, quantity, and price
    const $cardTitle = $('.card-title');
    const $cardDate = $('.card-date');
    const $cardPrice = $('.card-price');

    $('#item').on('input', function () {
        $cardTitle.text($(this).val());
    });

    $('#quantity').on('input', function () {
        $cardDate.text($(this).val());
    });

    $('#SalePrice, #PurchasePrice').on('input', function () {
        const salePriceValue = parseFloat($('#SalePrice').val()) || 0;
        const quantityValue = parseFloat($('#quantity').val()) || 1;
        const totalSale = salePriceValue * quantityValue;
        $cardPrice.text(totalSale.toFixed(2));
    });

    // Sidebar toggling and submenu interactions
    const $sidebar = $('.sidebar');
    const $closeBtn = $('#btn');
    const $searchBtn = $('.bx-search');
    const $dropdownIcons = $('.dropdown-icon');

    $closeBtn.on('click', function () {
        $sidebar.toggleClass('open');
        menuBtnChange();
    });

    $searchBtn.on('click', function () {
        $sidebar.toggleClass('open');
        menuBtnChange();
    });

    $dropdownIcons.on('click', function () {
        const $subMenu = $(this).parent().next('.sub-menu');
        $subMenu.toggleClass('show');
        $(this).toggleClass('bx-chevron-up bx-chevron-down');
    });

    function menuBtnChange() {
        if ($sidebar.hasClass('open')) {
            $closeBtn.removeClass('bx-menu').addClass('bx-menu-alt-right');
        } else {
            $closeBtn.removeClass('bx-menu-alt-right').addClass('bx-menu');
        }
    }

    // Custom select dropdown functionality
    $('.custom-select').each(function () {
        var $customSelect = $(this);
        var $input = $customSelect.find('input');
        var $optionsList = $customSelect.find('.options-list');
        var $options = $optionsList.find('.option');

        $input.on('click', function () {
            $optionsList.show();
        });

        $(document).on('click', function (e) {
            if (!$customSelect.is(e.target) && $customSelect.has(e.target).length === 0) {
                $optionsList.hide();
            }
        });

        $input.on('input', function () {
            var filter = $input.val().toLowerCase();
            $options.each(function () {
                var text = $(this).text().toLowerCase();
                $(this).toggle(text.includes(filter));
            });
        });

        $options.on('click', function () {
            var value = $(this).data('value');
            $input.val(value);
            $optionsList.hide();
        });
    });
});
