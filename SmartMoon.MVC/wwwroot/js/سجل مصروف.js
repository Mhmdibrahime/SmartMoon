$(document).ready(function() {
    var editMode = false;
    var currentRow;

$('.save-item-btn').click(function() {
    var newItem = $('#expense-item').val();

    if (newItem.trim() !== '') {
        $('#expense-category-options').append('<div class="option" data-value="' + newItem + '">' + newItem + '</div>');
        $('#expense-item').val('');

        Swal.fire({
            text: 'تم إضافة البند بنجاح.',
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
    } else {
        Swal.fire({
            text: 'يرجى إدخال البند.',
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


    $(document).on('click', '#expense-category-options .option', function() {
        var selectedCategory = $(this).text();
        $('#expense-category').val(selectedCategory);
        $('#expense-category-options').hide();
    });

    $('#expense-category').focus(function() {
        $('#expense-category-options').show();
    });

    $(document).on('click', '#cash-register-options .option', function() {
        var selectedCashRegister = $(this).text();
        $('#cash-register').val(selectedCashRegister);
        $('#cash-register-options').hide();
    });

    $('#cash-register').focus(function() {
        $('#cash-register-options').show();
    });

    $('.add-expense-btn').click(function() {
        var amount = $('#expense-amount').val();
        var category = $('#expense-category').val();
        var cashRegister = $('#cash-register').val();

        if (amount.trim() !== '' && category.trim() !== '' && cashRegister.trim() !== '') {
            if (!editMode) {
                var newRow = '<tr><td>' + category + '</td><td>' + amount + '</td><td>' + cashRegister + '</td><td><i class="bx bxs-edit edit-row-btn"></i></td><td><i class="bx bxs-trash delete-row-btn"></i></td></tr>';
                $('#expense-body').append(newRow);
            } else {
                $(currentRow).find('td:nth-child(1)').text(category);
                $(currentRow).find('td:nth-child(2)').text(amount);
                $(currentRow).find('td:nth-child(3)').text(cashRegister);
                editMode = false;
                currentRow = null;
            }

            updateTotal();
            $('#expense-amount').val('');
            $('#expense-category').val('');
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

$(document).on('click', '.delete-row-btn', function() {
    var row = $(this).closest('tr');
    Swal.fire({
        title: 'تأكيد الحذف',
        text: 'هل أنت متأكد أنك تريد حذف هذا الصف؟',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'حذف',
        cancelButtonText: 'إلغاء',
        customClass: {
            confirmButton: 'btn btn-danger',
            cancelButton: 'btn btn-secondary'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            row.remove();
            updateTotal();
        }
    });
});

    $(document).on('click', '.edit-row-btn', function() {
        var row = $(this).closest('tr');
        var category = row.find('td:nth-child(1)').text();
        var amount = row.find('td:nth-child(2)').text();
        var cashRegister = row.find('td:nth-child(3)').text();

        $('#expense-category').val(category);
        $('#expense-amount').val(amount);
        $('#cash-register').val(cashRegister);

        editMode = true;
        currentRow = row;
    });

    function updateTotal() {
        var total = 0;
        $('#expense-body tr').each(function() {
            var amount = parseFloat($(this).find('td:nth-child(2)').text());
            total += amount;
        });
        $('#expense-total-amount-text').text(total);
    }
});
