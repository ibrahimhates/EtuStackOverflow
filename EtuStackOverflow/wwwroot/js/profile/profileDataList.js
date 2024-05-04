

var app = new Vue({
    el: '#profileList',
    data: {
        profileQuestionData: [],
        profileInteractionData:[]
    },
    mounted() {
        this.$nextTick(() => {
            if (this.$refs.profileList) {
                this.getDataForQuestion(1);
                this.getDataForInteraction(1);
            }
        });
    },
    methods: {
        getDataForQuestion(userId) {
            axios.get(`/Question/AllQuestionForUser/${userId}`)
                .then(response => {
                    this.profileQuestionData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti '));
        },
        getDataForInteraction(userId) {
            axios.get(`/Question/AllInteractionForUser/${userId}`)
                .then(response => {
                    this.profileInteractionData = response.data;
                })
                .catch(error => console.error('Birseyler ters gitti '));
        },
        disLike(id) {
            console.log("Id:", id)
            this.profileInteractionData[id].disLikeCount++;
        },
        like(id) {
            console.log("Id:", id)
            this.profileInteractionData[id].likeCount++;
        },
        timeSince(date) {
            return moment(date).fromNow();
        }
    }
});