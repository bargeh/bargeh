async function logout() {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "/Api/Logout",
            processData: false,
            contentType: false,
            headers: {
                "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
            },
            statusCode: {
                403: function (responseObject, textStatus, jqXHR) {
                    resolve(403);
                }
            },
            success: function (data) {
                resolve(200);
                location.reload();
            }
        });
    });
}
