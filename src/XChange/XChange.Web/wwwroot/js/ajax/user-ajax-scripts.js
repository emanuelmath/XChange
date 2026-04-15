// Funciones JQuery + Ajax para manejar eventos del layout protegido de XChange.

$(document).ready(function () {
    /**
     * Gestiona la solicitud de activación del Autenticador de Dos Factores (MFA).
     * * @event submit
     * @param {Event} event - Evento submit del formulario del modal de MFA.
     * * @description
     * 1. Previene el comportamiento por defecto del formulario.
     * 2. Valida que el código tenga exactamente 6 dígitos.
     * 3. Envía el código al backend para validarlo contra el Secreto temporal.
     * 4. Proporciona retroalimentación por medio de un toast dependiendo de la respuesta.
     * -Cod 1: Activación exitosa (MFA habilitado en la BD).
     * -Cod 0: Código incorrecto o expirado.
     * -Cod 99: Error interno / Sesión expirada.
     */
    $("#activateMfaForm").on("submit", function (event) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const $form = $(this);
        const $btn = $form.find('button[type="submit"]');

        // Prevenir doble envío
        if ($btn.prop('disabled')) return;

        // Validación personalizada de 6 dígitos
        const code = $("#mfaCode").val().trim();
        if (!code || code.length !== 6) {
            toastr.warning("Ingresa los 6 dígitos exactos de tu aplicación.");
            return;
        }

        $btn.prop('disabled', true);

        $.ajax({
            url: "/User/ConfirmMfaActivation", 
            type: "POST",
            data: $form.serialize(),
            timeout: 10000,

            success: function (resp) {
                switch (resp.cod) {
                    case 1:
                        toastr.success(resp.msg || "Autenticación de dos pasos activada.");
                        $("#mfaModal").modal('hide');
                        setTimeout(() => {
                            window.location.reload();
                        }, 2000);
                        break;

                    case 0:
                        toastr.warning(resp.msg || "Código incorrecto. Intenta nuevamente.");
                        $btn.prop('disabled', false);
                        $("#mfaCode").val('').focus();
                        break;

                    case 99:
                        toastr.error(resp.msg || "Error de sesión. Recarga la página.");
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
                $btn.prop('disabled', false);
            }
        });
    });
});