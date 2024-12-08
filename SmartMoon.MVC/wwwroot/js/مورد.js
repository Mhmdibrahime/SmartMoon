$(document).ready(function() {
    $('.error').hide();
  
    function validateField(fieldId, errorId, validationFn, errorMessages) {
        var value = $(fieldId).val().trim();
        var validationResult = validationFn(value);
        if (validationResult === true) {
            $(errorId).text('').hide();
            return true;
        } else {
            $(errorId).text(errorMessages[validationResult] || 'خطأ غير محدد').show(); 
            return false;
        }
    }

    $('#first-representative-phone').on('input', function() {
        var value = $(this).val();
        var numericValue = value.replace(/[^\d]/g, '');
        $(this).val(numericValue);
    });
  
    $('.add-supplier-form').on('submit', function(event) {
        event.preventDefault();
  
        var isValid = true;
  
        var nameValidation = value => {
            if (value.length === 0) return 'required';
            if (value.length < 3) return 'too_short';
            return true;
        };
        
        var addressValidation = value => {
            if (value.length === 0) return 'required';
            if (value.length < 5) return 'too_short';
            return true;
        };
        
        var phoneValidation = value => {
            if (value.length === 0) return 'required';
            if (!/^\d+$/.test(value)) return 'invalid_format';
            return true;
        };
  
        var errorMessages = {
            'required': 'هذا الحقل مطلوب',
            'too_short': 'يجب أن يحتوي النص على عدد الأحرف المطلوب',
            'invalid_format': 'التنسيق غير صحيح'
        };
  
        isValid &= validateField('#supplier-name', '#error-supplier-name', nameValidation, errorMessages);
        isValid &= validateField('#supplier-address', '#error-supplier-address', addressValidation, errorMessages);
        isValid &= validateField('#first-representative-name', '#error-first-representative-name', nameValidation, errorMessages);
        isValid &= validateField('#first-representative-phone', '#error-first-representative-phone', phoneValidation, errorMessages);
  
        if (isValid) {
            Swal.fire({
                icon: 'success',
                title: 'تمت إضافة المورد بنجاح',
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                $('.add-supplier-form')[0].reset();
                $('.error').hide();
            });
        }
    });
  });
  