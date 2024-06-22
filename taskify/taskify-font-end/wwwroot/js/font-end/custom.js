$(document).ready(function () {
    var $iconOpen = $(this).find('.collapse-open').hide();
    $('.accordion-button').click(function () {
        var $iconClose = $(this).find('.collapse-close');
        var $iconOpen = $(this).find('.collapse-open');
        console.log($iconOpen);
        console.log($iconClose);
        if ($(this).attr('aria-expanded') === 'true') {
            $iconClose.hide();
            $iconOpen.show();
        } else {
            $iconClose.show();
            $iconOpen.hide();
        }
    });
});
$(document).ready(function () {
    $('#contactUsSubmit').on("click", function () {
        var formData = $('#contact_us_form').serialize();
        // Show loading indicator
        // $('#loading-overlay').fadeIn();
        // Send AJAX request
        $.ajax({
            url: $('#contact_us_form').attr('action'),
            type: 'POST',
            data: formData,
            headers: {
               
            },
            success: function (response) {
                // Hide loading indicator
                // $('#loading-overlay').fadeOut();
                // Handle success response
                toastr.success(response.message);
                // Clear form fields if needed
                $('#contact_us_form')[0].reset();
            },
            error: function (xhr, status, error) {
                // Hide loading indicator
                // $('#loading-overlay').fadeOut();
                // Handle error response
                var errors = xhr.responseJSON.errors;
                $.each(errors, function (key, value) {
                    toastr.error(value);
                });
            }
        });
    });

});


// $(document).ready(function () {
//     $('#togglePassword').on("click", function () {
//         var passwordInput = $('#password');
//         var toggleButton = $(this);

//         // Toggle password visibility
//         if (passwordInput.attr('type') === 'password') {
//             passwordInput.attr('type', 'text');
//             toggleButton.html('<i class="far fa-eye"></i>');
//         } else {
//             passwordInput.attr('type', 'password');
//             toggleButton.html('<i class="far fa-eye-slash"></i>');
//         }
//     });
// });

let eyeicon = document.getElementById("eyeicon");
let password = document.getElementById("password");

eyeicon.onclick = function () {
    if (password.type == "password") {
        password.type = "text";
        eyeicon.src = "far fa-eye";
    }
    else {
        password.type = "password";
        eyeicon.src = "far fa-eye-slash";
    }
}



//$(document).ready(function () {
//    $('#loginBtn').on('click', function (e) {
//        e.preventDefault();
//        var formData = $('#formAuthentication').serialize();

//        $.ajax({
//            url: $('#formAuthentication').attr('action'),
//            type: 'POST',
//            data: formData,
//            headers: {
//            },
//            success: function (response) {
//                if (response.error) {
//                    console.log(response);

//                    toastr.error(response.message);

//                }
//                else {

//                    toastr.success(response.message);
//                    window.location.href = response.redirect_url;

//                }


//            },
//            error: function (xhr, status, error) {
//                // Handle error response


//                var errors = xhr.responseJSON.errors;
//                console.log(errors);
//                // Check if there are any validation errors
//                if (errors) {
//                    // Loop through each error and display it using toastr
//                    $.each(errors, function (key, value) {
//                        toastr.error(value);
//                    });
//                } else {
//                    if (xhr.responseJSON.error) {

//                        console.log(xhr.responseJSON);
//                        $.each(xhr.responseJSON.message, function (key, value) {

//                            toastr.error(value);
//                        })

//                    } else {
//                        // If there are no validation errors, display a generic error message
//                        toastr.error('An error occurred. Please try again.');
//                    }
//                }

//            }

//        });
//    });
//});
$(document).ready(function () {
    $('#registerCustomer').on('click', function (e) {
        e.preventDefault();
        var formData = $('#formRegister').serialize();

        $.ajax({
            url: $('#formRegister').attr('action'),
            type: 'POST',
            data: formData,
            dataType: 'json',
            headers: {
               
            },
            success: function (response) {
                console.log(response);

                toastr.success(response.message);
                setTimeout(function () {
                    window.location = response.redirect_url;
                }, 2000);

            },
            error: function (xhr, status, error) {
                // Handle error response


                var errors = xhr.responseJSON.errors;
                console.log(errors);
                // Check if there are any validation errors
                if (errors) {
                    // Loop through each error and display it using toastr
                    $.each(errors, function (key, value) {
                        toastr.error(value);
                    });
                } else {
                    if (xhr.responseJSON.error) {

                        console.log(xhr.responseJSON);
                        $.each(xhr.responseJSON.message, function (key, value) {

                            toastr.error(value);
                        })

                    } else {
                        // If there are no validation errors, display a generic error message
                        toastr.error('An error occurred. Please try again.');
                    }
                }

            }

        });
    });
});

// $(document).on('click', '.superadmin-login', function (e) {
//     e.preventDefault();
//     $('#email').val('superadmin@gmail.com');
//     $('#password').val('12345678');
// });
// $(document).on('click', '.admin-login', function (e) {
//     e.preventDefault();
//     $('#email').val('admin@gmail.com');
//     $('#password').val('12345678');
// });
// $(document).on('click', '.member-login', function (e) {
//     e.preventDefault();
//     $('#email').val('teammember@gmail.com');
//     $('#password').val('12345678');
// });
// $(document).on('click', '.client-login', function (e) {
//     e.preventDefault();
//     $('#email').val('client@gmail.com');
//     $('#password').val('12345678');
// });

if (document.getElementById("state1")) {
    const countUp = new CountUp("state1", document.getElementById("state1").getAttribute("countTo"));
    if (!countUp.error) {
        countUp.start();
    } else {
        console.error(countUp.error);
    }
}
if (document.getElementById("state2")) {
    const countUp1 = new CountUp("state2", document.getElementById("state2").getAttribute("countTo"));
    if (!countUp1.error) {
        countUp1.start();
    } else {
        console.error(countUp1.error);
    }
}
if (document.getElementById("state3")) {
    const countUp2 = new CountUp("state3", document.getElementById("state3").getAttribute("countTo"));
    if (!countUp2.error) {
        countUp2.start();
    } else {
        console.error(countUp2.error);
    };
}

if (document.querySelector('.datepicker-1')) {
    flatpickr('.datepicker-1', {}); // flatpickr
}

if (document.querySelector('.datepicker-2')) {
    flatpickr('.datepicker-2', {}); // flatpickr
}
if (document.getElementById("typed")) {
    var typed = new Typed("#typed", {
        stringsElement: "#typed-strings",
        typeSpeed: 70,
        backSpeed: 50,
        backDelay: 200,
        startDelay: 500,
        loop: true
    });
}