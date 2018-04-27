
/* ------------------Button Save--------------------*/
$('#btnSave').click(function () {
    DataTest.ExamtopicName = $('#TestName').val();
    DataTest.Explantion = $('#Explantion').val();
    DataTest.SubjectID = $('#asubject').data("asubj");
    DataTest.DatetoBegin = $('#DatetoBegin').val();
    DataTest.TimetoBegin = $('#TimetoBegin').val();
    DataTest.Sequences = $('#Sequences').val()
    DataTest.TimetoEnd = $('#TimetoEnd').val();
    DataTest.ExamtopicPW = $('#PwTest').val();
    DataTest.GroupID = $('#Groupstudy').val();
    DataTest.NumberOfTimes = $("[name='NumberOfTimes']").val();
    DataTest.InNetWork = $('#IPsubnet').val();
    //alert($("[name='Explantion']").val());
    if ($("#TForm0").valid()) {
        $.ajax({
            type: "POST",
            cache: false,
            url: "CreateTesting",
            dataType: "json",
            data: DataTest,
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
$('#btnCrtTest').click(function () {
    DataTest.isUpdateable = 0;
    //swal("Good job!", "You clicked the button!", "success");
    $('#title').text("เพิ่มแบบทดสอบ");
    $('#TForm0').find("input").val("");
    tinyMCE.activeEditor.setContent(''); // Reset Text Editor.
    var valida = $('#TForm0').validate();
    valida.resetForm();
    $('#TForm0').find('.form-group').removeClass('has-error');

});

/* ----------------Button Edit ---------------- */
$('.btnEdit').click(function () {
    DataTest.isUpdateable = 1;
    var id = $(this).data("examtopicid");
    DataTest.ExamtopicID = id;
    $("#title").text("แก้ไขแบบทดสอบ");
   // alert(id);
    $.ajax({
        type: "POST",
        cache: false,
        url: "EditExamTopic",
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
                $('#TestName').val(res.data.ExamtopicName);
                $('#Explantion').val(res.data.Explantion);
                var textExplantion = (res.data.Explantion);
                tinyMCE.activeEditor.setContent(textExplantion);
               
                $('#DatetoBegin').html(res.data.DatetoBegin);
                var phtime = res.data.TimetoBegin.Hours;
                var pmtime = res.data.TimetoBegin.Minutes;
                var Sphtime = "";
                if (phtime < "10" && pmtime < "10") Sphtime += "0" + phtime + ":0" + pmtime;
                else if (phtime < "10" && pmtime > "10") Sphtime += "0" + phtime + ":" + pmtime;
                else if (phtime > "10" && pmtime < "10") Sphtime += phtime + ":0" + pmtime;
                else Sphtime += phtime + ":" + pmtime;
                $('#TimetoBegin').val(Sphtime);
                var chtime = res.data.TimetoEnd.Hours;
                var cmtime = res.data.TimetoEnd.Minutes;
                var Schtime = "";
                if (chtime < "10" && cmtime < "10") Schtime += "0" + chtime + ":0" + cmtime;
                else if (chtime < "10" && cmtime > "10") Schtime += "0" + chtime + ":" + cmtime;
                else if (chtime > "10" && cmtime < "10") Schtime += chtime + ":0" + cmtime;
                else Schtime += chtime + ":" + cmtime;
                $('#TimetoEnd').val(Schtime);
                $("[name='Sequences']").val(res.data.Sequences);
                $('#PwTest').val(res.data.ExamtopicPW);
                $('#Groupstudy').val(res.data.GroupID);
                $("[name='NumberOfTimes']").val(res.data.NumberOfTimes);
                $('#IPsubnet').val(res.data.InNetWork);
                // Set Read only
                $('#TestName').prop("readonly", false);
                $('#Explantion').prop("readonly", false);
                $('#PwTest').prop("readonly", false);
                $('NumberOfTimes').prop("readonly", false);
                $('#IPsubnet').prop("readonly", false);
                $('#btnSave').show();
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
    var id = $(this).data("examtopicid");
    $("#title").text("รายละเอียดแบบทดสอบ");
    //alert(id);
    $.ajax({
        type: "POST", // method ที่จะส่ง
        cache: false,
        url: "EditExamtopic",  // ส่งไปให้ที่ได้ ระบุ
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

                $('#TestName').val(res.data.ExamtopicName);
                $('#Explantion').val(res.data.Explantion);
                var textExplantion = (res.data.Explantion);
                tinyMCE.activeEditor.setContent(textExplantion);

                // $('#DatetoBegin').val(res.data.DatetoBegin);
                var tss = res.data.DatetoBegin;
                tss.toString();
                var ss = /\d+/.exec(tss);
                var Datadate = ss[0];
                // alert(ss[0] * -1+"__"+tss);
                var toDay = new Date(parseInt(Datadate));               
                $('#DatetoBegin').val(toDay.getDate() + "-" + (toDay.getMonth() + 1) + "-" + (toDay.getFullYear()));
                var phtime = res.data.TimetoBegin.Hours;
                var pmtime = res.data.TimetoBegin.Minutes;
                var Sphtime = "";
                if (phtime < "10" && pmtime < "10") Sphtime += "0" + phtime + ":0" + pmtime;
                else if (phtime < "10" && pmtime > "10") Sphtime += "0" + phtime + ":" + pmtime;
                else if (phtime > "10" && pmtime < "10") Sphtime += phtime + ":0" + pmtime;
                else Sphtime += phtime + ":" + pmtime;
                $('#TimetoBegin').val(Sphtime);
                var chtime = res.data.TimetoEnd.Hours;
                var cmtime = res.data.TimetoEnd.Minutes;
                var Schtime = "";
                if (chtime < "10" && cmtime < "10") Schtime += "0" + chtime + ":0" + cmtime;
                else if (chtime < "10" && cmtime > "10") Schtime += "0" + chtime + ":" + cmtime;
                else if (chtime > "10" && cmtime < "10") Schtime += chtime + ":0" + cmtime;
                else Schtime += chtime + ":" + cmtime;
                $('#TimetoEnd').val(Schtime);
                $("[name='Sequences']").val(res.data.Sequences);
                $('#PwTest').val(res.data.ExamtopicPW);
                $('#Groupstudy').val(res.data.GroupID);
                $("[name='NumberOfTimes']").val(res.data.NumberOfTimes);
                $('#IPsubnet').val(res.data.InNetWork);
                // Set Read only
                $('#TestName').prop("readonly", true);
                $('#Explantion').prop("readonly", true);
                $('#PwTest').prop("readonly", true);
                $('NumberOfTimes').prop("readonly", true);
                $('#IPsubnet').prop("readonly", true);
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
    var id = $(this).data("examtopicid");
    var Exname = $(this).data("Exname");
    //alert(id);
    swal({
        title: "ต้องการลบ : " + Exname + " ?",
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
                    url: "DeleteExamTopic",
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
$("#TForm0").validate({
    errorElement: 'span', //default input error message container
    errorClass: 'help-block', // default input error message class
    focusInvalid: false, // do not focus the last invalid input
    rules: {
        TestName: {
            required: true
        },
        yearTest: {
            required: true
        },
    },

    messages: {
        TestName: {
            required: "กรุณาป้อนชื่อแบบทดสอบ"
        },
        yearTest: {
            required: "กรุณาป้อนปีการศึกษา"
        },
    },
    invalidHandler: function (event, validator) { //display error alert on form submit
        $('.alert-danger', $("#TForm0")).show();
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
        submit($("#TForm0"));
        return false;
        form.submit();
    }
});

