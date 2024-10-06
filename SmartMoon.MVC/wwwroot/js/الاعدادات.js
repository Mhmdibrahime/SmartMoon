$(document).ready(function() {
    $('.add').hide();

    function showAlert(message, type = 'success') {
      Swal.fire({
        text: message,
        icon: type,
        showConfirmButton: false,
        timer: 2000,
        toast: true,
        position: 'top-end',
        background: '#ffffff',
        iconColor: type === 'success' ? '#28a745' : '#dc3545',
        customClass: {
          popup: 'small-toast',
          title: 'small-toast-title',
          content: 'small-toast-content'
        }
      });
    }

    $('.add-user-btn').on('click', function() {
      $('.add').slideDown();
      $('html, body').animate({ scrollTop: $('.add').offset().top }, 500);
    });
  
    function saveUserData() {
      const username = $('#username').val().trim();
      const password = $('#password').val().trim();
      const confirmPassword = $('#confirm-password').val().trim();
      const salesPoint = $('#sales-point').val();
      const userRole = $('#user-role').val();
      const permissions = [];
      $('.permissions input[type="checkbox"]:checked').each(function() {
        permissions.push($(this).val());
      });
  
      if (!username || !password || password !== confirmPassword || permissions.length === 0) {
        showAlert('يرجى التحقق من البيانات المدخلة.', 'error');
        return;
      }
  
      const userData = { username, password, salesPoint, userRole, permissions };
      const userKey = username;

      localStorage.setItem(userKey, JSON.stringify(userData));
  
      const row = $('.edit-user-btn').data('row');
      if (row) {
        row.find('td:eq(0)').text(username);
        row.find('td:eq(1)').text(userRole);
        row.find('td:eq(2)').text(salesPoint);
  
        showAlert('تم تعديل بيانات المستخدم بنجاح.');
      } else {
        $('#user-body').append(`
          <tr>
            <td>${username}</td>
            <td>${userRole}</td>
            <td>${salesPoint}</td>
            <td><i class="bx bx-edit edit-user-btn"></i></td>
            <td><i class="bx bx-trash delete-user-btn"></i></td>
          </tr>
        `);
        showAlert('تم إضافة المستخدم بنجاح.');
      }
      $('.add').slideUp();
      $('html, body').animate({ scrollTop: $('.user-table').offset().top }, 500);
      $('#username, #password, #confirm-password').val(''); 
      $('.permissions input[type="checkbox"]').prop('checked', false);
      $('.edit-user-btn').removeData('row');
    }
  
    $('.save-user-btn').on('click', saveUserData);
    $(document).on('click', '.edit-user-btn', function() {
      const row = $(this).closest('tr');
      const username = row.find('td:eq(0)').text();
      const userRole = row.find('td:eq(1)').text();
      const salesPoint = row.find('td:eq(2)').text();

      const userData = JSON.parse(localStorage.getItem(username)) || {};
      $('#username').val(userData.username || username);
      $('#password').val(userData.password || '');
      $('#confirm-password').val(userData.password || '');
      $('#sales-point').val(userData.salesPoint || salesPoint);
      $('#user-role').val(userData.userRole || userRole);
      $('.permissions input[type="checkbox"]').each(function() {
        $(this).prop('checked', userData.permissions ? userData.permissions.includes($(this).val()) : false);
      });

      $('.add').slideDown();
      $('html, body').animate({ scrollTop: $('.add').offset().top }, 500);
  
      $('.edit-user-btn').data('row', row);
    });

    $(document).on('click', '.delete-user-btn', function() {
      const row = $(this).closest('tr');
      const username = row.find('td:eq(0)').text();
      localStorage.removeItem(username);
      Swal.fire({
        text: 'هل أنت متأكد من حذف المستخدم؟',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'نعم، احذفه',
        cancelButtonText: 'إلغاء',
        background: '#ffffff',
        iconColor: '#dc3545',
        customClass: {
          popup: 'small-toast',
          title: 'small-toast-title',
          content: 'small-toast-content'
        }
      }).then((result) => {
        if (result.isConfirmed) {
          row.remove();
          showAlert('تم حذف المستخدم بنجاح.');
        }
      });
    });
  });
  