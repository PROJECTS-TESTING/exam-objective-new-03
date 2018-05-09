
function good() {
    swal("Good job!", "You clicked the button!", "success");
   
} 

function alertSuccess(str) {
    
        swal({
            title: str + "สำเร็จ",
            type: "success",
            showConfirmButton: false,
            timer: 2000,
            closeOnConfirm: false
        },
            function () {
                // location.reload(true);
              //  $('#result').load('_PartialData');
            }
        );
    
}

function alertError(str) {
    swal("เกิดข้อผิดพลาด", str+"", "error"); 
}

