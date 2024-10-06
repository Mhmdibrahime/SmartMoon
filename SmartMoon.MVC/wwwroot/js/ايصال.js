$(document).ready(function() {
    var editMode = false;
    var currentRow;

    $('#client-name').focus(function() {
        $('#client-name-options').show();
    });

    $(document).on('click', '#client-name-options .option', function() {
        var selectedClient = $(this).text();
        var clientBalance = $(this).data('balance');
        $('#client-name').val(selectedClient);
        $('#previous-balance-amount').text(clientBalance);
        $('#cash-register').focus();
        $('#client-name-options').hide();
    });

    $('#cash-register').focus(function() {
        $('#cash-register-options').show();
    });

    $(document).on('click', '#cash-register-options .option', function() {
        var selectedCashRegister = $(this).text();
        $('#cash-register').val(selectedCashRegister);
        $('#cash-register-options').hide();
    });

    $('.add-expense-btn').click(function() {
        var amount = parseFloat($('#payment-amount').val());
        var client = $('#client-name').val();
        var cashRegister = $('#cash-register').val();

        if (amount && client && cashRegister) {
            var previousBalance = parseFloat($('#previous-balance-amount').text());
            var newBalance = previousBalance - amount;

            if (!editMode) {
                var newRow = '<tr><td>' + client + '</td><td>' + amount + '</td><td><i class="bx bxs-edit edit-row-btn"></i></td><td><i class="bx bxs-trash delete-row-btn"></i></td></tr>';
                $('#receipt-body').append(newRow);

                $('#previous-balance-amount').text(newBalance);
                $('#new-balance-amount').text(newBalance);
            } else {
                var oldAmount = parseFloat($(currentRow).find('td:nth-child(2)').text());
                $(currentRow).find('td:nth-child(1)').text(client);
                $(currentRow).find('td:nth-child(2)').text(amount);
                editMode = false;
                currentRow = null;

                var updatedBalance = (previousBalance + oldAmount) - amount;
                $('#previous-balance-amount').text(updatedBalance);
                $('#new-balance-amount').text(updatedBalance);
            }

            $('#payment-amount').val('');
            $('#client-name').val('');
            $('#cash-register').val('');
        } else {
            Swal.fire({
                text: 'يرجى ملء جميع الحقول المطلوبة.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
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
        }
    });

    $(document).on('click', '.edit-row-btn', function() {
        var row = $(this).closest('tr');
        var client = row.find('td:nth-child(1)').text();
        var amount = row.find('td:nth-child(2)').text();

        $('#client-name').val(client);
        $('#payment-amount').val(amount);

        editMode = true;
        currentRow = row;
    });

    $(document).on('click', '.delete-row-btn', function() {
        var row = $(this).closest('tr');
        var amount = parseFloat(row.find('td:nth-child(2)').text());
        var previousBalance = parseFloat($('#previous-balance-amount').text());

        Swal.fire({
            title: 'هل أنت متأكد؟',
            text: "لن تتمكن من التراجع عن هذا الإجراء!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'نعم، احذفه!',
            cancelButtonText: 'إلغاء'
        }).then((result) => {
            if (result.isConfirmed) {
                row.remove();
                var newBalance = previousBalance + amount;
                $('#previous-balance-amount').text(newBalance);
                $('#new-balance-amount').text(newBalance);

                Swal.fire('تم الحذف!', 'تم حذف العميل بنجاح.', 'success');
            }
        });
    });

$('.save-receipt-btn').click(function() {
    var receiptData = [];

    if ($('#receipt-body tr').length === 0) {
        Swal.fire({
            text: 'لا يوجد أي بيانات لحفظها.',
            icon: 'warning',
            showConfirmButton: false,
            timer: 2000,
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

    $('#receipt-body tr').each(function() {
        var client = $(this).find('td:nth-child(1)').text();
        var amount = $(this).find('td:nth-child(2)').text();
        receiptData.push({ client: client, amount: amount });
    });

    console.log('بيانات الإيصال:', receiptData);

    $('#previous-balance-amount').text('0');
    $('#new-balance-amount').text('0');

    $('#receipt-body').empty();

    Swal.fire({
        text: 'تم حفظ السجل بنجاح.',
        icon: 'success',
        showConfirmButton: false,
        timer: 2000,
        toast: true,
        position: 'top-end',
        background: '#ffffff',
        iconColor: '#28a745',
        customClass: {
            popup: 'small-toast',
            title: 'small-toast-title',
            content: 'small-toast-content'
        }
    });
});

});
