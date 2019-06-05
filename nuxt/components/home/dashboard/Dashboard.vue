<template>
  <div>
    <h1>Welcome to the Dashboard</h1>
    <h2>You are logged in!</h2>
    <br>
    <br>
    <div class="row">
      <div v-if="!submitStatus">
        <div class="col-md-12">
          <p>So, since you are here, I assume you came from the medium post. What did you think?</p>
        </div>
        <div class="col-md-4" v-if="!success">
          <app-dropdown
            label="How would you rate the tutorial?"
            input-name="rating"
            ref="rating"
            :input-value="rating"
            :options="options"
            :display-validations="true"
            :touch-validation="touchValidation"
            @output="rating = $event"
            @valid="validStatus.rating = $event"
          ></app-dropdown>
          <button class="btn btn-default" @click="handleSubmit">Submit</button>
        </div>
        <div class="col-md-4" v-else>
          <p>Thank you for your feedback!</p>
        </div>
      </div>
      <div class="col-md-6" v-else>
        <p>You have already submitted your feedback. Log out and see it listed on the home page!</p>
      </div>
    </div>
  </div>
</template>

<script>
  import Dropdown from '../../form/Dropdown.vue';
  import { mapGetters } from 'vuex';

  export default {
    data() {
      return {
        options: [
          { name: "Best tutorial I've seen!", value: "Best tutorial I've seen!" },
          { name: 'It was okay...', value: 'It was okay...' },
          { name: 'Could have been better', value: 'Could have been better' },
          {
            name: "I don't know why tf I'm here?",
            value: "I don't know why tf I'm here?"
          }
        ],
        touchValidation: false,
        rating: '',
        validStatus: {
          rating: false
        },
        success: false,
        preventSubmit: false
      };
    },
    components: {
      appDropdown: Dropdown
    },
    computed: {
      ...mapGetters({
        submitStatus: 'submitStatus'
      })
    },
    methods: {
      handleSubmit() {
        if (!this.preventSubmit) {
          let valid = true;
          // if all properties are not valid, do not submit
          for (let key in this.validStatus) {
            if (
              this.validStatus.hasOwnProperty(key) &&
              this.validStatus[key] === false
            ) {
              valid = false;
            }
          }
          if (valid) {
            const data = {
              response: this.rating
            };
            this.$axios.post('/response/submit', data).then(response => {
              if (response) {
                this.success = true;
                this.$store.dispatch('renewTokens');
              }
            });
          } else {
            this.touchValidation = true;
          }
        }
      }
    }
  };
</script>
