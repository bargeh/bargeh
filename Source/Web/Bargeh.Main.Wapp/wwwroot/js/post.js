const cta = '<a style="color: blue; text-decoration: underline;" onclick="showPostForm(this)">پاسخ دهید!</a>';

function positionPosts() {
    const posts = document.querySelectorAll('[id^="post_"]');

    for (var i = 0; i < posts.length; i++) {
        if (posts[i].id.split('_')[2] !== '') {
            let childId = posts[i].id.split('_')[2];
            let child = $('[id*="_' + childId + '_"]');

            $(child).appendTo(posts[i]);
        }
    }
    //var list = $('#theposts');
    //var listItems = list.children('div');
    //list.append(listItems.get().reverse());

    $("[id^='post_']").addClass('clild');

    $("[id$='_']").append(cta);

    $('.scrollable-div').show();
}

function showPostForm(obj) {
    $('#postform').replaceWith(cta);
    $(obj).replaceWith('<div id="postform"><textarea></textarea><button onclick="post()">ارسال</button></div>');
}

function post() {
    debugger;
    const textarea = $('#postform').find('textarea').val();
    const parentId = $('#postform').parent().prop('id').split('_')[1];

    $.ajax({
        type: "POST",
        url: "/Api/Topic/Post",
        processData: false,
        contentType: false,
        headers: {
            //"RequestVerificationToken": $("input[name='__RequestVerificationToken']").val(),
            // TODO: Uncomment this
            "text": textarea,
            "parentId": parentId,
            "topicId": window.location.href.substring(window.location.href.lastIndexOf('/') + 1)
        },
        statusCode: {
            403: function (responseObject, textStatus, jqXHR) {
                // TODO: Add deatils to error
                alert('متاسفانه مشکلی پیش آمد. لطفا بعدا دوباره امتحان کنید');
            }
        },
        success: function (data) {
            location.reload();
        }
    });
}