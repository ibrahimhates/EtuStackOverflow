import { getCookie, deleteCookie } from '../cookieManager.js';

export const getAllQuestionWithPage = async (app) => {
    app.questions = [];
    try {
        const response = await axios.get(`/api/questions?pageNumber=${app.pagging.currentPage}`);
        app.questions = response.data.data;
        app.pagging = response.data.pagination;
    } catch (error) {
        const statusCode = error.response.status;
        if (statusCode === 401) {
            app.isLogin = false;
            deleteCookie("accessToken");
            window.location.pathname = "/";
        }
        throw error;
    }
}

export const createQuestion = async (app) => {
    const tokenCookieValue = getCookie("accessToken")

    if (tokenCookieValue === "") {
        app.isLogin = false;
        deleteCookie("accessToken");
        window.location.pathname = "/";
        return;
    } else {
        app.token = tokenCookieValue;
    }

    try {
        await axios.post(`/api/questions`, app.questionCreate,
            { headers: { 'Authorization': 'Bearer ' + app.token } });

    } catch (err) {
        throw err;
    }
}

export const getOneQuestion = async (app) => {
    var routeId = window.location.href.split('/').reverse()[0];
    console.log(app.question)
    try {
        const response = await axios.get(`/api/questions/${routeId}`)
        app.question = response.data.data;
        console.log(app.question)
    } catch (err) {
        console.error('Birseyler ters gitti')
    }
}