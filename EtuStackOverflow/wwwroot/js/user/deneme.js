var questionApp = new Vue({
    el: "#questionPage",
    data: {
        allQuestionList: [],
        question: {},
        searchTerm: ""
    },
    mounted() {
        this.$nextTick(() => {
            if (this.$refs.questionPage) {
                this.getAll();
            }
            if (this.$refs.questionDetail) {
                this.getOneQuestion()
            }
        });
    },
    methods: {
        getAll() {
            axios.get(`/api/question`)
                .then(response => {
                    this.allQuestionList = response.data;
                })
                .catch(error => console.error("Hata olustu"));
        },
        getOneQuestion() {
            const queryString = window.location.search;
            // Parametreleri çözümle
            const urlParams = new URLSearchParams(queryString);
            // Örnek bir parametre adý "id" olsun
            const gelenid = urlParams.get('questionId');

            this.searchTerm = gelenid;
            // Ýlgili id'yi konsola yazdýr
            console.log("ID:", urlParams);

            axios.get(`/api/question/${gelenid}`)
                .then(response => {
                    console.log(response.data)
                    this.question = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti'));
        }
    }
})