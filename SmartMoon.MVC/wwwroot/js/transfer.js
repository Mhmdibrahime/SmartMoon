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
            }
        });
    }

    // Save the transfer when "حفظ التحويل" is clicked
    $('.save-transfer-btn').on('click', function () {
        const fromInventoryId = $('#from-store').val();
        const toInventoryId = $('#to-store').val();
        if (!fromInventoryId || !toInventoryId) {
            alert("Please select both inventories.");
            return;
        }

        const transfers = [];
        $('.transfer-qty').each(function () {
            const quantity = parseInt($(this).val());
            if (quantity > 0) {
                transfers.push({
                    ProductId: $(this).data('product-id'),
                    Quantity: quantity
                });
            }
        });

        if (transfers.length === 0) {
            alert("Please enter quantities to transfer.");
            return;
        }
        $.ajax({
            url: '/Admin/SaveTransferBetweenInventories',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',  // specify charset encoding
            dataType: 'json',                                // expecting a JSON response
            data: JSON.stringify({
                FromInventoryId: fromInventoryId,
                ToInventoryId: toInventoryId,
                Transfers: transfers
            }),
            success: function (response) {
                alert(response);
                loadProducts(fromInventoryId, $('#search-item').val());
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });
});