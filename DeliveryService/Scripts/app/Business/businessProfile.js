
function GetControlIDByFileTypeID(fileTypeID) {
    switch (fileTypeID) {
        case 1: return "CAPUCPERMIT";
        case 2: return "COMMERCIALINSURANCECERTIFICATE";
    }
}

$("#currVehicleId").val("0");
var controlID;

$(function () {

    $(".fileupload-preview").hide();


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

function InitVehicleFileupload(controlId) {
    $('#pagefileupload').fileupload({
        url: "/BusinessProfile/Upload?controlID=" + controlId,
        dataType: 'json',
        autoUpload: true,
        start: function () {
            $("#manageVehicleModal").attr("style", "display: block; z-index: 1000!important");
            window.BlockUi();
            $(".vehicle-fileuploads").removeClass("vehicle-fileuploads-active");
        },
        done: function (e, data) {
            $("#" + controlId + "Doc").attr("src", data.result.Files[0].ThumbnailUrl.replace("~", ""));
            $("#" + controlId + "Uploader").hide();
            $("#" + controlId + "Content").show();
            $("#txt" + controlId).val(data.result.Files[0].Name);
            $("#txt" + controlId + "-error").hide();
            $(".vehicle-fileuploads").addClass("vehicle-fileuploads-active");
            $("#manageVehicleModal").attr("style", "display: block");
            window.UnBlockUi();
        },
        stop: function () {
            $(".vehicle-fileuploads").addClass("vehicle-fileuploads-active");
            $("#manageVehicleModal").attr("style", "display: block");
            window.UnBlockUi();
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

function DeleteDocument(controlId) {
    var fileName = $("#txt" + controlId).val();

    var fileId = $("#txt" + controlId).attr("data-fileid");
    if (fileId == undefined || fileId == null || fileId === "") {
        fileId = "0";
    }

    var id = $("#currVehicleId").val();
    var postData = {
        controlID: controlId,
        file: fileName,
        id: fileId
    }
    $("#manageVehicleModal").attr("style", "display: block; z-index: 1000!important");
    window.BlockUi();
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
            $("#" + controlId + "Doc").attr("src", "");
            $("#" + controlId + "Uploader").show();
            $("#" + controlId + "Content").hide();
            $("#txt" + controlId).val("");
            $("#txt" + controlId).removeAttr("data-fileid");
            $("#manageVehicleModal").attr("style", "display: block");
            window.UnBlockUi();
        }
    });
}





