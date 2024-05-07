
var userApp = new Vue({
    el: "#userPageList",
    data: {
        userData: [],
        searchTerm: ""
    },
    mounted() {
        this.$nextTick(() => {
            if (this.$refs.userPageList) {
                this.getAllUser();
            }
        });
    },
    methods: {
        getAllUser() {
            axios.get(`/api/users`)
                .then(response => {
                    this.userData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti'));
        },
        async getAllUserWithSearchTerm() {
            if (this.searchTerm === "") {
                this.getAllUser()
                return;
            }
            await axios.get(`/api/users/filter?searchTerm=${this.searchTerm}`)
                .then(response => {
                    this.userData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti'));
        }
    }
})

