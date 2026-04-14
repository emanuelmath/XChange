// Funciones JQuery + Ajax para manejar eventos del layout Auth de XChange.

$(document).ready(function () {

    /**
     * Gestiona la solicitud de autnticación del usuario.
     * 
     * @event submit
     * @param {Event} event - Evento submit del formulario.
     * 
     * @description
     * 1. Previene el comportamiento por defecto del formulario (para validar antes de enviar).
     * 2. Valida los campos con los required de la vista.
     * 3. Envía los datos hacia el backend.
     * 4. Proporciona retroalimentación por medio de un toast dependiendo de la respuesta.
     *      -Cod 1: Proceso exitoso.
     *      -Cod 0: Credenciales incorrectas.
     *      -Cod 99: Error interno.
    */
    $("#loginForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');

        if (!this.checkValidity()) {
            $form.addClass("was-validated");
            return;
        }

        $btn.prop('disabled', true);


        $.ajax({
            url: "/Auth/LoginUser",
            type: "POST",
            data: $form.serialize(),
            timeout: 10000,

            success: function (resp) {
                switch (resp.cod) {

                    case 1:
                        toastr.success(resp.msg || "Inicio de sesión exitoso");
                        setTimeout(() => {
                            window.location.href = "/User/Index";
                        }, 3000);
                        break;

                    case 0:
                        toastr.warning(resp.msg || "Credenciales incorrectas");
                        break;

                    case 99:
                        toastr.error("Error interno. Intenta nuevamente.");
                        break;

                    default:
                        toastr.info(resp.msg || "Procesando...");
                        break;
                }
            },

            error: function (jqxhr) {
                toastr.error("Error inesperado en el servidor");
                console.error("Error AJAX:", textStatus, errorThrown);
            },
            complete: function () {
                $btn.prop('disabled', false);
            }
        });
    });

    /**
     * Gestiona y valida el proceso registro de un nuevo usuario de forma local.
     * 
     * @event submit
     * @param {Event} event - Evento de submit del form de registro.
     * 
     * @description
     * 1. Previene el comportamiento por defecto del formulario (para validar antes de enviar).
     * 2. Valida los campos con los required de la vista.
     * 3. Verifica que el checkbox esté activo.
     * 4. Compara ambas contraseñas para validar que sean las mismas.
     * 5. Envía los datos hacia el backend.
     * 6. Proporciona retroalimentación por medio de un toast dependiendo de la respuesta.
     *      -Cod 1: Proceso exitoso.
     *      -Cod 0: Campos inválidos o insuficientes.
     *      -Cod 99: Error interno.
     */
    $("#registerForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');

        if ($btn.prop('disabled')) return;

        if (!this.checkValidity()) {
            $form.addClass("was-validated");
            return;
        }

        if (!$("#terminos").is(":checked")) {
            toastr.warning("Debes aceptar los términos y condiciones");
            return;
        }

        let password = $("#Password").val();
        let confirm = $("#ConfirmPassword").val();

        if (password !== confirm) {
            toastr.warning("Las contraseñas no coinciden");
            return;
        }

        $btn.prop('disabled', true);

        $.ajax({
            url: "/Auth/RegisterUser",
            type: "POST",
            data: $form.serialize(),
            timeout: 10000,

            success: function (resp) {

                switch (resp.cod) {

                    case 1:
                        toastr.success(resp.msg || "Cuenta creada correctamente");
                        setTimeout(() => {
                            window.location.href = "/Auth/Login";
                        }, 3000);
                        break;

                    case 0:
                        toastr.warning(resp.msg || "No se pudo completar el registro");
                        break;

                    case 99:
                        toastr.error("Error interno. Intenta nuevamente.");
                        break;

                    default:
                        toastr.info(resp.msg || "Procesando...");
                        break;
                }
            },

            error: function (jqxhr) {
                toastr.error("Error inesperado en el servidor");
                console.error("Error AJAX:", textStatus, errorThrown);
            },

            complete: function () {
                $btn.prop('disabled', false);
            }
        });
    });
});