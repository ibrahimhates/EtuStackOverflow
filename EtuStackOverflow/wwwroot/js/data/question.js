

export const getAllQuestionWithPage = async (app) => {
    app.questions = [];
    try {
        const response = await axios.get(`/api/questions?pageNumber=${app.questionPagging.currentPage}`);
        app.questions = response.data.data;
        app.questionPagging = response.data.pagination;
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


function getOneQuestion() {
    var routeId = window.location.href.split('/').reverse()[0];

    axios.get(`/api/questions/one/${routeId}`)
        .then(response => {
            this.question = response.data;
        })
        .catch(error => {
            console.error('Birseyler ters gitti')
            window.location.pathname = "/questions";
        });
}