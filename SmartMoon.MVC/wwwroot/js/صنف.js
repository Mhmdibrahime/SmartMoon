$(document).ready(function() {
  $('.error').hide();
  let itemsPerPage = 8;
  let currentPage = 1
  let allItems = $('#items-table-body tr').toArray(); 
  let filteredItems = allItems; 
  let editingRow = null;

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
      let storeFilter = $('#filter-store').val();

      filteredItems = allItems.filter(function(item) {
          let itemName = $(item).find('td').eq(0).text().toLowerCase();
          let supplierName = $(item).find('td').eq(3).text();
          let storeName = $(item).find('td').eq(4).text();

          let matchesSearch = itemName.includes(searchValue);
          let matchesSupplier = supplierFilter === "" || supplierName === supplierFilter;
          let matchesStore = storeFilter === "" || storeName === storeFilter;

          return matchesSearch && matchesSupplier && matchesStore;
      });

      currentPage = 1;
      updateTable(); 
  }

  $('#search-item').on('input', filterTable);
  $('#filter-supplier, #filter-store').on('change', filterTable);

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

  $('#add-item-btn').click(function() {
      let itemName = $('#item-name').val().trim();

      if (itemName.length < 10) {
          $('.error').show();
          $('#error-supplier-name').text('اسم الصنف يجب أن يحتوي على 10 أحرف على الأقل.');
      } else {
          $('.error').hide();
          $('#error-supplier-name').text('');
          showCustomAlert('تم إضافة الصنف بنجاح.');

          if (editingRow) {
              $(editingRow).find('td').eq(0).text(itemName);
              editingRow = null;
              $('#add-item-btn').text('إضافة صنف');
          }

          $('#item-name').val('');
      }
  });

  $('#items-table-body').on('click', '.edit-btn', function() {
      let row = $(this).closest('tr');
      let itemName = row.find('td').eq(0).text();

      $('#item-name').val(itemName);
      $('#add-item-btn').text('تعديل صنف');
      editingRow = row;
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
});
