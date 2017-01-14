﻿
var UploadType = {
    Passport: { value: 0, name: "Passport", code: "S" },
    Insurance: { value: 1, name: "Insurance", code: "M" },
    ProofOfAddress: { value: 2, name: "ProofOfAddress", code: "L" },
    Photo: { value: 3, name: "Photo", code: "S" },
    License: { value: 4, name: "License", code: "M" },
    Other: { value: 5, name: "Other", code: "L" }
};
$(document).ready(function () {

    if (Modernizr.touch) {
        // show the close overlay button
        $(".close-overlay").removeClass("hidden");
        // handle the adding of hover class when clicked
        $(".img").click(function (e) {
            if (!$(this).hasClass("hover")) {
                $(this).addClass("hover");
            }
        });
        // handle the closing of the overlay
        $(".close-overlay").click(function (e) {
            e.preventDefault();
            e.stopPropagation();
            if ($(this).closest(".img").hasClass("hover")) {
                $(this).closest(".img").removeClass("hover");
            }
        });
    } else {
        // handle the mouseenter functionality
        $(".img").mouseenter(function () {
                $(this).addClass("hover");
        })
            // handle the mouseleave functionality
            .mouseleave(function () {
                $(this).removeClass("hover");
            });
    }

    //Initialize tooltips
    $('.nav-tabs > li a[title]').tooltip();
    //Wizard
    $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {

        var $target = $(e.target);

        if ($target.parent().hasClass('disabled')) {
            return false;
        }
    });

    $(".next-step").click(function (e) {

        var $active = $('.wizard .nav-tabs li.active');
        $active.next().removeClass('disabled');
        nextTab($active);

    });
    $(".prev-step").click(function (e) {

        var $active = $('.wizard .nav-tabs li.active');
        prevTab($active);

    });

    $(document).on('click', ".acceptDriverDocument", function (e) {
        e.preventDefault();
        var documentId = $(this).parent().parent().attr('data-id');
        ApproveDriverDocument(documentId, e);
    });

    $(document).on('click', ".previewDriverDocument", function (e) {
        e.preventDefault();
        var documentId = $(this).parent().parent().attr('data-id');
    });

    $(document).on('click', ".rejectDriverDocument", function (e) {
        e.preventDefault();
        var documentId = $(this).parent().parent().attr('data-id');
        RejectDriverDocument(documentId, e);
    });

});

function nextTab(elem) {
    $(elem).next().find('a[data-toggle="tab"]').click();
}
function prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
}
function getDriverDocuments(driverId) {
    window.BlockUi();
    ClearModal();
    $.post("/Drivers/GetDriverDocuments",
        { driverId: driverId },
        function (data) {
            $("#previewDriver").modal("show");
            window.UnBlockUi();
            InitDocuments(data);
        });
}

function ApproveDriverDocument(documentId, e) {
    var currentButton = $(e.currentTarget);
    if (documentId) {
        window.BlockUi();
        $.post("/Drivers/ApproveDriverDocument",
            { documentId: documentId },
            function (data) {
                window.UnBlockUi();
                if (data.Success) {
                    currentButton.addClass('disabled approveSelected');
                    for (let i = 0; i < data.Messages.length; i++) {
                        toastr.success(data.Messages[i].Value);
                    }
                } else {
                    for (let i = 0; i < data.Messages.length; i++) {
                        toastr.error(data.Messages[i].Value);
                    }
                }
            });
    }
}

function RejectDriverDocument(documentId, e) {
    var currentButton = $(e.currentTarget);
    var approveButton = currentButton.closest(".row").find(".acceptDriverDocument");
    if (documentId != undefined) {
        var data = {
            DocumentId: documentId,
            RejectionComment: "ddd"
        };

        window.BlockUi();
        $.post("/Drivers/RejectDriverDocument",
            { model: data },
            function (data) {
                window.UnBlockUi();
                if (data.Success) {
                    currentButton.addClass('disabled rejectSelected');
                    approveButton.addClass('disabled');
                    for (let i = 0; i < data.Messages.length; i++) {
                        toastr.success(data.Messages[i].Value);
                    }
                } else {
                    for (let i = 0; i < data.Messages.length; i++) {
                        toastr.error(data.Messages[i].Value);
                    }
                }
            });
    }
}

function ClearModal() {
    var proofOfAdddress = document.getElementById(UploadType.ProofOfAddress.name);
    $('#proofOfAddressOverlay').removeClass('overlay');
    proofOfAdddress.setAttribute("src", "Images/proofOFAddres.png");
    proofOfAdddress.setAttribute("class", "no-img");
    var insurance = document.getElementById(UploadType.Insurance.name);
    $('#insuranceOverlay').removeClass('overlay');
    insurance.setAttribute("src", "Images/insurance.png");
    insurance.setAttribute("class", "no-img");
    var license = document.getElementById(UploadType.License.name);
    $('#licenseOverlay').removeClass('overlay');
    license.setAttribute("src", "Images/license.png");
    license.setAttribute("class", "no-img");
    var passport = document.getElementById(UploadType.Passport.name);
    $('#passportOverlay').removeClass('overlay');
    passport.setAttribute("src", "Images/passport.png");
    passport.setAttribute("class", "no-img");
}

function InitDocuments(data) {
    var result = JSON.parse(data);
    for (var i = 0; i < result.length; i++) {
        var documentId = result[i].Id;
        var docType = result[i].UploadType;
        var documentState = result[i].DocumentState;
        var filePath = result[i].FileName;
        var fullPath = createDocumentFullPath(docType, filePath);
        createDocumentsView(fullPath, documentId, docType, result);
    }
}

function createDocumentsView(fullPath, documentId, docType, data) {
    var elem = document.createElement("img");
    elem.setAttribute("src", fullPath);
    elem.setAttribute("data-id", documentId);
    elem.setAttribute("class", "driverDocuments");

    switch (docType) {
        case UploadType.ProofOfAddress.value:
            var proofOfAdddress = document.getElementById(UploadType.ProofOfAddress.name);
            var proofOfAdddressOverlay = document.getElementById('proofOfAddressOverlay');
            proofOfAdddressOverlay.setAttribute("class", "overlay");
            proofOfAdddress.setAttribute("src", fullPath);
            proofOfAdddress.removeAttribute("class");
            proofOfAdddress.setAttribute("data-id", documentId);
            proofOfAdddressOverlay.setAttribute("data-id", documentId);
            var proofOfAddressconteiner = $("." + UploadType.ProofOfAddress.name);
            proofOfAddressconteiner.attr("data-id", documentId);

            setStatuses(UploadType.ProofOfAddress.name, data.filter(a => a.UploadType === UploadType.ProofOfAddress.value)[0].DocumentStatus);

            break;
        case UploadType.Insurance.value:
            var insurance = document.getElementById(UploadType.Insurance.name);
            var insuranceOverlay = document.getElementById('insuranceOverlay');
            insuranceOverlay.setAttribute("class", "overlay");
            insurance.setAttribute("src", fullPath);
            insurance.setAttribute("data-id", documentId);
            insuranceOverlay.setAttribute("data-id", documentId);
            insurance.removeAttribute("class");
            var insuranceConteiner = $("." + UploadType.Insurance.name);
            insuranceConteiner.attr("data-id", documentId);

            setStatuses(UploadType.Insurance.name, data.filter(a => a.UploadType === UploadType.Insurance.value)[0].DocumentStatus);

            break;
        case UploadType.License.value:
            var license = document.getElementById(UploadType.License.name);
            var licenseOverlay = document.getElementById('licenseOverlay');
            licenseOverlay.setAttribute("class", "overlay");
            license.setAttribute("src", fullPath);
            license.setAttribute("data-id", documentId);
            licenseOverlay.setAttribute("data-id", documentId);
            license.removeAttribute("class");
            var licenseConteiner = $("." + UploadType.License.name);
            licenseConteiner.attr("data-id", documentId);
            setStatuses(UploadType.License.name, data.filter(a => a.UploadType === UploadType.License.value)[0].DocumentStatus);

            break;
        case UploadType.Passport.value:
            var passport = document.getElementById(UploadType.Passport.name);
            var passportClass = document.getElementById('passportOverlay');
            passportClass.setAttribute("class", "overlay");
            passport.setAttribute("src", fullPath);
            passport.setAttribute("data-id", documentId);
            passportClass.setAttribute("data-id", documentId);
            passport.removeAttribute("class");
            var passportConteiner = $("." + UploadType.Passport.name);
            passportConteiner.attr("data-id", documentId);

            setStatuses(UploadType.Passport.name, data.filter(a => a.UploadType === UploadType.Passport.value)[0].DocumentStatus);
                
            break;
        //case UploadType.Photo.value:
        //    var photo = document.getElementById(UploadType.Photo.name);
        //    photo.setAttribute("src", fullPath);
        //    photo.setAttribute("data-id", documentId);
        //    photo.removeAttribute("class");
        //    var photoConteiner = $("." + UploadType.Photo.name);
        //    photoConteiner.attr("data-id", documentId);
        //    setStatuses(UploadType.Photo.name, data.filter(a => a.UploadType === UploadType.Photo.value)[0].DocumentStatus);

        //    break;
        default:
            break;
    }

}

function setStatuses(uploadTypeNmae, documentStatus) {
    if (documentStatus === 1) {
        $("." + uploadTypeNmae + " .acceptDriverDocument").addClass('disabled approveSelected');
    } else if (documentStatus === 2) {
        $("." + uploadTypeNmae + " .rejectDriverDocument").addClass('disabled rejectSelected');
        $("." + uploadTypeNmae + " .acceptDriverDocument").addClass('disabled');
    }
}

function createDocumentFullPath(doctype, fileName) {
    var filePath = "/Uploads/DriverDocuments/";
    switch (doctype) {
        case UploadType.ProofOfAddress.value:
            filePath += UploadType.ProofOfAddress.name;
            break;
        case UploadType.Insurance.value:
            filePath += UploadType.Insurance.name;
            break;
        case UploadType.License.value:
            filePath += UploadType.License.name;
            break;
        case UploadType.Passport.value:
            filePath += UploadType.Passport.name;
            break;
        case UploadType.Photo.value:
            filePath += UploadType.Photo.name;
            break;
        default:
            filePath += "Other";
            break;
    }
    filePath += "/" + fileName;
    return filePath;
}