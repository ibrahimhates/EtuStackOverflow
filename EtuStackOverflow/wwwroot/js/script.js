import { getCookie, deleteCookie } from './cookieManager.js';
import { getUserProfileDetail, updateUserProfileDetail } from './data/user.js';
import { getAllQuestionWithPage } from './data/question.js';

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
        questions: [],
        questionPagging: {
            currentPage: 1,
            totalPage: 0,
            pageSize: 0,
            totalCount: 0,
            hasPrevios: false,
            hasNext: false
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
                const urlParams = new URLSearchParams(window.location.search);
                let pageNumber = urlParams.get('pageNumber');

                // pageNumber'ý kontrol et ve geçerli bir sayýya dönüþtür
                if (!pageNumber || isNaN(pageNumber) || parseInt(pageNumber) <= 0) {
                  pageNumber = 1;
                } else {
                  pageNumber = parseInt(pageNumber);
                }

                this.questionPagging.currentPage = pageNumber;

                this.isLoading = true;
                getAllQuestionWithPage(this)
                    .then(() => this.isLoading = false)
                    .catch(() => this.isLoading = false)
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
                    localStorage.removeItem('userProfileDetail');
                    this.isLogin = false;
                } else {
                    this.token = tokenCookieValue;
                    this.isLogin = true;
                }
                getUserProfileDetail(this);
            }
        });
    },
    computed: {
        pages() {
            const totalPage = this.questionPagging.totalPage;
            const currentPage = this.questionPagging.currentPage;
            let pages = [];

            if (totalPage <= 5) {
                for (let i = 1; i <= totalPage; i++) {
                    pages.push(i);
                }
            } else {
                if (currentPage <= 3) {
                    pages = [1, 2, 3, 4, '...', totalPage];
                } else if (currentPage >= totalPage - 2) {
                    pages = [1, '...', totalPage - 3, totalPage - 2, totalPage - 1, totalPage];
                } else {
                    pages = [1, '...', currentPage - 1, currentPage, currentPage + 1, '...', totalPage];
                }
            }

            return pages;
        }
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

            var maxSizeInBytes = 1024 * 1024 * 2; // 2MB
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
        goToPage(page) {
            if (page === '...') return;
            if (page < 1 || page > this.questionPagging.totalPage) return;
            this.questionPagging.currentPage = page;
            this.isLoading = true;
            getAllQuestionWithPage(this)
                .then(() => this.isLoading = false)
                .catch(() => this.isLoading = false)
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
            axios.post(`/api/auth/logout`, null,
                { headers: { 'Authorization': 'Bearer ' + this.token } })
                .then(() => {
                    deleteCookie("accessToken");
                    window.location.reload();
                })
                .catch(error => console.error('Birseyler ters gitti '));
        }
    }
})


