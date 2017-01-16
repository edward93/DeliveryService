$(document).ready(function () {

    var businessName = $("#businessName");
    var businessPhone = $("#businessPhone");
    var businessEmail = $("#businessEmail");
    var contPersFName = $("#contPersFName");
    var contPersLName = $("#contPersLName");
    var contPersPhone = $("#contPersPhone");
    var addressLine1 = $("#addressLine1");
    var addressLine2 = $("#addressLine2");
    var country = $("#country");
    var city = $("#city");
    var state = $("#state");
    var zipCode = $("#zipCode");
    var submitBussinessBtn = $("#submitBussinessBtn");
    var heading = $("#Heading");
    var submitBusinessForm = $("#submitBusinessForm");


    $("#addNewBusinessBtn").on("click",
        function (e) {
            e.preventDefault();
            clearBusinessModal();
            submitBussinessBtn.val("Create");
            heading.text("Add New Business");
            $("#addNewBusinessModal").modal("show");
        });

    $("#submitBussinessBtn").on("click",
       function (e) {
           e.preventDefault();
           var form = $("#submitBusinessForm");
           var businessId = form.attr("data-id");
           if (businessId > 0)
               editBusiness();
           else
               addBusiness(form);
       });

    $(".btnPreviewBusiness").on("click",
        function (e) {
            var businessIdForPreview = $(this).attr('data-id');
            e.preventDefault();
            clearBusinessModal();
            submitBussinessBtn.val("Update");
            heading.text("Update Business");
            getBusinessById(businessIdForPreview);
        });

    $(document).on('click',
      '.btnDeleteBusiness',
      function () {
          var businessIdForDelete = $(this).attr('data-id');
          swal({
              title: "Are you sure?",
              text: "You will not be able to recover your business!",
              type: "warning",
              showCancelButton: true,
              confirmButtonColor: "#DD6B55",
              confirmButtonText: "Yes, delete!",
              closeOnConfirm: true
          },
              function (isConfirm) {
                  if (isConfirm)
                      deleteBusiness(parseInt(businessIdForDelete));
              });
      });

    function getBusinessById(businessId) {
        window.BlockUi();
        $.get("/Business/GetBusiness",
            { businessId: businessId },
            function (data) {
                initBusinessModal(data);
                $("#addNewBusinessModal").modal("show");
                window.UnBlockUi();
            });
    }

    function editBusiness() {
        var data = {
            BusinessName: businessName.val(),
            BusinessPhone: businessPhone.val(),
            BusinessEmail: businessEmail.val(),
            ContactPersonFirstName: contPersFName.val(),
            ContactPersonLastName: contPersLName.val(),
            ContactPersonPhoneNumber: contPersPhone.val(),
            AddressLine1: addressLine1.val(),
            AddressLine2: addressLine2.val(),
            Country: country.val(),
            City: city.val(),
            State: state.val(),
            ZipCode: zipCode.val(),
            BusinessId: submitBusinessForm.attr("data-id")
        };

        window.BlockUi();
        $.post("/Business/UpdateBusiness",
                  { registerBusiness: data },
                  function (data) {
                      window.UnBlockUi();
                      if (data.Success) {
                          for (let i = 0; i < data.Messages.length; i++) {
                              window.toastr.success(data.Messages[i].Value);
                          }
                      } else {
                          for (let i = 0; i < data.Messages.length; i++) {
                              window.toastr.error(data.Messages[i].Value);
                          }
                      }
                      $("#addNewBusinessModal").modal('toggle');
                  });
    }

    function addBusiness(form) {
        $.post("/Business/RegisterBusiness",
                                form.serialize(), function (data) {
                                    window.UnBlockUi();
                                    if (data.Success) {
                                        for (let i = 0; i < data.Messages.length; i++) {
                                            window.toastr.success(data.Messages[i].Value);
                                        }
                                    } else {
                                        for (let i = 0; i < data.Messages.length; i++) {
                                            window.toastr.error(data.Messages[i].Value);
                                        }
                                    }
                                    $("#addNewBusinessModal").modal('toggle');
                                });
    }

    function initBusinessModal(data) {
        data = JSON.parse(data);
        businessName.val(data.BusinessName);
        businessPhone.val(data.BusinessPhone);
        businessEmail.val(data.BusinessEmail);
        contPersFName.val(data.ContactPersonFirstName);
        contPersLName.val(data.ContactPersonLastName);
        contPersPhone.val(data.ContactPersonPhoneNumber);
        addressLine1.val(data.AddressLine1);
        addressLine2.val(data.AddressLine2);
        country.val(data.Country);
        city.val(data.City);
        state.val(data.State);
        zipCode.val(data.ZipCode);
        submitBusinessForm.attr("data-id", data.BusinessId);
    }

    function clearBusinessModal() {
        businessName.val("");
        businessPhone.val("");
        businessEmail.val("");
        contPersFName.val("");
        contPersLName.val("");
        contPersPhone.val("");
        addressLine1.val("");
        addressLine2.val("");
        country.val("");
        city.val("");
        state.val("");
        zipCode.val("");
        submitBusinessForm.attr("data-id", 0);
    }

    function deleteBusiness(businessId) {
        window.BlockUi();
        $.post("/Business/DeleteBusiness",
            { businessId: businessId },
            function (data) {
                window.UnBlockUi();
                $("#business_" + businessId).remove();
                swal("Deleted!", "Your business has been deleted.", "success");
            });
    }
});