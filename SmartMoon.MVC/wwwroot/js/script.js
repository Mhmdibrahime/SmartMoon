$(window).on('load', function() {
    $('#loading-screen').fadeOut(1000, function() {
      $(this).remove();
    });
  });
 $(document).ready(function() {
    $('.add-price-btn').on('click', function() {
      const item = $('#item').val();
      let quantity = parseFloat($('#quantity').val());
      let price = parseFloat($('#price').val());
      const store = $('#store').val();
  
      if (item === '' || isNaN(quantity) || isNaN(price) || store === '') {
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
        
        return;
      }
  
      if (quantity <= 0 || price <= 0) {
        Swal.fire({
          text: 'القيم المدخلة للكمية أو السعر يجب أن تكون أكبر من الصفر.', 
          icon: 'error', 
          showConfirmButton: false, 
          timer: 3000,
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
  
      const totalPrice = quantity * price;
      $('#invoice-body').append(`
        <tr>
          <td>${item}</td>
          <td>${quantity}</td>
          <td>${price.toFixed(2)}</td>
          <td>${totalPrice.toFixed(2)}</td>
          <td>${store}</td>
          <td><i class='bx bxs-edit edit-btn'></i></td>
          <td><i class='bx bxs-trash delete-btn'></i></td>
        </tr>
      `);
  
      updateTotal();
      resetForm();
    });
  
    $('#invoice-body').on('click', '.delete-btn', function() {
      $(this).closest('tr').remove();
      updateTotal();
    });
  
    $('#invoice-body').on('click', '.edit-btn', function() {
      const $row = $(this).closest('tr');
      const item = $row.find('td').eq(0).text();
      const quantity = $row.find('td').eq(1).text();
      const price = $row.find('td').eq(2).text();
      const store = $row.find('td').eq(4).text();
  
      $('#item').val(item);
      $('#quantity').val(quantity);
      $('#price').val(price);
      $('#store').val(store);
  
      $row.remove();
      updateTotal();
    });

    function updateTotal() {
      let total = 0;
      $('#invoice-body tr').each(function() {
        const rowTotal = parseFloat($(this).find('td').eq(3).text());
        total += rowTotal;
      });
  
      const discount = parseFloat($('#discount').val()) || 0;
      const discountType = $('#discount-type').val();
      const totalDiscount = discountType === 'نسبة' ? (total * discount / 100) : discount;
      const finalTotal = total - totalDiscount;
  
      $('#total-amount-text').text(finalTotal.toFixed(2));
  
      const paymentMethod = $('#payment-method').val();
      const cashAmount = parseFloat($('#cash-amount').val()) || 0;
      let remaining = 0;
  
      if (paymentMethod === 'كاش') {
        remaining = 0;
      } else if (paymentMethod === 'آجل') {
        if (cashAmount < 0 || cashAmount > finalTotal) {
          Swal.fire({
            text: 'المبلغ النقدي يجب أن يكون ضمن حدود الفاتورة.',
            icon: 'error',
            showConfirmButton: false, 
            timer: 3000,
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
          
          $('#cash-amount').val('');
          return;
        }
        remaining = finalTotal - cashAmount;
      }
  
      $('#remaining-amount-text').text(remaining.toFixed(2));
    }
  
    function resetForm() {
      $('#item').val('');
      $('#quantity').val('');
      $('#price').val('');
      $('#store').val('');
    }

    $('#payment-method').on('change', updateTotal);
    $('#cash-amount').on('input', updateTotal);
    $('#discount, #discount-type').on('input change', updateTotal);
  
    $('.save-btn').on('click', function() {
      Swal.fire({
        icon: 'info',
        title: 'حفظ الفاتورة',
        text: 'حفظ الفاتورة غير مفعل بعد.',
      });
    });
  
    $('.print-btn').on('click', function() {
      const invoiceNumber = '001'; 
      const currentDate = new Date().toLocaleDateString('ar-EG');
      const clientName = $('#client').val() || 'عميل غير محدد';

      $('.invoice-container .header .details p span').eq(0).text(invoiceNumber);
      $('.invoice-container .header .details p span').eq(1).text(currentDate);
      $('.invoice-container .header .details p span').eq(2).text(clientName);
    
      const invoiceRows = $('#invoice-body tr').clone().each(function() {
        $(this).find('td:eq(5), td:eq(6)').remove(); 
      });
    
      $('.invoice-container .invoice-table tbody').empty().append(invoiceRows);
    
      const totalAmount = $('#total-amount-text').text();
      $('.invoice-container .totals tr').eq(0).find('td').text(totalAmount + ' جنيه');
    
      const cashAmount = $('#cash-amount').val() || '0';
      $('.invoice-container .totals tr').eq(1).find('td').text(cashAmount + ' جنيه');
    
      const remainingAmount = $('#remaining-amount-text').text();
      $('.invoice-container .totals tr').eq(2).find('td').text(remainingAmount + ' جنيه');
    
      window.print();
    });
    
  
  
    const $cardTitle = $('.card-title');
    const $cardDate = $('.card-date');
    const $cardPrice = $('.card-price');
  
    $('#item').on('input', function() {
      $cardTitle.text($(this).val());
    });
  
    $('#quantity').on('input', function() {
      $cardDate.text($(this).val());
    });
  
    $('#price').on('input', function() {
      const priceValue = parseFloat($(this).val()) || 0;
      const quantityValue = parseFloat($('#quantity').val()) || 1;
      const total = priceValue * quantityValue;
      $cardPrice.text(total.toFixed(2));
    });
  
    const $sidebar = $('.sidebar');
    const $closeBtn = $('#btn');
    const $searchBtn = $('.bx-search');
    const $dropdownIcons = $('.dropdown-icon');
  
    $closeBtn.on('click', function() {
      $sidebar.toggleClass('open');
      menuBtnChange();
    });
  
    $searchBtn.on('click', function() {
      $sidebar.toggleClass('open');
      menuBtnChange();
    });
  
    $dropdownIcons.on('click', function() {
      const $subMenu = $(this).parent().next('.sub-menu');
      $subMenu.toggleClass('show');
      $(this).toggleClass('bx-chevron-up bx-chevron-down');
    });
  
    function menuBtnChange() {
      if ($sidebar.hasClass('open')) {
        $closeBtn.removeClass('bx-menu').addClass('bx-menu-alt-right');
      } else {
        $closeBtn.removeClass('bx-menu-alt-right').addClass('bx-menu');
      }
    }
    $(document).ready(function() {
      $('.custom-select').each(function() {
        var $customSelect = $(this);
        var $input = $customSelect.find('input');
        var $optionsList = $customSelect.find('.options-list');
        var $options = $optionsList.find('.option');
    
        $input.on('click', function() {
          $optionsList.show();
        });
    
        $(document).on('click', function(e) {
          if (!$customSelect.is(e.target) && $customSelect.has(e.target).length === 0) {
            $optionsList.hide();
          }
        });
        $input.on('input', function() {
          var filter = $input.val().toLowerCase();
          $options.each(function() {
            var text = $(this).text().toLowerCase();
            $(this).toggle(text.includes(filter));
          });
        });
    
        $options.on('click', function() {
          var value = $(this).data('value');
          $input.val(value);
          $optionsList.hide();
        });
      });
    });
    $("#card1").click(function(){
      window.location.href = "عمليه بيع جديده .html";
    });       
    $("#card2").click(function(){
      window.location.href = "إضافة مشتريات.html";
    });       
    $("#card3").click(function(){
      window.location.href = "فاتورة مرتجع مشتريات.html";
    });       
    $("#card4").click(function(){
      window.location.href = "فاتورة مرتجع مبيعات.html";
    });       
    $("#card5").click(function(){
      window.location.href = "عميل جديد.html";
    });       
    $("#add-client").click(function(){
      window.location.href = "عميل جديد.html";
    });       
    $("#card6").click(function(){
      window.location.href = "مورد جديد.html";
    });       
    $("#add-custom").click(function(){
      window.location.href = "مورد جديد.html";
    });       
    $("#card7").click(function(){
      window.location.href = "عرض الاصناف.html";
    });       
    $("#card8").click(function(){
      window.location.href = "تحويل بين الخزن.html";
    });       
    $("#card9").click(function(){
      window.location.href = "سجـل مصــــروف.html";
    });       
    $("#card10").click(function(){
      window.location.href = "تحويل بين المخازن .html";
    });       
    $(".uniqe").click(function(){
      window.location.href = "درج.html";
    });       
    $("#add-item").click(function(){
      window.location.href = "عرض الاصناف.html";
    });       
    $("#log_out").click(function(){
      window.location.href = "index.html";
    });       
  });