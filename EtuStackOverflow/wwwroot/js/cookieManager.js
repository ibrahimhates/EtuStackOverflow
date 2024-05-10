
export function getCookie(name) {
    var cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        if (cookies[i].trim().startsWith(name + '=')) {
            return cookies[i].split("=")[1].trim()
        }
    }
    return ""
}
 export function setCookie(key, value, days) {
    const expires = new Date();
    expires.setTime(expires.getTime() + (days * 24 * 60 * 60 * 1000));

    const expiresString = "expires=" + expires.toUTCString();
    document.cookie = key + "=" + value + ";" + expiresString + ";path=/";
}

 export function deleteCookie(key) {
    const expires = new Date();
    expires.setTime(expires.getTime() + (-1 * 24 * 60 * 60 * 1000));

    const expiresString = "expires=" + expires.toUTCString();
    document.cookie = key + "=" + " " + ";" + expiresString + ";path=/";
}
