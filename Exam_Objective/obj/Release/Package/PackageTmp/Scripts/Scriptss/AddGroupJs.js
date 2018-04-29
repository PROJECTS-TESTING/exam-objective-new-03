
/* ------------------Button Save--------------------*/
$('#btnSave').click(function () {
    Data.StudyGroup = $('#StudyGroup').val();
    Data.GroupPW = $('#GroupPW').val();
    if ($("#createGroupForm").valid()) {
        $.ajax({
            type: "POST",
            cache: false,
            url: "CreateGroup",
            dataType: "json",
            data: Data,
            traditional: true,
            beforeSend: function () {

            },

            success: function (res) {
                var status = false;
                var message = 'ดำเนินการไม่สำเร็จ';
                if (res != null && res != undefined) {
                    status = res.status;
                    message = res.message != '' ? res.message : message;
                }
                if (status == false) {
                    swal("เกิดข้อผิดพลาด", message, "error");
                } if (message == 'มีกลุ่มเรียนนี้ในวิชาแล้ว') {
                    swal({
                        title: message,
                        type: "warning",
                        showConfirmButton: false,
                        timer: 1500,
                        closeOnConfirm: false
                    },
                        function () {
                            location.reload(true);
                        });
                } else {
                    swal({
                        title: message,
                        type: "success",
                        showConfirmButton: false,
                        timer: 1000,
                        closeOnConfirm: false
                    },
                        function () {
                            location.reload(true);
                        });
                }
            },
            error: function (xhr, ajaxOption, thrownError) {
                swal("เกิดข้อผิดพลาด", message, "error");
            },
            complete: function () {

            },
        });
    }
});

/* ---------------Button Create------------------ */
$("#btnCrtGroup").click(function () {
    Data.isUpdateable = 0;
    //swal("Good job!", "You clicked the button!", "success");
    $("#title").text("เพิ่มกลุ่มเรียน");
    $("#createGroupForm").find("input").val("");
   // $("#StudyGroup").inputmask("aaa.99999", { placeholder: "XXX.XXXXX" });
    $("[name='StudyGroup']").prop("readonly", false);
    $("[name='GroupPW']").prop("readonly", false);
    $("#btnSave").show();
    $('#randompassword').show();
    var valida = $("#createGroupForm").validate();
    valida.resetForm();
    $("#createGroupForm").find('.form-group').removeClass('has-error');

});

/* ----------------Button Edit ---------------- */
$('.btnEdit').click(function () {
    Data.isUpdateable = 1;
    var id = $(this).data("groupid");
    Data.GroupID = id;
    $("[name='StudyGroup']").prop("readonly", false)
    $("[name='GroupPW']").prop("readonly", false)
    $('#btnSave').show();
    $('#randompassword').show();
    $("#title").text("แก้ไขกลุ่มเรียน");
    //alert(id);
    $.ajax({
        type: "POST",
        cache: false,
        url: "EditGroup",
        dataType: "json",
        data: { id: id },
        traditional: true,
        beforeSend: function () {
        },

        success: function (res) {
            var status = false;
            var message = 'ดำเนินการไม่สำเร็จ';
            if (res != null && res != undefined) {
                status = res.status;
                message = res.message != '' ? res.message : message;
            }
            if (status == false) {
                swal("เกิดข้อผิดพลาด", message, "error");
            } else {
                $("[name='StudyGroup']").val(res.data.StudyGroup);
                $("[name='GroupPW']").val(res.data.GroupPW);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            swal("เกิดข้อผิดพลาด", message, "error");
        },
        complete: function () {

        },
    });
});
/* ---------------------Button View------------------------- */
$('.btnView').click(function () {
    var id = $(this).data("groupid");
    $("#title").text("รายละเอียดกลุ่มเรียน");
    $('#randompassword').hide();
    //alert(id);
    $.ajax({
        type: "POST", // method ที่จะส่ง
        cache: false,
        url: "EditGroup",  // ส่งไปให้ที่ได้ ระบุ
        dataType: "json", // ชนิดข้อมูลที่ส่ง
        data: { id: id },//$('#formSubject').serialize(), // ข้อมูลที่ส่ง
        traditional: true, // การจดจำข้อมูล
        beforeSend: function () { // ก่องส่ง data จะให้ทำไร

        },

        success: function (res) {

            var status = false;
            var message = 'ดำเนินการไม่สำเร็จ';
            if (res != null && res != undefined) {
                status = res.status;
                message = res.message != '' ? res.message : message;
            }
            if (status == false) {
                swal("เกิดข้อผิดพลาด", message, "error");

            } else {
                $("[name='StudyGroup']").val(res.data.StudyGroup);
                $("[name='GroupPW']").val(res.data.GroupPW);
                $("[name='StudyGroup']").prop("readonly", true);
                $("[name='GroupPW']").prop("readonly", true);
                $('#btnSave').hide();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            swal("เกิดข้อผิดพลาด", message, "error");
        },
        complete: function () {

        },
    });
});


/* -----------------Button Delete---------------------*/
$('.btnDelete').click(function () {
    var id = $(this).data("groupid");
    var gname = $(this).data("gname");
    //alert(id);
    swal({
        title: "ต้องการลบกลุ่มเรียน : " + gname + " ?",
        text: "กลุ่มเรียนที่ลบจะไม่สามารถกู้คืนกลับมาได้ !",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "ใช่ ต้องการลบ",
        cancelButtonText: "ยกเลิก !",
        closeOnConfirm: true,
        colseOnCancel: true
    },
        function (isConfirm) {
            if (isConfirm) {
                $.ajax({
                    type: "POST",
                    cache: false,
                    url: "DeleteGroup",
                    dataType: "json",
                    data: { id: id },
                    traditional: true,
                    beforeSend: function () {
                    },
                    success: function (res) {

                        var status = false;
                        var message = 'ดำเนินการไม่สำเร็จ';
                        if (res != null && res != undefined) {
                            status = res.status;
                            message = res.message != '' ? res.message : message;
                        }
                        if (status == false) {
                            swal("เกิดข้อผิดพลาด", message, "error");
                        } else {
                            swal({
                                title: "ลบกลุ่มเรียนสำเร็จ",
                                type: "success",
                                showConfirmButton: false,
                                timer: 1000,
                                closeOnConfirm: false
                            },
                                function () {
                                    location.reload(true);
                                });
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        swal("เกิดข้อผิดพลาด", message, "error");
                    },
                    complete: function () {

                    },
                });
            }
        }


    );

});

//check form
$("#createGroupForm").validate({
    errorElement: 'span', //default input error message container
    errorClass: 'help-block', // default input error message class
    focusInvalid: false, // do not focus the last invalid input
    rules: {
        StudyGroup: {
            required: true
        },
        GroupPW: {
            required: true
        },
    },

    messages: {
        StudyGroup: {
            required: "กรุณาป้อนชื่อกลุ่มเรียน"
        },
        GroupPW: {
            required: "กรุณาป้อนรหัสเข้ากลุ่ม"
        },
    },
    invalidHandler: function (event, validator) { //display error alert on form submit
        $('.alert-danger', $("#createGroupForm")).show();
    },
    highlight: function (element) { // hightlight error inputs
        $(element)
            .closest('.form-group').addClass('has-error'); // set error class to the control group
    },
    unhighlight: function (element) {
        $(element)
            .closest('.form-group')
            .removeClass('has-error');
    },
    success: function (label) {
        label.closest('.form-group').removeClass('has-error');
        label.remove();
    },
    errorPlacement: function (error, element) {
        if (element.parent(".input-group").size() > 0) {
            error.insertAfter(element.parent(".input-group"));
        } else if (element.attr("data-error-container")) {
            error.appendTo(element.attr("data-error-container"));
        } else {
            error.insertAfter(element); // for other inputs, just perform default behavior
        }
    },
    submitHandler: function (form) {
        submit($("#createGroupForm"));
        return false;
        form.submit();
    }
});