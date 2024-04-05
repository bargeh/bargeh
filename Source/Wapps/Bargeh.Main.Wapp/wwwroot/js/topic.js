let topicsDotnetHelper

window.setTopicsDotnetHelper = (dotnetHelper) => {
    topicsDotnetHelper = dotnetHelper
    initTopics()
}

function initTopics() { 
    let splide = new Splide('.splide', {
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
        const lastVisibleSlideIndex = splide.index;

        if (lastVisibleSlideIndex + 7 <= splide.length) {
            return;
        }
        
        const rawPostchains = await topicsDotnetHelper.invokeMethodAsync('GetMorePostchains')
        const jsonPostchains = JSON.parse(rawPostchains)
        
        $(jsonPostchains.Posts).each(function (index, e) {
            const parentId = e.Parent;
            const parentIndex = jsonPostchains.Posts.some(item => item.Id === parentId) ? 1 : 0;
            createPost(e.Body, e.AuthorUsername, e.Attachment, '', e.Likes + e.Loves + e.Funnies + e.Insights + e.Dislikes, e.Id, parentIndex)
        })
        addReplyButtons()
    }

    function addReplyButtons() {
        let elements = $('li.splide__slide:not(:has(a.reply-button)):not(:has(textarea))');

        elements.each((index, element) => {
            $('<a class="button-primary reply-button"><img src="img/Reply.svg" alt="پاسخ دادن"><p>پاسخ دادن</p></a>').appendTo(element)
        })
    }

    function createPost(text, author, attachment, image, reactions, id, index) {
        let imageElement = ''
        let attachElement = ''

        if (image !== '') {
            imageElement = '<img class="post-image" src="' + image + '" alt="' + getFileName(image) + '"></img>'
        }

        if (attachment !== '') {
            attachElement = '<div class="post-attachment"><div><strong>فایل پیوست‌شده</strong><p class="no-block-margin">' + getFileName(attachment, true) + '</p></div><img src="/img/File.svg" class="post-file footer-links" alt="فایل"></div>'
        }

        let element = '<div class="shadow-box-nohover"><input type="hidden" value="' + id + '"><p class="post-text no-block-margin">' + text + '</p>' + imageElement + attachElement + '<div class="topic-info"><p>توسط <a href="/User/' + author + '">@' + author + '</a></p><div class="footer-links button-bubble reactions"><span class="reactions-count">' + toPersianDigits(reactions) + '</span><img src="/img/Like.svg" class="reaction-icon" alt="پسند"> <img src="img/Love.svg" class="reaction-icon" alt="قلب"> <img src="/img/Light.svg" class="reaction-icon" alt="چراغ"></div></div></div>'

        if (index === 0) {
            splide.add('<li class="splide__slide">' + element + '</li>')
        } else {
            $('.splide__list li:last-child').append(element)
        }
    }

    function getFileName(path, keepExtention = false) {
        let file = path.split('/')[path.split('/').length - 1]
        if (keepExtention) {
            return file
        } else {
            return file.split('.')[0]
        }
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
        }, 200);
    })
}

function replaceFirstAndLastChar(str) {
    if (str.length < 2) {
        return str;
    }

    let charToReplace = "'";

    return charToReplace + str.substring(1, str.length - 1) + charToReplace;
}
