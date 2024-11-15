// JavaScript code to populate the form with dynamic data
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

    // Save the transfer using a traditional form submission
    $('.save-transfer-btn').on('click', function (event) {
        event.preventDefault();  // Prevents default button behavior

        const fromInventoryId = $('#from-store').val();
        const toInventoryId = $('#to-store').val();
        if (!fromInventoryId || !toInventoryId) {
            alert("Please select both inventories.");
            return;
        }

        // Set the from and to inventory IDs in hidden form fields
        $('#fromInventoryId').val(fromInventoryId);
        $('#toInventoryId').val(toInventoryId);

        // Prepare transfers array and add each item to the form as hidden inputs
        const transfers = [];
        $('#transferItemsContainer').empty(); // Clear previous items
        $('.transfer-qty').each(function (index) {
            const quantity = parseInt($(this).val());
            if (quantity > 0) {
                const productId = $(this).data('product-id');

                // Dynamically add transfer items to the form as hidden fields
                $('<input>').attr({
                    type: 'hidden',
                    name: Transfers[${ index }].ProductId,
                    value: productId
                                }).appendTo('#transferItemsContainer');

        $('<input>').attr({
            type: 'hidden',
            name: Transfers[${ index }].Quantity,
            value: quantity
                                }).appendTo('#transferItemsContainer');
}
                        });

if ($('#transferItemsContainer').children().length === 0) {
    alert("Please enter quantities to transfer.");
    return;
}

// Submit the form
$('#transferForm').submit();
                    });
                });
