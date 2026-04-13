$(document).ready(function () {

    // Manejo de registro de usuarios
    $("#registerForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        const $btn = $(this).find('button[type="submit"]');
        $btn.prop('disabled', true);

        if (!$("#terminos").is(":checked")) {
            toastr.warning("Debes aceptar los términos y condiciones...");
            return;
        }

        let password = $("#Password").val();
        let confirm = $("#ConfirmPassword").val();

        if (password !== confirm) {
            toastr.warning("Las contraseñas no coinciden...");
            $btn.prop('disabled', false);
            return;
        }

        $.ajax({
            url: "/Auth/RegisterUser", 
            type: "POST",
            data: $(this).serialize(), 
            success: function (resp) {
                switch (resp.cod) {
                    case 1:
                        toastr.success(resp.msg);
                        setTimeout(() => window.location.href = "/Auth/Login", 3000);
                        break;
                    case 0: 
                        toastr.warning(resp.msg);
                        $btn.prop('disabled', false);
                        break;
                    case 99: 
                        toastr.error(resp.msg);
                        $btn.prop('disabled', false);
                        console.error(resp.detail);
                        break;
                    default:
                        toastr.info(resp.msg || "Procesando...");
                        $btn.prop('disabled', false);
                        break;
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor");
                console.error("Error AJAX:", textStatus, errorThrown);
            }
        });
    });

    // Manejo de Inicio de Sesión del Usuario
    $("#loginForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();
        const $btn = $(this).find('button[type="submit"]');
        $btn.prop('disabled', true);


        $.ajax({
            url: "/Auth/LoginUser",
            type: "POST",
            data: $(this).serialize(),
            success: function (resp) {
                switch (resp.cod) {
                    case 1:
                        toastr.success(resp.msg);
                        setTimeout(() => window.location.href = "/Home/Index", 3000);
                        break;
                    case 0:
                        toastr.warning(resp.msg);
                        $btn.prop('disabled', false);
                        break;
                    case 99:
                        toastr.error(resp.msg);
                        $btn.prop('disabled', false);
                        console.error(resp.detail);
                        break;
                    default:
                        toastr.info(resp.msg || "Procesando...");
                        $btn.prop('disabled', false);
                        break;
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor");
                console.error("Error AJAX:", textStatus, errorThrown);
            }
        });
    });




    $("#frmRecuperar").on("submit", function (event) {
        event.preventDefault();
        $.ajax({
            url: "/Usuario/RecuperarContrasenha",
            type: "POST",
            data: new FormData(this),
            processData: false,
            contentType: false,
            success: function (resp) {
                if (resp.success) {
                    toastr.success(resp.msg);
                    setTimeout(function () {
                        window.location.href = "/Usuario/Login";
                    }, 3000);
                } else {
                    toastr.error(resp.msg);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor");
                console.error('Error AJAX:', textStatus, errorThrown);
                console.error('Respuesta:', jqXHR.responseText);
            }
        });
    });

    $("#frmAgregarUsuario").on("submit", function (event) {
        event.preventDefault();

        $.ajax({
            url: "/Usuario/AgregarUsuario",
            type: "POST",
            data: new FormData(this),
            processData: false,
            contentType: false,
            success: function (resp) {
                if (resp.success) {
                    switch (resp.cod) {
                        case 0:
                            toastr.warning(resp.msg);
                            break;
                        case 1:
                            $('#agregarUsuarioModal').modal("hide");
                            toastr.success(resp.msg);
                            console.log("soy success")
                            setTimeout(function () {
                                location.reload();
                           }, 3000);
                            break;
                        case 99:
                            toastr.error(resp.msg);
                            break;
                    }
                } else {
                    toastr.error(resp.msg)
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor.");
                console.error('Error AJAX: ', textStatus, errorThrown);
                console.error('Respuesta del servidor: ', jqxhr.responseText);
            }
        });
    });

    $("#usuario_a").on('blur', function () {
        var $input = $(this);

        $input.removeClass("is-invalid is-valid");
        if ($input.val() === "") {
            return;
        }

        $.ajax({
            url: "/Usuario/ValidarUsuario",
            type: "POST",
            data: { usuario: $input.val().trim() },
            success: function (resp) {
                if (resp.success) {
                    $input.addClass("is-invalid");
                    $input.focus();
                } else {
                    if (resp.isEmpty) {
                        $input.removeClass("is-invalid is-valid");
                        $input.focus();
                        toastr.info("Rellena el campo de usuario.");
                    } else {
                        $input.addClass("is-valid");
                    }
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor.");
                console.error('Error AJAX: ', textStatus, errorThrown);
                console.error('Respuesta del servidor: ', jqxhr.responseText);
            }
        });
    });

    $("#correo_a").on('blur', function () {
        var $input = $(this);

        $input.removeClass("is-invalid is-valid");
        if ($input.val() === "") {
            return;
        }

        $.ajax({
            url: "/Usuario/ValidarCorreo",
            type: "POST",
            data: { correo: $input.val().trim() },
            success: function (resp) {
                if (resp.success) {
                    $input.addClass("is-invalid");
                    $input.focus();
                } else {
                    $input.addClass("is-valid");
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor.");
                console.error('Error AJAX: ', textStatus, errorThrown);
                console.error('Respuesta del servidor: ', jqxhr.responseText);
            }
        });
    });

    $("#correo_s").on('blur', function () {
        var $input = $(this);

        $input.removeClass("is-invalid is-valid");
        if ($input.val() === "") {
            return;
        }

        $.ajax({
            url: "/Usuario/ValidarCorreoUsuarioYSolicitud",
            type: "POST",
            data: { correo: $input.val().trim() },
            success: function (resp) {
                if (resp.success) {
                    $input.addClass("is-invalid");
                    $input.focus();
                } else {
                    $input.addClass("is-valid");
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor.");
                console.error('Error AJAX: ', textStatus, errorThrown);
                console.error('Respuesta del servidor: ', jqxhr.responseText);
            }
        });
    });

    $("#solicitudForm").on("submit", function (event) {
        event.preventDefault();

        $.ajax({
            url: "/Solicitud/AgregarSolicitud",
            type: "POST",
            data: new FormData(this),
            processData: false,
            contentType: false,
            success: function (resp) {
                if (resp.success) {
                    switch (resp.cod) {
                        case 0:
                            toastr.warning(resp.msg);
                            break;
                        case 1:
                            toastr.success(resp.msg);
                            setTimeout(function () {
                                location.reload();
                            }, 3000);
                            break;
                        case 99:
                            toastr.error(resp.msg);
                            break;
                    }
                } else {
                    toastr.error(resp.msg)
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor.");
                console.error('Error AJAX: ', textStatus, errorThrown);
                console.error('Respuesta del servidor: ', jqxhr.responseText);
            }
        });
    });

    $('#tbl tbody').on('click', '.btn-aceptar', function (e) {
        e.preventDefault();
        var idSolicitud = $(this).data('id');
        var nombres = $(this).data('nombres');
        var apellidos = $(this).data('apellidos');
        $('#solicitud_nombreCompleto_a').text(nombres + " " + apellidos);
        $('#id_solicitud_a').val(idSolicitud);

        $('#aceptarSolicitudModal').modal('show');
    });

    $('#btnConfirmarAceptar').on('click', function () {
        var idSolicitud = $('#id_solicitud_a').val();

        $.ajax({
            url: '/Usuario/CrearUsuarioPorSolicitud',
            type: 'POST',
            data: { id_solicitud: idSolicitud },
            success: function (resp) {
                if (resp.success) {
                    $('#aceptarSolicitudModal').modal('hide');
                    toastr.success(resp.msg);
                    setTimeout(function () {
                        location.reload();
                    }, 3000);
                } else {
                    toastr.error(resp.msg);
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor.");
                console.error('Error AJAX: ', textStatus, errorThrown);
                console.error('Respuesta del servidor: ', jqxhr.responseText);
            }
        });
    });

    $('#tbl tbody').on('click', '.btn-rechazar', function (e) {
        e.preventDefault();
        var idSolicitud = $(this).data('id');
        var nombres = $(this).data('nombres');
        var apellidos = $(this).data('apellidos');
        $('#solicitud_nombreCompleto_r').text(nombres + " " + apellidos);
        $('#id_solicitud_r').val(idSolicitud);

        $('#rechazarSolicitudModal').modal('show');
    });

    $('#btnConfirmarRechazar').on('click', function () {
        var idSolicitud = $('#id_solicitud_r').val();

        $.ajax({
            url: '/Solicitud/EliminarSolicitud',
            type: 'POST',
            data: { id: idSolicitud },
            success: function (resp) {
                if (resp.success) {
                    $('#rechazarSolicitudModal').modal('hide');
                    toastr.success(resp.msg);
                    setTimeout(function () {
                        location.reload();
                    }, 3000);
                } else {
                    toastr.error(resp.msg);
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor.");
                console.error('Error AJAX: ', textStatus, errorThrown);
                console.error('Respuesta del servidor: ', jqxhr.responseText);
            }
        });
    });
}); // Este es el de cierre. 