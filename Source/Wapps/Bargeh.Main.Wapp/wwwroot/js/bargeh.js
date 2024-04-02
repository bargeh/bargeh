let mainLayoutDotnetHelper

window.setMainLayoutDotnetHelper = (dotnetHelper) => {
    mainLayoutDotnetHelper = dotnetHelper
}

$(document).ready(() => {
    formatMentions()
})

function onAfterRender() {
    formatMentions();
    $(".button-bubble, .button-bubble-static, .button-bubble-static-inline").hover(function () {
        $(this).addClass("bubble-hovered");
    });
}

function updateLocalStorage() {
    const token = localStorage.getItem('login.access_token')
    const timeIndex = token.indexOf('@')
    const timeString = token.substring(timeIndex + 1)
    const expiryTime = new Date(timeString)

    const localCurrentTime = new Date()
    const utcCurrentTime = new Date(
        localCurrentTime.getUTCFullYear(),
        localCurrentTime.getUTCMonth(),
        localCurrentTime.getUTCDate(),
        localCurrentTime.getUTCHours(),
        localCurrentTime.getUTCMinutes(),
        localCurrentTime.getUTCSeconds()
    )
    const thirtySecondsBeforeExpiry = new Date(expiryTime - 30 * 1000)

    if (utcCurrentTime >= thirtySecondsBeforeExpiry) {
        mainLayoutDotnetHelper.invokeMethodAsync('UpdateLoginTokens')
    }
}

const observer = new MutationObserver(function (mutations) {
    mutations.forEach(function () {
        formatMentions()
    })
})

const config = {childList: true, subtree: true, characterData: true};

observer.observe(document.querySelector('body'), config)

function formatMentions() {
    const MENTIONS = $('a').filter(function () {
        return $(this).text().startsWith('@')
    })

    MENTIONS.each(function () {
        $(this).prop('dir', 'ltr')
    })
}

function toPersianDigits(number) {
    let chars = number.toString().split('')
    let out = ''

    $(chars).each((index, element) => {
        switch (element) {
            case '0':
                out += '۰'
                break

            case '1':
                out += '۱'
                break

            case '2':
                out += '۲'
                break

            case '3':
                out += '۳'
                break

            case '4':
                out += '۴'
                break

            case '5':
                out += '۵'
                break

            case '6':
                out += '۶'
                break

            case '7':
                out += '۷'
                break

            case '8':
                out += '۸'
                break

            case '9':
                out += '۹'
                break
        }

    })

    return out
}

setInterval(() => {
    updateLocalStorage()
}, 1000)