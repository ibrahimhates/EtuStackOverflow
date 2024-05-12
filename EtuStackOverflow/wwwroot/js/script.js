import { getCookie, deleteCookie } from './cookieManager.js';
import { getUserProfileDetail, updateUserProfileDetail } from './data/user.js';

new Vue({
    el: "#baseBody",
    data: {
        userProfileDetail: {
            fullName: "",
            name: "",
            surName: "",
            dateOfBirth: "" | null,
            profilePhoto: [],
            email: "",
            verifyEmail: false,
            userName: "",
            commentCount: 0,
            interactionCount: 0
        },
        userProfileDetailEdit: {
            name: "",
            surName: "",
            dateOfBirth: "" | null,
            profilePhoto: "",
            userName: "",
        },
        searchTerm: "",
        token: "",
        isLogin: false,
        isLoading: false,
        errorMessage: "",
        isError: false
    },
    mounted() {
        this.$nextTick(() => {
            if (this.$refs.questionPage) {

            }
            if (this.$refs.questionDetail) {

            }
            if (this.$refs.userPageList) {

            }
            if (this.$refs.profileList) {
                getUserProfileDetail(this);
            }
            if (this.$refs.homePage) {
                const tokenCookieValue = getCookie("accessToken")
                if (tokenCookieValue === "") {
                    this.isLogin = false;
                } else {
                    this.token = tokenCookieValue;
                    this.isLogin = true;
                }
                getUserProfileDetail(this);
            }
        });
    },
    methods: {
        updateProfile() {
            this.isLoading = true;
            updateUserProfileDetail(this)
                .catch(err => {
                    this.isLoading = false;
                })
        },
        getProfilePhoto(base64String) {

            return 'data:image/png+svg+xml;base64,' + base64String;
        },
        uploadProfilePhoto(event) {
            var file = event.target.files[0];

            if (!(file.type === 'image/png'
                || file.type === 'image/jpg'
                || file.type === 'image/jpeg')) {
                this.isError = true;
                this.errorMessage = 'Lutfen PNG,JPG,JPEG dosyasi yukleyin.';
                return;
            }

            var maxSizeInBytes = 1024 * 1024 * 4; // 1MB
            if (file.size > maxSizeInBytes) {
                this.isError = true;
                this.errorMessage = 'Yuklemek istediginiz resim cok buyuk. Lutfen boyutu max 4MB bir resim secin.';
                console.log(file.size / (1024 * 1024));
                return;
            }

            var reader = new FileReader();

            const app = this;
            reader.onload = function (event) {
                var arrayBuffer = event.target.result;
                var uint8Array = new Uint8Array(arrayBuffer);

                app.userProfileDetailEdit.profilePhoto = btoa(String.fromCharCode(...uint8Array));
            };

            reader.readAsArrayBuffer(file);
            console.log(app.userProfileDetailEdit.profilePhoto);
        },
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
        timeSince(date) {
            return moment(date).fromNow();
        },
        getFormatedDate(date) {
            const formatedDate = moment(date).format('DD-MM-YYYY');
            this.userProfileDetailEdit.dateOfBirth = moment(date).format('YYYY-MM-DD');
            return formatedDate;
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


