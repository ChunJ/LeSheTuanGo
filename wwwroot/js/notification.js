function createNotification(groupType,orderId,senderId,notifyContent) {
    $.ajax({
        url: "/Notifications/CreateNotification",
        data: { groupType: groupType, orderId: orderId, senderId: senderId, notifyContent: notifyContent },
        type: "get",
        success: function (data) {
            var memberList = JSON.parse(data);

        }
    })
}