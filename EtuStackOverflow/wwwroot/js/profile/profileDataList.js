

var app = new Vue({
    el: '#profileList',
    data: {
        profileQuestionData: [],
        profileInteractionData: [],
        islike: false,
        isDisLike: false
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
        }
    }
});