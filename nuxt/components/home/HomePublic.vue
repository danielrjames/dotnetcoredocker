<template>
  <div>
    <h1>You are not logged in.</h1>
    <h3>
      If you're thinking of using UpCloud, use promo code:
      <strong>6SXQH7</strong>, for a free $25 account credit.
    </h3>
    <br>
    <h3>What other's have said about the tutorial:</h3>
    <div v-if="!loading">
      <div v-if="responses.length > 0">
        <div v-for="(response, index) in responses" :key="index">
          <p
            v-if="response.submitted"
          >{{ response.name + ' (' + response.username + ') says: ' + response.response }}</p>
          <br>
        </div>
      </div>
      <div v-else>
        <p>No one has given any feedback yet!</p>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    data() {
      return {
        responses: [],
        loading: true
      };
    },
    mounted() {
      this.loading = true;
      this.$axios.get('/response').then(response => {
        this.responses = response.data.responseList;
        this.loading = false;
      });
    }
  };
</script>

