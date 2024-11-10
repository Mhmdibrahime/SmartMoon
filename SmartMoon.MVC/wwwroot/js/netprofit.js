$(document).ready(function () {
    $('#view-report-btn').on('click', function () {

        const startDate = $('#start-date').val();
        const endDate = $('#end-date').val();
        const salesPoint = $('#sales-point').val();


        if (!startDate || !endDate) {
            Swal.fire('خطأ', 'يرجى تحديد التواريخ!', 'error');
            return;
        }


        $.ajax({
            url: '/Admin/GetNetProfitReport',
            type: 'GET',
            data: {
                startDate: startDate,
                endDate: endDate,
                moneyDrawer: salesPoint
            },
            success: function (data) {

                $('#operations-table-body').empty();


                const rowHtml = `
          <tr>
            <td>${new Date().toLocaleTimeString()}</td>
            <td>${new Date().toLocaleDateString()}</td>
            <td>${data.TotalRevenue.toFixed(2)}</td>
            <td>${data.TotalPurchases.toFixed(2)}</td>
            <td>${data.TotalExpenses.toFixed(2)}</td>
            <td>${data.NetProfit.toFixed(2)}</td>
          </tr>
        `;
                $('#operations-table-body').append(rowHtml);
            },
            error: function () {
                Swal.fire('خطأ', 'حدث خطأ أثناء جلب البيانات. الرجاء المحاولة مرة أخرى.', 'error');
            }
        });
    });
});