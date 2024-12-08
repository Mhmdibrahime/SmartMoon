$(document).ready(function() {
    $('.error').hide();
    let customersPerPage = 8;
    let currentPage = 1;
    let allCustomers = $('#customers-table-body tr').toArray(); 
    let filteredCustomers = allCustomers; 
    let editingRow = null;

    function updateTable() {
        $('#customers-table-body').empty();
        let start = (currentPage - 1) * customersPerPage;
        let end = start + customersPerPage;
        filteredCustomers.slice(start, end).forEach(customer => {
            $('#customers-table-body').append(customer);
        });

        let totalPages = Math.ceil(filteredCustomers.length / customersPerPage);
        $('#current-page').text(currentPage);
        $('#total-pages').text(totalPages);

        $('#prev-page').prop('disabled', currentPage === 1);
        $('#next-page').prop('disabled', currentPage === totalPages || totalPages === 0);
    }

    function filterTable() {
        let searchValue = $('#search-customer').val().toLowerCase();
        let balanceFilter = $('#filter-balance').val();
    
        filteredCustomers = allCustomers.filter(function(customer) {
            let customerName = $(customer).find('td').eq(0).text().toLowerCase();
            let matchesSearch = customerName.includes(searchValue);
            return matchesSearch; 
        });
    
        if (balanceFilter === "credit") {
            filteredCustomers.sort(function(a, b) {
                return parseInt($(a).find('td').eq(3).text()) - parseInt($(b).find('td').eq(3).text()); 
            });
        } else if (balanceFilter === "debt") {
            filteredCustomers.sort(function(a, b) {
                return parseInt($(b).find('td').eq(3).text()) - parseInt($(a).find('td').eq(3).text());
            });
        }
    
        currentPage = 1;
        updateTable(); 
    }
    

    $('#search-customer').on('input', filterTable);
    $('#filter-balance').on('change', filterTable);

    $('#prev-page').click(function() {
        if (currentPage > 1) {
            currentPage--;
            updateTable();
        }
    });

    $('#next-page').click(function() {
        let totalPages = Math.ceil(filteredCustomers.length / customersPerPage);
        if (currentPage < totalPages) {
            currentPage++;
            updateTable();
        }
    });

    $('#customers-table-body').on('click', '.delete-btn', function() {
        let row = $(this).closest('tr');
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
                allCustomers = $('#customers-table-body tr').toArray(); 
                filterTable(); 
                Swal.fire('تم الحذف!', 'تم حذف العميل بنجاح.', 'success');
            }
        });
    });

    $('#customers-table-body').on('click', '.edit-btn', function() {
        let row = $(this).closest('tr');
        let nameCell = row.find('td').eq(0); 
        let phoneCell = row.find('td').eq(1); 

        if (!editingRow) {
            nameCell.html(`<input type="text" value="${nameCell.text()}">`);
            phoneCell.html(`<input type="text" value="${phoneCell.text()}">`);

            $(this).removeClass('bxs-edit').addClass('bxs-save');
            editingRow = row;
        } else {
            Swal.fire({
                title: 'هل أنت متأكد من حفظ التعديلات؟',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'نعم، احفظ',
                cancelButtonText: 'إلغاء'
            }).then((result) => {
                if (result.isConfirmed) {
                    nameCell.text(nameCell.find('input').val());
                    phoneCell.text(phoneCell.find('input').val());

                    $(this).removeClass('bxs-save').addClass('bxs-edit');
                    editingRow = null;

                    Swal.fire('تم الحفظ!', 'تم تعديل العميل بنجاح.', 'success');
                }
            });
        }
    });

    function showCustomAlert(message) {
        Swal.fire({
            text: message,
            icon: 'success',
            showConfirmButton: false,
            timer: 2000,
            toast: true,
            position: 'top-end'
        });
    }

    updateTable();
    $(".bx-show-alt").click(function(){
        window.location.href = "كشف حساب.html";
      });  
});
