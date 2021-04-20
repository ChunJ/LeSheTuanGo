var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
}).build();
$(function () {
    //接收事件
    connection.on("ReceiveNotification", function (groupName, groupType, orderId, message, newOrUpdate) {
        //$("#LayoutNotificationList").prepend(`<a class="dropdown-item" href="/ChatMessageRecords/Index?grouptype=${groupType}&groupid=${orderId}">${message}</a>`)
        //if (newOrUpdate == "true") {
        //    let notifynum = parseInt($(".LayoutNotificationNumber").first().text(), 10);
        //    $(".LayoutNotificationNumber").text((notifynum + 1).toString());
        //}
        getNotification(sessionMemberId);
    })

    //啟動時連線&依據登入ID加入group
    if (sessionMemberId != "") {
        var group = "memberid_" + sessionMemberId;
        if (connection.connectionState != "Connected") {
            connection.start().then(function () {
                connection.invoke("AddToGroup", group);
                $("#user_group").val(group);
                //斷線重連
                connection.onclose(function () {
                    var checkconn = setInterval(function () {
                        connection.start().then(function () {
                            connection.invoke("AddToGroup", group);
                            $("#user_group").val(group);
                            if ($("#roomid").val() != 0 && $("#roomid").val() != undefined) {
                                connection.invoke("AddToGroup", $("#roomid").val())
                            }
                        });
                        if (connection.connectionState != "Disconnected") {
                            clearInterval(checkconn);
                        }
                    }, 100);
                })

            })
        }
        //撈通知
        getNotification(sessionMemberId);

        //點小鈴鐺消通知
        $("#alertDropdown").on('click', function () {
            $.ajax({
                url: "/Notifications/ClearNotification",
                data: { senderId: sessionMemberId },
                type: "get",
                success: function (data) {
                    $(".LayoutNotificationNumber").text("0");
                }
            })
        })
    }
})


//撈通知
function getNotification(senderid) {
    $.ajax({
        url: "/Notifications/GetNotification",
        data: { senderId: senderid },
        type: "get",
        success: function (data) {
            var s = JSON.parse(data[0]);
            var txt = "";
            for (let i = 0; i < s.length; i++) {
                let grouptype = s[i].SourceType == 1 ? "購物團" : "垃圾團";

                txt += `<a class="dropdown-item border-top py-2" href="/ChatMessageRecords/Index?grouptype=${s[i].SourceType}&groupid=${s[i].SourceId}">
                                        <div class="d-flex align-items-center">
                                            <div class="notify-title">${grouptype} (編號：${s[i].SourceId})</div>
                                            <div class="notify-time">${(new Date(s[i].SentTime)).toLocaleTimeString()}</div>
                                        </div>
                                        <div class="notify-content">${s[i].ContentText}</div>
                                    </a>`
            }
            $("#LayoutNotificationList").html(txt);
            $(".LayoutNotificationNumber").text(data[1]);
        }
    })
}

//寫入通知
function createNotification(groupType, orderId, senderId, notifyContent) {
    $.ajax({
        url: "/Notifications/CreateNotification",
        data: { groupType: groupType, orderId: orderId, senderId: senderId, notifyContent: notifyContent.toString() },
        type: "get",
        success: function (data) {
            var memberList = JSON.parse(data);
            for (let i = 0; i < memberList.length; i++) {
                var group = "memberid_" + memberList[i]["Item1"];

                connection.invoke("SendNotificationToGroup", group, groupType, orderId, notifyContent, memberList[i]["Item2"].toString())
                    .catch(function (err) {
                        return console.error(err.toString());
                    });
            }
        }
    })
}