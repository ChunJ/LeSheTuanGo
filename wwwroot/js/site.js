// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getDistrict(distTagId) {
    fillDistrict(event.target.value, distTagId, null);
}
function fillDistrict(cityId, distTagId, callBack) {
    $.get(
        "/Api/getDistrictBy",
        { cityId: cityId },
        function (data, status) {
            if (status == "success") {
                let stringList = data.split(",");
                let htmlString = "";
                for (let i = 0; i < stringList.length; i += 2) {
                    htmlString += `<option value=${stringList[i]}>${stringList[i + 1]}</option>`;
                }
                document.querySelector(distTagId).innerHTML = htmlString;
                if (callBack) callBack();
            }
        }
    )
}
function getProduct(prodTagId) {
    fillProduct(event.target.value, prodTagId);
}
function fillProduct(cateId, prodTagId, callBack) {
    $.get(
        "/Api/getProductBy",
        { CategoryId: cateId },
        function (data, status) {
            if (status == "success") {
                let stringList = data.split(",");
                let htmlString = "";
                for (let i = 0; i < stringList.length; i += 2) {
                    htmlString += `<option value=${stringList[i]}>${stringList[i + 1]}</option>`;
                }
                document.querySelector(prodTagId).innerHTML = htmlString;
                if (callBack) callBack();
            }
        }
    )
} 


//function getLatLng(addressInput, callBack) {
//    $.getJSON(
//        "https://maps.googleapis.com/maps/api/geocode/json?",
//        {
//            address: addressInput,
//            key: "AIzaSyBircB99P_RvzxWdQT-hk40-h3Ofzlb_vQ"
//        },
//        function (result) {
//            let coor = result["results"][0]["geometry"]["location"];
//            return(coor["lat"] + "," + coor["lng"]);
//        }
//    )
//}