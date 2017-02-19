var rideType;
var map;
var directionsService = new google.maps.DirectionsService();
var mapElement = document.getElementById('businessLiveMap');
var markers = [];

function mapOptions(center, zoom) {
    return {
        zoom: zoom,
        center: center,
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

// TODO: London coordinates. Replace with business location
var center = {
    lat: 51.5076514,
    lng: -0.1256326
};

var zoom = 14;

map = new window.google.maps.Map(mapElement, mapOptions(center, zoom));

function recenterTheMap() {
    var bounds = new window.google.maps.LatLngBounds();
    if (markers.length > 0) {
        for (var i = 0; i < markers.length; i++) {
            bounds.extend(markers[i].getPosition());
        }
        bounds.extend(center);
        map.fitBounds(bounds);
    }
}

function getAndShowOnlineRdiers() {
    $.post("/LiveMap/getOnlineRiders",
                    function (data) {
                        if (data.Success) {
                            showOnlineRiders(data.Data);
                            recenterTheMap();
                        } else {
                        }
                    }).always(function () {
                    }).fail(function () {
                        window.toastr.error("Internal Server Error.");
                    });
}

function getAndShowBusinessRdiers() {
    $.post("/LiveMap/getBusinessRiders",
                    function (data) {
                        if (data.Success) {
                            showBusinessRiders(data.Data);
                            recenterTheMap();
                        } else {
                        }
                    }).always(function () {
                    }).fail(function () {
                        window.toastr.error("Internal Server Error.");
                    });
}


function createInfoWindow(driverDetail) {
    return "<div class='info'>" +
        "<p><strong>Rider Full Name: </strong>" + driverDetail.DriveFullName + "</p>" +
        "<p><strong>Rider Status: </strong>" + driverDetail.DriverStatus + "</p>" +
        "<p><strong>Vehicle Type: </strong>" + driverDetail.VehicleType + "</p>" +
        "<p><strong>Rating: </strong>" + driverDetail.DriverRating + "</p>" +
        "<p><strong>Order Id: </strong>" + driverDetail.OrderId + "</p>" +
        "<p><strong>Order Status: </strong>" + driverDetail.OrderStatus + "</p>" +
        "</div>";
}

function showOnlineRiders(driverDetails) {
    driverDetails.forEach(function (details) {
        var marker = new window.google.maps.Marker({
            position: new window.google.maps.LatLng(details.DriverLat, details.DriverLong),
            icon: icons["onlineRider"],
            map: map
        });

        markers.push(marker);

        var content = createInfoWindow(details);

        var infowindow = new window.google.maps.InfoWindow();

        window.google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
            return function () {
                infowindow.setContent(content);
                infowindow.open(map, marker);
            };
        })(marker, content, infowindow));
    });
}

function showBusinessRiders(driverDetails) {
    driverDetails.forEach(function (details) {
        var marker = new window.google.maps.Marker({
            position: new window.google.maps.LatLng(details.DriverLat, details.DriverLong),
            icon: icons["businessRider"],
            map: map
        });

        markers.push(marker);

        var content = createInfoWindow(details);

        var infowindow = new window.google.maps.InfoWindow();

        window.google.maps.event.addListener(marker, 'click', (function (marker, content, infowindow) {
            return function () {
                infowindow.setContent(content);
                infowindow.open(map, marker);
            };
        })(marker, content, infowindow));
    });
}

function drawRidersOnMap() {
    getAndShowOnlineRdiers();
    getAndShowBusinessRdiers();
}


// Update map every 10 seconds
setInterval(drawRidersOnMap, 10000);

$("#refreshLiveMapBtn")
    .on("click",
        function(e) {
            drawRidersOnMap();
        });
