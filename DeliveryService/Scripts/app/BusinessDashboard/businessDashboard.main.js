$(function () {
    var earningsMonth = $("#earningsMonth").html();
    var arr;
    if (earningsMonth.trim() !== "") {
        earningsMonth = earningsMonth.replace(",", ".");
        arr = earningsMonth.split(".");
        if (arr.length > 1)
            earningsMonth = arr[0] + "." + arr[1].substring(0, 2);
        else
            earningsMonth = earningsMonth + ".00";
    }
    else {
        earningsMonth = '0.00';
    }

    earningsMonth = "$ " + earningsMonth;

    $("#earningsMonth").html(earningsMonth);

    var earningsToday = $("#earningsToday").html();

    if (earningsToday.trim() !== "") {
        earningsToday = earningsToday.replace(",", ".");
        arr = earningsToday.split(".");
        if (arr.length > 1)
            earningsToday = arr[0] + "." + arr[1].substring(0, 2);
        else
            earningsToday = earningsToday + ".00";
    }
    else {
        earningsToday = '0.00';
    }

    earningsToday = "$ " + earningsToday;

    $("#earningsToday").html(earningsToday);
});