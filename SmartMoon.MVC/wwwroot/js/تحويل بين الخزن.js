$(document).ready(function() {
    var editingVault = null; 

    $('.save-vault-btn').on('click', function() {
        var vaultName = $('#new-vault').val().trim();
        if (vaultName.length < 10) {
            Swal.fire({
                text: 'يجب أن يكون اسم الخزنة 10 حروف على الأقل.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#ffc107',
                customClass: {
                    popup: 'small-toast',
                    title: 'small-toast-title',
                    content: 'small-toast-content'
                }
            });
            return;
        }

        if (editingVault) {
            editingVault.find('td:first').text(vaultName);
            Swal.fire({
                text: 'تم تعديل اسم الخزنة بنجاح.',
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
            editingVault = null;
        } else {
            if ($('#from-vault option[value="' + vaultName + '"]').length === 0) {
                $('#from-vault, #to-vault').append('<option value="' + vaultName + '">' + vaultName + '</option>');
                $('#vault-transfer-body').append('<tr><td>' + vaultName + '</td><td>0</td><td><input type="number" class="transfer-amount" placeholder="أدخل مبلغ التحويل"></td><td><i class="bx bxs-edit edit-btn"></i></td><td><i class="bx bxs-trash delete-btn"></i></td></tr>');
                Swal.fire({
                    text: 'تم إضافة الخزنة بنجاح.',
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
                    text: 'الخزنة موجودة بالفعل.',
                    icon: 'warning',
                    showConfirmButton: false,
                    timer: 2000,
                    toast: true,
                    position: 'top-end',
                    background: '#ffffff',
                    iconColor: '#ffc107',
                    customClass: {
                        popup: 'small-toast',
                        title: 'small-toast-title',
                        content: 'small-toast-content'
                    }
                });
            }
        }
        $('#new-vault').val('');
    });

    $(document).on('click', '.edit-btn', function() {
        editingVault = $(this).closest('tr');
        var currentVaultName = editingVault.find('td:first').text();
        $('#new-vault').val(currentVaultName);
        Swal.fire({
            text: 'يمكنك تعديل اسم الخزنة الآن.',
            icon: 'info',
            showConfirmButton: false,
            timer: 2000,
            toast: true,
            position: 'top-end',
            background: '#ffffff',
            iconColor: '#17a2b8',
            customClass: {
                popup: 'small-toast',
                title: 'small-toast-title',
                content: 'small-toast-content'
            }
        });
    });

    $(document).on('click', '.delete-btn', function() {
        var vaultRow = $(this).closest('tr');
        var vaultName = vaultRow.find('td:first').text();
        var vaultAmount = parseFloat(vaultRow.find('td:eq(1)').text());

        if (vaultAmount > 0) {
            Swal.fire({
                text: 'لا يمكن حذف الخزنة إلا إذا كانت فارغة. يرجى تحويل الأموال أولاً.',
                icon: 'error',
                showConfirmButton: false,
                timer: 3000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#dc3545',
                customClass: {
                    popup: 'small-toast',
                    title: 'small-toast-title',
                    content: 'small-toast-content'
                }
            });
        } else {
            Swal.fire({
                text: 'هل أنت متأكد أنك تريد حذف الخزنة "' + vaultName + '"؟',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'نعم، احذفها!',
                cancelButtonText: 'إلغاء'
            }).then((result) => {
                if (result.isConfirmed) {
                    vaultRow.remove();
                    $('#from-vault option[value="' + vaultName + '"], #to-vault option[value="' + vaultName + '"]').remove();
                    Swal.fire({
                        text: 'تم حذف الخزنة بنجاح.',
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
                }
            });
        }
    });
    
    $('#from-vault').on('change', function() {
        var selectedVault = $(this).val();

        if (selectedVault) {
            $('#vault-transfer-body tr').hide();
            $('#vault-transfer-body tr').each(function() {
                if ($(this).find('td:first').text() === selectedVault) {
                    $(this).show();
                }
            });
        } else {
            $('#vault-transfer-body tr').show();
        }
    });
    $('.save-transfer-btn').on('click', function() {
        var fromVault = $('#from-vault').val();
        var toVault = $('#to-vault').val();
        var transferAmount = parseFloat($('#vault-transfer-body tr:visible').find('.transfer-amount').val());

        if (!fromVault || !toVault) {
            Swal.fire({
                text: 'يرجى اختيار الخزن بشكل صحيح.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#ffc107',
                customClass: {
                    popup: 'small-toast',
                    title: 'small-toast-title',
                    content: 'small-toast-content'
                }
            });
            return;
        }

        if (fromVault === toVault) {
            Swal.fire({
                text: 'لا يمكن التحويل بين نفس الخزنة.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#ffc107',
                customClass: {
                    popup: 'small-toast',
                    title: 'small-toast-title',
                    content: 'small-toast-content'
                }
            });
            return;
        }

        if (isNaN(transferAmount) || transferAmount <= 0) {
            Swal.fire({
                text: 'يرجى إدخال مبلغ صحيح للتحويل.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#ffc107',
                customClass: {
                    popup: 'small-toast',
                    title: 'small-toast-title',
                    content: 'small-toast-content'
                }
            });
            return;
        }

        var fromVaultRow = $('#vault-transfer-body tr').filter(function() {
            return $(this).find('td:first').text() === fromVault;
        });

        var toVaultRow = $('#vault-transfer-body tr').filter(function() {
            return $(this).find('td:first').text() === toVault;
        });

        var fromCurrentAmount = parseFloat(fromVaultRow.find('td:eq(1)').text());
        var toCurrentAmount = parseFloat(toVaultRow.find('td:eq(1)').text());

        if (transferAmount > fromCurrentAmount) {
            Swal.fire({
                text: 'المبلغ المراد تحويله أكبر من المبلغ الموجود في الخزنة.',
                icon: 'warning',
                showConfirmButton: false,
                timer: 2000,
                toast: true,
                position: 'top-end',
                background: '#ffffff',
                iconColor: '#ffc107',
                customClass: {
                    popup: 'small-toast',
                    title: 'small-toast-title',
                    content: 'small-toast-content'
                }
            });
            return;
        }
        fromVaultRow.find('td:eq(1)').text(fromCurrentAmount - transferAmount);
        toVaultRow.find('td:eq(1)').text(toCurrentAmount + transferAmount);

        Swal.fire({
            text: 'تم التحويل بنجاح.',
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
