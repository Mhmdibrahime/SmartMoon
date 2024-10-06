$(document).ready(function () {
    const rowsPerPage = 8;
    let currentPage = 1;
    let totalPages = 1;
    let stores = ['مخزن 1', 'مخزن 2', 'مخزن 3']; 
    let items = {};

    function initializeTable() {
        $('#transfer-body tr').each(function () {
            const item = $(this).find('td:first-child').text().trim();
            const store = $(this).find('td:nth-child(2)').text().trim();
            const quantity = parseInt($(this).find('td:nth-child(3)').text().trim());

            if (!items[item]) {
                items[item] = {};
            }
            if (!items[item][store]) {
                items[item][store] = 0;
            }

            items[item][store] += quantity;
        });
    }

    function updateStoreOptions() {
        const storeOptions = stores.map(store => `<option value="${store}">${store}</option>`).join('');
        $('#from-store, #to-store').html('<option value="">اختر المخزن</option>' + storeOptions);
    }

    function updateTable(store) {
        $('#transfer-body').empty();

        if (!store) {
            Object.keys(items).forEach(function (item) {
                Object.keys(items[item]).forEach(function (store) {
                    if (items[item][store] > 0) {
                        $('#transfer-body').append(`
                            <tr>
                                <td>${item}</td>
                                <td>${store}</td>
                                <td>${items[item][store]}</td>
                                <td><input type="number" class="transfer-qty" placeholder="ادخل الكميه"></td>
                            </tr>
                        `);
                    }
                });
            });
        } else {
            Object.keys(items).forEach(function (item) {
                if (items[item][store] && items[item][store] > 0) {
                    $('#transfer-body').append(`
                        <tr>
                            <td>${item}</td>
                            <td>${store}</td>
                            <td>${items[item][store]}</td>
                            <td><input type="number" class="transfer-qty" placeholder="ادخل الكميه"></td>
                        </tr>
                    `);
                }
            });
        }
        
        updatePagination();
    }

    function showPage(page) {
        $('#transfer-body tr').hide();
        const start = (page - 1) * rowsPerPage;
        const end = start + rowsPerPage;
        $('#transfer-body tr').slice(start, end).show();
        $('#current-page').text(page);
        $('#total-pages').text(totalPages);

        $('#prev-page').prop('disabled', page === 1);
        $('#next-page').prop('disabled', page === totalPages);
    }

    function updatePagination() {
        totalPages = Math.ceil($('#transfer-body tr').length / rowsPerPage);
        showPage(currentPage);
    }

    $('.save-store-btn').click(function () {
        const newStore = $('#new-store').val().trim();
        if (newStore === '') {
            Swal.fire({
                text: 'يرجى إدخال المخزن.',
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
        } else if (!stores.includes(newStore)) {
            stores.push(newStore);
            updateStoreOptions();
            Swal.fire({
                text: 'تم حفظ المخزن بنجاح',
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
            $('#new-store').val('');
        } else {
            Swal.fire({
                text: 'المخزن موجود بالفعل',
                icon: 'error',
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

    $('#from-store').change(function () {
        const selectedStore = $(this).val();
        updateTable(selectedStore);
    });

    $('#to-store').change(function () {
        const toStore = $(this).val();
        if (toStore === '') {
            updateTable(null);
        }
    });

    $('#search-item').on('input', function () {
        const searchValue = $(this).val().trim();
        if (searchValue) {
            $('#transfer-body tr').each(function () {
                const item = $(this).find('td:first-child').text();
                const qty = $(this).find('.transfer-qty').val();
                
                if (qty && parseInt(qty) > 0) {
                    $(this).show(); 
                } else if (item.includes(searchValue)) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });
        } else {
            $('#transfer-body tr').show(); 
        }
    });
    

    function calculateTotalTransferred() {
        let totalTransferred = 0;
        $('#transfer-body tr:visible').each(function () {
            const qty = $(this).find('.transfer-qty').val();
            if (qty && parseInt(qty) > 0) {
                totalTransferred += parseInt(qty);
            }
        });

        $('#transfer-total-amount-text').text(totalTransferred);
    }

    $('.save-transfer-btn').click(function () {
        const fromStore = $('#from-store').val();
        const toStore = $('#to-store').val();

        if (fromStore === '') {
            Swal.fire({
                text: 'يجب اختيار المخزن الذي ستنقل منه',
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
        } else if (toStore === '') {
            Swal.fire({
                text: 'يجب اختيار المخزن الذي ستنقل إليه',
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
        } else if (fromStore === toStore) {
            Swal.fire({
                text: 'لا يمكنك نقل الصنف إلى نفس المخزن',
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
        } else {
            let hasQuantity = false;
            let transferSuccess = true;

            $('#transfer-body tr:visible').each(function () {
                const item = $(this).find('td:first-child').text();
                const qty = $(this).find('.transfer-qty').val();
                const availableQty = parseInt($(this).find('td:nth-child(3)').text());

                if (qty && parseInt(qty) > 0) {
                    hasQuantity = true;
                    const transferQty = parseInt(qty);

                    if (transferQty > availableQty) {
                        Swal.fire({
                            text: `الكمية المتاحة من ${item} في ${fromStore} غير كافية`,
                            icon: 'error',
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
                        transferSuccess = false;
                        return false;
                    }

                    items[item][fromStore] -= transferQty;
                    if (!items[item][toStore]) {
                        items[item][toStore] = 0;
                    }
                    items[item][toStore] += transferQty;
                }
            });

            if (!hasQuantity) {
                Swal.fire({
                    text: 'يجب إدخال كمية لنقلها',
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
            } else if (transferSuccess) {
                calculateTotalTransferred();
                Swal.fire({
                    text: 'تم التحويل بنجاح',
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
                updateTable(fromStore);
                $('#transfer-total-amount-text').text('0');
            }
        }
    });

    $(document).on('input', '.transfer-qty', function () {
        calculateTotalTransferred();
    });

    $('#prev-page').click(function () {
        if (currentPage > 1) {
            currentPage--;
            showPage(currentPage);
        }
    });

    $('#next-page').click(function () {
        if (currentPage < totalPages) {
            currentPage++;
            showPage(currentPage);
        }
    });

    initializeTable();
    updateStoreOptions();
});
