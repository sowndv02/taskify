'use strict';
var urlPrefix = window.location.pathname.split('/')[1];
$(document).ready(function () {
    $('.js-example-basic-multiple,#plan_id , #filter_plans ,#filter_by_users , #user_id').select2();
});
$(document).on('click', '.delete', function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    var type = $(this).data('type');
    var reload = $(this).data('reload'); // Get the value of data-reload attribute
    if (typeof reload !== 'undefined' && reload === true) {
        reload = true;
    } else {
        reload = false;
    }
    console.log(reload);
    console.log(type);
    
    // return;
    var tableID = $(this).data('table') || 'table';
    var destroy = type == 'users' ? 'delete_user' : (type == 'contract-type' ? 'delete-contract-type' : (type == 'project-media' || type == 'task-media' ? 'delete-media' : (type == 'expense-type' ? 'delete-expense-type' : (type == 'milestone' ? 'delete-milestone' : 'destroy'))));
    type = type == 'contract-type' ? 'contracts' : (type == 'project-media' ? 'projects' : (type == 'task-media' ? 'tasks' : (type == 'expense-type' ? 'expenses' : (type == 'milestone' ? 'projects' : type))));
    var urlPrefix = window.location.pathname.split('/')[1];
    console.log(urlPrefix);
    $('#deleteModal').modal('show'); // show the confirmation modal
    $('#deleteModal').off('click', '#confirmDelete');
    $('#deleteModal').on('click', '#confirmDelete', function (e) {
        $('#confirmDelete').html(label_please_wait).attr('disabled', true);
        $.ajax({
            url: '/' + urlPrefix + '/Delete' + '/' + id,
            type: 'DELETE',
            headers: {
            },
            success: function (response) {
                console.log(response);
                $('#confirmDelete').html(label_yes).attr('disabled', false);
                $('#deleteModal').modal('hide');
                if (response.error == false) {
                    toastr.success(response.message);
                    location.reload();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (data) {
                $('#confirmDelete').html(label_yes).attr('disabled', false);
                $('#deleteModal').modal('hide');
                toastr.error(label_something_went_wrong);
            }
        });
    });
});
$(document).on('click', '.delete-selected', function (e) {
    e.preventDefault();
    var $this = $(this);
    console.log($this);
    var table = $(this).data('table');
    var type = $(this).data('type');
    console.log(type);
    console.log(table);
    // return;
    var destroy = type == 'users' ? 'delete_multiple_user' : (type == 'contract-type' ? 'delete-multiple-contract-type' : (type == 'project-media' || type == 'task-media' ? 'delete-multiple-media' : (type == 'expense-type' ? 'delete-multiple-expense-type' : (type == 'milestone' ? 'delete-multiple-milestone' : 'destroy_multiple'))));
    type = type == 'contract-type' ? 'contracts' : (type == 'project-media' ? 'projects' : (type == 'task-media' ? 'tasks' : (type == 'expense-type' ? 'expenses' : (type == 'milestone' ? 'projects' : type))));
    var urlPrefix = window.location.pathname.split('/')[1];
    var selections = $('#' + table).bootstrapTable('getSelections');
    console.log(selections);
    var selectedIds = selections.map(function (row) {
        return row.id; // Replace 'id' with the field containing the unique ID
    });
    if (selectedIds.length > 0) {
        $('#confirmDeleteSelectedModal').modal('show'); // show the confirmation modal
        $('#confirmDeleteSelectedModal').off('click', '#confirmDeleteSelections');
        $('#confirmDeleteSelectedModal').on('click', '#confirmDeleteSelections', function (e) {
            $('#confirmDeleteSelections').html(label_please_wait).attr('disabled', true);
            $.ajax({
                url: '/' + urlPrefix + '/' + type + '/' + destroy + '/',
                type: 'DELETE',
                data: {
                    'ids': selectedIds,
                },
                headers: {
                },
                success: function (response) {
                    $('#confirmDeleteSelections').html(label_yes).attr('disabled', false);
                    $('#confirmDeleteSelectedModal').modal('hide');
                    $('#' + table).bootstrapTable('refresh');
                    if (response.error == false) {
                        toastr.success(response.message);
                    } else {
                        toastr.error(response.message);
                    }
                },
                error: function (data) {
                    $('#confirmDeleteSelections').html(label_yes).attr('disabled', false);
                    $('#confirmDeleteSelectedModal').modal('hide');
                    console.log(data);
                    toastr.error(label_something_went_wrong);
                }
            });
        });
    } else {
        toastr.error(label_please_select_records_to_delete);
    }
});
function update_status(e) {
    var id = e['id'];
    var name = e['name'];
    var status;
    var is_checked = $('input[name=' + name + ']:checked');
    var url = $(e).data('url'); // Access data-url from the element
    if (is_checked.length >= 1) {
        status = 1;
    } else {
        status = 0;
    }
    $.ajax({
        url: url,
        type: 'PUT',
        headers: {
        },
        data: {
            id: id,
            status: status
        },
        success: function (response) {
            if (response.error == false) {
                toastr.success(response.message); // show a success message
                $('#' + id + '_title').toggleClass('striked');
                location.reload();
            } else {
                toastr.error(response.message);
            }
        }
    });
}
$(document).on('click', '.edit-todo', function () {
    var id = $(this).data('id');
    var url = $(this).data('url');
    $('#edit_todo_modal').modal('show');
    $.ajax({
        url: url,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            $('#todo_id').val(response.todo.id)
            $('#todo_title').val(response.todo.title)
            $('#todo_priority').val(response.todo.priority)
            $('#todo_description').val(response.todo.description)
        },
    });
});
$(document).on('click', '.edit-note', function () {
    var id = $(this).data('id');
    var url = $(this).data('url');
    $('#edit_note_modal').modal('show');
    $.ajax({
        url: url,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            console.log(response)
            $('#note_id').val(response.id)
            $('#note_createdDate').val(response.createdDate)
            $('#note_title').val(response.title)
            $('#note_color').val(response.colorId)
            $('#note_description').val(response.description)
        },
    });
});
$(document).on('click', '.edit-status', function () {
    var id = $(this).data('id');
    var routePrefix = $("#table").data('routePrefix');
    $('#edit_status_modal').modal('show');
    $.ajax({
        url: routePrefix + '/get/' + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            console.log(response)
            $('#status_id').val(response.id)
            $('#status_title').val(response.title)
            $('#status_color').val(response.colorId)
            $('#status_description').val(response.description)
        },
    });
});
$(document).on('click', '.edit-tag', function () {
    var id = $(this).data('id');
    var routePrefix = $("#table").data('routePrefix');
    $('#edit_tag_modal').modal('show');
    $.ajax({
        url: routePrefix + '/get/' + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            console.log(response)
            $('#tag_id').val(response.id)
            $('#tag_title').val(response.title)
            $('#tag_color').val(response.colorId)
            $('#tag_description').val(response.description)
        },
    });
});
$(document).on('click', '.edit-leave-request', function () {
    var id = $(this).data('id');
    var routePrefix = $('#lr_table').data('routePrefix');
    $('#edit_leave_request_modal').modal('show');
    $.ajax({
        url: routePrefix + '/leave-requests/get/' + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            $('#lr_id').val(response.lr.id);
            $("input[name=status][value=" + response.lr.status + "]").prop('checked', true);
        }
    });
});
$(document).on('click', '.edit-contract-type', function () {
    var routePrefix = $('#table').data('routePrefix');
    var id = $(this).data('id');
    $('#edit_contract_type_modal').modal('show');
    $.ajax({
        url: '' + routePrefix + '/contracts/get-contract-type/' + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            $('#update_contract_type_id').val(response.ct.id);
            $('#contract_type').val(response.ct.type);
        }
    });
});
$(document).on('click', '.edit-contract', function () {
    var id = $(this).data('id');
    var routePrefix = $('#contracts_table').data('routePrefix');
    console.log(routePrefix);
    $('#edit_contract_modal').modal('show');
    $.ajax({
        url: routePrefix + "/contracts/get/" + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            if (response.error == false) {
                var formattedStartDate = moment(response.contract.start_date).format(js_date_format);
                var formattedEndDate = moment(response.contract.end_date).format(js_date_format);
                $('#contract_id').val(response.contract.id);
                $('#title').val(response.contract.title);
                $('#value').val(response.contract.value);
                $('#client_id').val(response.contract.client_id);
                $('#project_id').val(response.contract.project_id);
                $('#contract_type_id').val(response.contract.contract_type_id);
                $('#update_contract_description').val(response.contract.description);
                $('#update_start_date').val(formattedStartDate);
                $('#update_end_date').val(formattedEndDate);
                initializeDateRangePicker('#update_start_date, #update_end_date');
            } else {
                location.reload();
            }
        }
    });
});
function initializeDateRangePicker(inputSelector) {
    $(inputSelector).daterangepicker({
        alwaysShowCalendars: true,
        showCustomRangeLabel: true,
        minDate: moment($(inputSelector).val(), js_date_format),
        singleDatePicker: true,
        showDropdowns: true,
        autoUpdateInput: true,
        locale: {
            cancelLabel: 'Clear',
            format: js_date_format
        }
    });
}
$(document).on('click', '#set-as-default', function (e) {
    e.preventDefault();
    var lang = $(this).data('lang');
    var url = $(this).data('url');
    $('#default_language_modal').modal('show'); // show the confirmation modal
    $('#default_language_modal').on('click', '#confirm', function () {
        $.ajax({
            url: url,
            type: 'PUT',
            headers: {
            },
            data: {
                lang: lang
            },
            success: function (response) {
                if (response.error == false) {
                    location.reload();
                } else {
                    toastr.error(response.message);
                }
            }
        });
    });
});
$(document).on('click', '#remove-participant', function (e) {
    e.preventDefault();
    var routePrefix = $(this).data('routePrefix');
    $('#leaveWorkspaceModal').modal('show'); // show the confirmation modal
    $('#leaveWorkspaceModal').on('click', '#confirm', function () {
        $.ajax({
            url: routePrefix + '/workspaces/remove_participant',
            type: 'GET',
            headers: {
            },
            success: function (response) {
                location.reload();
            },
            error: function (data) {
                location.reload();
            }
        });
    });
});
$(document).ready(function () {
    // Define the IDs you want to process
    var idsToProcess = ['#start_date', '#end_date'];
    // Loop through the IDs
    for (var i = 0; i < idsToProcess.length; i++) {
        var id = idsToProcess[i];
        if ($(id).length) {
            if ($(id).val() == '') {
                $(id).val(moment(new Date()).format(js_date_format));
            }
            $(id).daterangepicker({
                alwaysShowCalendars: true,
                showCustomRangeLabel: true,
                minDate: moment($(id).val(), js_date_format),
                singleDatePicker: true,
                showDropdowns: true,
                autoUpdateInput: true,
                locale: {
                    cancelLabel: 'Clear',
                    format: js_date_format
                }
            });
        }
    }
    // Define the IDs you want to process
    var idsToProcess = ['#payment_date', '#dob', '#doj'];
    // Loop through the IDs
    for (var i = 0; i < idsToProcess.length; i++) {
        var id = idsToProcess[i];
        if ($(id).length) {
            $(id).daterangepicker({
                alwaysShowCalendars: true,
                showCustomRangeLabel: true,
                singleDatePicker: true,
                showDropdowns: true,
                autoUpdateInput: false,
                minDate: '01/01/1950',
                locale: {
                    cancelLabel: 'Clear',
                    format: js_date_format
                }
            });
            $(id).on('apply.daterangepicker', function (ev, picker) {
                // Update the input with the selected date
                $(this).val(picker.startDate.format(js_date_format));
            });
        }
    }
});
if ($("#total_days").length) {
    $('#end_date').on('apply.daterangepicker', function (ev, picker) {
        // Calculate the inclusive difference in days between start_date and end_date
        var start_date = moment($('#start_date').val(), js_date_format);
        var end_date = picker.startDate;
        var total_days = end_date.diff(start_date, 'days') + 1;
        // Display the total_days in the total_days input field
        $('#total_days').val(total_days);
    });
}
$(document).ready(function () {
    $('#project_start_date_between,#project_end_date_between,#task_start_date_between,#task_end_date_between,#lr_start_date_between,#lr_end_date_between,#contract_start_date_between,#contract_end_date_between,#timesheet_start_date_between,#timesheet_end_date_between,#meeting_start_date_between,#meeting_end_date_between,#activity_log_between_date,#start_date_between,#end_date_between').daterangepicker({
        alwaysShowCalendars: true,
        showCustomRangeLabel: true,
        singleDatePicker: false,
        showDropdowns: true,
        autoUpdateInput: false,
        locale: {
            cancelLabel: 'Clear',
            format: js_date_format
        },
    });
    $('#project_start_date_between,#project_end_date_between,#task_start_date_between,#task_end_date_between,#lr_start_date_between,#lr_end_date_between,#contract_start_date_between,#contract_end_date_between,#timesheet_start_date_between,#timesheet_end_date_between,#meeting_start_date_between,#meeting_end_date_between,#activity_log_between_date,#start_date_between,#end_date_between').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format(js_date_format) + ' To ' + picker.endDate.format(js_date_format));
    });
});
if ($("#project_start_date_between").length) {
    $('#project_start_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#project_start_date_from').val(startDate);
        $('#project_start_date_to').val(endDate);
        $('#projects_table').bootstrapTable('refresh');
    });
    $('#project_start_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#project_start_date_from').val('');
        $('#project_start_date_to').val('');
        $('#projects_table').bootstrapTable('refresh');
        $('#project_start_date_between').val('');
    });
    $('#project_end_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#project_end_date_from').val(startDate);
        $('#project_end_date_to').val(endDate);
        $('#projects_table').bootstrapTable('refresh');
    });
    $('#project_end_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#project_end_date_from').val('');
        $('#project_end_date_to').val('');
        $('#projects_table').bootstrapTable('refresh');
        $('#project_end_date_between').val('');
    });
}
if ($("#task_start_date_between").length) {
    $('#task_start_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#task_start_date_from').val(startDate);
        $('#task_start_date_to').val(endDate);
        $('#task_table').bootstrapTable('refresh');
    });
    $('#task_start_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#task_start_date_from').val('');
        $('#task_start_date_to').val('');
        $('#task_table').bootstrapTable('refresh');
        $('#task_start_date_between').val('');
    });
    $('#task_end_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#task_end_date_from').val(startDate);
        $('#task_end_date_to').val(endDate);
        $('#task_table').bootstrapTable('refresh');
    });
    $('#task_end_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#task_end_date_from').val('');
        $('#task_end_date_to').val('');
        $('#task_table').bootstrapTable('refresh');
        $('#task_end_date_between').val('');
    });
}
if ($("#timesheet_start_date_between").length) {
    $('#timesheet_start_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#timesheet_start_date_from').val(startDate);
        $('#timesheet_start_date_to').val(endDate);
        $('#timesheet_table').bootstrapTable('refresh');
    });
    $('#timesheet_start_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#timesheet_start_date_from').val('');
        $('#timesheet_start_date_to').val('');
        $('#timesheet_table').bootstrapTable('refresh');
        $('#timesheet_start_date_between').val('');
    });
    $('#timesheet_end_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#timesheet_end_date_from').val(startDate);
        $('#timesheet_end_date_to').val(endDate);
        $('#timesheet_table').bootstrapTable('refresh');
    });
    $('#timesheet_end_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#timesheet_end_date_from').val('');
        $('#timesheet_end_date_to').val('');
        $('#timesheet_table').bootstrapTable('refresh');
        $('#timesheet_end_date_between').val('');
    });
}
if ($("#meeting_start_date_between").length) {
    $('#meeting_start_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#meeting_start_date_from').val(startDate);
        $('#meeting_start_date_to').val(endDate);
        $('#meetings_table').bootstrapTable('refresh');
    });
    $('#meeting_start_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#meeting_start_date_from').val('');
        $('#meeting_start_date_to').val('');
        $('#meetings_table').bootstrapTable('refresh');
        $('#meeting_start_date_between').val('');
    });
    $('#meeting_end_date_between').on('apply.daterangepicker', function (ev, picker) {
        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');
        $('#meeting_end_date_from').val(startDate);
        $('#meeting_end_date_to').val(endDate);
        $('#meetings_table').bootstrapTable('refresh');
    });
    $('#meeting_end_date_between').on('cancel.daterangepicker', function (ev, picker) {
        $('#meeting_end_date_from').val('');
        $('#meeting_end_date_to').val('');
        $('#meetings_table').bootstrapTable('refresh');
        $('#meeting_end_date_between').val('');
    });
}
$('textarea#footer_text,textarea#contract_description,textarea#update_contract_description , #privacy_policy , #terms_and_conditions , #refund_policy , #company_address').tinymce({
    height: 250,
    menubar: false,
    plugins: [
        'link', 'a11ychecker', 'advlist', 'advcode', 'advtable', 'autolink', 'checklist', 'export',
        'lists', 'link', 'image', 'charmap', 'preview', 'anchor', 'searchreplace', 'visualblocks',
        'powerpaste', 'fullscreen', 'formatpainter', 'insertdatetime', 'media', 'table', 'help', 'wordcount'
    ],
    toolbar: 'link | undo redo | a11ycheck casechange blocks | bold italic backcolor | alignleft aligncenter alignright alignjustify | bullist numlist checklist outdent indent | removeformat | code table help'
});
//$(document).on('submit', '.form-submit-event', function (e) {
//    e.preventDefault();
//    if ($('#net_payable').length > 0) {
//        var net_payable = $('#net_payable').text();
//        $('#net_pay').val(net_payable);
//    }
//    var formData = new FormData(this);
//    var currentForm = $(this);
//    var submit_btn = $(this).find('#submit_btn');
//    var btn_html = submit_btn.html();
//    var btn_val = submit_btn.val();
//    var redirect_url = currentForm.find('input[name="redirect_url"]').val();
//    redirect_url = (typeof redirect_url !== 'undefined' && redirect_url) ? redirect_url : '';
//    var button_text = (btn_html != '' || btn_html != 'undefined') ? btn_html : btn_val;
//    var tableInput = currentForm.find('input[name="table"]');
//    var tableID = tableInput.length ? tableInput.val() : 'table';
//    $.ajax({
//        type: 'POST',
//        url: $(this).attr('action'),
//        data: formData,
//        headers: {
//        },
//        beforeSend: function () {
//            submit_btn.html(label_please_wait);
//            submit_btn.attr('disabled', true);
//        },
//        cache: false,
//        contentType: false,
//        processData: false,
//        dataType: 'json',
//        success: function (result) {
//            submit_btn.html(button_text);
//            submit_btn.attr('disabled', false);
//            if (result['error'] == true) {
//                toastr.error(result['message']);
//            } else {
//                if ($('.empty-state').length > 0) {
//                    window.location.reload();
//                } else {
//                    if (currentForm.find('input[name="dnr"]').length > 0) {
//                        var modalWithClass = $('.modal.fade.show');
//                        // Accessing the ID attribute of the element
//                        var idOfModal = modalWithClass.attr('id');
//                        $('#' + idOfModal).modal('hide');
//                        toastr.success(result['message']);
//                        $('#' + tableID).bootstrapTable('refresh');
//                    } else {
//                        if (result.hasOwnProperty('message')) {
//                            toastr.success(result['message']);
//                            // Show toastr for 3 seconds before reloading or redirecting
//                            setTimeout(function () {
//                                if (redirect_url === '') {
//                                    window.location.reload(); // Reload the current page
//                                } else {
//                                    window.location.href = redirect_url; // Redirect to specified URL
//                                }
//                            }, 3000);
//                        } else {
//                            // No 'message' key, proceed to redirection immediately
//                            if (redirect_url === '') {
//                                window.location.reload(); // Reload the current page
//                            } else {
//                                window.location.href = redirect_url; // Redirect to specified URL
//                            }
//                        }
//                    }
//                }
//            }
//        },
//        error: function (xhr, status, error) {
//            submit_btn.html(button_text);
//            submit_btn.attr('disabled', false);
//            if (xhr.status === 422) {
//                // Handle validation errors here
//                var response = xhr.responseJSON; // Assuming you're returning JSON
//                // You can access validation errors from the response object
//                var errors = response.errors;
//                for (var key in errors) {
//                    if (errors.hasOwnProperty(key) && Array.isArray(errors[key])) {
//                        errors[key].forEach(function (error) {
//                            toastr.error(error);
//                        });
//                    }
//                }
//                // Example: Display the first validation error message
//                toastr.error(label_please_correct_errors);
//                // Assuming you have a list of all input fields with error messages
//                var inputFields = currentForm.find('input[name], select[name], textarea[name]');
//                inputFields = $(inputFields.toArray().reverse());
//                // Iterate through all input fields
//                inputFields.each(function () {
//                    var inputField = $(this);
//                    var fieldName = inputField.attr('name');
//                    if (inputField.attr('type') !== 'radio') {
//                        var errorMessageElement = inputField.next('.error-message');
//                        if (errorMessageElement.length === 0) {
//                            errorMessageElement = inputField.parent().nextAll('.error-message').first();
//                        }
//                        if (errorMessageElement.length === 0) {
//                            // If it doesn't exist, create and append it
//                            errorMessageElement = $('<p class="text-danger text-xs mt-1 error-message"></p>');
//                            inputField.after(errorMessageElement);
//                        }
//                    }
//                    if (errors && errors[fieldName]) {
//                        // If there is a validation error message for this field, display it
//                        if (errorMessageElement && errorMessageElement.length > 0) {
//                            if (errors[fieldName][0].includes('required')) {
//                                errorMessageElement.text('This field is required');
//                            } else {
//                                errorMessageElement.text(errors[fieldName]);
//                            }
//                            inputField[0].scrollIntoView({ behavior: "smooth", block: "start" });
//                            inputField.focus();
//                        }
//                    } else {
//                        // If there is no validation error message, clear the existing message
//                        if (errorMessageElement && errorMessageElement.length > 0) {
//                            errorMessageElement.text('');
//                        }
//                    }
//                });
//            } else {
//                // Handle other errors (non-validation errors) here
//                toastr.error(error);
//            }
//        }
//    });
//});
// Click event handler for the favorite icon
$(document).on('click', '.favorite-icon', function () {
    var icon = $(this);
    var routePrefix = $(this).data('routePrefix');
    var projectId = $(this).data('id');
    var isFavorite = icon.attr('data-favorite');
    isFavorite = isFavorite == 1 ? 0 : 1;
    var reload = $(this).data("require_reload") !== undefined ? 1 : 0;
    var dataTitle = icon.data('bs-original-title');
    var temp = dataTitle !== undefined ? "data-bs-original-title" : "title";
    // Send an AJAX request to update the favorite status
    $.ajax({
        url: routePrefix + '/projects/update-favorite/' + projectId,
        type: 'POST',
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        },
        data: {
            is_favorite: isFavorite
        },
        success: function (response) {
            if (reload) {
                location.reload();
            } else {
                icon.attr('data-favorite', isFavorite);
                // Update the tooltip text
                if (isFavorite == 0) {
                    icon.removeClass("bxs-star");
                    icon.addClass("bx-star");
                    icon.attr(temp, add_favorite); // Update the tooltip text
                    toastr.success(label_project_removed_from_favorite_successfully);
                } else {
                    icon.removeClass("bx-star");
                    icon.addClass("bxs-star");
                    icon.attr(temp, remove_favorite); // Update the tooltip text
                    toastr.success(label_project_marked_as_favorite_successfully);
                }
            }
        },
        error: function (data) {
            // Handle errors if necessary
            toastr.error(error);
        }
    });
});
$(document).on('click', '.duplicate', function (e) {
    e.preventDefault();
    var urlPrefix = window.location.pathname.split('/')[1];
    var id = $(this).data('id');
    var type = $(this).data('type');
    var reload = $(this).data('reload'); // Get the value of data-reload attribute
    if (typeof reload !== 'undefined' && reload === true) {
        reload = true;
    } else {
        reload = false;
    }
    var tableID = $(this).data('table');
    $('#duplicateModal').modal('show'); // show the confirmation modal
    $('#duplicateModal').off('click', '#confirmDuplicate');
    $('#duplicateModal').on('click', '#confirmDuplicate', function (e) {
        e.preventDefault();
        $('#confirmDuplicate').html(label_please_wait).attr('disabled', true);
        $.ajax({
            url: '/' + urlPrefix + '/' + type + '/duplicate/' + id + '?reload=' + reload,
            type: 'GET',
            headers: {
                'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
            },
            success: function (response) {
                $('#confirmDuplicate').html(label_yes).attr('disabled', false);
                $('#duplicateModal').modal('hide');
                if (response.error == false) {
                    if (reload) {
                        location.reload();
                    } else {
                        toastr.success(response.message);
                        if (tableID) {
                            $('#' + tableID).bootstrapTable('refresh');
                        }
                    }
                } else {
                    console.log(response);
                    toastr.error(response.message);
                }
            },
            error: function (data) {
                $('#confirmDuplicate').html(label_yes).attr('disabled', false);
                $('#duplicateModal').modal('hide');
                if (data.responseJSON && data.responseJSON.error) {
                    var errors = data.responseJSON.error;
                    if (Array.isArray(errors)) {
                        errors.forEach(function (error) {
                            if (typeof error === 'object' && error.message) {
                                toastr.error(error.message);
                            } else if (typeof error === 'string') {
                                toastr.error(error);
                            }
                        });
                    } else if (typeof errors === 'string') {
                        toastr.error(errors);
                    } else if (typeof errors === 'object') {
                        toastr.error(errors.message || JSON.stringify(errors));
                    }
                } else if (data.statusText) {
                    toastr.error(data.statusText);
                } else {
                    toastr.error(label_something_went_wrong);
                }
            }
        });
    });
});
$('#deduction_type').on('change', function (e) {
    if ($('#deduction_type').val() == 'amount') {
        $('#amount_div').removeClass('d-none');
        $('#percentage_div').addClass('d-none');
    } else if ($('#deduction_type').val() == 'percentage') {
        $('#amount_div').addClass('d-none');
        $('#percentage_div').removeClass('d-none');
    } else {
        $('#amount_div').addClass('d-none');
        $('#percentage_div').addClass('d-none');
    }
});
$('#update_deduction_type').on('change', function (e) {
    if ($('#update_deduction_type').val() == 'amount') {
        $('#update_amount_div').removeClass('d-none');
        $('#update_percentage_div').addClass('d-none');
    } else if ($('#update_deduction_type').val() == 'percentage') {
        $('#update_amount_div').addClass('d-none');
        $('#update_percentage_div').removeClass('d-none');
    } else {
        $('#update_amount_div').addClass('d-none');
        $('#update_percentage_div').addClass('d-none');
    }
});
if (document.getElementById("system-update-dropzone")) {
    var is_error = false;
    if (!$("#system-update").hasClass("dropzone")) {
        var systemDropzone = new Dropzone("#system-update-dropzone", {
            url: $("#system-update").attr("action"),
            paramName: "update_file",
            autoProcessQueue: false,
            parallelUploads: 1,
            maxFiles: 1,
            acceptedFiles: ".zip",
            timeout: 360000,
            autoDiscover: false,
            headers: {
            },
            addRemoveLinks: true,
            dictRemoveFile: "x",
            dictMaxFilesExceeded: "Only 1 file can be uploaded at a time",
            dictResponseError: "Error",
            uploadMultiple: true,
            dictDefaultMessage: '<p><input type="button" value="Select Files" class="btn btn-primary" /><br> or <br> Drag & Drop System Update / Installable / Plugin\'s .zip file Here</p>',
        });
        systemDropzone.on("addedfile", function (file) {
            var i = 0;
            if (this.files.length) {
                var _i, _len;
                for (_i = 0, _len = this.files.length; _i < _len - 1; _i++) {
                    if (
                        this.files[_i].name === file.name &&
                        this.files[_i].size === file.size &&
                        this.files[_i].lastModifiedDate.toString() ===
                        file.lastModifiedDate.toString()
                    ) {
                        this.removeFile(file);
                        i++;
                    }
                }
            }
        });
        systemDropzone.on("error", function (file, response) {
            console.log(response);
        });
        systemDropzone.on("sending", function (file, xhr, formData) {
            formData.append("flash_message", 1);
            xhr.onreadystatechange = function (response) {
                console.log(response);
                // return;
                setTimeout(function () {
                    location.reload();
                }, 2000);
            };
        });
        $("#system_update_btn").on("click", function (e) {
            e.preventDefault();
            if (is_error == false) {
                if (systemDropzone.files.length === 0) {
                    // Show toast message if no file is selected
                    toastr.error("Please select a file to upload.");
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
                $("#system_update_btn").attr('disabled', true).text(label_please_wait);
                systemDropzone.processQueue();
            }
        });
    }
}
if (document.getElementById("media-upload-dropzone")) {
    var is_error = false;
    var mediaDropzone = new Dropzone("#media-upload-dropzone", {
        url: $("#media-upload").attr("action"),
        paramName: "media_files",
        autoProcessQueue: false,
        timeout: 360000,
        autoDiscover: false,
        headers: {
        },
        addRemoveLinks: true,
        dictRemoveFile: "x",
        dictResponseError: "Error",
        uploadMultiple: true,
        dictDefaultMessage:
            '<p><input type="button" value="Select" class="btn btn-primary" /><br> or <br> Drag & Drop Files Here</p>',
    });
    mediaDropzone.on("addedfile", function (file) {
        var i = 0;
        if (this.files.length) {
            var _i, _len;
            for (_i = 0, _len = this.files.length; _i < _len - 1; _i++) {
                if (
                    this.files[_i].name === file.name &&
                    this.files[_i].size === file.size &&
                    this.files[_i].lastModifiedDate.toString() ===
                    file.lastModifiedDate.toString()
                ) {
                    this.removeFile(file);
                    i++;
                }
            }
        }
    });
    mediaDropzone.on("error", function (file, response) {
        console.log(response);
    });
    mediaDropzone.on("sending", function (file, xhr, formData) {
        var id = $("#media_type_id").val();
        formData.append("flash_message", 1);
        formData.append("id", id);
        xhr.onreadystatechange = function (response) {
            setTimeout(function () {
                location.reload();
            }, 2000);
        };
    });
    $("#upload_media_btn").on("click", function (e) {
        e.preventDefault();
        if (mediaDropzone.getQueuedFiles().length > 0) {
            if (is_error == false) {
                $("#upload_media_btn").attr('disabled', true).text(label_please_wait);
                mediaDropzone.processQueue();
            }
        } else {
            toastr.error('No file(s) chosen.');
        }
    });
}
// Row-wise Select/Deselect All
$('.row-permission-checkbox').change(function () {
    var module = $(this).data('module');
    var isChecked = $(this).prop('checked');
    $(`.permission-checkbox[data-module="${module}"]`).prop('checked', isChecked);
});
$('#selectAllColumnPermissions').change(function () {
    var isChecked = $(this).prop('checked');
    $('.permission-checkbox').prop('checked', isChecked);
    if (isChecked) {
        $('.row-permission-checkbox').prop('checked', true).trigger('change'); // Check all row permissions when select all is checked
    } else {
        $('.row-permission-checkbox').prop('checked', false).trigger('change'); // Uncheck all row permissions when select all is unchecked
    }
    checkAllPermissions(); // Check all permissions
});
// Select/Deselect All for Rows
$('#selectAllPermissions').change(function () {
    var isChecked = $(this).prop('checked');
    $('.row-permission-checkbox').prop('checked', isChecked).trigger('change');
});
// Function to check/uncheck all permissions for a module
function checkModulePermissions(module) {
    var allChecked = true;
    $('.permission-checkbox[data-module="' + module + '"]').each(function () {
        if (!$(this).prop('checked')) {
            allChecked = false;
        }
    });
    $('#selectRow' + module).prop('checked', allChecked);
}
// Function to check if all permissions are checked and select/deselect "Select all" checkbox
function checkAllPermissions() {
    var allPermissionsChecked = true;
    $('.permission-checkbox').each(function () {
        if (!$(this).prop('checked')) {
            allPermissionsChecked = false;
        }
    });
    $('#selectAllColumnPermissions').prop('checked', allPermissionsChecked);
}
// Event handler for individual permission checkboxes
$('.permission-checkbox').on('change', function () {
    var module = $(this).data('module');
    checkModulePermissions(module);
    checkAllPermissions();
});
// Event handler for "Select all" checkbox
$('#selectAllColumnPermissions').on('change', function () {
    var isChecked = $(this).prop('checked');
    $('.permission-checkbox').prop('checked', isChecked);
});
// Initial check for permissions on page load
$('.row-permission-checkbox').each(function () {
    var module = $(this).data('module');
    checkModulePermissions(module);
});
checkAllPermissions();
$(document).ready(function () {
    $('.fixed-table-toolbar').each(function () {
        var $toolbar = $(this);
        var $data_type = $toolbar.closest('.table-responsive').find('#data_type');
        var $data_table = $toolbar.closest('.table-responsive').find('#data_table');
        if ($data_type.length > 0) {
            var data_type = $data_type.val();
            var data_table = $data_table.val() || 'table';
            // Create the "Delete selected" button
            var $deleteButton = $('<div class="columns columns-left btn-group float-left ">' +
                '<button type="button" class="btn btn-outline-danger delete-selected float-left " data-type="' + data_type + '" data-table="' + data_table + '">' +
                '<i class="bx bx-trash"></i>' + lable_delete_select + ' ' +
                '</button>' +
                '</div>');
            // Add the "Delete selected" button before the first element in the toolbar
            $toolbar.prepend($deleteButton);
        }
    });
});
$('#media_storage_type').on('change', function (e) {
    if ($('#media_storage_type').val() == 's3') {
        $('.aws-s3-fields').removeClass('d-none');
    } else {
        $('.aws-s3-fields').addClass('d-none');
    }
});
$(document).on('click', '.edit-milestone', function () {
    var id = $(this).data('id');
    var urlPrefix = window.location.pathname.split('/')[1];
    $.ajax({
        url: '/' + urlPrefix + '/projects/get-milestone/' + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            var formattedStartDate = moment(response.ms.start_date).format(js_date_format);
            var formattedEndDate = moment(response.ms.end_date).format(js_date_format);
            $('#milestone_id').val(response.ms.id)
            $('#milestone_title').val(response.ms.title)
            $('#update_milestone_start_date').val(formattedStartDate)
            $('#update_milestone_end_date').val(formattedEndDate)
            $('#milestone_status').val(response.ms.status)
            $('#milestone_cost').val(response.ms.cost)
            $('#milestone_description').val(response.ms.description)
            $('#milestone_progress').val(response.ms.progress)
            $('.milestone-progress').text(response.ms.progress + '%');
        },
    });
});
// subscriptions start and end date
$(document).ready(function () {
    if (window.location.href.includes('transactions') ||
        window.location.href.includes('plans') ||
        window.location.href.includes('customers')) {
        var deleteBtn = $('.delete-selected');
        // Hide the delete button
        deleteBtn.addClass('d-none');
    }
});
$(document).on('click', '.edit-expense-type', function () {
    var id = $(this).data('id');
    $('#edit_expense_type_modal').modal('show');
    var urlPrefix = window.location.pathname.split('/')[1];
    $.ajax({
        url: '/' + urlPrefix + "/expenses/get-expense-type/" + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            $('#update_expense_type_id').val(response.et.id);
            $('#expense_type_title').val(response.et.title);
            $('#expense_type_description').val(response.et.description);
        }
    });
});
$(document).on('click', '.edit-expense', function () {
    var id = $(this).data('id');
    $('#edit_expense_modal').modal('show');
    var urlPrefix = window.location.pathname.split('/')[1];
    $.ajax({
        url: '/' + urlPrefix + '/expenses/get/' + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            var formattedExpDate = moment(response.exp.expense_date).format(js_date_format);
            var amount = parseFloat(response.exp.amount);
            $('#update_expense_id').val(response.exp.id);
            $('#expense_title').val(response.exp.title);
            $('#expense_type_id').val(response.exp.expense_type_id);
            $('#expense_user_id').val(response.exp.user_id);
            $('#expense_amount').val(amount.toFixed(decimal_points));
            $('#update_expense_date').val(formattedExpDate);
            $('#expense_note').val(response.exp.note);
        }
    });
});
$(document).on('click', '.edit-payment', function () {
    var id = $(this).data('id');
    $('#edit_payment_modal').modal('show');
    var urlPrefix = window.location.pathname.split('/')[1];
    $.ajax({
        url: '/' + urlPrefix + '/payments/get/' + id,
        type: 'get',
        headers: {
        },
        dataType: 'json',
        success: function (response) {
            var formattedExpDate = moment(response.payment.payment_date).format(js_date_format);
            var amount = parseFloat(response.payment.amount);
            $('#update_payment_id').val(response.payment.id);
            $('#payment_user_id').val(response.payment.user_id);
            $('#payment_invoice_id').val(response.payment.invoice_id);
            $('#payment_pm_id').val(response.payment.payment_method_id);
            $('#payment_amount').val(amount.toFixed(decimal_points));
            $('#update_payment_date').val(formattedExpDate);
            $('#payment_note').val(response.payment.note);
        }
    });
});
function initializeDateRangePicker(inputSelector) {
    $(inputSelector).daterangepicker({
        alwaysShowCalendars: true,
        showCustomRangeLabel: true,
        minDate: moment($(inputSelector).val(), js_date_format),
        singleDatePicker: true,
        showDropdowns: true,
        autoUpdateInput: true,
        locale: {
            cancelLabel: 'Clear',
            format: js_date_format
        }
    });
}
$(document).ready(function () {
    $('#togglePassword').on("click", function () {
        var passwordInput = $('#password');
        var toggleButton = $(this);
        // Toggle password visibility
        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            toggleButton.html('<i class="far fa-eye"></i>');
        } else {
            passwordInput.attr('type', 'password');
            toggleButton.html('<i class="far fa-eye-slash"></i>');
        }
    });
});
$(document).on('click', '.superadmin-login', function (e) {
    e.preventDefault();
    $('#email').val('superadmin@gmail.com');
    $('#password').val('12345678');
});
$(document).on('click', '.admin-login', function (e) {
    e.preventDefault();
    $('#email').val('admin@gmail.com');
    $('#password').val('12345678');
});
$(document).on('click', '.member-login', function (e) {
    e.preventDefault();
    $('#email').val('teammember@gmail.com');
    $('#password').val('12345678');
});
$(document).on('click', '.client-login', function (e) {
    e.preventDefault();
    $('#email').val('client@gmail.com');
    $('#password').val('12345678');
});

$('#show_password').on('click', function () {
    var eyeicon = $('#eyeicon');
    let password = document.getElementById("password");
    console.log(password.type);
    if (password.type == "password") {
        password.type = "text";
        eyeicon.removeClass('bx-hide');
        eyeicon.addClass('bx-show');
    }
    else {
        password.type = "password";
        eyeicon.removeClass('bx-show');
        eyeicon.addClass('bx-hide');
    }
});
$('#show_confirm_password').on('click', function () {
    var eyeicon = $('#eyeicon');
    let confirm_password = document.getElementById("password_confirmation");
    console.log(confirm_password.type);
    if (confirm_password.type == "password") {
        confirm_password.type = "text";
        eyeicon.removeClass('bx-hide');
        eyeicon.addClass('bx-show');
    } else {
        confirm_password.type = "password";
        eyeicon.removeClass('bx-show');
        eyeicon.addClass('bx-hide');
    }
});
$('.min_0').on("change", function () {
    var amount = $(this).val();
    if (amount < 0) {
        toastr.error(lable_min_0);
    } else {
        // Clear error message if the value is valid
    }

});
$('.max_100').on("change", function () {
    var percentage = $(this).val();
    if (percentage > 100) {
        toastr.error(lable_max_100);
    } else {
        // Clear error message if the value is valid
    }
});
function clearModalContents($modal) {
    // Clear all input fields
    $modal.find('input:not([type="hidden"])').each(function () {
        if ($(this).attr('type') === 'checkbox' || $(this).attr('type') === 'radio') {
            $(this).prop('checked', false);
        } else {
            $(this).val('');
        }
    });
    // Clear all textarea fields
    $modal.find('textarea').val('');
    // Reset all select elements
    $modal.find('select').prop('selectedIndex', 0);
    // Clear any error messages or validation states
    $modal.find('.error-message').removeClass('.text-danger');
    $modal.find('.error-message').closest('p').text('');
    // If you're using any plugins that need to be reset (like Select2), reset them here
    // For example:
    $modal.find('select').select2('val', '');
}
// Usage for all modals
$(document).on('hidden.bs.modal', '.modal', function () {
    var $modal = $(this);
    if ($modal.attr('id') !== 'timerModal') {
        clearModalContents($modal);
    }
});


     