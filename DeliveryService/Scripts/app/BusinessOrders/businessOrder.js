
$(document).ready(function () {

    if (window.location.href.indexOf("BusinessOrders#addOrder") !== -1) {
        $("#addOrderModal").modal("show");
    }

    window.history.pushState("object or string", "Title", "/BusinessOrder/BusinessOrders");

    $(".location").keypress(function () {
        //getRoute();
    });

    var tableDriversList = $('#tblBusinessOrderList').dataTable({
        "processing": true, // control the processing indicator.
        "serverSide": true, // recommended to use serverSide when data is more than 10000 rows for performance reasons
        "info": true,   // control table information display field
        "stateSave": false,  //restore table state on page reload,
        "responsive": true,
        "columnDefs": [
           { responsivePriority: 1, targets: 0 },
           { responsivePriority: 2, targets: -2 }
           //{className: "dt-center", targets: "_all"}
        ],
        "lengthMenu": [[10, 20, 50], [10, 20, 50]],    // use the first inner array as the page length values and the second inner array as the displayed options
        "ajax": {
            "url": "/BusinessOrder/GetBusinessOrdesList",
            "type": "GET"
        },
        "columns": [
            { "data": "CustomerName", "orderable": true },
            { "data": "CustomerPhone", "orderable": true },
            { "data": "OrderNumber", "orderable": true },
            { "data": "TimeToReachPickUpLocation", "orderable": true },
            { "data": "TimeToReachDropOffLocation", "orderable": true },
            { "data": "OrderStatus", "orderable": true },
            {
                mRender: function (data, type, row) {
                    var retryBtnHtml =
                        '<button class="btn btn-success btn-xs btnRetry" style="" data-title="Retry" data-id="' +
                            row.Id +
                            '" id="btnRetry"><span class="fa fa-repeat" title="Retry"></span></button>';
                    var previewBtnHtml = '<button class="btn btn-primary btn-xs btnPreviewOrderDetails" style="" data-title="Preview" data-id="' +
                        row.Id +
                        '" id="btnPreviewOrder"><span class="fa fa-eye" title="Preview"></span></button>';
                    if (row.OrderStatus === 'Pending' || row.OrderStatus === 'Rejected By Driver') {
                        return previewBtnHtml + retryBtnHtml;

                    } else {
                        return previewBtnHtml;
                    }
                }
            }
        ],
        "order": [[0, "desc"]]
    });


    var customerName = $("#customerName");
    var customerPhone = $("#customerPhone");
    var timeToReachPickUpLocation = $("#timeToReachPickUpLocation");
    var timeToReachDropOffLocation = $("#timeToReachDropOffLocation");
    var pickUpLocation = $("#pickUpLocation");
    var dropOffLocation = $("#dropOffLocation");
    var orderNumber = $("#orderNumber");
    var vehicleType = $("#vehicleType-selectized");
    var submitOrderForm = $("#submitOrderForm");

    var validator = $("#submitOrderForm").validate({
        rules: {
            CustomerName: {
                required: true
            },
            CustomerNumber: {
                required: true
            },
            TimeToReachPickUpLocation: {
                required: true
            },
            TimeToReachDropOffLocation: {
                required: true
            },
            PickUpLocation: {
                required: true
            },
            DropOffLocation: {
                required: true
            },
            OrderNumber: {
                required: true
            }
        },
        highlight: function (element) {
            if (element.id === 'vehicleType-selectized') {
                $(element).addClass('invalid').removeClass('valid');
            } else {
                var icon = $(element).closest('div')[0].lastElementChild;
                $(icon).removeClass('glyphicon-ok-sign').addClass('glyphicon-remove-sign');
            }
        },
        unhighlight: function (element) {
            if (element.id === "vehicleType-selectized") {
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

    $("input").keydown(function () {
        validator.element(this);
    });

    function clearOrdersModal() {
        customerName.val("");
        customerPhone.val("");
        timeToReachPickUpLocation.val("");
        timeToReachDropOffLocation.val("");
        pickUpLocation.val("");
        dropOffLocation.val("");
        orderNumber.val("");
        vehicleType.val("");
        validator.resetForm();
        $('.message-icon').removeClass('glyphicon-ok-sign');
        $('.message-icon').removeClass('glyphicon-remove-sign');
        $('#vehicleType-selectized').removeClass('valid');
        $('#vehicleType-selectized').removeClass('invalid');
    }

    $(document).on('click', ".btnPreviewOrderDetails", function () {
        var orderId = $(this).attr('data-id');
        window.BlockUi();
        $.post("/BusinessOrder/GetOrderDetails",
            {
                orderId: orderId
            },
            function (data) {
                window.UnBlockUi();
                initPreviewOrderModal(data);
                $("#previewOrderDetailsModal").modal("show");
            });



    });

    $(document)
        .on("click", ".btnRetry",
            function (e) {
                var orderId = $(this).attr('data-id');

                e.preventDefault();
                window.BlockUi();
                $.post("/BusinessOrder/RetryOrder",
                    {
                        orderId: orderId
                    },
                    function (data) {
                        window.UnBlockUi();
                        
                        if (data.Success) {
                            for (let i = 0; i < data.Messages.length; i++) {
                                if (data.Messages[i].Key === 1) {
                                    window.toastr.warning(data.Messages[i].Value);
                                } else {
                                    window.toastr.success(data.Messages[i].Value);
                                }
                            }
                        } else {
                            for (let i = 0; i < data.Messages.length; i++) {
                                if (data.Messages[i].Key === 1) {
                                    window.toastr.warning(data.Messages[i].Value);
                                } else {
                                    window.toastr.error(data.Messages[i].Value);
                                }
                            }
                        }
                    });

            });

    $("#addOrderBtn").on("click",
        function (e) {
            var form = $("#submitOrderForm");
            form.validate();
            form.valid();
            validator.form();
            e.preventDefault();
            if (validator.form()) {
                addOrder(form);
            } else {
                $("#addOrderModal").modal("show");
                return;
            }
            $("#addOrderModal").modal("hide");
        });

    $(document).on('click', "#btnMakeNewOrder", function () {
        clearOrdersModal();
        $("#addOrderModal").modal("show");
    });

    function addOrder(form) {
        getLongLatPickUp();
        getLongLatDropOff();

        var data = form.serialize();

        $.post("/BusinessOrder/AddNewOrder",
            data, function (data) {
                window.UnBlockUi();
                tableDriversList.fnDraw();
                if (data.Success) {
                    for (let i = 0; i < data.Messages.length; i++) {
                        if (data.Messages[i].Key === 1) {
                            window.toastr.warning(data.Messages[i].Value);
                        } else {
                            window.toastr.success(data.Messages[i].Value);
                        }
                    }
                } else {
                    for (let i = 0; i < data.Messages.length; i++) {
                        if (data.Messages[i].Key === 1) {
                            window.toastr.warning(data.Messages[i].Value);
                        } else {
                            window.toastr.error(data.Messages[i].Value);
                        }
                    }
                }
            });
    }

    $("#vehicleType").selectize({
        searchField: ['text'],
        maxItems: 1,
        allowEmptyOption: false,
        plugins: ['remove_button'],
        preload: true,
        load: function (query, callback) {
            $.post("/home/GetVehicleTypes")
                .done(function (data) {
                    if (data !== '') {
                        callback(data);
                    }
                }).fail(function (xmlHttpRequest, textStatus, errorThrown) {
                });
        }
    });

    var rideType;
    var source, destination;
    var directionsDisplay;
    var map;
    var directionsService = new google.maps.DirectionsService();
    var mapElement = document.getElementById('map_canvas');
    function getMapStart() {
        var txtLatitude = $("#txtLatitude");
        var txtLongitude = $("#txtLongitude");
        return {
            zoom: 12,
            center: new window.google.maps.LatLng(txtLatitude.val(), txtLongitude.val()),
            styles: [
                {
                    "featureType": "water",
                    "stylers": [
                        { "saturation": 43 },
                        { "lightness": -11 },
                        { "hue": "#0088ff" }
                    ]
                },
                {
                    "featureType": "road",
                    "elementType": "geometry.fill",
                    "stylers": [
                        { "hue": "#ff0000" },
                        { "saturation": -100 },
                        { "lightness": 99 }
                    ]
                },
                {
                    "featureType": "road",
                    "elementType": "geometry.stroke",
                    "stylers": [
                        { "color": "#808080" },
                        { "lightness": 54 }
                    ]
                },
                {
                    "featureType": "landscape.man_made",
                    "elementType": "geometry.fill",
                    "stylers": [
                        { "color": "#ece2d9" }
                    ]
                },
                {
                    "featureType": "poi.park",
                    "elementType": "geometry.fill",
                    "stylers": [
                        { "color": "#ccdca1" }
                    ]
                },
                {
                    "featureType": "road",
                    "elementType": "labels.text.fill",
                    "stylers": [
                        { "color": "#767676" }
                    ]
                },
                {
                    "featureType": "road",
                    "elementType": "labels.text.stroke",
                    "stylers": [
                        { "color": "#ffffff" }
                    ]
                },
                {
                    "featureType": "poi",
                    "stylers": [
                        { "visibility": "off" }
                    ]
                },
                {
                    "featureType": "landscape.natural",
                    "elementType": "geometry.fill",
                    "stylers": [
                        { "visibility": "on" },
                        { "color": "#b8cb93" }
                    ]
                },
                {
                    "featureType": "poi.park",
                    "stylers": [
                        { "visibility": "on" }
                    ]
                },
                {
                    "featureType": "poi.sports_complex",
                    "stylers": [
                        { "visibility": "on" }
                    ]
                },
                {
                    "featureType": "poi.medical",
                    "stylers": [
                        { "visibility": "on" }
                    ]
                },
                {
                    "featureType": "poi.business",
                    "stylers": [
                        { "visibility": "simplified" }
                    ]
                }]
        };
    }

    function clearMap() {
        map = new window.google.maps.Map(mapElement, getMapStart());
        directionsDisplay.setMap(null);
    }

    var lblCustomerName = $("#lblCustomerName");
    var lblCustomerPhone = $("#lblCustomerPhone");
    var lblTimeToReachPickUpLocation = $("#lblTimeToReachPickUpLocation");
    var lblTimeToReachDropOffLocation = $("#lblTimeToReachDropOffLocation");
    var lblPickUpLocation = $("#lblPickUpLocation");
    var lblDropOffLocation = $("#lblDropOffLocation");
    var lblOrderNumber = $("#lblOrderNumber");

    function initPreviewOrderModal(data) {
        data = data.Data;
        lblCustomerName.text(data.CustomerName);
        lblCustomerPhone.text(data.CustomerPhone);
        lblTimeToReachPickUpLocation.text(data.TimeToReachPickUpLocation);
        lblTimeToReachDropOffLocation.text(data.TimeToReachDropOffLocation);
        lblPickUpLocation.text(data.PickUpLocation);
        lblDropOffLocation.text(data.DropOffLocation);
        lblOrderNumber.text(data.OrderNumber);
    }

    function getRoute() {
        var mapOptions = getMapStart();
        map = new window.google.maps.Map(document.getElementById('map_canvas'), mapOptions);
        directionsDisplay.setMap(map);

        source = $("#pickUpLocation").val();
        destination = $("#dropOffLocation").val();
        if (destination === "" && source !== "") {
            destination = source;
        }
        else if (source === "" && destination !== "") {
            source = destination;
        }


        var request = {
            origin: source,
            destination: destination,
            travelMode: window.google.maps.TravelMode.DRIVING
        };
        directionsService.route(request, function (response, status) {
            if (status === window.google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(response);
            }
        });

        var service = new window.google.maps.DistanceMatrixService();
        service.getDistanceMatrix({
            origins: [source],
            destinations: [destination],
            travelMode: window.google.maps.TravelMode.DRIVING,
            unitSystem: window.google.maps.UnitSystem.IMPERIAL,
            avoidHighways: false,
            avoidTolls: false
        }, function (response, status) {
            if (status === window.google.maps.DistanceMatrixStatus.OK && response.rows[0].elements[0].status === "OK") {

                var distanceToShow = response.rows[0].elements[0].distance.text;
                var durationToShow = response.rows[0].elements[0].duration.text;

                var distance = response.rows[0].elements[0].distance.value;
                var duration = response.rows[0].elements[0].duration.value;

                if ($("#pickUpLocation").val() &&
                    $("#dropOffLocation").val() &&
                    ($("#addOrderModal").data('bs.modal') || {}).isShown) {
                    var estimatedTimePlus10Mins = Math.round(duration / 60) + 10;
                    $("#timeToReachDropOffLocation").val(estimatedTimePlus10Mins);
                    var form = $("#submitOrderForm");
                    validator.form();

                    $("#durationWarning").show();
                }



                $("#distance").html(distanceToShow);
                $("#duration").html(durationToShow);

                $("#distance").data("value", distance);
                $("#duration").data("value", duration);

            } else {
                clearMap();
            }
        });
    }

    function getLongLatPickUp() {
        var geocoder = new window.google.maps.Geocoder();
        var address = $("#pickUpLocation").val();

        geocoder.geocode({ 'address': address }, function (results, status) {

            if (status === window.google.maps.GeocoderStatus.OK) {
                var latitude = results[0].geometry.location.lat();
                var longitude = results[0].geometry.location.lng();
                $("#txtLatitudePickUp").val(latitude);
                $("#txtLongitudePickUp").val(longitude);
            }
        });
    }

    function getLongLatDropOff() {
        var geocoder = new window.google.maps.Geocoder();
        var address = $("#dropOffLocation").val();

        geocoder.geocode({ 'address': address }, function (results, status) {

            if (status === window.google.maps.GeocoderStatus.OK) {
                var latitude = results[0].geometry.location.lat();
                var longitude = results[0].geometry.location.lng();
                $("#txtLatitudeDropOff").val(latitude);
                $("#txtLongitudeDropOff").val(longitude);
            }
        });
    }

    $("#addOrderModal").on("shown.bs.modal",
        function () {
            $("#durationWarning").hide();
            var mapOptions = getMapStart();
            var sourceSearchBox = new window.google.maps.places.SearchBox($('#pickUpLocation')[0]);
            sourceSearchBox.addListener('places_changed', function () {
                var places = sourceSearchBox.getPlaces();
                if (places.length === 0) {
                    $("#pickUpLocation").css({ "border": "1px solid red", "background": "#FFCECE" });
                }
                else {
                    $("#pickUpLocation").css({ "background": "", "border": "1px solid #D5D5D5" });

                }
            });
            var destinationSearchBox = new window.google.maps.places.SearchBox($('#dropOffLocation')[0]);
            destinationSearchBox.addListener('places_changed', function () {
                var places = destinationSearchBox.getPlaces();
                if (places.length === 0) {
                    $("#dropOffLocation").css({ "border": "1px solid red", "background": "#FFCECE" });
                }
                else {
                    $("#dropOffLocation").css({ "background": "", "border": "1px solid #D5D5D5" });

                }
            });
            directionsDisplay = new window.google.maps.DirectionsRenderer({
                'draggable': false
            });

            var map = new window.google.maps.Map(document.getElementById("map_canvas"), mapOptions);

            var pickUpAutocomplete = new window.google.maps.places.Autocomplete(document.getElementById('pickUpLocation'));
            var dropOffAutocomplete = new window.google.maps.places.Autocomplete(document.getElementById('dropOffLocation'));

            pickUpAutocomplete.addListener('place_changed',
                    function () {
                        getLongLatPickUp();
                        var place = pickUpAutocomplete.getPlace();
                        if (!place.geometry) {
                            clearMap();
                        }
                        getRoute();
                    });

            dropOffAutocomplete.addListener('place_changed',
                    function () {
                        getLongLatDropOff();
                        var place = dropOffAutocomplete.getPlace();
                        if (!place.geometry) {
                            clearMap();
                        }
                        getRoute();
                    });

        });

    $('#addOrderModal')
        .on('hidden.bs.modal',
            function () {
                $("#durationWarning").hide();
            });

    $(".btnPreviewOrder").on("click",
        function (e) {
            var ordersIdForPreview = $(this).attr('data-id');
            var form = $("#submitOrderForm");
            form.validate();
            form.valid();
            validator.form();
            e.preventDefault();
            clearOrdersModal();
            $("#addOrderBtn").val("Update");
            $("#Heading").text("Update Business");
            getBusinessById(ordersIdForPreview);
        });

    function getBusinessById(businessId) {
        window.BlockUi();
        $.get("/BusinessOrder/AddNewOrder",
            { businessId: businessId },
            function (data) {
                initOrderModal(data);
                $("#submitOrderForm").modal("show");
                window.UnBlockUi();
            });
    }

    function initOrderModal(data) {
        data = JSON.parse(data);
        customerName.val(data.customerName);
        customerPhone.val(data.CustomerPhone);
        timeToReachPickUpLocation.val(data.timeToReachPickUpLocation);
        timeToReachDropOffLocation.val(data.timeToReachDropOffLocation);
        pickUpLocation.val(data.pickUpLocation);
        dropOffLocation.val(data.dropOffLocation);
        orderNumber.val(data.orderNumber);
        vehicleType.val(data.vehicleType);
        submitOrderForm.attr("data-id", data.OrderId);
    }
    $(document).on('click', '.btnDeleteOrder',
      function () {
          var ordersIdForDelete = $(this).attr('data-id');
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
                      deleteOrders(parseInt(ordersIdForDelete));
              });
      });

    function deleteOrders(ordersId) {
        window.BlockUi();
        $.post("/BusinessOrder/AddNewOrder",
        { ordersId: ordersId },
        function () {
            window.UnBlockUi();
            $("#order_" + ordersId).remove();
            swal("Deleted!", "Your order has been deleted.", "success");
        });
    }

});