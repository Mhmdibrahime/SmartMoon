$(document).ready(function () {
    let totalAmount = 0;

    // Function to add an item to the invoice table
    $("#add-item-btn").click(function () {
        const productId = $("#ProductSelection").val();
        const productName = $("#ProductSelection option:selected").text();
        const quantity = parseFloat($("#Quantity").val()) || 0;
        const purchasePrice = parseFloat($("#PurchasePrice").val()) || 0;
        const salePrice = parseFloat($("#SalePrice").val()) || 0;
        const inventoryName = $("#InventorySelection option:selected").text();
        const totalPrice = quantity * purchasePrice;

        if (!productId || !quantity || !purchasePrice) {
            alert("Please fill all required fields!");
            return;
        }

        // Append row to invoice table
        $("#invoice-body").append(`
                        <tr>
                            <td>${productName}</td>
                            <td>${quantity}</td>
                            <td>${purchasePrice}</td>
                            <td>${salePrice}</td>
                            <td class="item-total">${totalPrice.toFixed(2)}</td>
                            <td>${inventoryName}</td>
                            <td><button type="button" class="edit-btn">تعديل</button></td>
                            <td><button type="button" class="delete-btn">حذف</button></td>
                        </tr>
                    `);

        totalAmount += totalPrice;
        updateTotals();

        // Clear input fields
        $("#ProductSelection").val("");
        $("#Quantity").val("");
        $("#PurchasePrice").val("");
        $("#SalePrice").val("");
        $("#InventorySelection").val("");
    });

    // Edit and update an item in the invoice table
    $("#invoice-body").on("click", ".edit-btn", function () {
        const row = $(this).closest("tr");
        const productName = row.find("td:nth-child(1)").text();
        const quantity = parseFloat(row.find("td:nth-child(2)").text());
        const purchasePrice = parseFloat(row.find("td:nth-child(3)").text());
        const salePrice = parseFloat(row.find("td:nth-child(4)").text());

        $("#ProductSelection option:contains(" + productName + ")").prop("selected", true);
        $("#Quantity").val(quantity);
        $("#PurchasePrice").val(purchasePrice);
        $("#SalePrice").val(salePrice);

        totalAmount -= parseFloat(row.find(".item-total").text());
        row.remove();
        updateTotals();
    });

    // Function to update total and remaining balance
    function updateTotals() {
        $("#total-amount-text").text(totalAmount.toFixed(2));
        $("#TotalAmount").val(totalAmount.toFixed(2));
        updateRemainingBalance();
    }

    // Delete an item from the invoice
    $("#invoice-body").on("click", ".delete-btn", function () {
        const row = $(this).closest("tr");
        const itemTotal = parseFloat(row.find(".item-total").text()) || 0;
        totalAmount -= itemTotal;
        row.remove();
        updateTotals();
    });

    // Update remaining balance when cash paid is entered
    $("#CashPaid, #DiscountAmount, #DiscountType").on("input change", updateRemainingBalance);

    function updateRemainingBalance() {
        const cashPaid = parseFloat($("#CashPaid").val()) || 0;
        const discountAmount = parseFloat($("#DiscountAmount").val()) || 0;
        const discountType = $("#DiscountType").val();

        // Calculate the discount based on its type
        let totalDiscount = 0;
        if (discountType === "Percentage") {
            totalDiscount = (discountAmount / 100) * totalAmount;
        } else if (discountType === "Amount") {
            totalDiscount = discountAmount;
        }

        // Calculate the remaining balance after discount
        const remainingBalance = totalAmount - cashPaid - totalDiscount;

        // Update the displayed remaining balance
        $("#remaining-amount-text").text(remainingBalance.toFixed(2));
        $("#RemainingBalance").val(remainingBalance.toFixed(2));
    }

    function saveBill() {
        // Gather all items from the invoice table
        let items = [];
        $("#invoice-body tr").each(function () {
            let row = $(this);
            let productId = $("#ProductSelection option:contains(" + row.find("td:eq(0)").text() + ")").val(); // Get the productId from the dropdown
            let quantity = parseFloat(row.find("td:eq(1)").text()) || 0;
            let purchasePrice = parseFloat(row.find("td:eq(2)").text()) || 0;
            let salePrice = parseFloat(row.find("td:eq(3)").text()) || 0;
            let inventoryId = $("#InventorySelection option:contains(" + row.find("td:eq(5)").text() + ")").val(); // Get the inventoryId from the dropdown

            items.push({
                ProductId: productId, // Adjusted to match your model
                Quantity: quantity,
                PurchasePrice: purchasePrice,
                SalePrice: salePrice,
                InventoryId: inventoryId // Added this field
            });
        });

        // Prepare the data to send to the server
        const dataToSend = {
            SupplierId: $("#SupplierId").val(),
            Items: items,
            TotalAmount: totalAmount, // Ensure you have the correct totalAmount calculated
            DiscountAmount: parseFloat($("#DiscountAmount").val()) || 0, // Ensure discount amount is included
            CashPaid: parseFloat($("#CashPaid").val()) || 0, // Cash paid amount
            MoneyDrawer: $("#MoneyDrawer").val(),
            PaymentMethod: $("#PaymentMethod").val(), // Include payment method if you have a dropdown for it
            RemainingBalance: remainingBalance
        };

        // Send data to the server using AJAX
        $.ajax({
            url: '/Admin/CreatePurchaseBill',
            method: 'POST',
            contentType: 'application/text', // Ensure this is set
            data: dataToSend, // Use the prepared dataToSend
            success: function (response) {
                alert("تم حفظ الفاتورة بنجاح!");
                // Reset the form
                $("#invoice-body").empty();
                $("#SupplierId").val('');
                totalAmount = 0;
                $("#total-amount-text").text(totalAmount.toFixed(2));
                $("#TotalAmount").val(totalAmount.toFixed(2));
            },
            error: function (xhr, status, error) {
                alert("حدث خطأ في حفظ الفاتورة: " + xhr.responseText);
            }
        });
    }

    // Optionally, bind saveBill function to a button if needed
    $("#save-bill-btn").click(saveBill);
});