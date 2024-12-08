$(document).ready(function () {
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

    $('.add-client-form').on('submit', function (event) {
        var isValid = true;

        var nameValidation = value => {
            if (value.length === 0) return 'required';
            if (value.length < 10) return 'too_short';
            return true;
        };

        var addressValidation = value => {
            if (value.length === 0) return 'required';
            if (value.length < 10) return 'too_short';
            return true;
        };

        var nationalIdValidation = value => {
            if (value.length === 0) return 'required';
            if (!/^\d{14}$/.test(value)) return 'invalid_format';
            return true;
        };

        var phoneValidation = value => {
            if (value.length === 0) return 'required';
            if (!/^\d+$/.test(value)) return 'invalid_format';
            return true;
        };

        var mobileValidation = value => {
            if (value.length === 0) return 'required';
            if (!/^\d+$/.test(value)) return 'invalid_format';
            return true;
        };

        var errorMessages = {
            'required': 'هذا الحقل مطلوب',
            'too_short': 'يجب أن يحتوي النص على 10 أحرف على الأقل',
            'invalid_format': 'التنسيق غير صحيح'
        };

        isValid &= validateField('#client-name', '#error-client-name', nameValidation, errorMessages);
        isValid &= validateField('#client-address', '#error-client-address', addressValidation, errorMessages);
        isValid &= validateField('#client-national-id', '#error-client-national-id', nationalIdValidation, errorMessages);
        isValid &= validateField('#client-phone', '#error-client-phone', phoneValidation, errorMessages);
        isValid &= validateField('#client-mobile', '#error-client-mobile', mobileValidation, errorMessages);

        if (isValid) {
            Swal.fire({
                icon: 'success',
                title: 'تمت إضافة العميل بنجاح',
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                $('.add-client-form')[0].reset();
                $('.error').hide();
            });
        } else {
            event.preventDefault();
        }
    });
});