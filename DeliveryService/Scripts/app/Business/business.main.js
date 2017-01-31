$(document).ready(function () {

    /* $('#tblBusinessList').DataTable({
         dom: '<"html5buttons"B>lTfgitp',
         buttons: [
             { extend: 'copy' },
             { extend: 'csv' },
             { extend: 'excel', title: 'Business' },
             {
                 extend: 'pdf', title: 'Business',
                 exportOptions: {
                     columns: [0, 1, 2, 3]
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
     });*/

   var country = $("#country").selectize({
        searchField: ['text'],
        maxItems: 1,
        allowEmptyOption: false,
        plugins: ['remove_button'],
        preload: true,
        //options: countries,
        load: function (query, callback) {
            $.post("/home/GetCountries")
              .done(function (data) {
                  if (data !== '') {
                      callback(data);
                  }
              }).fail(function (xmlHttpRequest, textStatus, errorThrown) {
              });
        }
    });

    var tableBusinessList = $('#tblBusinessList').dataTable({
        "processing": true, // control the processing indicator.
        "serverSide": true, // recommended to use serverSide when data is more than 10000 rows for performance reasons
        "info": true,   // control table information display field
        "stateSave": false,  //restore table state on page reload,
        "lengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]],    // use the first inner array as the page length values and the second inner array as the displayed options
        "ajax": {
            "url": "/Business/GetBusinessList",
            "type": "GET"
        },
        "columns": [
            { "data": "BusinessName", "orderable": false },
            { "data": "ContactPersonPhoneNumber", "orderable": true },
            { "data": "BusinessEmail", "orderable": true },
            { "data": "Approved", "orderable": true },
            { "data": "RatingAverageScore", "orderable": true },
             {
                 mRender: function (data, type, row) {
                     return '<div class="btn-group"> <button class="btn btn-primary btn-xs btnPreviewBusiness" data-title="Preview" data-id="' +
                         row.Id +
                         '" >' +
                         '<span class="fa fa-eye" title="Preview"></span></button>' +
                         '<button class="btn btn-danger btn-xs" data-title="Delete" data-id="' +
                         row.Id +
                         '" id="btnDeleteDriver">' +
                         '<span class="glyphicon glyphicon-trash" title="Delete"></span>' +
                         '</button> </div>';
                 }
             }
        ],
        "order": [[0, "asc"]]
    });


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
    //var country = $("#country");
    var city = $("#city");
    var state = $("#state");
    var zipCode = $("#zipCode");
    var submitBussinessBtn = $("#submitBussinessBtn");
    var heading = $("#Heading");
    var submitBusinessForm = $("#submitBusinessForm");

    var validator = $("#submitBusinessForm").validate({
        rules: {
            email: {
                required: true,
                email: true
            },
            BusinessName: {
                required: true,
                minlength: 3,
                maxlength: 20
            },
            PhoneNumber: {
                required: true,
                minlength: 3,
                maxlength: 20,
                number: true
            },
            Password: {
                required: true,
                minlength: 6,
                maxlength: 20
            },
            ConfirmPassword: {
                equalTo: "#password"
            },
            ContactPersonFirstName: {
                required: true,
                minlength: 2,
                maxlength: 20
            },
            ContactPersonLastName: {
                required: true,
                minlength: 2,
                maxlength: 20
            },
            ContactPersonPhoneNumber: {
                required: true,
                minlength: 3,
                maxlength: 20,
                number: true
            },
            AddressLine1: {
                required: true,
                minlength: 3,
                maxlength: 20
            },
            City: {
                required: true,
                minlength: 3,
                maxlength: 20
            },
            ZipCode: {
                required: true,
                minlength: 3,
                maxlength: 20
            }
        },
        highlight: function (element) {
            if (element.id === 'country') {
                $(element).addClass('invalid').removeClass('valid');
            } else {
                var icon = $(element).closest('div')[0].lastElementChild;
                $(icon).removeClass('glyphicon-ok-sign').addClass('glyphicon-remove-sign');
            }
        },
        unhighlight: function (element) {
            if (element.id === "country") {
                $(element).addClass('valid').removeClass('invalid');
            } else {
                var icon = $(element).closest('div')[0].lastElementChild;
                $(icon).removeClass('glyphicon-remove-sign').addClass('glyphicon-ok-sign');
            }
        },
        errorElement: 'span',
        errorClass: 'help-block',
        errorPlacement: function (error, element) {
            if (element.length) {
                error.insertAfter(element);
            } else {
                error.insertAfter(element);
            }
        }
    });

    $('#step1').show();

    $("#addNewBusinessBtn").on("click",
        function (e) {
            e.preventDefault();
            clearBusinessModal();
            submitBussinessBtn.val("Create");
            heading.text("Add New Business");
            $("#addNewBusinessModal").modal("show");

            $('#collapse').attr("style", "");
            $('#collapse').show();
            $(".wizard-inner").hide();
            $('#step1').hide();
            $('#step2').show();
            $('#BusinessEmail').show();
        });

    $("#submitBussinessBtn").on("click",
       function (e) {
           var form = $("#submitBusinessForm");
           form.validate();
           form.valid();
           validator.form();
           e.preventDefault();
           var businessId = form.attr("data-id");
           if (businessId > 0)
               editBusiness();
           else
               if (validator.form()) {
                   addBusiness(form);
               } else {
                   $("#addNewBusinessModal").modal("show");
                   return;
               }
       });

    function clearModal() {
        var passport = document.getElementById("Passport");
        passport.setAttribute("src", "Images/passport.png");
        passport.setAttribute("class", "no-img");
        var license = document.getElementById("License");
        license.setAttribute("src", "Images/license.png");
        license.setAttribute("class", "no-img");
        $(".wizard-inner").show();
        $("#businessDocument").parent().attr("class", "active");
        $("#businessData").parent().attr("class", "");
        $('#step2').hide();
        $('#step1').show();
        $('#collapse').hide();
        $('#BusinessEmail').hide();
    };
    $(document).on('click', '.btnPreviewBusiness', function (e) {
        var businessIdForPreview = $(this).attr('data-id');
        clearModal();
        validator.form();
        e.preventDefault();
        clearBusinessModal();
        submitBussinessBtn.val("Update");
        heading.text("Update Business");
        getBusinessById(businessIdForPreview);
    });

    $(document).on('click', '#businessData', function () {
        $('#step1').hide();
        $('#step2').show();
    });
    $(document).on('click', '#businessDocument', function () {
        $('#step2').hide();
        $('#step1').show();
    });

    $(document).on('click', '#btnDeleteDriver',
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

    $("input").keydown(function () {
        validator.element(this);
    });

    function getBusinessById(businessId) {
        window.BlockUi();
        $.get("/Business/GetBusiness",
        {
            businessId: businessId
        },
            function (data) {
                initBusinessModal(data);
                $("#addNewBusinessModal").modal("show");
                window.UnBlockUi();
            });
    }

    function editBusiness() {
        var data = {
            BusinessName: businessName.val(),
            PhoneNumber: businessPhone.val(),
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
        {
            registerBusiness: data
        },
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
                      if (validator.form()) {
                          $("#addNewBusinessModal").modal('toggle');
                      }
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
        businessPhone.val(data.PhoneNumber);
        businessEmail.val(data.BusinessEmail);
        contPersFName.val(data.ContactPersonFirstName);
        contPersLName.val(data.ContactPersonLastName);
        contPersPhone.val(data.ContactPersonPhoneNumber);
        addressLine1.val(data.AddressLine1);
        addressLine2.val(data.AddressLine2);
        //var countrySelectize = country.selectize();
        //countrySelectize.setValue(data.Country);
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
        validator.resetForm();
        $('.message-icon').removeClass('glyphicon-ok-sign');
        $('.message-icon').removeClass('glyphicon-remove-sign');
        $('#country').removeClass('valid');
        $('#country').removeClass('invalid');
    }

    function deleteBusiness(businessId) {
        window.BlockUi();
        $.post("/Business/DeleteBusiness",
        {
            businessId: businessId
        },
            function () {
                window.UnBlockUi();
                $("#business_" + businessId).remove();
                swal("Deleted!", "Your business has been deleted.", "success");
            });
    }
});