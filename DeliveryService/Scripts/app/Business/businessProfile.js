
function GetControlIDByFileTypeID(fileTypeId) {
    switch (fileTypeId) {
        case 0: return "Logo";
        case 1: return "Commercialinsurancecertificate";
        case 2: return "BusinessProfile";
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
});

$(document).on("click", ".editProfileData", function () {
    $("#businessProfileModal").modal("show");
});

$(document).on("click", ".vehicle-fileuploads-active", function () {
    controlID = $(this).attr("data-controlid");
    InitVehicleFileupload(controlID);
    $("#pagefileupload").attr("data-controlid", controlID);
    $("#pagefileupload input").click();
});

function InitVehicleFileupload(controlId) {
    $('#pagefileupload').fileupload({
        url: "/BusinessProfile/UploadBusinessDocument?uploadType=" + controlId,
        dataType: 'json',
        autoUpload: true,
        start: function () {
            $("#manageVehicleModal").attr("style", "display: block; z-index: 1000!important");
            window.BlockUi();
            $(".vehicle-fileuploads").removeClass("vehicle-fileuploads-active");
        },
        done: function (e, data) {
            if (data.result.Success) {
                var upload = data.result.Data;
                var relFilePath = "/Uploads/BusinessDocuments/" +
                    GetControlIDByFileTypeID(upload.DocumentType) +
                    "/thumbs/" +
                    upload.FileName;
                $("#" + controlId + "Doc").attr("src", relFilePath);
                $("#" + controlId + "Uploader").hide();
                $("#" + controlId + "Content").show();
                $("#txt" + controlId).val(upload.FileName);
                $("#txt" + controlId).attr("data-fileid", upload.DocumentId);
                $("#txt" + controlId + "-error").hide();
                $(".vehicle-fileuploads").addClass("vehicle-fileuploads-active");
                $("#manageVehicleModal").attr("style", "display: block");

                if (upload.DocumentType === 2) {
                    $("#contactPersonPhoto").attr("src", relFilePath);
                }

                if (upload.DocumentType === 0) {
                    $("#navigation-business-logo").attr("src", relFilePath);
                }
                window.UnBlockUi();
            }
            
        },
        stop: function () {
            $(".vehicle-fileuploads").addClass("vehicle-fileuploads-active");
            $("#manageVehicleModal").attr("style", "display: block");
            window.UnBlockUi();
        }
    });
}

function GetPartnerFiles() {
  
    window.BlockUi();
    $.ajax({
        type: "POST",
        url: "/BusinessProfile/GetFileList",
        contentType: "application/json; charset=utf-8",
        error: function (xmlHttpRequest, textStatus, errorThrown) {
            window.UnBlockUi();
        },
        success: function (response) {
              $(".partnerFiles .files-uploaders-profile").show();
              $(".partnerFiles .fileupload-preview-profile").hide();
              $(".btn-save-exp-date").hide();
  
              if (response !== "") {
                  for (var i = 0; i < response.length; i++) {
                      var controlName = GetControlIDByFileTypeID(response[i].UploadType);
                      if (response[i].IsFileExist) {
                          $("#" + controlName + "Doc").attr("src", "/Uploads/BusinessDocuments/" + controlName + "/thumbs/" + response[i].FileName);
                          $("#txt" + controlName).val(response[i].FileName);
                          $("#txt" + controlName).attr("data-fileid", response[i].DocumentId);
                          if (controlName === 'BusinessProfile') {
                              $("#contactPersonPhoto")
                                  .attr("src",
                                      "/Uploads/BusinessDocuments/" + controlName + "/thumbs/" + response[i].FileName);
                          }
                      }
                      else {
                          $("#" + controlName + "Doc").attr("src", "/Images/boldmindLogo.pngfault.png");
                      }
                      $("#" + controlName + "ExpDate").val(response[i].ExpDate);
                      $("#" + controlName + "Uploader").hide();
                      $("#" + controlName + "Content").show();
                  }
              }
              window.UnBlockUi();
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
            if (controlId === 'BusinessProfile') {
                $("#contactPersonPhoto").attr("src", "/Images/boldmindLogo.png");
            }
            if (controlId === 'Logo') {
                $("#navigation-business-logo").attr("src", "/Images/boldmindLogo.png");
            }
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





