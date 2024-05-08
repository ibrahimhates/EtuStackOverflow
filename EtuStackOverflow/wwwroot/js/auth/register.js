
var regApp = new Vue({
    el: "#registerPage",
    data: {
        register: {
            name: "",
            surName: "",
            email: "",
            facultyId: 0,
            majorId: 0,
            grade: 0,
            userName: "",
            password: "",
            passwordAgain: ""
        },
        faculties: [],
        majors: [],
        isLoading: false,
        isError: false,
        isSuccess: false,
        errorMessage:"",
    },
    mounted() {
        this.getAllFaculty()
    },
    methods: {
        getAllFaculty() {
            axios.get(`/api/departments/faculties`)
                .then(response => {
                    this.faculties = response.data.data;
                })
                .catch(error => console.error("Hata olustu"));
        },
        getAllMajors(id) {
            axios.get(`/api/departments/majors/${id}`)
                .then(response => {
                    this.majors = response.data.data;
                })
                .catch(error => console.error("Hata olustu"));
        },
        registerNow() {
            this.isError = false
            this.isSuccess = false
            this.isLoading = true
            var domain = this.register.email.split("@")[1]

            if (domain === undefined
                || domain === null
                || domain !== "erzurum.edu.tr") {

                this.isLoading = false;
                this.isError = true;
                this.errorMessage = "Yalnizca 'erzurum.edu.tr' uzantili mail adresi kabul edilebilir."
                return;
            }

            axios.post(`/api/auth/register`, this.register)
                .then(() => {
                    this.isLoading = false
                    this.isSuccess = true;
                    this.register = {}
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
