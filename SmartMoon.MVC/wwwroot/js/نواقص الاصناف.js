$(document).ready(function() {

    let itemsPerPage = 8;
    let currentPage = 1;
    let allItems = $('#items-table-body tr').toArray();
    let filteredItems = allItems; 

    function updateTable() {
        $('#items-table-body').empty();
        let start = (currentPage - 1) * itemsPerPage;
        let end = start + itemsPerPage;
        
        filteredItems.slice(start, end).forEach(item => {
            $('#items-table-body').append(item);
        });

        let totalPages = Math.ceil(filteredItems.length / itemsPerPage);
        $('#current-page').text(currentPage);
        $('#total-pages').text(totalPages);

        $('#prev-page').prop('disabled', currentPage === 1);
        $('#next-page').prop('disabled', currentPage === totalPages || totalPages === 0);
    }


    function filterTable() {
        let searchValue = $('#search-item').val().toLowerCase();
        let supplierFilter = $('#filter-supplier').val();

        filteredItems = allItems.filter(function(item) {
            let itemName = $(item).find('td').eq(0).text().toLowerCase();
            let supplierName = $(item).find('td').eq(2).text();

            let matchesSearch = itemName.includes(searchValue);
            let matchesSupplier = supplierFilter === "" || supplierName === supplierFilter;

            return matchesSearch && matchesSupplier;
        });

        currentPage = 1;
        updateTable();
    }

    $('#search-item').on('input', filterTable);
    $('#filter-supplier').on('change', filterTable);

    $('#prev-page').click(function() {
        if (currentPage > 1) {
            currentPage--;
            updateTable();
        }
    });

    $('#next-page').click(function() {
        let totalPages = Math.ceil(filteredItems.length / itemsPerPage);
        if (currentPage < totalPages) {
            currentPage++;
            updateTable();
        }
    });

    $('#items-table-body').on('click', '.delete-btn', function() {
        let row = $(this).closest('tr'); 
        let itemName = row.find('td').eq(0).text(); 
        
        Swal.fire({
            text: 'هل أنت متأكد أنك تريد حذف الصنف "' + itemName + '"؟',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم، احذفه!',
            cancelButtonText: 'إلغاء'
        }).then((result) => {
            if (result.isConfirmed) {
                let index = allItems.indexOf(row[0]);
                if (index > -1) {
                    allItems.splice(index, 1); 
                    filterTable();
                }

                Swal.fire({
                    text: 'تم حذف الصنف بنجاح.',
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
    });

    updateTable();
});
