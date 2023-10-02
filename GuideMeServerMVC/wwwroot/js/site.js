// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function apagarRegistro(id, controller) {

    swal({
        title: "Tem certeza?",
        text: "O registro será apagado para sempre!",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        cancelButtonClass: "btn-info",
        confirmButtonText: "Sim",
        cancelButtonText: "Não!",
        closeOnConfirm: false
    },
        function () {
            location.href = '/' + controller + '/Delete?id=' + id;
        });
}