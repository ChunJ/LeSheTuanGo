// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getDistrict(disId) {
    $.get(
        "/Api/getDistrictBy",
        { cityId: event.target.value },
        function (data, status) {
            if (status == "success") {
                let stringList = data.split(",");
                let htmlString = "";
                for (let i = 0; i < stringList.length; i += 2) {
                    htmlString += `<option value=${stringList[i]}>${stringList[i + 1]}</option>`;
                }
                document.querySelector(disId).innerHTML = htmlString;
            }
        }
    )
}
function getProduct(proId) {
    $.get(
        "/Api/getProductBy",
        { CategoryId: event.target.value },
        function (data, status) {
            if (status == "success") {
                let stringList = data.split(",");
                let htmlString = "";
                for (let i = 0; i < stringList.length; i += 2) {
                    htmlString += `<option value=${stringList[i]}>${stringList[i + 1]}</option>`;
                }
                document.querySelector(proId).innerHTML = htmlString;
            }
        }
    )
}