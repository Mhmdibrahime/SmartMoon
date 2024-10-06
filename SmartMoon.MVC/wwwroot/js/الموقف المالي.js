$(document).ready(function () {
    function parseCurrency(value) {
        return parseFloat(value.replace(/,/g, '').replace(' جنيه', '').trim());
    }

    function calculateTotals() {
        let totalLana = 0;
        $('.financial-part:eq(0) table tbody tr:not(.total-row) td:nth-child(2)').each(function () {
            totalLana += parseCurrency($(this).text());
        });
        $('.financial-part:eq(0) .total-row td:nth-child(2)').text(totalLana.toLocaleString() + ' جنيه');

        let totalAlayna = 0;
        $('.financial-part:eq(1) table tbody tr:not(.total-row) td:nth-child(2)').each(function () {
            totalAlayna += parseCurrency($(this).text());
        });
        $('.financial-part:eq(1) .total-row td:nth-child(2)').text(totalAlayna.toLocaleString() + ' جنيه');


        let totalFaqi = 0;
        $('.financial-part:eq(2) table tbody tr:not(.total-row) td:nth-child(2)').each(function () {
            totalFaqi += parseCurrency($(this).text());
        });
        $('.financial-part:eq(2) .total-row td:nth-child(2)').text(totalFaqi.toLocaleString() + ' جنيه');

        let totalMowqif = totalFaqi + totalLana - totalAlayna;
        $('.financial-summary .total-row td:nth-child(2)').text(totalMowqif.toLocaleString() + ' جنيه');

        $('.financial-summary tbody tr:eq(0) td:nth-child(2)').text(totalFaqi.toLocaleString() + ' جنيه');
        $('.financial-summary tbody tr:eq(1) td:nth-child(2)').text(totalLana.toLocaleString() + ' جنيه');
        $('.financial-summary tbody tr:eq(2) td:nth-child(2)').text(totalAlayna.toLocaleString() + ' جنيه');
    }

    calculateTotals();
});