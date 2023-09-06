// Write your JavaScript code.

function setCountryCode(event) {
    if (event.value.length > 0 && event.value.charAt(0) != '+')
        event.value = "+" + event.value;
    if (event.value.length == 5)
        event.value = event.value.slice(0, -1);
}

function AvoidSpace(event) {
    var k = event ? event.which : window.event.keyCode;
    if (k == 32) return false;
}


function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode != 43 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

//function ShowPassword(ctrl) {
//    var pass = $(ctrl).closest("div").find("input")[0];
//    var eye = $(ctrl).find("span");
//    pass.type = "text";
//    eye.removeClass('fa fa-eye');
//    eye.addClass('fa fa-eye-slash');
//}

//function HidePassword(ctrl) {
//    var pass = $(ctrl).closest("div").find("input")[0];
//    var eye = $(ctrl).find("span");
//    pass.type = "password";
//    eye.removeClass('fa fa-eye-slash');
//    eye.addClass('fa fa-eye');
//}

function RemoveSpace(ctrl) {
    setTimeout(function (e) {
        var oldtext = $(ctrl).val();
        var newtext = oldtext.replace(/ /g, '');
        $(ctrl).val(newtext);
    }, 1000);
}
