var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
$(function () {
    //接收事件
    connection.on("ReceiveNotification", function (groupName, groupType, orderId, message, newOrUpdate) {
        //$("#LayoutNotificationList").prepend(`<a class="dropdown-item" href="/ChatMessageRecords/Index?grouptype=${groupType}&groupid=${orderId}">${message}</a>`)
        getNotification(sessionMemberId);
        //if (newOrUpdate == "true") {
        //    let notifynum = parseInt($(".LayoutNotificationNumber").first().text(), 10);
        //    $(".LayoutNotificationNumber").text((notifynum + 1).toString());
        //}
        
    })
    //啟動時連線&依據登入ID加入group
    if (sessionMemberId != "") {
        var group = "memberid_" + sessionMemberId;
        if (connection.connectionState != "Connected") {
            connection.start().then(function () {
                connection.invoke("AddToGroup", group);
                console.log(group);
            })
        }
        //撈通知
        getNotification(sessionMemberId);

        //小鈴鐺消通知
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
                txt += `<a class="dropdown-item" href="/ChatMessageRecords/Index?grouptype=${s[i].SourceType}&groupid=${s[i].SourceId}">${s[i].ContentText}</a>`
            }
            $("#LayoutNotificationList").html(txt);
            $(".LayoutNotificationNumber").text(data[1]);
        }
    })
}





function createNotification(groupType, orderId, senderId, notifyContent) {
    $.ajax({
        url: "/Notifications/CreateNotification",
        data: { groupType: groupType, orderId: orderId, senderId: senderId, notifyContent: notifyContent.toString() },
        type: "get",
        success: function (data) {
            var memberList = JSON.parse(data);
            for (let i = 0; i < memberList.length; i++) {
                console.log(memberList[i]["Item1"]);
                var group = "memberid_" + memberList[i]["Item1"];

                connection.invoke("SendNotificationToGroup", group, groupType, orderId, notifyContent, memberList[i]["Item2"].toString())
                    .catch(function (err) {
                        return console.error(err.toString());
                    });
            }
        }
    })
}