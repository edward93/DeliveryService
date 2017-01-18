
function OpenAddVehicleDialog() {
    $("#headText1").html("ADD A VEHICLE");
    GetDrivers($("#vehiclePartner").val(), true);
    $("#txtColor").val("Black");
    $("#vehicleForm-t-0").click();
    $("#manageVehicleModal").modal("show");
};

function OpenEditVehicleDialog(vehicleID) {

    BlockUi();
    $("#headText1").html("EDIT A VEHICLE");

    $.post(
        "/VehiclesList/GetVehicleById",
        {
            vehicleId: vehicleID
        },
        function (data) {

            $("#txtColor").val(data.vehicle.Color);
            $("#txtLicencePlate").val(data.vehicle.LicensePlate);
            $("#spnModel").html(data.vehicle.Make);
            $("#currVehicleId").val(data.vehicle.Id);
            carMake = data.vehicle.Make;
            $("#selectCarType").val(data.vehicle.CarType);

            $("#drpModelsBlack").hide();
            $("#drpModelsSuv").hide();
            $("#drpModelsLux").hide();


            if (data.vehicle.CarType == "1") {
                $("#drpModelsBlack").show();
                $("#drpModelsBlack").val(data.vehicle.Model);
            }
            else if (data.vehicle.CarType == "2") {
                $("#drpModelsSuv").show();
                $("#drpModelsSuv").val(data.vehicle.Model);
            }
            else if (data.vehicle.CarType == "3") {
                $("#drpModelsLux").show();
                $("#drpModelsLux").val(data.vehicle.Model);
            }

            $("#rideDate").val(data.vehicle.Year);
            carModel = data.vehicle.Model;
            $("#inpCarModel").val(data.vehicle.Model);

            $("#vehicleEnabled").val((data.vehicle.Enabled ? "1" : "0"));
            $("#vehiclePartner").val(data.vehicle.PartnerId);

            if (data.driver != undefined && data.driver != null) {
                var option = '<option value="' + data.driver.Id + '">' + data.driver.FirstName + ' ' + data.driver.LastName + '</option>';
                GetDrivers(data.vehicle.PartnerId, false, option);
            }
            else {
                GetDrivers(data.vehicle.PartnerId, false);
            }

            GetVehicleFiles(data.vehicle.Id);

            $("#vehicleForm-t-0").click();
            UnBlockUi();
            $("#manageVehicleModal").modal("show");
        });
};

function OpenDeleteVehicleDialog(vehicleID) {
    var vehicleIdForDelete = vehicleID;
    swal({
        title: "Are you sure?",
        text: "You will not be able to recover your driver!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete!",
        closeOnConfirm: false
    }, function (isConfirm) {
        if (isConfirm)
            deleteVehicle(vehicleIdForDelete);
    });
};

function deleteVehicle(vId) {

    BlockUi();
    swal.close();
    $.post("/VehiclesList/DeleteVehicle", { vehicleId: vId },
        function (data) {
            UnBlockUi();

            if (data.toString().indexOf("error") == -1) {
                if (!data) {
                    toastr.error('The vehicle can not be deleted because there are related data', 'Error');
                }
                else {
                    ReloadGrid("vehiclesList");
                    swal("Deleted!", "Your vehicle has been deleted.", "success");
                }
            }

        });
}

function GetControlIDByFileTypeID(fileTypeID) {
    switch (fileTypeID) {
        case 1: return "InspectionForm";
            break;
        case 2: return "LAXPermit";
            break;
        case 3: return "InspectionReceipt";
            break;
        case 4: return "CACommercialRegistration";
            break;
        case 5: return "Front";
            break;
        case 6: return "BackSeat";
            break;
        case 7: return "BackBamper";
            break;
        case 8: return "RightSide";
            break;
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

    $("#drpModelsLux").hide();
    $("#drpModelsSuv").hide();

    /* $("#vehicleForm").steps({
         bodyTag: "fieldset",
         labels: {
             finish: "Save<label id='btnSaveVehicle'></label>",
         },
         onStepChanging: function (event, currentIndex, newIndex) {
             if (currentIndex > newIndex) {
                 return true;
             }

             if (newIndex === 3 && Number($("#age").val()) < 18) {
                 return false;
             }

             var form = $(this);

             if (currentIndex < newIndex) {
                 $(".body:eq(" + newIndex + ") label.error", form).remove();
                 $(".body:eq(" + newIndex + ") .error", form).removeClass("error");
             }


             form.validate().settings.ignore = ":disabled,:hidden";

             return form.valid();
         },
         onStepChanged: function (event, currentIndex, priorIndex) {
             if (currentIndex === 2 && Number($("#age").val()) >= 18) {
                 $(this).steps("next");
             }
             if (currentIndex === 2 && priorIndex === 3) {
                 $(this).steps("previous");
             }
             if (currentIndex === 2) {
                 $("#btnSaveVehicle").parent().parent().show();
             }
             else {
                 $("#btnSaveVehicle").parent().parent().hide();
             }
             $(".exp-date-input").datepicker({
                 format: 'MM/DD/YYYY'
             });

         },
         onFinishing: function (event, currentIndex) {
             var form = $(this);
             form.validate().settings.ignore = ":disabled";
             return form.valid();
         },
         onFinished: function (event, currentIndex) {
             /*var form = $(this);
             form.submit();*/
    /*        saveVehicle();
        },
        onCanceled: function () {
            $("#manageVehicleModal").modal("hide");
        }
    }).validate({
        errorPlacement: function (error, element) {
            element.before(error);
        },
        rules: {
            confirm: {
                equalTo: "#password"
            }
        }
    });*/

    $("#btnSaveVehicle").parent().parent().hide();

    $("#manageVehicleModal").on('hidden.bs.modal', function (e) {
        RefreshManageModal();
    });

    $("#vehicleForm > .steps li").removeClass("disabled").addClass("done");

    //GetDrivers();
    RefreshManageModal();

    $(".fileinput-button span").html("Drag & drop<br> or click here <br> to upload");
    $(".fileinput-button i").remove();

    $("#uploadContent").show();


    $("#vehiclePartner").change(function () {
        GetDrivers($(this).val(), true);
    });

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
        url: "/VehiclesList/DeleteFile",
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            UnBlockUi();
            $("#manageVehicleModal").attr("style", "display: block");
            console.log("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
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

function foo() {

    if ($("#selectCarType").val() == "1") {
        $("#drpModelsBlack").show();
        $("#drpModelsLux").hide();
        $("#drpModelsSuv").hide();
    }
    else if ($("#selectCarType").val() == "2") {
        $("#drpModelsSuv").show();
        $("#drpModelsLux").hide();
        $("#drpModelsBlack").hide();
    }
    else if ($("#selectCarType").val() == "3") {
        $("#drpModelsLux").show();
        $("#drpModelsSuv").hide();
        $("#drpModelsBlack").hide();
    }
}

function foo2(elem) {
    if ($(elem.options[elem.selectedIndex]).val() != "-1") {
        $("#inpCarModel").val($(elem.options[elem.selectedIndex]).val());
    }
    else {
        $("#inpCarModel").val("");
    }
    carMake = $(elem.options[elem.selectedIndex]).closest('optgroup').prop('label');
    $("#spnModel").html(carMake);
    carModel = $(elem.options[elem.selectedIndex]).val();
}

$('#drpModelsSuv').on('change', function () {
    var label = $(this.options[this.selectedIndex]).closest('optgroup').prop('label');
    console.log(label);
});

$('#drpModelsBlack').on('change', function () {
    var label = $(this.options[this.selectedIndex]).closest('optgroup').prop('label');
    console.log(label);
});

$('#drpModelsLux').on('change', function () {
    var label = $(this.options[this.selectedIndex]).closest('optgroup').prop('label');
    console.log(label);
});

function dateChanged(ev) {
}

function GetVehicleData(vehicleID) {

    var vehicle = {
        ID: vehicleID,
        PartnerId: $("#vehiclePartner").val(),
        Make: carMake,
        Model: carModel,
        Year: $("#rideDate").val(),
        LicensePlate: $("#txtLicencePlate").val(),
        Color: $("#txtColor").val(),
        CarType: $("#selectCarType").val(),
        Enabled: ($("#vehicleEnabled").val() == 0 ? false : true),
    }
    return vehicle;
}

function GetVehicleFilesForAdd(vehicleID) {
    var files = [];
    var i = 0;

    $(".doc-hidden-inputs").each(function () {

        var fileid;
        if ($(this).val().trim() != "") {

            if ($(this).attr("data-fileid") == undefined || $(this).attr("data-fileid") == null) {
                fileid = "0";
            }
            else {
                fileid = $(this).attr("data-fileid");
            }

            var file = {
                Id: fileid,
                VehicleId: vehicleID,
                FileName: $(this).val(),
                Name: $(this).val(),
                //ExtraInfo: "ExtraInfo",
                FileTypeID: $(this).attr("data-filetypeid"),
                ExpDate: $(this).parent().find(".exp-date-input").val()
            }
            files[i] = file;
            i++;
        }
    });

    return files;
}

function AddZero(num) {
    return (num >= 0 && num < 10) ? "0" + num : num + "";
}

function saveVehicle() {
    var isValid = true;
    $(".doc-hidden-inputs").each(function () {

        var jsDate;

        var now = new Date();
        var strDateNow = [AddZero(now.getMonth() + 1), AddZero(now.getDate()), now.getFullYear()].join("/");
        var dateNow = moment(strDateNow, 'MM/DD/YYYY').toDate();

        var dateInput = $(this).parent().find(".exp-date-input");

        var docImageBlock = dateInput.parent().parent().parent();

        if (dateInput.length != 0 && (docImageBlock.attr("style") == undefined || (docImageBlock.attr("style") != undefined && docImageBlock.attr("style").indexOf("none") == -1))) {
            var errorMessageBlock = $(dateInput).parent().parent().find(".lbl-exp-date-error");
            if (dateInput.val().trim() == "") {
                $(dateInput).css("border-color", "red");
                $(errorMessageBlock).html("This field can not be empty.");
                $(errorMessageBlock).show();
                isValid = false;
            }
            else {
                $(dateInput).removeAttr("style");
                $(errorMessageBlock).html("");
                $(errorMessageBlock).hide();
                var momentDate = moment(dateInput.val().replace(".", "/").replace(".", "/").replace("-", "/").replace("-", "/"), 'MM/DD/YYYY');

                jsDate = momentDate.toDate();
                if (jsDate < dateNow) {
                    $(dateInput).css("border-color", "red");
                    $(errorMessageBlock).html("Expire date can not be less than the current date.");
                    $(errorMessageBlock).show();
                    isValid = false;
                }
                else {
                    $(errorMessageBlock).html("");
                    $(errorMessageBlock).hide();
                }
            }
        }
    });

    if (!isValid) {
        $("#form-t-1").click();
        return;
    }

    var postData = {
        vehicle: GetVehicleData($("#currVehicleId").val()),
        vehicleFiles: GetVehicleFilesForAdd($("#currVehicleId").val()),
        driverId: $("#drpVehicleDrivers").val()
    }

    $("#manageVehicleModal").attr("style", "display: block; z-index: 1000!important");
    BlockUi();
    $.ajax({
        type: "POST",
        url: "/VehiclesList/CreateNewVehicle",
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            UnBlockUi();
            $("#manageVehicleModal").attr("style", "display: block");
            console.log("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        success: function (response) {


            $("#manageVehicleModal").attr("style", "display: block;");
            $("#manageVehicleModal").modal("hide");
            UnBlockUi();
            ReloadGrid('vehiclesList');
        }
    });
}

function GetVehicleFiles(vehicleID) {
    var postData = {
        vehicleId: vehicleID
    }

    $("#manageVehicleModal").attr("style", "display: block; z-index: 1000!important");
    BlockUi();
    $.ajax({
        type: "POST",
        url: "/VehiclesList/GetVehicleFiles",
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            UnBlockUi();
            $("#manageVehicleModal").attr("style", "display: block");
            console.log("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        success: function (response) {
            $("#manageVehicleModal").attr("style", "display: block;");

            for (var i = 0; i < response.length; i++) {
                var controlID = GetControlIDByFileTypeID(response[i].FileTypeID);
                $("#" + controlID + "Uploader").hide();

                $("#txt" + controlID).attr("data-fileid", response[i].Id);
                $("#txt" + controlID).val(response[i].Name);
                $("#" + controlID + "Doc").attr("src", response[i].FileName);

                $("#" + controlID + "ExpDate").val(response[i].ExpDate);

                $("#" + controlID + "Content").show();
            }

            UnBlockUi();
        }
    });
}

function GetDrivers(partnerID, isAsync, currentDriver) {

    var postData = {
        partnerID: partnerID
    }

    if (isAsync) {
        $("#manageVehicleModal").attr("style", "display: block; z-index: 10!important;");
        BlockUi();
    }

    $.ajax({
        type: "POST",
        url: "/VehiclesList/GetAllMembers",
        data: JSON.stringify(postData),
        contentType: "application/json; charset=utf-8",
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (isAsync) {
                $("#manageVehicleModal").attr("style", "display: block");
                UnBlockUi();
            }

            console.log("Request: " + XMLHttpRequest.toString() + "\n\nStatus: " + textStatus + "\n\nError: " + errorThrown);
        },
        success: function (response) {
            FillDriverDropdown(response, currentDriver);
            if (isAsync) {
                $("#manageVehicleModal").attr("style", "display: block");
                UnBlockUi();
            }
        }
    });
}

function FillDriverDropdown(response, currentDriver) {

    var options = '<option value="-1">-- Select driver --</option>';

    for (var i = 0; i < response.length; i++) {
        if (response[i].Driver && response[i].VehicleId == null) {
            options += '<option value="' + response[i].Id + '">' + response[i].FirstName + " " + response[i].LastName + '</option>'
        }
    }

    $("#drpVehicleDrivers").html(options);

    if (currentDriver != undefined && currentDriver != null) {
        $("#drpVehicleDrivers").append(currentDriver);
        $("#drpVehicleDrivers").val($(currentDriver).attr("value"));
    }
    else {
        $("#drpVehicleDrivers").val("-1");
    }

}

function RefreshManageModal() {

    $("#selectCarType").val("1");
    $("#drpModelsLux").hide();
    $("#drpModelsSuv").hide();
    $("#drpModelsBlack").show();

    $("#spnModel").html("Audi")
    $("#drpModelsBlack").val("A6");

    carMake = 'Audi';
    carModel = 'A6';

    $("#txtColor").val("");
    $("#txtLicencePlate").val("");
    $("#rideDate").val("");
    $(".doc-hidden-inputs").val("");
    $("#currVehicleId").val("0");

    $(".files-uploaders").show();
    $(".fileupload-preview").hide();
    $(".doc-hidden-inputs").removeAttr("data-fileid");

    $("#btnSaveVehicle").parent().parent().hide();

    $("#form-t-0").click();
    $("label.error").remove();
    $(".error").removeClass("error");

    $(".exp-date-input").val("");
}
