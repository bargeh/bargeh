const FADE_SPEED = 250
let postToReactId;

$(document).ready(function () {
    let tooltip = $('<div class="reactions-tooltip"><div><button class="tooltip-button" onclick="reactOnPost(0)"><img src="/img/Like.svg" alt="کاربر" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button" onclick="reactOnPost(1)"><img src="img/Love.svg" alt="حمایت" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button" onclick="reactOnPost(2)"><img src="/img/Funny.svg" alt="گفت و گو" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button" onclick="reactOnPost(3)"><img src="img/Light.svg" alt="گفت و گو" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button" onclick="reactOnPost(4)"><img src="img/Dislike.svg" alt="گفت و گو" class="forum-icon"><p class="forum-count">...</p></button></div></div>').appendTo('body')

    $(document).on('click', '.reactions', function (event) {
        event.stopPropagation()

        if (tooltip.is(':visible')) {
            tooltip.fadeOut(FADE_SPEED)
            return
        }

        if (tooltip.css('display') === 'block') {
            setTimeout(() => {
                showTooltip(this)
            }, 400)
        } else {
            showTooltip(this)
        }
    })

    function showTooltip(reactions) {
        const parent = $(reactions).parent().parent()
        const likes = parent.find('input[id$="_likes"]').val()
        const loves = parent.find('input[id$="_loves"]').val()
        const funnies = parent.find('input[id$="_funnies"]').val()
        const insights = parent.find('input[id$="_insights"]').val()
        const dislikes = parent.find('input[id$="_dislikes"]').val()
        const iconPosition = $(reactions).offset()
        const tooltipWidth = tooltip.outerWidth()
        
        postToReactId = parent.find('input[id$="_id"]').val()

        if (screen.width < 900) {
            tooltip.css({
                top: iconPosition.top - tooltip.outerHeight() - 10,
                left: iconPosition.left
            })
        } else {
            tooltip.css({
                top: iconPosition.top - tooltip.outerHeight() - 10,
                left: iconPosition.left - (tooltipWidth / 2) + ($(reactions).outerWidth() / 2)
            })
        }

        tooltip.fadeIn(FADE_SPEED)

        tooltip.find('p').each((index, element) => {
            switch (index) {
                case 0:
                    $(element).text(toPersianDigits(likes))
                    break

                case 1:
                    $(element).text(toPersianDigits(loves))
                    break

                case 2:
                    $(element).text(toPersianDigits(funnies))
                    break

                case 3:
                    $(element).text(toPersianDigits(insights))
                    break

                case 4:
                    $(element).text(toPersianDigits(dislikes))
                    break
            }
        })
    }

    $(document).on('click', function () {
        if (tooltip.is(':visible')) {
            tooltip.fadeOut(FADE_SPEED)
        }
    })

    $(document).on('click', '.tooltip-button', function () {
        setTimeout(() => {
            $('.reactions').removeClass('active')
            tooltip.fadeOut(FADE_SPEED)
        }, 200)
    })


    $(document).on('click', '.reactions', function () {
        if ($(this).hasClass('active')) {
            $(this).removeClass('active')
        } else {
            $('.reactions').removeClass('active')
            $(this).addClass('active')
        }
    })

    $(document).on('click', function (e) {
        if (!$(e.target).is('.reactions') && !$(e.target).closest('.tooltip-button').length) {
            $('.reactions').removeClass('active')
        }
    })
})

async function reactOnPost(type) {
    await topicsDotnetHelper.invokeMethodAsync('ReactOnPost', postToReactId, type)
}