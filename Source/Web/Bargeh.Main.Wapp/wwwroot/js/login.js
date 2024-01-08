var suffix = 11;

$(document).ready(function () {
    suffix = Math.floor(Math.random() * (99 - 11 + 1)) + 11;
});

async function login(phone, password, captcha) {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "/Api/Login",
            headers: {
                "phone": phone,
                "password": password,
                "captcha": captcha,
                "RequestVerificationToken": $("input[name='__RequestVerificationToken']").val()
            },
            processData: false,
            contentType: false,
            statusCode: {
                400: function (responseObject, textStatus, jqXHR) {
                    resolve(400);
                },
                404: function (responseObject, textStatus, jqXHR) {
                    resolve(404);
                },
                403: function (responseObject, textStatus, jqXHR) {
                    resolve(403);
                }
            },
            success: function (data) {
                location = "/";
            }
        });
    });
}

function onUsernameInput(input) {
    let value = $(input).val()

    $('#usernameWillBe').text('به صورت ' + value + suffix + ' خواهد بود. بعد از ثبت نام می‌توانید آن را تغییر دهید')
}