$(document).ready(function () {
    
    $('#view-report-btn').on('click', function () {
        const selectedDate = $('#start-date').val();
        const drawer = $('#sales-point').val();

       
        if (!selectedDate) {
            Swal.fire('تنبيه', 'يرجى اختيار تاريخ', 'warning');
            return;
        }

       
        $.ajax({
            url: '/Admin/GetDailySalesReport', 
            type: 'GET',
            data: { selectedDate: selectedDate, drawer: drawer },
            success: function (data) {
                
                

                data.forEach(salesBill => {
                    salesBill.Items.forEach(item => {
                        
                        const profitPercentage = item.Profit / (item.Quantity * item.PurchasePrice) * 100;

                        
                        const row = `
                                        <tr>
                                            <td>${item.Name}</td>
                                            <td>${item.Quantity}</td>
                                            <td>${item.SalePrice.toFixed(2)}</td>
                                            <td>${item.TotalPrice.toFixed(2)}</td>
                                            <td>${item.PurchasePrice.toFixed(2)}</td>
                                            <td>${item.Profit.toFixed(2)}</td>
                                            <td>${profitPercentage.toFixed(2)}%</td>
                                            <td>${new Date(salesBill.Date).toLocaleTimeString()}</td>
                                            <td>${salesBill.User}</td>
                                        </tr>`;
                        $('#operations-table-body').append(row);
                    });
                });
            },
            error: function (error) {
                console.error('Error fetching daily sales report:', error);
                Swal.fire('خطأ', 'حدث خطأ أثناء جلب البيانات', 'error');
            }
        });
    });
});