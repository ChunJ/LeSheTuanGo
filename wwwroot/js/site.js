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
                for (let i = 0; i < stringList.length; i += 3) {
                    htmlString += `<option imgPath=${stringList[i+2]} value=${stringList[i]}>${stringList[i + 1]}</option>`;
                }
                document.querySelector(prodTagId).innerHTML = htmlString;
                if (callBack) callBack();
            }
        }
    )
}
function getDistrict2(distTagId) {
    fillDistrict(event.target.value, distTagId, null);
    changeDist();
}
function changeDist() {
    let distV = $('#DistrictId').val();
    $('#ComeDistrictID').val(distV);
}

function formatDatetime(inputString) {
    let list = inputString.split(/-|T|:/);
    return `${list[1]}月${list[2]}日${list[3]}點${list[4]}分`;
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
function payment() {
    $.ajax({
        url: "/Shopping/change?" + $('#formCreditCard').serialize(),
        type: "get",
        success: function (data) {
            $.each(data, function (key, item) {
                const formElement = document.getElementById("formCreditCard")
                formElement[15].value = item[0].checkMacValue
                formElement[1].value = item[0].merchantTradeNo
                formElement[2].value = item[0].merchantTradeDate
                formElement[10].value = item[0].returnURL
                formElement[7].value = item[0].successUrl
                formElement[4].value = item[0].totalAmount
                formElement[6].value = item[0].itemName
            })
        }
    })
}