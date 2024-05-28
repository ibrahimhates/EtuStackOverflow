import { getCookie, deleteCookie } from './cookieManager.js';
import { getUserProfileDetail, updateUserProfileDetail, getAllUsers, getUserProfileDetailById } from './data/user.js';
import { getAllQuestionWithPage, createQuestion, deleteQuestion, getOneQuestion, createComment, solvedQuestion } from './data/question.js';

new Vue({
    el: "#baseBody",
    data: {
        userProfileDetail: {
            id: 0,
            fullName: "",
            name: "",
            surName: "",
            dateOfBirth: "" | null,
            profilePhoto: [],
            email: "",
            verifyEmail: false,
            userName: "",
            commentCount: 0,
            interactionCount: 0,
            interactions: [],
            questions: []
        },
        userProfileDetailEdit: {
            name: "",
            surName: "",
            dateOfBirth: "" | null,
            profilePhoto: "",
            userName: "",
        },
        anotherUsersProfileDetail: {
            id: 0,
            fullName: "",
            name: "",
            surName: "",
            dateOfBirth: "" | null,
            profilePhoto: [],
            email: "",
            verifyEmail: false,
            userName: "",
            commentCount: 0,
            interactionCount: 0,
            interactions: [],
            questions: []
        },
        userList: [],
        questions: [],
        question: {
            id: 0,
            userId: 0,
            profilePhoto: [],
            userName: "",
            createdDate: "",
            title: "",
            content: "",
            isSolved: false,
            comments: []
        },
        questionCreate: {
            title: "",
            content: ""
        },
        commentCreate: {
            content: "",
            questionId: 0
        },
        pagging: {
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
        isLoadingForNewComment: false,
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

                this.pagging.currentPage = pageNumber;

                this.isLoading = true;
                getAllQuestionWithPage(this)
                    .then(() => this.isLoading = false)
                    .catch(() => this.isLoading = false)
            }
            if (this.$refs.questionDetail) {
                this.isLoading = true;
                getOneQuestion(this)
                    .then(() => this.isLoading = false)
                    .catch(() => this.isLoading = false)
            }
            if (this.$refs.userPageList) {
                this.getAllUsersEvent();
            }
            if (this.$refs.anotherUsersProfileDetail) {
                this.isLoading = true;
                getUserProfileDetailById(this)
                    .then(() => this.isLoading = false)
                    .catch(() => this.isLoading = false)
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
                    const data = localStorage.getItem('userProfileDetail');

                    if (!data) {
                        getUserProfileDetail(this);
                    } else {
                        this.userProfileDetail = JSON.parse(data);
                    }
                }
            }
        });
    },
    computed: {
        pages() {
            const totalPage = this.pagging.totalPage;
            const currentPage = this.pagging.currentPage;
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
        createQuestionEvent() {
            this.isLoading = true;
            createQuestion(this)
                .then(() => {
                    this.isLoading = false;
                    window.location.reload();
                }).catch(err => {
                    if (err.response.status == 401) {
                        deleteCookie("accessToken");
                        window.location.reload();
                        return;
                    }
                    this.isLoading = false;
                    this.isError = true;
                    this.errorMessage = err.response.data.messages[0];
                })
        },
        deleteQuestionEvent(deletedId) {
            this.userProfileDetail.questions = this.userProfileDetail.questions.filter(question => question.id !== deletedId)
            deleteQuestion(this, deletedId);
        },
        markSolvedQuestionEvent(questionId) {
            solvedQuestion(this, questionId);
        },
        createCommentEvent() {
            this.isLoadingForNewComment = true;
            createComment(this)
                .then(() => {
                    this.isLoadingForNewComment = false;
                }).catch(err => {
                    this.isLoadingForNewComment = false;
                })
        },
        goToPage(page) {
            if (page === '...') return;
            if (page < 1 || page > this.pagging.totalPage) return;
            this.pagging.currentPage = page;
            this.isLoading = true;
            getAllQuestionWithPage(this)
                .then(() => this.isLoading = false)
                .catch(() => this.isLoading = false)
        },
        changeLocationRoute(route) {
            window.location.href = `${route}`
        },
        getAllUsersEvent() {
            this.userList = [];
            this.isLoading = true;
            getAllUsers(this)
                .then(() => this.isLoading = false)
                .catch(() => this.isLoading = false)
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
                .catch(error => {
                    if (error.response.status == 401) {
                        deleteCookie("accessToken");
                        window.location.reload();
                    }
                    console.log(error);
                });
        }
    }
})


