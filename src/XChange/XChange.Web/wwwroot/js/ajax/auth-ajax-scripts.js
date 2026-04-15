// Funciones JQuery + Ajax para manejar eventos del layout Auth de XChange.

$(document).ready(function () {

    /* ===== SECCIÓN DE AJAX DE FORMULARIOS ===== */

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
     *      -Cod 2: Proceso exitoso, pero debe hacer verificación de 2 pasos.
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
                            window.location.href = "/User/Dashboard";
                        }, 3000);
                        break;
                    case 2:
                        toastr.success(resp.msg || "Inicio de sesión exitoso");
                        setTimeout(() => {
                            window.location.href = "/Auth/VerifyMfa";
                        }, 3000);
                        break;
                    case 3:
                        toastr.success(resp.msg || "Inicio de sesión exitoso, a confirmar correo.");
                        setTimeout(() => {
                            window.location.href = "/Auth/VerifyEmail";
                        }, 3000);
                        break;
                    case 0:
                        toastr.warning(resp.msg || "Credenciales incorrectas");
                        $btn.prop('disabled', false);
                        break;
                    case 99:
                        toastr.error(resp.msg || "Error interno. Intenta nuevamente.");
                        break;
                        $btn.prop('disabled', false);
                    default:
                        toastr.info(resp.msg || "Procesando...");
                        break;
                        $btn.prop('disabled', false);
                }
            },
            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor");
                console.error("Error AJAX:", textStatus, errorThrown);
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
                        $btn.prop('disabled', false);
                        break;

                    case 99:
                        toastr.error("Error interno. Intenta nuevamente." || resp.msg);
                        $btn.prop('disabled', false);
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

    /**
     * Valida el código de MFA que ingresa el usuario.
     * 
     * @event submit
     * @param {Event} event - Captura el envío del formulario del usuario.
     * 
     * @description
     * Verifica que el código que el usuario ha insertado, sea el asignado en la base de datos y el que recibió en el correo electrónico.
     */
    $("#verifyCodeForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');

        if ($btn.prop('disabled')) return;

        if (!this.checkValidity()) {
            $form.addClass("was-validated");
            return;
        }

        const code = $("#Code").val();

        if (!code || code.length < 6) {
            toastr.warning("Ingresa un código válido");
            return;
        }

        $btn.prop('disabled', true);

        $.ajax({
            url: "/Auth/VerifyMfaUser",
            type: "POST",
            data: $form.serialize(),
            timeout: 10000,

            success: function (resp) {

                switch (resp.cod) {

                    case 1:
                        toastr.success(resp.msg || "Verificación exitosa.");

                        setTimeout(() => {
                            window.location.href = "/User/Dashboard";
                        }, 2000);
                        break;

                    case 0:
                        toastr.warning(resp.msg || "Código incorrecto o expirado.");
                        $btn.prop('disabled', false);
                        break;

                    case 2:
                        toastr.info("Se requiere nueva verificación.");
                        $btn.prop('disabled', false);
                        break;

                    case 99:
                        toastr.error(resp.msg || "Error interno. Intenta nuevamente.");
                        $btn.prop('disabled', false);
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

    /**
     * Gestiona el proceso de obtener el correo electrónico del usuario que desea reestablecer su contraseña.
     * 
     * @event submit
     * @param {Event} event - Evento de confirmación del formulario.
     * 
     * @description
     * Obtiene el correo electrónico del usuario que desea reestablecer su contraseña para crear un token de seguridad en la base de datos
     * para enviarlo de nuevo hacia ese correo.
     */
    $("#forgotPasswordForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const $form = $(this);
        const $btn = $('#confirmModal button[type="submit"]');

        if ($btn.prop('disabled')) return;

        
        if (!this.checkValidity()) {
            $form.addClass("was-validated");
            return;
        }

        const email = $("#Email").val();

        
        if (!email || !email.includes("@")) {
            toastr.warning("Ingresa un correo válido");
            return;
        }

        $btn.prop('disabled', true);

        $.ajax({
            url: "/Auth/ForgotPassword",
            type: "POST",
            data: $form.serialize(),
            timeout: 10000,

            success: function (resp) {

                switch (resp.cod) {

                    case 1:
                        toastr.success(resp.msg || "Si el correo existe, recibirás instrucciones");

                        
                        setTimeout(() => {
                            window.location.href = "/Auth/ResetPassword";
                        }, 2500);
                        break;

                    case 0:
                        toastr.warning(resp.msg || "No se pudo procesar la solicitud");
                        break;

                    case 99:
                        toastr.error("Error interno. Intenta nuevamente.");
                        break;

                    default:
                        toastr.info(resp.msg || "Procesando...");
                        break;
                }
            },

            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor");
                console.error("Error AJAX:", textStatus, errorThrown);
            },

            complete: function () {
                $btn.prop('disabled', false);

                const modal = bootstrap.Modal.getInstance(document.getElementById('confirmModal'));
                if (modal) modal.hide();
            }
        });
    });

    /**
     * Reestablece la contraseña del usuario solicitante una vez cumple con todos los requisitos.
     * 
     * @event submit
     * @param {Event} event - Evento de submit del formulario de Reestablecer contraseña.
     * 
     * @description
     * 
     * 1. Valida fuertemente que todos los campos cumplan con los requisitos.
     * 2. Verifica que tanto el correo como el token pertenecen al mismo usuario.
     * 3. Reestablece la contraseña asignada por el usuario, de forma hasheada en la base de datos.
     */
    $("#resetPasswordForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');

        if ($btn.prop('disabled')) return;

        if (!this.checkValidity()) {
            $form.addClass("was-validated");
            return;
        }

        const email = $("#Email").val();
        const token = $("#Token").val();
        const password = $("#NewPassword").val();

        if (!email || !email.includes("@")) {
            toastr.warning("Correo inválido");
            return;
        }

        if (!token || token.length < 4) {
            toastr.warning("Código inválido");
            return;
        }

        if (!password || password.length < 6) {
            toastr.warning("La contraseña debe tener al menos 6 caracteres");
            return;
        }

        if (!$("#confirmChange").is(":checked")) {
            toastr.warning("Debes autorizar el cambio de contraseña");
            return;
        }

        $btn.prop('disabled', true);

        $.ajax({
            url: "/Auth/ResetPassword",
            type: "POST",
            data: $form.serialize(),
            timeout: 10000,

            success: function (resp) {

                switch (resp.cod) {

                    case 1:
                        toastr.success(resp.msg || "Contraseña actualizada correctamente");

                        setTimeout(() => {
                            window.location.href = "/Auth/Login";
                        }, 2500);
                        break;

                    case 0:
                        toastr.warning(resp.msg || "Código inválido o expirado");
                        break;

                    case 2:
                        toastr.warning("El token ya fue utilizado");
                        break;

                    case 3:
                        toastr.warning("El token ha expirado");
                        break;

                    case 99:
                        toastr.error("Error interno. Intenta nuevamente.");
                        break;

                    default:
                        toastr.info(resp.msg || "Procesando...");
                        break;
                }
            },

            error: function (jqxhr, textStatus, errorThrown) {
                toastr.error("Error inesperado en el servidor");
                console.error("Error AJAX:", textStatus, errorThrown);
            },

            complete: function () {
                $btn.prop('disabled', false);
            }
        });
    });

    $("#verifyEmailForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');

        if ($btn.prop('disabled')) return;

        const code = $("#Code").val();

        if (!code || code.length < 6) {
            toastr.warning("Ingresa un código válido");
            return;
        }

        $btn.prop('disabled', true);

        $.ajax({
            url: "/Auth/VerifyEmailUser",
            type: "POST",
            data: $form.serialize(),
            timeout: 10000,

            success: function (resp) {

                switch (resp.cod) {

                    case 1:
                        toastr.success(resp.msg || "Verificación exitosa.");

                        setTimeout(() => {
                            window.location.href = "/User/Dashboard";
                        }, 2000);
                        break;

                    case 0:
                        toastr.warning(resp.msg || "Código incorrecto o expirado.");
                        $btn.prop('disabled', false);
                        break;

                    case 98:
                        toastr.info("Se requiere nueva verificación.");
                        $btn.prop('disabled', false);
                        break;

                    case 99:
                        toastr.error(resp.msg || "Error interno. Intenta nuevamente.");
                        $btn.prop('disabled', false);
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

    /* ===== SECCIÓN DE AJAX DE UI BEHAVIOR ===== */

    /**
     * Alterna el valor del atributo 'type' del input de contraseña para que el usuario pueda visualizar lo que estpá escribiendo.
     * 
     * @event change
     * @param {Event} event - Evento de toggle o cambio de estado del checkbox
     * 
     * @description
     * 1. Captura los eventos de intercambio de estado del checkbox 'Mostrar contraseña' de la pantalla de login.
     * 2. Alterna el tipo del input de contraseña para mostrar u ocultar el contenido.
     */
    $("#showPassword").on("change", function () {
        const type = this.checked ? "text" : "password";
        $("#Password").attr("type", type);
    });

    /**
     * Alterna el valor del atributo 'type' de ambos inputs de contraseña para que el usuario pueda verlas.
     * @event change
     * @param {Event} event - Evento de toggle o cambio de estado del checkbox
     * 
     * @description
     * 1. Captura los eventos de intercambio de estado del checkbox 'Mostrar contraseñas' de la pantalla de registro.
     * 2. Alterna el tipo del input de contraseña para mostrar u ocultar el contenido.
     */
    $("#showPasswords").on("change", function () {
        const type = this.checked ? "text" : "password";
        $("#Password, #ConfirmPassword").attr("type", type);
    });

    /**
     * Alterna el valor del atributo 'type' de ambos inputs de contraseña para que el usuario pueda verlas.
     * @event change
     * @param {Event} event - Evento de toggle o cambio de estado del checkbox
     * 
     * @description
     * 1. Captura los eventos de intercambio de estado del checkbox 'Mostrar contraseñas' de la pantalla de registro.
     * 2. Alterna el tipo del input de contraseña para mostrar u ocultar el contenido.
     */
    $("#showNewPassword").on("change", function () {
        const type = this.checked ? "text" : "password";
        $("#NewPassword").attr("type", type);
    });
});