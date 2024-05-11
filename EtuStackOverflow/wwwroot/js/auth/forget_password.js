
var forgetPassword = new Vue({
    el: "#forgetPasswordPage",
    data: {
        resetPasswordRequest: {
            userNameOrEmail: ""
        },
        resetPasswordResponse: {
            userIdentifier: 0,
            refCode: "",
            expiresTime: ""
        },
        enterVerifyCodeRequest: {
            userIdentifier: 0,
            verifyCode: "",
            refCode: ""
        },
        changePasswordRequest: {
            userIdentifier: 0,
            token: "",
            newPassword: "",
            newPasswordConfirm: ""
        },
        inChangePassword: false,
        inEnterVerifyCode: false,
        inSendVerifyCode: true,
        isLoading: false,
        isError: false,
        errorMessage: "",
        isSuccess:false
    },
    mounted() {
        this.$nextTick(() => {
            if (this.$refs.forgetPasswordPage) {

            }
        });
    },
    methods: {
        sendVerifyCode() {
            this.isError = false;
            this.isLoading = true;

            axios.post(`/api/auth/forget-password`, this.resetPasswordRequest)
                .then((response) => {
                    this.isLoading = false;
                    this.resetPasswordResponse = response.data.data;
                    this.inSendVerifyCode = false;
                    this.inEnterVerifyCode = true;
                })
                .catch(error => {
                    this.isError = true
                    this.isLoading = false
                    this.errorMessage = error.response.data.messages[0]
                });
        },
        enterVerifyCode() {
            this.isError = false;
            this.isLoading = true;

            this.enterVerifyCodeRequest.refCode = this.resetPasswordResponse.refCode;
            this.enterVerifyCodeRequest.userIdentifier = this.resetPasswordResponse.userIdentifier;

            axios.post(`/api/auth/verify-reset-code`, this.enterVerifyCodeRequest)
                .then((response) => {
                    this.isLoading = false;
                    console.log("token",response.data)
                    this.changePasswordRequest.token = response.data.data;
                    this.inEnterVerifyCode = false;
                    this.inChangePassword = true;
                })
                .catch(error => {
                    this.isError = true
                    this.isLoading = false
                    console.log(error.response)
                    this.errorMessage = error.response.data.messages[0]
                });
        },
        changePassword() {
            this.isError = false;
            this.isLoading = true;

            this.changePasswordRequest.userIdentifier = this.resetPasswordResponse.userIdentifier;

            axios.post(`/api/auth/change-password-reset`, this.changePasswordRequest)
                .then((response) => {
                    this.isLoading = false;
                    this.isSuccess = true;
                })
                .catch(error => {
                    this.isError = true
                    this.isLoading = false
                    this.errorMessage = error.response.data.messages[0]
                });
        }
    }
})

