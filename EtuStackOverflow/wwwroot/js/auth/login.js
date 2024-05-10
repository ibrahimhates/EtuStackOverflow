import { getCookie ,setCookie} from '../cookieManager.js';

var logApp = new Vue({
    el: "#loginPage",
    data: {
        login: {
            userNameOrEmail: "",
            password: ""
        },
        isLoading: false,
        isError: false,
        errorMessage: "",
    },
    mounted() {
        this.$nextTick(() => {
            if (this.$refs.loginPageAccess) {
                const tokenCookieValue = getCookie("accessToken")
                if (tokenCookieValue !== "") {
                    window.location.pathname = "/";
                }
            }
        });
    },
    methods: {
        loginNow() {
            this.isError = false;
            this.isSuccess = false;
            this.isLoading = true;

            axios.post(`/api/auth/login`, this.login)
                .then((response) => {
                    this.isLoading = false;
                    const token = response.data.data.token;
                    setCookie("accessToken", token, 7);
                    window.location.pathname = "/";
                })
                .catch(error => {
                    console.log(error)
                    this.isError = true
                    this.isLoading = false
                    this.errorMessage = error.response.data.messages[0]
                });
        }
    }
})