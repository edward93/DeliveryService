$(function () {
    $("#amountToday").html("$ " + parseFloat($("#amountToday").html().replace(",", ".")).toFixed(2));
    $("#amountAll").html("$ " + parseFloat($("#amountAll").html().replace(",", ".")).toFixed(2));
    $("#earningsToday").html("$ " + parseFloat($("#earningsToday").html().replace(",", ".")).toFixed(2));
    $("#earningsAll").html("$ " + parseFloat($("#earningsAll").html().replace(",", ".")).toFixed(2));
    $("#addRiderfeeToday").html("$ " + parseFloat($("#addRiderfeeToday").html().replace(",", ".")).toFixed(2));
    $("#addRiderfeeAll").html("$ " + parseFloat($("#addRiderfeeAll").html().replace(",", ".")).toFixed(2));
})