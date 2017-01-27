
function GetControlIDByFileTypeID(fileTypeId) {
    switch (fileTypeId) {
        case 1: return "CAPUCPERMIT";
        case 2: return "COMMERCIALINSURANCECERTIFICATE";
    }
}

$("#currVehicleId").val("0");
var controlID;

$(function () {

    $(".fileupload-preview").hide();
    GetPartnerFiles();
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
   

    $(".datepicker").datepicker("option", "minDate");
    var minDate = $(".datepicker").datepicker("option", "minDate", new Date(2017, 1 - 1, 1));
    
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

function GetPartnerFiles() {
  
    //window.BlockUi();
    $.ajax({
        type: "POST",
        url: "/BusinessProfile/GetFileList",
        contentType: "application/json; charset=utf-8",
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            window.UnBlockUi();
            console.log("Request: " + xmlHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        success: function (response) {
            console.log(response);
            /*  $(".partnerFiles .files-uploaders-profile").show();
              $(".partnerFiles .fileupload-preview-profile").hide();
              $(".btn-save-exp-date").hide();
  
              if (response != "") {
                  for (var i = 0; i < response.length; i++) {
  
                      if (response[i].isFileExists) {
                          $("#" + response[i].controlId + "Doc").attr("src", "/Documents/PartnerProfile/" + response[i].controlId + "/thumbs/" + response[i].fileName + ".80x80.jpg");
                      }
                      else {
                          $("#" + response[i].controlId + "Doc").attr("src", "/Documents/defaultImages/document-default.png");
                      }
                      $("#" + response[i].controlId + "ExpDate").val(response[i].ExpDate);
                      $("#" + response[i].controlId + "Uploader").hide();
                      $("#" + response[i].controlId + "Content").show();
  
  
                  }
              }
              window.UnBlockUi();*/
        }
    });
}

var carMake;
var carModel;

function ConfirmDeleteDocument(controlId) {
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
            DeleteDocument(controlId);
        }
    });
}

function DeleteDocument(controlId) {
    var fileName = $("#txt" + controlId).val();

    var fileId = $("#txt" + controlId).attr("data-fileid");
    if (fileId == undefined || fileId === "") {
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
        error: function (xmlHttpRequest, textStatus, errorThrown) {
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





