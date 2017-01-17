$(document).ready(function () {

    var businessName = $("#businessName");
    var businessPhone = $("#businessPhone");
    var businessEmail = $("#businessEmail");
    var password = $("#password");
    var confirmPassword = $("#confirmPassword");
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
    var valModel = {
        businessName: 4,
        businessPhone: 4,
        businessEmail: 5,
        password: 6,
        confirmPassword: 6,
        contPersFName: 2,
        contPersLName: 2,
        contPersPhone: 2,
        addressLine1:2,
        addressLine2: 2,
        country: 2,
        city: 2,
        state: 2,
        zipCode: 2
    };


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
           validate(businessName, businessName, valModel.businessName);
           validate(businessPhone, businessPhone, valModel.businessPhone);
           validate(businessEmail, businessEmail, valModel.businessEmail);
           validate(password, password, valModel.password);
           validate(confirmPassword, confirmPassword, valModel.confirmPassword);
           validate(contPersFName, contPersFName, valModel.contPersFName);
           validate(contPersLName, contPersLName, valModel.contPersLName);
           validate(contPersPhone, contPersPhone, valModel.contPersPhone);
           validate(addressLine1, addressLine1, valModel.addressLine1);
           validate(addressLine2, addressLine2, valModel.addressLine2);
           validate(country, country, valModel.country);
           validate(city, city, valModel.city);
           validate(state, state, valModel.state);
           validate(zipCode, zipCode, valModel.zipCode);
           var form = $("#submitBusinessForm");
           var businessId = form.attr("data-id");
           if (businessId > 0)
               editBusiness();
           else
               addBusiness(form);
       });
    //validation
    function validate(validationObj, validationObjValue, validationModel) {
        if ($(validationObjValue).val().length >= validationModel) {
            validationObj.removeClass('invalid');
            validationObj.addClass('valid');
            if ($(password).val() !== $(confirmPassword).val()) {
                $(password).removeClass('valid');
                $(confirmPassword).removeClass('valid');
                $(password).addClass('invalid');
                $(confirmPassword).addClass('invalid');
                password.val("");
                confirmPassword.val("");
                $(confirmPassword).attr("title", "Password and Confirm Password must be the same");
            }
        } else {
            validationObj.removeClass('valid');
            validationObj.addClass('invalid');
            validationObj.attr("title", "Must be greater than" + ' ' + validationModel + ' ' + "symbols");
            if (password.val() !== confirmPassword.val()) {
                $(password).attr("title", "Password and Confirm Password must be the same");
            }
        }
    }
    //validation end
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
       // $.post("/Business/RegisterBusiness",
         //                       form.serialize(), function (data) {
           //                         window.UnBlockUi();
             //                       if (data.Success) {
               //                         for (let i = 0; i < data.Messages.length; i++) {
                 //                           window.toastr.success(data.Messages[i].Value);
                   //                     }
                     //               } else {
                       //                 for (let i = 0; i < data.Messages.length; i++) {
                         //                   window.toastr.error(data.Messages[i].Value);
                           //             }
                             //       }
                                  //  $("#addNewBusinessModal").modal('toggle');
                                //});
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
        password.val("");
        confirmPassword.val("");
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
        $('.form-control').removeClass('valid');
        $('.form-control').removeClass('invalid');
        //$('.form-control').attr('autocomplete', 'off');
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