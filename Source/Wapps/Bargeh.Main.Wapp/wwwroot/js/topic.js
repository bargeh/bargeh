let topicsDotnetHelper

window.setTopicsDotnetHelper = (dotnetHelper) => {
    topicsDotnetHelper = dotnetHelper
}

let splide

function initTopics() {
    splide = new Splide('.splide', {
        perMove: 1,
        direction: 'rtl',
        gap: '25pt',
        pagination: false,
        keyboard: 'global',
        updateOnMove: true,
        mediaQuery: 'min',
        height: 'fit-content',
        focus: 'center',
        trimSpace: false,
        breakpoints: {
            0: {
                perPage: 1,
                drag: true,
            },
            1024: {
                perPage: 3,
                drag: false,
            }
        }
    })

    splide.on('mounted', async function () {
        await ajaxCheck()
    })

    splide.mount()

    splide.on('moved', async function () {
        await ajaxCheck()
    })

    async function ajaxCheck() {
        const lastVisibleSlideIndex = splide.index

        if (lastVisibleSlideIndex + 7 <= splide.length) {
            return
        }

        const rawPostchains = await topicsDotnetHelper.invokeMethodAsync('GetMorePostchains')
        addPosts(rawPostchains)
    }

    let isInputFocused = false

    function disableKeyboardNavigation() {
        splide.options = {
            keyboard: false
        }
    }

    function enableKeyboardNavigation() {
        splide.options = {
            keyboard: 'global'
        }
    }

    $(document).on('focusin', 'input, textarea', function () {
        isInputFocused = true
        disableKeyboardNavigation()
    })

    $(document).on('focusout', 'input, textarea', function () {
        isInputFocused = false
        enableKeyboardNavigation()
    })

    configTextareas()

    function configTextareas() {
        $("textarea").each(function () {
            this.setAttribute("style", "height:" + (this.scrollHeight) + "pxoverflow-y:hidden")
        }).on("input", function () {
            this.style.height = 0
            this.style.height = (this.scrollHeight) + "px"
        })
    }

    $(document).on('click', '.reply-button', function () {
        let chainInput = $('.new-postchain').html().replace('رشته‌برگ', 'برگه‌ی')
        let postInput = $('<div class="shadow-box">' + chainInput + '</div>').appendTo('body')

        setTimeout(() => {
            $(this).replaceWith(postInput)
            configTextareas()
            $(postInput).children('textarea').focus()
        }, 200)
    })
}

// noinspection JSUnusedGlobalSymbols
async function submitPost(obj) {
    obj = $(obj).parent().parent()
    const id = obj.parent().children().eq(-2).find("[id$='_id']").val() ?? $('.topic-first-post').find('[id$=_id]').val()
    const body = obj.find('.text-input').val()
    await topicsDotnetHelper.invokeMethodAsync('OnNewPostSubmit', body, id)

    createPost(body, username, '', '', 'hehe', id, 0, 0, 0, 0, 0)
}

function addPosts(rawPostchains) {
    const jsonPostchains = JSON.parse(rawPostchains)

    $(jsonPostchains.Posts).each(function (index, e) {
        createPost(e.Body, e.AuthorUsername, e.Attachment, '', e.Id, e.Parent, e.Likes, e.Loves, e.Funnies, e.Insights, e.Dislikes)
    })

    if (!isUserLoggedIn()) {
        const element = '<div class="tip"><img src="img/Lamp.svg" alt="چراغ"><p>برای دیدن برگه‌های بیشتر باید وارد حساب برگه‌ت بشی</p></div>'
        splide.add('<li class="splide__slide">' + element + '</li>')
        return
    }

    addReplyButtons()
}

function createPost(text, author, attachment, image, id, parentId, likes, loves, funnies, insights, dislikes) {
    let imageElement = ''
    let attachElement = ''
    const reactions = likes + loves + funnies + insights + dislikes

    const parent = $('.post_' + parentId)

    if (image !== '') {
        imageElement = '<img class="post-image" src="' + image + '" alt="' + getFileName(image) + '">'
    }

    if (attachment !== '') {
        attachElement = '<div class="post-attachment"><div><strong>فایل پیوست‌شده</strong><p class="no-block-margin">' + getFileName(attachment, true) + '</p></div><img src="/img/File.svg" class="post-file footer-links" alt="فایل"></div>'
    }

    let element = '<div class="shadow-box-nohover post_' + id + '"><input type="hidden" id="' + id + '_id" value="' + id + '"/><input type="hidden" id="' + id + '_likes" value="' + likes + '"/><input type="hidden" id="' + id + '_loves" value="' + loves + '"/><input type="hidden" id="' + id + '_funnies" value="' + funnies + '"/><input type="hidden" id="' + id + '_insights" value="' + insights + '"/><input type="hidden" id="' + id + '_dislikes" value="' + dislikes + '"/><p class="post-text no-block-margin">' + text + '</p>' + imageElement + attachElement + '<div class="topic-info"><p>توسط <a href="/User/' + author + '">@' + author + '</a></p><div class="footer-links button-bubble reactions"><span class="reactions-count">' + toPersianDigits(reactions) + '</span><img src="/img/Like.svg" class="reaction-icon" alt="پسند"> <img src="img/Love.svg" class="reaction-icon" alt="قلب"> <img src="/img/Light.svg" class="reaction-icon" alt="چراغ"></div></div><a onclick="onReportButtonClick(this)" class="small-text">گزارش</a></div>'

    if (parent.length) {
        parent.parent().append(element)
    } else {
        splide.add('<li class="splide__slide">' + element + '</li>')
    }
}

function addReplyButtons() {
    let elements = $('li.splide__slide:not(:has(a.reply-button)):not(:has(textarea))')

    elements.each((index, element) => {
        $('<a class="button-primary reply-button"><img src="img/Reply.svg" alt="پاسخ دادن"><p>پاسخ دادن</p></a>').appendTo(element)
    })
}

function getFileName(path, keepExtension = false) {
    let file = path.split('/')[path.split('/').length - 1]
    if (keepExtension) {
        return file
    } else {
        return file.split('.')[0]
    }
}

// noinspection JSUnusedGlobalSymbols
function getSeenPostchains() {
    let ids = []

    $('.splide__slide').each(function () {
        const firstChild = $(this).children().first();

        if (firstChild.find('input[id$="_id"]').length) {
            const inputValue = firstChild.find('input[id$="_id"]').val()
            ids.push(inputValue)
        }
    })

    return [...new Set(ids)]
}

async function onReportButtonClick(caller) {
    //const confirm = prompt('آیا از گزارش این نوشته مطمئن هستید؟')

    //if (!confirm) {
    //return
    //}

    caller = $(caller)

    const id = caller.parent().find('input[id$="_id"]').val()

    await topicsDotnetHelper.invokeMethodAsync('ReportPost', id)

    alert('ممنون. گزارشت رو دریافت کردیم و خیلی سریع بررسی‌ش خواهیم کرد')
}