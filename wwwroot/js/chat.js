"use strict";

//ready事件，radioBTN及send鍵掛事件，預先指定group
$(function () {

    //掛事件
    $('input[type=radio][name="grouptype"]').change(function () {
        $("#rec_grouptype").val($(this).val());
        getOrderList($("#rec_grouptype").val(), $("#rec_selfother").val(), $("#rec_onoff").val());
        $("#detail").removeClass('d-none').siblings().addClass('d-none');
    });
    $('input[type=radio][name="selfother"]').change(function () {
        $("#rec_selfother").val($(this).val());
        getOrderList($("#rec_grouptype").val(), $("#rec_selfother").val(), $("#rec_onoff").val());
        $("#detail").removeClass('d-none').siblings().addClass('d-none');
    });
    $('input[type=radio][name="onoff"]').change(function () {
        $("#rec_onoff").val($(this).val());
        getOrderList($("#rec_grouptype").val(), $("#rec_selfother").val(), $("#rec_onoff").val());
        $("#detail").removeClass('d-none').siblings().addClass('d-none');
    });
    $('#message').keypress(function (e) {
        if (e.which == 13) {
            $('#btnSend').click();
            return false;
        }

    });
    //各編輯表格縣市連動
    $("#serviceEditForm #cityId").on("change", function () { getDistrictc('#serviceEditForm') });
    $("#orderEditForm #cityId").on("change", function () { getDistrictc('#orderEditForm') });

    //發送按鈕
    $('#btnSend').click(function () {
        var message = $("#message").val();
        if (message == "") {
            return
        }
        $("#message").val('');
        var orderid = $("#OrderId").val();
        var grouptype = $("#rec_grouptype").val();

        $.ajax({
            type: "POST",
            url: "/ChatMessageRecords/Create",
            data: { message: message, orderid: orderid, memberid: gmemberid, grouptype: grouptype },
            success: function (data) {
                var s = JSON.parse(data);
                let sendtime = s["Item1"];
                let photopath = s["Item2"];
                connection.invoke("SendMessageToGroup", $("#roomid").val(), gmemberid, username, message, sendtime, photopath)
                    .catch(function (err) {
                        return console.error(err.toString());
                    });
                let txt = grouptype == 1 ? "(團購團)" : "(垃圾團)";
                let str = $("#chatheader").text().replace(" 的聊天室", "").replace("(編號", "@編號").split('@')[0];
                createNotification(grouptype, orderid, gmemberid, str + "的聊天室有新訊息!")
            }
        });
    })

    //預先選團

    if (gt != 0) {
        $(`input[type=radio][name="grouptype"][value="${gt}"]`).click();

    }
})


//建立SignalR連線，接收訊息用事件
connection.on("ReceiveGroupMessage", function (groupName, memberid, username, message, sendtime, photopath) {
    let shortsendtime = (new Date(sendtime)).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    let other = `
                <div class="d-flex">
                    <img class="profile m-1 align-self-start" src="${photopath}" />
                    <div class="msg-box m-1 msg-in" style="word-break: break-all">
                        ${message}
                    </div>
                    <div class="m-1 msg-time">${shortsendtime}</div>
                </div >`
    let self = `
                <div class="d-flex flex-row-reverse">
                    <div class="msg-box m-1 msg-out" style="word-break: break-all">
                        ${message}
                    </div>
                    <div class="m-1 msg-time">${shortsendtime}</div>
                </div>`
    let msg = (gmemberid == memberid) ? self : other;
    $("#chat").append(msg)
    $("#chat").animate({ scrollTop: $('#chat').prop("scrollHeight") }, 500);
});

//選團(點產出的案件按鈕)
function chatGetOrder(oid, gt, rn, hid) {

    $("#oid").val(oid);
    $("#gt").val(gt);
    $("#hid").val(hid);
    $("#detail").removeClass('d-none').siblings().addClass('d-none');
    //加入SignalR聊天室
    var group = gt.toString() + '_' + oid.toString();
    //if (connection.connectionState != "Connected") {
    if (connection.connectionState == "Disconnected") {
        connection.start()
    }
    var checkConnection = setInterval(function () {
        console.log("連線中")
        if (connection.connectionState == "Connected") {
            if ($("#roomid").val() != 0) {
                connection.invoke("RemoveFromGroup", $("#roomid").val()).catch(function (err) {
                    return console.error(err.toString());
                });
            }
            connection.invoke("AddToGroup", group).then(function () { $("#roomid").val(group) }).catch(function (err) {
                return console.error(err.toString());
            });
            clearInterval(checkConnection);
        }
    }, 100);
    ;

    //取得團明細
    getdetail(oid, gt, hid);
    //取得聊天紀錄   
    getchat(oid, gt, rn);
    //取得參加者清單
    getjointmember(oid, gt);
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
                    if (s[i].IsActive) {
                        txt += `<button id="edit" onclick="editorder()">編輯</button>`
                    }
                    $("#self").val(s[i].self);
                }
                $("#detail").removeClass("d-none").siblings().addClass("d-none")
                $("#detail").html(txt);
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
                    if (s[i].IsActive) {
                        txt += `<button id="edit" onclick="editorder()">編輯</button>`
                    }
                    $("#self").val(s[i].self);
                }
            }
            $("#detail").removeClass("d-none").siblings().addClass("d-none")
            $("#detail").html(txt);
        },
    })
}
//取得聊天紀錄
function getchat(oid, gt, rn) {
    $.ajax({
        url: "/ChatMessageRecords/chatGetChat",
        type: "GET",
        data: { orderId: oid, grouptype: gt },
        success: function (data) {
            var s = JSON.parse(data);
            var txt = "";
            for (let i = 0; i < s.length; i++) {
                //let msgclass = (s[i].SentMemberId == gmemberid) ? 'self' : 'other';
                //txt += "<div class='" + msgclass + "'>" + s[i].username + "說：<div><div>" + s[i].Message + "</div></div></div>"
                let sendtime = (new Date(s[i].SentTime)).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
                let other = `
                <div class="d-flex">
                    <img class="profile m-1 align-self-start" src="${s[i].ProfileImagePath}" />
                    <div class="msg-box m-1 msg-in" style="word-break: break-all">
                        ${s[i].Message}
                    </div>
                    <div class="m-1 msg-time">${sendtime}</div>
                </div >`
                let self = `
                <div class="d-flex flex-row-reverse">
                    <div class="msg-box m-1 msg-out" style="word-break: break-all">
                        ${s[i].Message}
                    </div>
                    <div class="m-1 msg-time">${sendtime}</div>
                </div>`

                txt += (s[i].SentMemberId == gmemberid) ? self : other;

            }
            $("#chatheader").text(rn + " 的聊天室");
            $("#chat").html(txt);
            $("#chat").scrollTop($('#chat').prop("scrollHeight"));
            $(`#link_btn div[id=btn${oid}]`).addClass("active_chat").siblings().removeClass("active_chat");
            document.getElementById("btnSend").disabled = false;
        },
    })
}
//取得參加者清單
function getjointmember(oid, gt) {
    $.ajax({
        url: "/ChatMessageRecords/getJointMember",
        type: "GET",
        data: { orderId: oid, grouptype: gt },
        success: function (data) {
            var s = JSON.parse(data);
            var txt = "";
            for (let i = 0; i < s.length; i++) {
                txt += `
                <a class="p-1" rel="popover" data-img="${s[i].ProfileImagePath}" data-username="${s[i].username}">
                    <img class="profile-big" src="${s[i].ProfileImagePath}" />
                </a>`
            }
            $("#chatMemberIcon").html(txt).promise().done(function () {
                $('a[rel=popover]').popover({
                    html: true,
                    trigger: 'hover',
                    content: function () {
                        //<div style="border-radius:50%;width:80px;height:80px;background-image:url('${$(this).data('img')}');background-size:cover"></div>
                        return `<img  src='${$(this).data('img')}' width=80/><div>${$(this).data('username')}</div>`;
                    }
                });
            });
        },
    })
}


//選團型(團購/垃圾)
function getOrderList(gt, so, of) {
    $.ajax({
        url: "/ChatMessageRecords/getOrderList",
        data: { groupType: gt, selfother: so, onoff: of },
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


//編輯案件
function editorder() {
    let orderid = $("#OrderId").val();
    let grouptype = $("#rec_grouptype").val();

    $.ajax({
        url: "/ChatMessageRecords/editOrder",
        data: { grouptype: grouptype, orderid: orderid, memberid: gmemberid, self: $("#self").val() },
        type: "GET",
        success: function (data) {
            console.log(data);
            let detail = JSON.parse(data[0]);
            let range = JSON.parse(data[1]);
            let city = JSON.parse(data[2]);

            //購物團
            if (grouptype == 1) {
                //自開團(ServiceUse)
                if ($("#self").val() == "true") {
                    //city
                    let citytxt = "";
                    for (let i = 0; i < city.length; i++) {
                        citytxt += `<option value=${city[i].CityId}>${city[i].CityName}</option>`
                    }
                    $("#orderEditForm #cityId").html(citytxt);

                    //range
                    let rangetxt = "";
                    for (let i = 0; i < range.length; i++) {
                        rangetxt += `<option value=${range[i].RangeId}>${range[i].RangeInMeters}</option>`
                    }
                    $("#orderEditForm #GoRangeId").html(rangetxt);

                    //帶入編輯資料(ServiceOffer)
                    for (let i = 0; i < detail.length; i++) {
                        $("#orderEditForm #cityId").val(detail[i].CityId).change();
                        var checkExist = setInterval(function () {
                            if ($('#orderEditForm #DistrictId').length) {
                                $(`#orderEditForm #DistrictId option[value="${detail[i].DistrictId}"]`).attr('selected', 'selected')
                                clearInterval(checkExist);
                            }
                        }, 100);
                        $("#orderEditForm #ProductId").append(`<option value=${detail[i].ProductId}>${detail[i].ProductName}</option>`);
                        $("#orderEditForm #CategoryId").append(`<option value=${detail[i].CategoryId}>${detail[i].CategoryName}</option>`);
                        $("#orderEditForm #prodImage").attr('src', `${detail[i].ProductImagePath}`);
                        $("#orderEditForm #Address").val(detail[i].Address);
                        $("#orderEditForm #EndTime").val(detail[i].EndTime);
                        $("#orderEditForm #UnitPrice").val(detail[i].UnitPrice);
                        $("#orderEditForm #MaxCount").val(detail[i].MaxCount);
                        $("#orderEditForm #OrderDescription").val(detail[i].OrderDescription);
                        $("#orderEditForm #CanGo").prop('checked', detail[i].CanGo);
                        $("#orderEditForm #GoRangeId").val(detail[i].GoRangeId);
                        $("#orderEditForm #OrderId").val(detail[i].OrderId);
                        $("#orderEditForm #IsActive").val(detail[i].IsActive);
                        $("#orderEditForm #StartTime").val(detail[i].StartTime);
                        $("#orderEditForm #HostMemberId").val(detail[i].HostMemberId);

                    }
                    $("#detail").addClass("d-none");
                    $("#orderEditForm").removeClass("d-none")
                }
            }

            //垃圾團
            else if (grouptype == 2) {

                //自開團(ServiceOffer)
                if ($("#self").val() == "true") {
                    //city
                    let citytxt = "";
                    for (let i = 0; i < city.length; i++) {
                        citytxt += `<option value=${city[i].CityId}>${city[i].CityName}</option>`
                    }
                    $("#serviceEditForm #cityId").html(citytxt);

                    //range
                    let rangetxt = "";
                    for (let i = 0; i < range.length; i++) {
                        rangetxt += `<option value=${range[i].RangeId}>${range[i].RangeInMeters}</option>`
                    }
                    $("#serviceEditForm #GoRangeId").html(rangetxt);

                    //帶入編輯資料(ServiceOffer)
                    for (let i = 0; i < detail.length; i++) {
                        $("#serviceEditForm #cityId").val(detail[i].CityId).change();
                        var checkExist = setInterval(function () {
                            if ($('#serviceEditForm #DistrictId').length) {
                                $(`#serviceEditForm #DistrictId option[value="${detail[i].DistrictId}"]`).attr('selected', 'selected')
                                clearInterval(checkExist);
                            }
                        }, 100);
                        $("#serviceEditForm #Address").val(detail[i].Address);
                        $("#serviceEditForm #EndTime").val(detail[i].EndTime);
                        $("#serviceEditForm #CanGo").prop('checked', detail[i].CanGo);
                        $("#serviceEditForm #GoRangeId").val(detail[i].GoRangeId);
                        $("#serviceEditForm #L3maxCount").val(detail[i].L3maxCount);
                        $("#serviceEditForm #L5maxCount").val(detail[i].L5maxCount);
                        $("#serviceEditForm #L14maxCount").val(detail[i].L14maxCount);
                        $("#serviceEditForm #L25maxCount").val(detail[i].L25maxCount);
                        $("#serviceEditForm #L33maxCount").val(detail[i].L33maxCount);
                        $("#serviceEditForm #L75maxCount").val(detail[i].L75maxCount);
                        $("#serviceEditForm #L120maxCount").val(detail[i].L120maxCount);
                        $("#serviceEditForm #GarbageServiceId").val(detail[i].GarbageServiceId);
                        $("#serviceEditForm #IsActive").val(detail[i].IsActive);
                        $("#serviceEditForm #StartTime").val(detail[i].StartTime);
                        $("#serviceEditForm #ServiceTypeId").val(detail[i].ServiceTypeId);
                        $("#serviceEditForm #HostMemberId").val(detail[i].HostMemberId);
                    }
                    $("#detail").addClass("d-none");
                    $("#serviceEditForm").removeClass("d-none")
                }

                //加團(ServiceUseRecord)
                else {

                }
            }

            //$("#serviceEditForm_ServiceOffer").removeClass("d-none")
        },
    })
}


//儲存編輯(ServiceOffer)
function saveeditServiceOffer(s) {
    $.ajax({
        url: "/ServiceOffer/EditGarbageOffer",
        data: {
            DistrictId: $(`#${s} #DistrictId`).val(),
            Address: $(`#${s} #Address`).val(),
            EndTime: $(`#${s} #EndTime`).val(),
            GoRangeId: $(`#${s} #GoRangeId`).val(),
            CanGo: $(`#${s} #CanGo`).prop('checked'),
            L3maxCount: $(`#${s} #L3maxCount`).val(),
            L5maxCount: $(`#${s} #L5maxCount`).val(),
            L14maxCount: $(`#${s} #L14maxCount`).val(),
            L25maxCount: $(`#${s} #L25maxCount`).val(),
            L33maxCount: $(`#${s} #L33maxCount`).val(),
            L75maxCount: $(`#${s} #L75maxCount`).val(),
            L120maxCount: $(`#${s} #L120maxCount`).val(),
            GarbageServiceId: $(`#${s} #GarbageServiceId`).val(),
            IsActive: $(`#${s} #IsActive`).val(),
            StartTime: $(`#${s} #StartTime`).val(),
            ServiceTypeId: $(`#${s} #ServiceTypeId`).val(),
            HostMemberId: $(`#${s} #HostMemberId`).val(),
        },
        type: "GET",
        success: function (data) {
            getdetail($("#oid").val(), $("#gt").val(), $("#hid").val())
            let s = $("#chatheader").text().replace(" 的聊天室", "").replace("(編號", "@編號").split('@')[0];
            createNotification($("#gt").val(), $("#oid").val(), gmemberid, s + "的資訊更新了!")
        }
    })
}

function saveeditOrder() {
    $.ajax({
        url: "/Buy/editOrderOffer",
        data: {
            DistrictId: $("#orderEditForm #DistrictId").val(),
            ProductId: $("#orderEditForm #ProductId").val(),
            CategoryId: $("#orderEditForm #CategoryId").val(),
            ProductImagePath: $("#orderEditForm #prodImage").attr('src'),
            Address: $("#orderEditForm #Address").val(),
            EndTime: $("#orderEditForm #EndTime").val(),
            UnitPrice: $("#orderEditForm #UnitPrice").val(),
            MaxCount: $("#orderEditForm #MaxCount").val(),
            OrderDescription: $("#orderEditForm #OrderDescription").val(),
            CanGo: $("#orderEditForm #CanGo").prop('checked'),
            GoRangeId: $("#orderEditForm #GoRangeId").val(),
            OrderId: $("#orderEditForm #OrderId").val(),
            IsActive: $("#orderEditForm #IsActive").val(),
            StartTime: $("#orderEditForm #StartTime").val(),
            HostMemberId: $("#orderEditForm #HostMemberId").val(),
        },
        type: "GET",
        success: function (data) {
            getdetail($("#oid").val(), $("#gt").val(), $("#hid").val())
            let s = $("#chatheader").text().replace(" 的聊天室", "").replace("(編號", "@編號").split('@')[0];
            createNotification($("#gt").val(), $("#oid").val(), gmemberid, s + "的資訊更新了!")
        }
    })
}

function editcancel() {
    $("#detail").removeClass("d-none").siblings().addClass("d-none");
}


