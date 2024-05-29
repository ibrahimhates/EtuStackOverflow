import { getCookie, deleteCookie } from '../cookieManager.js';

export const deleteUserByAdmin = async (app, userId) => {
    const tokenCookieValue = getCookie("accessToken")

    if (tokenCookieValue === "") {
        app.isLogin = false;
        deleteCookie("accessToken");
        window.location.pathname = "/auth/login";
        return;
    } else {
        app.token = tokenCookieValue;
    }

    try {
        await axios.delete(`/api/users/${userId}`,
            { headers: { 'Authorization': 'Bearer ' + app.token } });
        window.location.reload();
    } catch (err) {
        if (err.response.status == 401) {
            deleteCookie("accessToken");
            window.location.pathname = "/auth/login";
        }
        throw err;
    }
}

export const deleteCommentByAdmin = async (app, commentId) => {
    const tokenCookieValue = getCookie("accessToken")

    if (tokenCookieValue === "") {
        app.isLogin = false;
        deleteCookie("accessToken");
        window.location.pathname = "/auth/login";
        return;
    } else {
        app.token = tokenCookieValue;
    }

    try {
        await axios.delete(`/api/comments/${commentId}`,
            { headers: { 'Authorization': 'Bearer ' + app.token } });
        window.location.reload();
    } catch (err) {
        if (err.response.status == 401) {
            deleteCookie("accessToken");
            window.location.pathname = "/auth/login";
        }
        throw err;
    }
}

export const deleteQuestionByAdmin = async (app, questionId) => {
    const tokenCookieValue = getCookie("accessToken")

    if (tokenCookieValue === "") {
        app.isLogin = false;
        deleteCookie("accessToken");
        window.location.pathname = "/auth/login";
        return;
    } else {
        app.token = tokenCookieValue;
    }

    try {
        await axios.delete(`/api/questions/force/${questionId}`,
            { headers: { 'Authorization': 'Bearer ' + app.token } });
        window.location.reload();
    } catch (err) {
        if (err.response.status == 401) {
            deleteCookie("accessToken");
            window.location.pathname = "/auth/login";
        }
        throw err;
    }
}