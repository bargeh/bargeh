const FADE_SPEED = 250

$(document).ready(function () {
    let tooltip = $('<div class="reactions-tooltip"><div><button class="tooltip-button"><img src="img/Like.svg" alt="کاربر" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button"><img src="img/Love.svg" alt="حمایت" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button"><img src="img/Funny.svg" alt="گفت و گو" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button"><img src="img/Light.svg" alt="گفت و گو" class="forum-icon"><p class="forum-count">...</p></button><button class="tooltip-button"><img src="img/Dislike.svg" alt="گفت و گو" class="forum-icon"><p class="forum-count">...</p></button></div></div>').appendTo('body')

    $(document).on('click', '.reactions', function (event) {
        event.stopPropagation()

        if (tooltip.is(':visible')) {
            tooltip.fadeOut(FADE_SPEED)
        }

        if (tooltip.css('display') === 'block') {
            setTimeout(() => {
                showTooltip(this)
            }, 400);
        } else {
            showTooltip(this)
        }
    })

    function showTooltip(reactions) {
        //let id = $(reactions).parent().parent().find('input').val()
        debugger
        let iconPosition = $(reactions).offset()
        let tooltipWidth = tooltip.outerWidth()

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

        $.ajax({
            url: '/reactions.json',
            type: 'GET',
            success: function (data) {
                tooltip.find('p').each((index, element) => {
                    switch (index) {
                        case 0:
                            $(element).text(toPersianDigits(data.reactions.likes))
                            break;

                        case 1:
                            $(element).text(toPersianDigits(data.reactions.loves))
                            break;

                        case 2:
                            $(element).text(toPersianDigits(data.reactions.funnies))
                            break;

                        case 3:
                            $(element).text(toPersianDigits(data.reactions.insightfuls))
                            break;

                        case 4:
                            $(element).text(toPersianDigits(data.reactions.dislikes))
                            break;
                    }
                })
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