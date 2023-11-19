window.setDotNetReference = function (dotNetReference) {
    window.dotNetReference = dotNetReference;
}

window.captchaSuccess = function (token) {
    setTimeout(() => {
        window.dotNetReference.invokeMethodAsync('OnCaptchaSubmit', token);
    }, 1000)
}
