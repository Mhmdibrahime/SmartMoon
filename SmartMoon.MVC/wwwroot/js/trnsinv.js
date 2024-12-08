$(document).ready(function () {
    // Load products when "from-store" changes
    $('#from-store').on('change', function () {
        const fromInventoryId = $(this).val();
        const searchQuery = $('#search-item').val();
        loadProducts(fromInventoryId, searchQuery);
    });

    // Filter products by name as the user types
    $('#search-item').on('input', function () {
        const fromInventoryId = $('#from-store').val();
        const searchQuery = $(this).val();
        loadProducts(fromInventoryId, searchQuery);
    });

    // Load products based on selected inventory and search query
    function loadProducts(fromInventoryId, searchQuery) {
        if (!fromInventoryId) return;

        $.ajax({
            url: '/Admin/GetProducts',
            data: { fromInventoryId, searchQuery },
            success: function (products) {
                const tbody = $('#transfer-body');
                tbody.empty();

                if (!Array.isArray(products) || products.length === 0) {
                    tbody.append('<tr><td colspan="4">لا توجد منتجات متاحة.</td></tr>');
                    return;
                }

                products.forEach(product => {
                    tbody.append(`
                                        <tr>
                                            <td>${product.name}</td>
                                            <td>${product.inventory}</td>
                                            <td>${product.quantity}</td>
                                            <td><input type="number" class="transfer-qty" data-product-id="${product.id}" placeholder="ادخل الكميه"></td>
                                        </tr>
                                    `);
                });
            },
            error: function () {
                Swal.fire({
                    text: 'خطأ في تحميل البيانات. يرجى المحاولة مرة أخرى.',
                    icon: 'error',
                    showConfirmButton: false,
                    timer: 3000,
                    toast: true,
                    position: 'top-end',
                    background: '#ffffff',
                    iconColor: '#d9534f',
                });
            }
        });
    }

    // Save the transfer using a traditional form submission
    $('.save-transfer-btn').on('click', function (event) {
        event.preventDefault();  // Prevents default button behavior

        const fromInventoryId = $('#from-store').val();
        const toInventoryId = $('#to-store').val();

        // Check if from and to inventories are the same
        if (fromInventoryId === toInventoryId) {
            Swal.fire({
                text: 'لا يمكن تحويل المنتجات إلى نفس المخزن.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#f0ad4e',
            });
            return;
        }

        // Check if both from and to inventories are selected
        if (!fromInventoryId || !toInventoryId) {
            Swal.fire({
                text: 'يرجى اختيار المخزن.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#f0ad4e',
            });
            return;
        }

        // Prepare transfers array and add each item to the form as hidden inputs
        $('#fromInventoryId').val(fromInventoryId);
        $('#toInventoryId').val(toInventoryId);

        const transfers = [];
        $('#transferItemsContainer').empty(); // Clear previous items
        $('.transfer-qty').each(function (index) {
            const quantity = parseInt($(this).val());
            if (quantity > 0) {
                const productId = $(this).data('product-id');

                // Dynamically add transfer items to the form as hidden fields
                $('<input>').attr({
                    type: 'hidden',
                    name: `Transfers[${index}].ProductId`,
                    value: productId
                }).appendTo('#transferItemsContainer');

                $('<input>').attr({
                    type: 'hidden',
                    name: `Transfers[${index}].Quantity`,
                    value: quantity
                }).appendTo('#transferItemsContainer');
            }
        });

        if ($('#transferItemsContainer').children().length === 0) {
            Swal.fire({
                text: 'يرجى إدخال الكميات المراد نقلها.',
                icon: 'info',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#5bc0de',
            });
            return;
        }

        $('#transferForm').submit();
    });

    // Validate Add Store Form
    $('#addStoreForm').submit(function (event) {
        var storeName = $('#new-store').val().trim();

        if (storeName === '') {
            event.preventDefault();
            Swal.fire({
                text: 'يرجى إدخال اسم المخزن.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#f0ad4e',
            });
        }
    });
});