function CheckBlockPW(id) {
    var x = document.getElementById(id);
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}
function RandomPass(num, id) {
    var pass = "";
    for (var i = 0; i < num; i++) { pass += Math.floor(Math.random() * 10).toString() }
    $(id).val(pass);
}