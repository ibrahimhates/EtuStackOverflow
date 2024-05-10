import { getCookie,deleteCookie } from './cookieManager.js';

new Vue({
    el: "#baseBody",
    data: {
        allQuestionList: [],
        question: {},
        userData: [],
        searchTerm: "",
        profileQuestionData: [],
        profileInteractionData: [],
        islike: false,
        isDisLike: false,
        token: "",
        isLogin: false
    },
    mounted() {
        this.$nextTick(() => {
            if (this.$refs.questionPage) {
                this.getAll();
            }
            if (this.$refs.questionDetail) {
                this.getOneQuestion()
            }
            if (this.$refs.userPageList) {
                this.getAllUser();
            }
            if (this.$refs.profileList) {
                this.getDataForQuestion(1);
                this.getDataForInteraction(1);
            }
            if (this.$refs.homePage) {
                const tokenCookieValue = getCookie("accessToken")
                if (tokenCookieValue === "") {
                    this.isLogin = false;
                } else {
                    this.token = tokenCookieValue;
                    this.isLogin = true;
                }
            }
        });
    },
    methods: {
        getAll() {
            axios.get(`/api/questions`)
                .then(response => {
                    this.allQuestionList = response.data;
                })
                .catch(error => console.error("Hata olustu"));
        },
        getOneQuestion() {
            var routeId = window.location.href.split('/').reverse()[0];

            axios.get(`/api/questions/one/${routeId}`)
                .then(response => {
                    this.question = response.data;
                })
                .catch(error => {
                    console.error('Birseyler ters gitti')
                    window.location.pathname = "/questions";
                });
        },
        changeLocationRoute(id) {
            window.location.href += `/${id}`
        },
        getAllUser() {
            axios.get(`/api/users`)
                .then(response => {
                    this.userData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti'));
        },
        getAllUserWithSearchTerm() {
            if (this.searchTerm === "") {
                this.getAllUser()
                return;
            }
            axios.get(`/api/users/filter?searchTerm=${this.searchTerm}`)
                .then(response => {
                    this.userData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti'));
        },
        getDataForQuestion(userId) {
            axios.get(`/api/Questions/allForUser/${userId}`)
                .then(response => {
                    this.profileQuestionData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti '));
        },
        getDataForInteraction(userId) {
            axios.get(`/api/Questions/interactions/${userId}`)
                .then(response => {
                    this.profileInteractionData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti '));
        },
        disLike(id) {

            if (this.isDisLike) {
                this.profileInteractionData[id].disLikeCount--;

            } else {
                this.profileInteractionData[id].disLikeCount++;

                if (this.isLike) {
                    this.profileInteractionData[id].likeCount--;
                    this.isLike = false;
                }
            }
            this.isDisLike = !this.isDisLike;
        },
        like(id) {

            if (this.isLike) {
                this.profileInteractionData[id].likeCount--;

            } else {
                this.profileInteractionData[id].likeCount++;

                if (this.isDisLike) {
                    this.profileInteractionData[id].disLikeCount--;
                    this.isDisLike = false;
                }
            }
            this.isLike = !this.isLike;
        },
        timeSince(date) {
            return moment(date).fromNow();
        },
        logout() {
            axios.post(`/api/auth/logout`, null, { headers: { 'Authorization': 'Bearer ' + this.token } })
                .then(() => {
                    deleteCookie("accessToken");
                    window.location.reload();
                })
                .catch(error => console.error('Birseyler ters gitti '));
        }
    }
})
