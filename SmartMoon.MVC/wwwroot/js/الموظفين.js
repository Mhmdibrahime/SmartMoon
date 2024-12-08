let employees = [];
let currentEditIndex = -1;

function calculateTotalSalary() {
  let total = employees.reduce((sum, employee) => sum + (parseFloat(employee.salary) || 0), 0);
  $('#employee-total-amount-text').text(total);
}

function renderEmployees() {
  let employeeBody = $('#employee-body');
  employeeBody.empty();
  employees.forEach((employee, index) => {
    employeeBody.append(`
      <tr>
        <td>${employee.name}</td>
        <td>${employee.role}</td>
        <td>${employee.phone}</td>
        <td>${employee.salary} جنيه و ${employee.commission}% من المبيعات</td>
        <td><i class="bx bx-edit edit-employee" data-index="${index}"></i></td>
        <td><i class="bx bx-trash delete-employee" data-index="${index}"></i></td>
      </tr>
    `);
  });
  calculateTotalSalary();
}

function resetForm() {
  $('#employee-name').val('');
  $('#employee-phone').val('');
  $('#employee-role').val('');
  $('#employee-salary').val('');
  $('#employee-commission').val('');
  currentEditIndex = -1;
}

$(document).on('click', '.add-employee-btn', function() {
  let name = $('#employee-name').val();
  let phone = $('#employee-phone').val();
  let role = $('#employee-role').val();
  let salary = $('#employee-salary').val();
  let commission = $('#employee-commission').val();

  if (name.length < 10) {
    Swal.fire({
      text: 'يجب أن يكون اسم الموظف لا يقل عن 10 أحرف.',
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

  if (phone.length !== 11) {
    Swal.fire({
      text: 'يجب أن يكون رقم التليفون 11 رقمًا.',
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

  if (!role) {
    Swal.fire({
      text: 'يجب اختيار الوظيفة.',
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

  if (!salary || !commission) {
    Swal.fire({
      text: 'يجب إدخال كل من الراتب ونسبة المبيعات.',
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

  if (currentEditIndex === -1) {
    employees.push({ name, phone, role, salary, commission });
    Swal.fire({
      text: 'تم إضافة الموظف بنجاح.',
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
    employees[currentEditIndex] = { name, phone, role, salary, commission };
    currentEditIndex = -1;
    Swal.fire({
      text: 'تم تعديل بيانات الموظف بنجاح.',
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

  renderEmployees();
  resetForm();
});

$(document).on('click', '.edit-employee', function() {
  let index = $(this).data('index');
  let employee = employees[index];
  $('#employee-name').val(employee.name);
  $('#employee-phone').val(employee.phone);
  $('#employee-role').val(employee.role);
  $('#employee-salary').val(employee.salary);
  $('#employee-commission').val(employee.commission);
  currentEditIndex = index;
});

$(document).on('click', '.delete-employee', function() {
  let index = $(this).data('index');
  Swal.fire({
    text: 'هل أنت متأكد أنك تريد حذف هذا الموظف؟',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonColor: '#d33',
    cancelButtonColor: '#3085d6',
    confirmButtonText: 'نعم، احذفه!',
    cancelButtonText: 'إلغاء'
  }).then((result) => {
    if (result.isConfirmed) {
      employees.splice(index, 1);
      renderEmployees();
      Swal.fire({
        text: 'تم حذف الموظف بنجاح.',
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

$(document).on('click', '.save-employee-btn', function() {
  localStorage.setItem('employees', JSON.stringify(employees));
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

  $(document).ready(function() {
    let savedEmployees = localStorage.getItem('employees');
    if (savedEmployees) {
      employees = JSON.parse(savedEmployees);
      renderEmployees();
    }

    var today = new Date();
    var currentMonth = today.getMonth() + 1;
    var currentYear = today.getFullYear();

    $('#month-select').val(currentMonth).prop('disabled', true);
    $('#year-select').val(currentYear).prop('disabled', true);

    var isCashRegisterSelected = false;
    var isTableDataDisplayed = false;

    $('#cash-register-options .option').click(function() {
        var selectedCashRegister = $(this).data('value');
        $('#cash-register').val(selectedCashRegister);
        $('#cash-register-options').hide();
        isCashRegisterSelected = true;
    });


    $('.confirm-salary-btn').click(function() {
        if (!isCashRegisterSelected) {
            Swal.fire({
                text: 'يرجى اختيار الخزنة أولاً',
                icon: 'warning',
                confirmButtonText: 'موافق',
                background: '#ffffff',
                iconColor: '#ffc107'
            });
            return;
        }

        var employeeData = [
            { name: 'سامي ابو عبده', job: 'مدير', salary: 1000, incentive: 500, discount: 0, loan: 300 },
            { name: 'مصطفي سامي', job: 'مندوب', salary: 3500, incentive: 300, discount: 0, loan: 200 }
        ];

        var tableRows = '';
        employeeData.forEach(function(employee) {
            var netSalary = employee.salary + employee.incentive - employee.discount - employee.loan;
            tableRows += `<tr>
                <td>${employee.name}</td>
                <td>${employee.job}</td>
                <td><input type="number" class="salary-input" value="${employee.salary}" /></td>
                <td><input type="number" class="incentive-input" value="${employee.incentive}" /></td>
                <td><input type="number" class="discount-input" value="${employee.discount}" /></td>
                <td><input type="number" class="loan-input" value="${employee.loan}" /></td>
                <td class="net-salary">${netSalary}</td>
            </tr>`;
        });

        $('#salary-body').html(tableRows);
        isTableDataDisplayed = true;
    });

    $(document).on('input', '.salary-input, .incentive-input, .discount-input, .loan-input', function() {
        var row = $(this).closest('tr');
        var salary = parseFloat(row.find('.salary-input').val()) || 0;
        var incentive = parseFloat(row.find('.incentive-input').val()) || 0;
        var discount = parseFloat(row.find('.discount-input').val()) || 0;
        var loan = parseFloat(row.find('.loan-input').val()) || 0;
        var netSalary = salary + incentive - discount - loan;
        row.find('.net-salary').text(netSalary);
    });

    $('.save-salary-btn').click(function() {
        if (!isTableDataDisplayed) {
            Swal.fire({
                text: 'يرجى تأكيد البيانات في الجدول أولاً',
                icon: 'warning',
                confirmButtonText: 'موافق',
                background: '#ffffff',
                iconColor: '#ffc107'
            });
            return;
        }

        Swal.fire({
            text: 'تم حفظ الرواتب بنجاح',
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
    $('#transaction-date').val(new Date().toISOString().split('T')[0]).attr('readonly', true);
    $('#transaction-date').attr('readonly', true);

    $('.add-transaction-btn').click(function() {
        let employeeName = $('#employee-name').val();
        let transactionType = $('#transaction-type').val();
        let transactionAmount = $('#transaction-amount').val();
    
        if (employeeName && transactionType && transactionAmount) {
            if (editingRow) {
                editingRow.find('td').eq(0).text(employeeName);
                editingRow.find('td').eq(1).text(transactionType);
                editingRow.find('td').eq(2).text(transactionAmount);
                editingRow.removeClass('editing');
                editingRow = null;
                Swal.fire({
                    text: 'تم تحديث السجل بنجاح.',
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
                let newRow = `
                    <tr>
                        <td>${employeeName}</td>
                        <td>${transactionType}</td>
                        <td>${transactionAmount}</td>
                        <td><i class="bx bx-edit edit-transaction"></i></td>
                        <td><i class="bx bx-trash delete-transaction"></i></td>
                    </tr>`;
    
                $('#transaction-body').append(newRow);
    
                Swal.fire({
                    text: 'تم إضافة السجل بنجاح.',
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
    
            // Clear fields
            $('#employee-name').val('');
            $('#transaction-type').val('');
            $('#transaction-amount').val('');
        } else {
            Swal.fire({
                text: 'يرجى ملء جميع الحقول قبل الإضافة.',
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
    });
    
    $(document).on('click', '.delete-transaction', function() {
        let row = $(this).closest('tr');
        Swal.fire({
            text: 'هل أنت متأكد أنك تريد حذف هذا السجل؟',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم، احذفه!',
            cancelButtonText: 'إلغاء'
        }).then((result) => {
            if (result.isConfirmed) {
                row.remove();
                Swal.fire({
                    text: 'تم حذف السجل بنجاح.',
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
    
    let editingRow = null;
    
    $(document).on('click', '.edit-transaction', function() {
        editingRow = $(this).closest('tr');
        let employeeName = editingRow.find('td').eq(0).text();
        let transactionType = editingRow.find('td').eq(1).text();
        let transactionAmount = editingRow.find('td').eq(2).text();
    
        $('#employee-name').val(employeeName);
        $('#transaction-type').val(transactionType);
        $('#transaction-amount').val(transactionAmount);
    
        editingRow.addClass('editing');
    });
    
    $('.save-transaction-btn').click(function() {
        let rowCount = $('#transaction-body tr').length;
    
        if (rowCount > 0) {
            Swal.fire({
                text: 'تم حفظ البيانات بنجاح!',
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
    
            $('#transaction-body').empty();
        } else {
            Swal.fire({
                text: 'الجدول فارغ، لا توجد بيانات لحفظها.',
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
    });    
    });