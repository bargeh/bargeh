function onAfterRender() {
    formatMentions();
    $(".button-bubble, .button-bubble-static, .button-bubble-static-inline").hover(function () {
        $(this).addClass("bubble-hovered");
    });
}

function formatMentions() {
    const MENTIONS = $('a').filter(function () {
        return $(this).text().startsWith('@');
    });

    MENTIONS.each(function () {
        $(this).prop('dir', 'ltr');
    });
}

function toPersianDigits(number) {
    let chars = number.toString().split('');
    let out = '';

    $(chars).each((index, element) => {
        switch (element) {
            case '0':
                out += '۰';
                break;

            case '1':
                out += '۱';
                break;

            case '2':
                out += '۲';
                break;

            case '3':
                out += '۳';
                break;

            case '4':
                out += '۴';
                break;

            case '5':
                out += '۵';
                break;

            case '6':
                out += '۶';
                break;

            case '7':
                out += '۷';
                break;

            case '8':
                out += '۸';
                break;

            case '9':
                out += '۹';
                break;
        }

    });

    return out;
}