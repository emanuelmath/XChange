$(document).ready(function () {
    $("#btnRandom").on("click", function (event) {
        $("#contrasenha_r").val(Math.random().toString(36).slice(2).substring(0, 8));
    });
});