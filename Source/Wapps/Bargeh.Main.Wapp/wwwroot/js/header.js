$(document).ready(() => {
    const SEARCH_OVERLAY = $('.search-overlay')

    $('.mobile-search').click(() => {
        SEARCH_OVERLAY.fadeIn()
    })

    $('.search-close').click(() => {
        SEARCH_OVERLAY.fadeOut()
    })
})