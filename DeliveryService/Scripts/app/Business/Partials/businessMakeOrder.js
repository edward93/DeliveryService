var rideType;
var source, destination;
var directionsDisplay;
var map;
var directionsService = new google.maps.DirectionsService();
var mapElement = document.getElementById('dvMap');
function getMapStart() {
    return {
        zoom: 11,
        center: new google.maps.LatLng(34.052235, -118.243683),
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

google.maps.event.addDomListener(window, 'load', function () {
    var sourceSearchBox = new google.maps.places.SearchBox(document.getElementById('txtSource'));
    sourceSearchBox.addListener('places_changed', function () {
        var places = sourceSearchBox.getPlaces();
        if (places.length == 0) {
            $("#txtSource").css({ "border": "1px solid red", "background": "#FFCECE" });
        }
        else {
            $("#txtSource").css({ "background": "", "border": "1px solid #D5D5D5" });
            $("#airportContent").fadeIn("slow");
        }
    });
    var destinationSearchBox = new google.maps.places.SearchBox(document.getElementById('txtDestination'));
    destinationSearchBox.addListener('places_changed', function () {
        var places = destinationSearchBox.getPlaces();
        if (places.length == 0) {
            $("#txtDestination").css({ "border": "1px solid red", "background": "#FFCECE" });
        }
        else {
            $("#txtDestination").css({ "background": "", "border": "1px solid #D5D5D5" });
            $("#airportContent").fadeIn("slow");
        }
    });
    directionsDisplay = new google.maps.DirectionsRenderer({
        'draggable': false
    });

    map = new google.maps.Map(mapElement, getMapStart());
});

function ClearMap() {
    map = new google.maps.Map(mapElement, getMapStart());
    directionsDisplay.setMap(null);
}

function GetRoute() {
    var mapOptions = getMapStart();
    map = new google.maps.Map(document.getElementById('dvMap'), mapOptions);
    directionsDisplay.setMap(map);

    source = $("#txtSource").val();
    destination = $("#txtDestination").val();

    var request = {
        origin: source,
        destination: destination,
        travelMode: google.maps.TravelMode.DRIVING
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            directionsDisplay.setDirections(response);
        }
    });

    var service = new google.maps.DistanceMatrixService();
    service.getDistanceMatrix({
        origins: [source],
        destinations: [destination],
        travelMode: google.maps.TravelMode.DRIVING,
        unitSystem: google.maps.UnitSystem.IMPERIAL,
        avoidHighways: false,
        avoidTolls: false
    }, function (response, status) {
        if (status == google.maps.DistanceMatrixStatus.OK && response.rows[0].elements[0].status == "OK") {
            // console.log(response.rows[0].elements[0]);

            var distanceToShow = response.rows[0].elements[0].distance.text;
            var durationToShow = response.rows[0].elements[0].duration.text;

            var distance = response.rows[0].elements[0].distance.value;
            var duration = response.rows[0].elements[0].duration.value;

            $("#distance").html(distanceToShow);
            $("#duration").html(durationToShow);

            $("#distance").data("value", distance);
            $("#duration").data("value", duration);

            GetApproximatePrice();

        } else {
            ClearMap();
        }
    });


}


function GetAddress() {
    var lat = parseFloat(document.getElementById("txtLatitude").value);
    var lng = parseFloat(document.getElementById("txtLongitude").value);
    var latlng = new google.maps.LatLng(lat, lng);
    var geocoder = geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[1]) {
                if (rideType == "pick")
                    $("#txtSource").val(results[1].formatted_address);
                else
                    $("#txtDestination").val(results[1].formatted_address);
                GetRoute();
            }
        }
    });
}

$("#drpSelectRideType").change(function () {
    ClearMap();
    if ($('option:selected', this).attr('type') == "drop") {
        $("#pickUpDestination").fadeIn("slow");
        $("#pickPart").insertAfter("#pickUpDestination");
        $("#dropDestination").fadeOut("slow");
        rideType = "drop";
    }
    else {
        $("#pickUpDestination").fadeOut("slow");
        $("#dropDestination").fadeIn("slow");
        $("#dropDestination").insertAfter("#pickPart");
        rideType = "pick";
    }

    if ($(this).val() == "4") {
        $("#trnasportTitle").html("Select station");
    }

    if ($(this).val() == "6") {
        $("#trnasportTitle").html("Select port");
    }

    if ($(this).val() == "3" && $('option:selected', this).attr('type') == "pick") {
        $("#meetGreetContainer").show();
    }
    else {
        $("#meetGreetContainer").hide();
    }

    if ($(this).val() == "7") {
        $("#pickUpDestination").fadeIn("slow");
        $("#dropDestination").fadeIn("slow");
    }
    else {
        //$("#dropDestination").hide();
    }

    $("#terminalsContent").fadeOut("slow");
    $("#airportContent").fadeOut("slow");
    $("#airlinesContent").fadeOut("slow");
    BlockUi();
    $.post("/Dashboard/GetAiroprts",
       { rideTypeId: $(this).val() },
       function (datas) {
           if (datas.length) {
               if (rideType == "pick")
                   $("#airportContent").fadeIn("slow");
               var list = $("#drpSelectAiroprts");
               list.html("");

               if ($("#drpSelectRideType").val() == "4") {
                   list.append(new Option("--Select Station--", "-1"))
               }

               else if ($("#drpSelectRideType").val() == "6") {
                   list.append(new Option("--Select Port--", "-1"))
               }
               else {

                   list.append(new Option("--Select Airoport--", "-1"))
               }

               $.each(datas, function (index, item) {
                   var option = new Option(item.Name, item.Id);
                   option.setAttribute("latidute", item.Latitude);
                   option.setAttribute("longitude", item.Longitude);
                   list.append(option);
               });

           }
           UnBlockUi();
           //GetApproximatePrice();


       });
});

$("#drpSelectAiroprts").change(function () {

    //hard coded
    if ($(this).val() == "1094") {
        $("#laxFeeContainer").show();
    }
    else {
        $("#laxFeeContainer").hide();
    }
    if (rideType == "drop")
        $("#pickUpDestination").fadeIn("slow");
    else
        $("#dropDestination").fadeIn("slow");

    var lat = $('option:selected', this).attr('latidute');
    var long = $('option:selected', this).attr('longitude');
    setCoordinates(lat, long);
    $("#terminalsContent").fadeOut("slow");
    $("#airlinesContent").fadeOut("slow");
    BlockUi();
    $.post("/Dashboard/GetTerminals", { parentId: $(this).val(), }, function (datas) {
        if (datas.length > 0) {
            $("#terminalsContent").fadeIn("slow");
            var list = $("#drpSelectTerminals");
            list.html("");
            list.append(new Option("--Select Terminal--", "-1"))
            $.each(datas, function (index, item) {

                var option = new Option(item.Name, item.Id);
                option.setAttribute("latidute", item.Latitude);
                option.setAttribute("longitude", item.Longitude);
                list.append(option);
            });
        }
        UnBlockUi();
        //GetApproximatePrice();

    });
});

$("#drpSelectTerminals").change(function () {
    var lat = $('option:selected', this).attr('latidute');
    var long = $('option:selected', this).attr('longitude');
    setCoordinates(lat, long);

    BlockUi();
    $.post("/Dashboard/GetAirlines", { parentId: $(this).val(), }, function (datas) {
        if (datas.length > 0) {
            $("#airlinesContent").fadeIn("slow");
            var list = $("#drpSelectAirlines");
            list.html("");
            list.append(new Option("--Select Airline--", "-1"))
            $.each(datas, function (index, item) {

                var option = new Option(item.Name, item.Id);
                option.setAttribute("latidute", item.Latitude);
                option.setAttribute("longitude", item.Longitude);
                list.append(option);

            });
        }
        UnBlockUi();
        //GetApproximatePrice();

    });
});

$("#drpSelectAirlines").change(function () {
    var lat = $('option:selected', this).attr('latidute');
    var long = $('option:selected', this).attr('longitude');
    setCoordinates(lat, long);
    //GetApproximatePrice();
});

$("#selectCarType").change(function () {
    var rideT = $("#drpSelectAiroprts");
    var dest = $("#txtDestination");
    if (rideT.val() != "-1" && dest.val() != "")
        GetApproximatePrice();
});


$('#chkMeetGreet').change(function () {
    if ($("#chkMeetGreet").is(':checked'))
        $("#meetGrContainer").show();
    else
        $("#meetGrContainer").hide();

    var distance = $("#distance").html().replace("mi", "").replace(" ", "");
    if (distance != "" && distance != 0)
        GetApproximatePrice();
});

function setCoordinates(latitude, longitude) {
    $("#txtLatitude").val(latitude);
    $("#txtLongitude").val(longitude);
    GetAddress();
}

$('#txtSource').blur(function () {
    if ($('#txtSource').val() == "") {
        ClearMap()
    } else {
        GetRoute();
    }
});

$('#txtSource').keyup(function (e) {
    var code = e.which; // recommended to use e.which, it's normalized across browsers
    if (code == 13) {
        if ($('#txtSource').val() == "") {
            ClearMap()
        } else {
            GetRoute();
        }
    }
});

$('#txtDestination').blur(function () {
    if ($('#txtDestination').val() == "") {
        ClearMap()
    } else {
        GetRoute();
    }
});

$('#txtDestination').keyup(function (e) {
    var code = e.which; // recommended to use e.which, it's normalized across browsers
    if (code == 13) {
        if ($('#txtDestination').val() == "") {
            ClearMap()
        } else {
            GetRoute();
        }
    }
});

function GetApproximatePrice() {
    // var allData = getAllDataForPrice();
    //  if (allData != null) {
    //  BlockUi();
    /* $.post("/Dashboard/CountApproximateAmount", getAllDataForPrice(), function (datas) {

         $("#baseFee").html("$ " + parseFloat(datas.baseFare).toFixed(2));
         $("#millage").html("$ " + parseFloat(datas.millage).toFixed(2));
         $("#laxFee").html("$ " + parseFloat(datas.laxFee).toFixed(2));
         $("#meetGreet").html("$ " + parseFloat(datas.meetAndGreet).toFixed(2));
         $("#totalAmount").html("$ " + (datas.estimated != null ? (parseFloat(datas.estimated).toFixed(2)) : "0.00"));
         $("#distance").html($("#distance").html().replace("km", "mi"));
         //$("#distance").data("value", allData.distance)
         UnBlockUi();
     });*/
    /* }
     else {
         console.log("incorrect data ");
     }*/
}

function getAllDataForPrice() {

    var carType = $("#selectCarType").val();
    console.log($("#distance").html());
    var distance = $("#distance").html().replace("mi", "").replace(" ", "").replace("&nbsp;", "");
    console.log(distance);
    var meetGreet = $("#chkMeetGreet").is(':checked');
    var lax = $("#drpSelectAiroprts").val() == "1094" ? true : false;

    return {
        carType: carType,
        distance: parseFloat(distance),
        isSmoking: true,
        isMeet: meetGreet,
        isLax: lax
    };
}