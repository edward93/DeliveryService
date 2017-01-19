
function GetControlIDByFileTypeID(fileTypeID) {
    switch (fileTypeID) {
        case 1: return "InspectionForm";
        case 2: return "LAXPermit";
        case 3: return "InspectionReceipt";
        case 4: return "CACommercialRegistration";
        case 5: return "Front";
        case 6: return "BackSeat";
        case 7: return "BackBamper";
        case 8: return "RightSide";
    }
}

$("#currVehicleId").val("0");
var controlID;

$(function () {

    $(".fileupload-preview").hide();

    $(".exp-date-input").datepicker({
        format: 'MM/DD/YYYY'
    });

    $(document).on("click", ".fileupload-preview img", function () {
        var thumbSrc = $(this).attr("src");
        var documentImage = "";

        documentImage = thumbSrc.replace("thumbs/", "").replace(".80x80.jpg", "");

        $("#documentFullImage").attr("src", documentImage);
        $("#documentImageModal").attr("style", "display: block; z-index: 4000");
        $(".ImageModalclose").click(function () {
            $("#documentImageModal").attr("style", "display: none");
        });
    });

    $("#vehicleForm > .steps li").removeClass("disabled").addClass("done");
    $(".fileinput-button span").html("Drag & drop<br> or click here <br> to upload");
    $(".fileinput-button i").remove();

    $("#uploadContent").show();


});

$(document).on("click", ".vehicle-fileuploads-active", function () {
    controlID = $(this).attr("data-controlid");
    InitVehicleFileupload(controlID);
    $("#pagefileupload").attr("data-controlid", controlID);
    $("#pagefileupload input").click();
});

function InitVehicleFileupload(controlID) {
    $('#pagefileupload').fileupload({
        url: "/BusinessProfile/Upload?controlID=" + controlID,
        dataType: 'json',
        autoUpload: true,
        start: function () {
            $("#manageVehicleModal").attr("style", "display: block; z-index: 1000!important");
            BlockUi();
            $(".vehicle-fileuploads").removeClass("vehicle-fileuploads-active");
        },
        done: function (e, data) {
            $("#" + controlID + "Doc").attr("src", data.result.Files[0].ThumbnailUrl.replace("~", ""));
            $("#" + controlID + "Uploader").hide();
            $("#" + controlID + "Content").show();
            $("#txt" + controlID).val(data.result.Files[0].Name);
            $("#txt" + controlID + "-error").hide();
            $(".vehicle-fileuploads").addClass("vehicle-fileuploads-active");
            $("#manageVehicleModal").attr("style", "display: block");
            UnBlockUi();
        },
        stop: function () {
            $(".vehicle-fileuploads").addClass("vehicle-fileuploads-active");
            $("#manageVehicleModal").attr("style", "display: block");
            UnBlockUi();
        }
    });
}

var carMake;
var carModel;

function ConfirmDeleteDocument(controlID) {
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete!",
        closeOnConfirm: true
    }, function (isConfirm) {
        if (isConfirm) {
            DeleteDocument(controlID);
        }

    });
}

function DeleteDocument(controlID) {
    var fileName = $("#txt" + controlID).val();

    var fileId = $("#txt" + controlID).attr("data-fileid");
    if (fileId == undefined || fileId == null || fileId === "") {
        fileId = "0";
    }

    var id = $("#currVehicleId").val();
    var postData = {
        controlID: controlID,
        file: fileName,
        id: fileId
    }
    $("#manageVehicleModal").attr("style", "display: block; z-index: 1000!important");
    BlockUi();
    $.ajax({
        type: "POST",
        url: "/BusinessProfile/DeleteFile",
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            window.UnBlockUi();
            $("#manageVehicleModal").attr("style", "display: block");
        },
        success: function (response) {
            $("#" + controlID + "Doc").attr("src", "");
            $("#" + controlID + "Uploader").show();
            $("#" + controlID + "Content").hide();
            $("#txt" + controlID).val("");
            $("#txt" + controlID).removeAttr("data-fileid");
            $("#manageVehicleModal").attr("style", "display: block");
            UnBlockUi();
        }
    });
}





