var rideType;
var source, destination;
var directionsDisplay;
var map;
var directionsService = new google.maps.DirectionsService();
var mapElement = document.getElementById('dvMap');

$(document).ready(function () {
    $(".location").keypress(function () {
        GetRoute();
    });
});

function getMapStart() {
    return {
        zoom: 11,
        center: new google.maps.LatLng(51.5076514, -0.1256326),
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
    if (destination === "" && source !== "") {
        destination = source;
    }
    else if (source === "" && destination !== "") {
        source = destination;
    }

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

            var distanceToShow = response.rows[0].elements[0].distance.text;
            var durationToShow = response.rows[0].elements[0].duration.text;

            var distance = response.rows[0].elements[0].distance.value;
            var duration = response.rows[0].elements[0].duration.value;

            $("#distance").html(distanceToShow);
            $("#duration").html(durationToShow);

            $("#distance").data("value", distance);
            $("#duration").data("value", duration);


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

$('#txtSource').blur(function () {
    if ($('#txtSource').val() === "") {
        ClearMap();
    } else {
        GetRoute();
    }
});

$('#txtSource').keyup(function (e) {
    var code = e.which; // recommended to use e.which, it's normalized across browsers
    if (code === 13) {
        if ($('#txtSource').val() === "") {
            ClearMap();
        } else {
            GetRoute();
        }
    }
});

$('#txtDestination').blur(function () {
    if ($('#txtDestination').val() === "") {
        ClearMap();
    } else {
        GetRoute();
    }
});

$('#txtDestination').keyup(function (e) {
    var code = e.which; // recommended to use e.which, it's normalized across browsers
    if (code === 13) {
        if ($('#txtDestination').val() === "") {
            ClearMap();
        } else {
            GetRoute();
        }
    }
});
