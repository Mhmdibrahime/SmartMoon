$('.print-btn1').on('click', function () {
    const invoiceNumber = '001';
    const currentDate = new Date().toLocaleDateString('ar-EG');
    const clientName = $('#client').val() || 'عميل غير محدد';

    $('.invoice-container .header .details p span').eq(0).text(invoiceNumber);
    $('.invoice-container .header .details p span').eq(1).text(currentDate);
    $('.invoice-container .header .details p span').eq(2).text(clientName);

    const invoiceRows = $('#invoice-body tr').clone().each(function () {
        const td5 = $(this).find('td:eq(5)');
        const td6 = $(this).find('td:eq(6)');
        if (td5.length) td5.remove();
        if (td6.length) td6.remove();
    });

    $('.invoice-container .invoice-table tbody').empty().append(invoiceRows);

    const totalAmount = $('#total-amount-text').text();
    $('.invoice-container .totals tr').eq(0).find('td').text(totalAmount + ' جنيه');

    const cashAmount = $('#CashPaid').val() || '0';
    $('.invoice-container .totals tr').eq(1).find('td').text(cashAmount + ' جنيه');

    const remainingAmount = $('#remaining-amount-text').text();
    $('.invoice-container .totals tr').eq(2).find('td').text(remainingAmount + ' جنيه');

    window.print();
});