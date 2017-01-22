$(document).ready(function () {
    var driverId;
    $('#tblDriversList').DataTable({
        dom: '<"html5buttons"B>lTfgitp',
        buttons: [
            { extend: 'copy' },
            { extend: 'csv' },
            { extend: 'excel', title: 'Drivers' },
            {
                extend: 'pdf', title: 'Drivers',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            },
            {
                extend: 'print',
                customize: function (win) {
                    $(win.document.body).addClass('white-bg');
                    $(win.document.body).css('font-size', '10px');
                    $(win.document.body).find('table')
                        .addClass('compact')
                        .css('font-size', 'inherit');
                }
            }
        ]

    });

    $(document).on('click',
        '#btnDeleteDriver',
        function () {
            var driverIdForDelete = $(this).attr('data-id');
            swal({
                title: "Are you sure?",
                text: "You will not be able to recover your rider!",
                type: "warning",
                showCancelButton: true,
                confirmButtonColor: "#DD6B55",
                confirmButtonText: "Yes, delete!",
                closeOnConfirm: true
            },
                function (isConfirm) {
                    if (isConfirm)
                        deleteUser(parseInt(driverIdForDelete));

                });
        });

    $(document).on('click', "#btnPreviewDriver", function () {
        var driverIdForPreview = $(this).attr('data-id');
        driverId = driverIdForPreview;
        getDriverDocuments(parseInt(driverIdForPreview));
    });
});

function deleteUser(driverId) {
    window.BlockUi();
    $.post("/Drivers/DeleteDriver",
        { driverId: driverId },
        function (data) {
            window.UnBlockUi();
            $("#driver_" + driverId).remove();
            swal("Deleted!", "Your driver has been deleted.", "success");
        });
}
