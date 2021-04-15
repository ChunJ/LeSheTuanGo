﻿"use strict";

//ready事件，radioBTN及send鍵掛事件，預先指定group
$(function () {

    //掛事件
    $('input[type=radio][name="grouptype"]').change(function () {
        $("#rec_grouptype").val($(this).val());
        getOrderList($("#rec_grouptype").val(), $("#rec_selfother").val(), $("#rec_onoff").val())
    });
    $('input[type=radio][name="selfother"]').change(function () {
        $("#rec_selfother").val($(this).val());
        getOrderList($("#rec_grouptype").val(), $("#rec_selfother").val(), $("#rec_onoff").val())
    });
    $('input[type=radio][name="onoff"]').change(function () {
        $("#rec_onoff").val($(this).val());
        getOrderList($("#rec_grouptype").val(), $("#rec_selfother").val(), $("#rec_onoff").val())
    });
    $('#message').keypress(function (e) {
        if (e.which == 13) {
            $('#btnSend').click();
            return false;
        }
    });

    //發送按鈕
    $('#btnSend').click(function () {
        var message = $("#message").val();
        if (message == "") {
            return
        }
        $("#message").val('');
        var orderid = $("#OrderId").val();
        var grouptype = $("#rec_grouptype").val();
        connection.invoke("SendMessageToGroup", $("#roomid").val(), gmemberid, username, message)
            .catch(function (err) {
                return console.error(err.toString());
            });
        $.ajax({
            type: "POST",
            url: "/ChatMessageRecords/Create",
            data: { message: message, orderid: orderid, memberid: gmemberid, grouptype: grouptype },
            success: function () {

                let txt = grouptype == 1 ? "(團購團)" : "(垃圾團)";
                createNotification(grouptype, orderid, gmemberid, txt + $("#chatheader").text() + "有新的訊息")
            }
        });
    })

    //預先選團

    if (gt != 0) {
        $(`input[type=radio][name="grouptype"][value="${gt}"]`).click()
    }
})


//建立SignalR連線，接收訊息用事件
connection.on("ReceiveGroupMessage", function (groupName, memberid, username, message) {
    let msgclass = (gmemberid == memberid) ? 'self' : 'other';
    $("#chat").append("<div class='" + msgclass + "'>" + username + "說：<div><div>" + message + "</div></div></div>")
    $("#chat").animate({ scrollTop: $('#chat').prop("scrollHeight") }, 500);
});

//選團(產出的案件按鈕)
function chatGetOrder(oid, gt, rn, hid) {
    //加入SignalR聊天室
    $("#oid").val(oid);
    $("#gt").val(gt);
    $("#hid").val(hid);

    var group = gt.toString() + '_' + oid.toString();
    if (connection.connectionState != "Connected") {
        connection.start().then(function () {
            if ($("#roomid").val() != 0) {
                connection.invoke("RemoveFromGroup", $("#roomid").val()).catch(function (err) {
                    return console.error(err.toString());
                });
            }
            connection.invoke("AddToGroup", group).then(function () { $("#roomid").val(group) }).catch(function (err) {
                return console.error(err.toString());
            });
        }).catch(function (err) {
            return console.error(err.toString());
        })
    }
    else {
        if ($("#roomid").val() != 0) {
            connection.invoke("RemoveFromGroup", $("#roomid").val()).catch(function (err) {
                return console.error(err.toString());
            });
        }
        connection.invoke("AddToGroup", group).then(function () { $("#roomid").val(group) }).catch(function (err) {
            return console.error(err.toString());
        });
    };
    //取得團明細
    getdetail(oid, gt, hid);
    //取得聊天紀錄
    var urlchat = "/ChatMessageRecords/chatGetChat?orderId=" + oid + "&grouptype=" + gt;
    $.ajax({
        url: urlchat,
        type: "GET",
        success: function (data) {
            var s = JSON.parse(data);
            var txt = "";
            for (let i = 0; i < s.length; i++) {
                let msgclass = (s[i].SentMemberId == gmemberid) ? 'self' : 'other';
                txt += "<div class='" + msgclass + "'>" + s[i].username + "說：<div><div>" + s[i].Message + "</div></div></div>"
            }
            $("#chatheader").text(rn + " 的聊天室");
            $("#chat").html(txt);
            $("#chat").scrollTop($('#chat').prop("scrollHeight"));
            $(`#link_btn div[id=btn${oid}]`).addClass("active_chat").siblings().removeClass("active_chat");
            document.getElementById("btnSend").disabled = false;
        },
    })
}
//選團型(團購/垃圾)
function getOrderList(gt, so, of) {
    $.ajax({
        url: "/ChatMessageRecords/getOrderList?groupType=" + gt + "&selfother=" + so + "&onoff=" + of,
        type: "GET",
        success: function (data) {
            var s = JSON.parse(data);
            var txt = "";
            if (gt == 1) {
                for (let i = 0; i < s.length; i++) {
                    txt += `<div class="p-2 w-100 text-center" id="btn${s[i].OrderId}" data-hostid=${s[i].HostMemberId} onclick="chatGetOrder(${s[i].OrderId} , ${gt} ,'${s[i].ProductName}(編號：${s[i].OrderId}  )',${s[i].HostMemberId})"> ${s[i].ProductName} (編號：${s[i].OrderId})</div>`
                }
            }
            else if (gt == 2) {
                for (let i = 0; i < s.length; i++) {
                    txt += `<div class="p-2 w-100 text-center" id="btn${s[i].GarbageServiceId}" data-hostid=${s[i].HostMemberId} onclick="chatGetOrder(${s[i].GarbageServiceId} , ${gt} ,'${s[i].EndTime}(編號：${s[i].GarbageServiceId}  )',${s[i].HostMemberId})"> ${s[i].EndTime} (編號：${s[i].GarbageServiceId})</div>`
                }
            }
            $("#link_btn").html(txt).promise().done(function () {

                if (oid != 0) {
                    $(`#link_btn div[id=btn${oid}]`).click();
                    oid = 0;
                    gt = 0;
                }
            });
        },
    })
}
//取得明細
function getdetail(oid, gt, hid) {
    var urlorder = "/ChatMessageRecords/chatGetOrderDetail";
    $.ajax({
        url: urlorder,
        type: "GET",
        data: {
            orderId: oid,
            grouptype: gt,
            hostid: hid,
        },
        success: function (data) {
            var s = JSON.parse(data);
            var txt = "";
            txt += "<input type='hidden' id='OrderId' value='" + oid + "'/>"
            if (gt == 1) {
                for (let i = 0; i < s.length; i++) {
                    txt += `<div>團購編號：${s[i].OrderId} </div><br />`;
                    txt += `<div>商品名稱： ${s[i].ProductName} </div><br />`;
                    txt += `<div>商品敘述： ${s[i].OrderDescription} </div><br />`;
                    txt += `<div>地址： ${s[i].CityName}${s[i].DistrictName}${s[i].Address} </div><br />`;
                    let cango = s[i].CanGo == true ? "是" : "否";
                    txt += `<div>是否到府服務： ${cango} </div><br />`;
                    if (cango == "是") {
                        txt += `<div>可服務距離： ${s[i].RangeInMeters} </div><br />`;
                    }
                    txt += `<div>目標購買數量： ${s[i].MaxCount} </div><br />`;
                    txt += `<div>剩餘數量： ${s[i].AvailableCount} </div><br />`;
                    if (s[i].Count != undefined) {
                        txt += `<div>您的購買數量： ${s[i].Count} </div><br />`;
                    }
                    $("#self").val(s[i].self);
                }
            }
            else if (gt == 2) {
                for (let i = 0; i < s.length; i++) {
                    txt += `<div>倒垃圾團編號：${s[i].GarbageServiceId} </div><br />`;
                    txt += `<div>開團者地址： ${s[i].CityName}${s[i].DistrictName}${s[i].Address} </div><br />`;
                    let cango = s[i].CanGo == true ? "是" : "否";
                    txt += `<div>是否到府服務： ${cango} </div><br />`;
                    if (cango == "是") {
                        txt += `<div>可服務距離： ${s[i].RangeInMeters} </div><br />`;
                    }
                    if (s[i].L3maxCount != 0) {
                        let str = s[i].L3count == 0 || s[i].L3count == undefined ? "" : `, 您已委託 ${s[i].L3count}個`;
                        txt += `<div>L3&nbsp;&nbsp;：可收 ${s[i].L3maxCount}個 , 尚可收 ${s[i].L3available}個 ${str}</div><br />`;
                    }
                    if (s[i].L5maxCount != 0) {
                        let str = s[i].L5count == 0 || s[i].L5count == undefined ? "" : `, 您已委託 ${s[i].L5count}個`;
                        txt += `<div>L5&nbsp;&nbsp;：可收 ${s[i].L5maxCount}個 , 尚可收 ${s[i].L5available}個 ${str}</div><br />`;
                    }
                    if (s[i].L14maxCount != 0) {
                        let str = s[i].L14count == 0 || s[i].L14count == undefined ? "" : `, 您已委託 ${s[i].L14count}個`;
                        txt += `<div>L14：可收 ${s[i].L14maxCount}個 , 尚可收 ${s[i].L14available}個 ${str}</div><br />`;
                    }
                    if (s[i].L25maxCount != 0) {
                        let str = s[i].L25count == 0 || s[i].L25count == undefined ? "" : `, 您已委託 ${s[i].L25count}個`;
                        txt += `<div>L25：可收 ${s[i].L25maxCount}個 , 尚可收 ${s[i].L25available}個 ${str}</div><br />`;
                    }
                    if (s[i].L33maxCount != 0) {
                        let str = s[i].L33count == 0 || s[i].L33count == undefined ? "" : `, 您已委託 ${s[i].L33count}個`;
                        txt += `<div>L33：可收 ${s[i].L33maxCount}個 , 尚可收 ${s[i].L33available}個 ${str}</div><br />`;
                    }
                    if (s[i].L75maxCount != 0) {
                        let str = s[i].L75count == 0 || s[i].L75count == undefined ? "" : `, 您已委託 ${s[i].L75count}個`;
                        txt += `<div>L75：可收 ${s[i].L75maxCount}個 , 尚可收 ${s[i].L75available}個 ${str}</div><br />`;
                    }
                    if (s[i].L120maxCount != 0) {
                        let str = s[i].L120count == 0 || s[i].L120count == undefined ? "" : `, 您已委託 ${s[i].L120count}個`;
                        txt += `<div>L120：可收 ${s[i].L120maxCount}個 , 尚可收 ${s[i].L120available}個 ${str}</div><br />`;
                    }
                    if (s[i].NeedCome == true) {
                        txt += `<div>是否到府服務： 是 </div><br />`;
                        txt += `<div>到府服務地址： ${s[i].ComeCityName}${s[i].ComeDistrictName}${s[i].ComeAddress} </div><br />`;
                    }
                    else if (s[i].NeedCome == false) {
                        txt += `<div>是否到府服務： 否 </div><br />`;
                    }
                    txt += `<button id="edit" onclick="editorder()">編輯</button>`
                    $("#self").val(s[i].self);
                }
            }

            $("#detail").html(txt);
        },
    })
}


//編輯案件
function editorder() {
    let orderid = $("#OrderId").val();
    let grouptype = $("#rec_grouptype").val();
    $.ajax({
        url: "/ChatMessageRecords/editOrder",
        data: { grouptype: grouptype, orderid: orderid, memberid: gmemberid, self: $("#self").val() },
        type: "GET",
        success: function (data) {

            let detail = JSON.parse(data[0]);
            let range = JSON.parse(data[1]);
            let city = JSON.parse(data[2]);

            if (grouptype == 2) {
                if ($("#self").val() == "true") {
                    //city
                    let citytxt = "";
                    for (let i = 0; i < city.length; i++) {
                        citytxt += `<option value=${city[i].CityId}>${city[i].CityName}</option>`
                    }
                    $("#cityId").html(citytxt);

                    //range
                    let rangetxt = "";
                    for (let i = 0; i < range.length; i++) {
                        rangetxt += `<option value=${range[i].RangeId}>${range[i].RangeInMeters}</option>`
                    }
                    $("#GoRangeId").html(rangetxt);

                    //很長的框架
                    let txt1 = `
                        <div class="border-bottom-dark mb-2">
                            <h4>編輯</h4>
                        </div>
                        <div class="row w-50">
                            <div class="col-6">
                                <div class="form-group">
                                    <label class="control-label">縣市</label>
                                    <select class="form-control" id="cityId" name="cityId" onchange="getDistrictc('#DistrictId')" data-val="true" data-val-required="The CityId field is required.">
                                        ${citytxt}
                                    </select>
                                </div>
                            </div>
                            <div class="col-6">
                                <div class="form-group">
                                    <label class="control-label" for="DistrictId">鄉鎮市區</label>
                                    <select class="form-control" id="DistrictId" name="DistrictId" data-val="true" data-val-required="The 鄉鎮市區 field is required.">
                                    </select>
                                </div>
                            </div>
                            <div class="col-12">
                                <div class="form-group">
                                    <label class="control-label" for="Address">地址</label>
                                    <input class="form-control" id="Address" name="Address" type="text" value="">
                                        <span class="text-danger field-validation-valid" data-valmsg-for="Address" data-valmsg-replace="true"></span>
                                </div>
                            </div>
                            <div class="col-6">
                                    <div class="form-group">
                                        <label class="control-label" for="EndTime">結束時間</label>
                                        <input class="form-control" id="EndTime" name="EndTime" type="datetime-local" data-val="true" data-val-required="The 結束時間 field is required." value="">
                                            <span class="text-danger field-validation-valid" data-valmsg-for="EndTime" data-valmsg-replace="true"></span>
                                    </div>
                                    <div class="form-group form-check">
                                            <label class="form-check-label">
                                                <input class="form-check-input" type="checkbox" data-val="true" data-val-required="The 是否可到府服務 field is required." id="CanGo" name="CanGo" value="true"> 是否可到府服務
                                        </label>
                                    </div>
                                            <div class="form-group">
                                                <label class="control-label" for="GoRangeId">可服務距離</label>
                                                <select class="form-control" data-val="true" data-val-required="The 可服務距離 field is required." id="GoRangeId" name="GoRangeId">
                                                    ${rangetxt}
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-12">

                                        </div>
                                    </div>

                                    <div class="row w-50 pb-5">
                                        <div class="col-12">
                                            <div class="border-bottom-dark mb-2">
                                                <h5>垃圾袋尺寸</h5>
                                            </div>
                                        </div>
                                        <div class="col-3">
                                            <div class="form-group">
                                                <label class="control-label" for="L3maxCount">3公升</label>
                                                <input class="form-control" id="L3maxCount" name="L3maxCount" value="0" type="number" data-val="true" data-val-required="The 3公升 field is required.">
                                                    <span class="text-danger field-validation-valid" data-valmsg-for="L3maxCount" data-valmsg-replace="true"></span>
            </div>
                                            </div>
                                            <div class="col-3">
                                                <div class="form-group">
                                                    <label class="control-label" for="L5maxCount">5公升</label>
                                                    <input class="form-control" id="L5maxCount" name="L5maxCount" value="0" type="number" data-val="true" data-val-required="The 5公升 field is required.">
                                                        <span class="text-danger field-validation-valid" data-valmsg-for="L5maxCount" data-valmsg-replace="true"></span>
            </div>
                                                </div>
                                                <div class="col-3">
                                                    <div class="form-group">
                                                        <label class="control-label" for="L14maxCount">14公升</label>
                                                        <input class="form-control" id="L14maxCount" name="L14maxCount" value="0" type="number" data-val="true" data-val-required="The 14公升 field is required.">
                                                            <span class="text-danger field-validation-valid" data-valmsg-for="L14maxCount" data-valmsg-replace="true"></span>
            </div>
                                                    </div>
                                                    <div class="col-3">
                                                        <div class="form-group">
                                                            <label class="control-label" for="L25maxCount">25公升</label>
                                                            <input class="form-control" id="L25maxCount" name="L25maxCount" value="0" type="number" data-val="true" data-val-required="The 25公升 field is required.">
                                                                <span class="text-danger field-validation-valid" data-valmsg-for="L25maxCount" data-valmsg-replace="true"></span>
            </div>
                                                        </div>
                                                        <div class="col-3">
                                                            <div class="form-group">
                                                                <label class="control-label" for="L33maxCount">33公升</label>
                                                                <input class="form-control" id="L33maxCount" name="L33maxCount" value="0" type="number" data-val="true" data-val-required="The 33公升 field is required.">
                                                                    <span class="text-danger field-validation-valid" data-valmsg-for="L33maxCount" data-valmsg-replace="true"></span>
            </div>
                                                            </div>
                                                            <div class="col-3">
                                                                <div class="form-group">
                                                                    <label class="control-label" for="L75maxCount">75公升</label>
                                                                    <input class="form-control" id="L75maxCount" name="L75maxCount" value="0" type="number" data-val="true" data-val-required="The 75公升 field is required.">
                                                                        <span class="text-danger field-validation-valid" data-valmsg-for="L75maxCount" data-valmsg-replace="true"></span>
            </div>
                                                                </div>
                                                                <div class="col-3">
                                                                    <div class="form-group">
                                                                        <label class="control-label" for="L120maxCount">120公升</label>
                                                                        <input class="form-control" id="L120maxCount" name="L120maxCount" value="0" type="number" data-val="true" data-val-required="The 120公升 field is required.">
                                                                            <span class="text-danger field-validation-valid" data-valmsg-for="L120maxCount" data-valmsg-replace="true"></span>
            </div>
                                                                    </div>
                                                                </div>
                                                                <div class="column-btn py-1">
                                                                    <input type="button" value="儲存" onclick=saveedit() class="btn btn-primary">
                                                                    <button type="button">取消</button>
    </div>
<input type="hidden" id="GarbageServiceId" name="GarbageServiceId"/>
<input type="hidden" id="IsActive" name="IsActive" style="display:none" />
<input type="hidden" id="StartTime" name="StartTime"/>
<input type="hidden" id="ServiceTypeId" name="ServiceTypeId"/>
<input type="hidden" id="HostMemberId" name="HostMemberId"/>

                                                                        `;

                    let txt = `        
    <div class="px-2 pt-2" id="serviceEditForm">
            <div class="border-bottom-dark mb-2">
                <h4>編輯</h4>
            </div>
            <div class="row">
                <div class="col-6">
                    <div class="form-group">
                        <label class="control-label">縣市</label>
                        <select class="form-control" id="cityId" name="cityId" onchange="getDistrictc('#DistrictId')" data-val="true" data-val-required="The CityId field is required.">
                            ${citytxt}
                        </select>
                    </div>
                </div>
                <div class="col-6">
                    <div class="form-group">
                        <label class="control-label" for="DistrictId">鄉鎮市區</label>
                        <select class="form-control" id="DistrictId" name="DistrictId" data-val="true" data-val-required="The 鄉鎮市區 field is required.">
                        </select>
                    </div>
                </div>
                <div class="col-12">
                    <div class="form-group">
                        <label class="control-label" for="Address">地址</label>
                        <input class="form-control" id="Address" name="Address" type="text" value="">
                        <span class="text-danger field-validation-valid" data-valmsg-for="Address" data-valmsg-replace="true"></span>
                    </div>
                </div>
                <div class="col-6">
                    <div class="form-group">
                        <label class="control-label" for="EndTime">結束時間</label>
                        <input class="form-control" id="EndTime" name="EndTime" type="datetime-local" data-val="true" data-val-required="The 結束時間 field is required." value="">
                        <span class="text-danger field-validation-valid" data-valmsg-for="EndTime" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group form-check">
                        <label class="form-check-label">
                            <input class="form-check-input" type="checkbox" data-val="true" data-val-required="The 是否可到府服務 field is required." id="CanGo" name="CanGo" value="true"> 是否可到府服務
                        </label>
                    </div>
                    <div class="form-group">
                        <label class="control-label" for="GoRangeId">可服務距離</label>
                        <select class="form-control" data-val="true" data-val-required="The 可服務距離 field is required." id="GoRangeId" name="GoRangeId">
                            ${rangetxt}
                        </select>
                    </div>
                </div>
            </div>
            <div class="row pb-5">
                <div class="col-12">
                    <div class="border-bottom-dark mb-2">
                        <h5>垃圾袋尺寸</h5>
                    </div>
                </div>
                <div class="col-3">
                <div class="form-group">
                    <label class="control-label" for="L3maxCount">3公升</label>
                    <input class="form-control" id="L3maxCount" name="L3maxCount" value="0" type="number" data-val="true" data-val-required="The 3公升 field is required.">
                    <span class="text-danger field-validation-valid" data-valmsg-for="L3maxCount" data-valmsg-replace="true"></span>
                </div>
            </div>
            <div class="col-3">
                <div class="form-group">
                    <label class="control-label" for="L5maxCount">5公升</label>
                    <input class="form-control" id="L5maxCount" name="L5maxCount" value="0" type="number" data-val="true" data-val-required="The 5公升 field is required.">
                    <span class="text-danger field-validation-valid" data-valmsg-for="L5maxCount" data-valmsg-replace="true"></span>
                </div>
            </div>
            <div class="col-3">
                <div class="form-group">
                    <label class="control-label" for="L14maxCount">14公升</label>
                    <input class="form-control" id="L14maxCount" name="L14maxCount" value="0" type="number" data-val="true" data-val-required="The 14公升 field is required.">
                    <span class="text-danger field-validation-valid" data-valmsg-for="L14maxCount" data-valmsg-replace="true"></span>
                </div>
            </div>
            <div class="col-3">
                <div class="form-group">
                    <label class="control-label" for="L25maxCount">25公升</label>
                    <input class="form-control" id="L25maxCount" name="L25maxCount" value="0" type="number" data-val="true" data-val-required="The 25公升 field is required.">
                    <span class="text-danger field-validation-valid" data-valmsg-for="L25maxCount" data-valmsg-replace="true"></span>
                </div>
            </div>
            <div class="col-3">
                <div class="form-group">
                    <label class="control-label" for="L33maxCount">33公升</label>
                    <input class="form-control" id="L33maxCount" name="L33maxCount" value="0" type="number" data-val="true" data-val-required="The 33公升 field is required.">
                    <span class="text-danger field-validation-valid" data-valmsg-for="L33maxCount" data-valmsg-replace="true"></span>
                </div>
            </div>
            <div class="col-3">
                <div class="form-group">
                    <label class="control-label" for="L75maxCount">75公升</label>
                    <input class="form-control" id="L75maxCount" name="L75maxCount" value="0" type="number" data-val="true" data-val-required="The 75公升 field is required.">
                    <span class="text-danger field-validation-valid" data-valmsg-for="L75maxCount" data-valmsg-replace="true"></span>
                </div>
            </div>
            <div class="col-3">
                <div class="form-group">
                    <label class="control-label" for="L120maxCount">120公升</label>
                    <input class="form-control" id="L120maxCount" name="L120maxCount" value="0" type="number" data-val="true" data-val-required="The 120公升 field is required.">
                    <span class="text-danger field-validation-valid" data-valmsg-for="L120maxCount" data-valmsg-replace="true"></span>
                </div>
            </div>
        </div>
    </div>
    <div class="column-btn py-1">
        <button class="btn btn-primary" onclick="saveedit()">儲存</button>
        <button class="btn btn-primary">取消</button>
    </div>
    <input type="hidden" id="GarbageServiceId" name="GarbageServiceId" />
    <input type="hidden" id="IsActive" name="IsActive" style="display:none" />
    <input type="hidden" id="StartTime" name="StartTime" />
    <input type="hidden" id="ServiceTypeId" name="ServiceTypeId" />
    <input type="hidden" id="HostMemberId" name="HostMemberId" />`;

                    //帶入編輯資料(ServiceOffer)
                    $("#detail").html(txt).promise().done(function () {
                        for (let i = 0; i < detail.length; i++) {
                            $("#cityId").val(detail[i].CityId).change();
                            var checkExist = setInterval(function () {
                                if ($('#DistrictId').length) {
                                    $(`#DistrictId option[value="${detail[i].DistrictId}"]`).attr('selected', 'selected')
                                    clearInterval(checkExist);
                                }
                            }, 100);
                            $("#Address").val(detail[i].Address);
                            $("#EndTime").val(detail[i].EndTime);
                            $("#CanGo").prop('checked', detail[i].CanGo);
                            $("#GoRangeId").val(detail[i].GoRangeId);
                            $("#L3maxCount").val(detail[i].L3maxCount);
                            $("#L5maxCount").val(detail[i].L5maxCount);
                            $("#L14maxCount").val(detail[i].L14maxCount);
                            $("#L25maxCount").val(detail[i].L25maxCount);
                            $("#L33maxCount").val(detail[i].L33maxCount);
                            $("#L75maxCount").val(detail[i].L75maxCount);
                            $("#L120maxCount").val(detail[i].L120maxCount);
                            $("#GarbageServiceId").val(detail[i].GarbageServiceId);
                            $("#IsActive").val(detail[i].IsActive);
                            $("#StartTime").val(detail[i].StartTime);
                            $("#ServiceTypeId").val(detail[i].ServiceTypeId);
                            $("#HostMemberId").val(detail[i].HostMemberId);
                        }
                    })
                }
                else {
                    //A
                }
            }

            //$("#serviceEditForm_ServiceOffer").removeClass("d-none")
        },
    })
}

//儲存編輯(ServiceOffer)
function saveedit() {
    $.ajax({
        url: "/ServiceOffer/EditGarbageOffer",
        data: {
            DistrictId: $("#DistrictId").val(),
            Address: $("#Address").val(),
            EndTime: $("#EndTime").val(),
            GoRangeId: $("#GoRangeId").val(),
            CanGo: $("#CanGo").prop('checked'),
            L3maxCount: $("#L3maxCount").val(),
            L5maxCount: $("#L5maxCount").val(),
            L14maxCount: $("#L14maxCount").val(),
            L25maxCount: $("#L25maxCount").val(),
            L33maxCount: $("#L33maxCount").val(),
            L75maxCount: $("#L75maxCount").val(),
            L120maxCount: $("#L120maxCount").val(),
            GarbageServiceId: $("#GarbageServiceId").val(),
            IsActive: $("#IsActive").val(),
            StartTime: $("#StartTime").val(),
            ServiceTypeId: $("#ServiceTypeId").val(),
            HostMemberId: $("#HostMemberId").val(),
        },
        type: "GET",
        success: function (data) {
            getdetail($("#oid").val(), $("#gt").val(), $("#hid").val())
        }
    })
}


