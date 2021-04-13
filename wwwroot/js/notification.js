var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
$(function () {
    //接收事件
    connection.on("ReceiveNotification", function (groupName, groupType, orderId, message) {
        console.log(message)
        $("#LayoutNotificationList").prepend(`<a class="dropdown - item" href="/ChatMessageRecords/Index?grouptype = ${groupType}& groupid = ${orderId}">${message}</a>`)
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
    }

})


function createNotification(groupType, orderId, senderId, notifyContent) {
    $.ajax({
        url: "/Notifications/CreateNotification",
        data: { groupType: groupType.toString(), orderId: orderId.toString(), senderId: senderId.toString(), notifyContent: notifyContent.toString() },
        type: "get",
        success: function (data) {
            var memberList = JSON.parse(data);
            for (let i = 0; i < memberList.length; i++) {
                var group = "memberid_" + memberList[i];

                connection.invoke("SendNotificationToGroup", group, groupType, orderId, notifyContent)
                    .catch(function (err) {
                        return console.error(err.toString());
                    });
            }


        }
    })
}