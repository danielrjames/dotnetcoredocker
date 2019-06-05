<template>
  <div class="row">
    <div class="col-md-6 col-md-offset-3">
      <div class="row">
        <div class="col-md-12">
          <h2>Register</h2>
        </div>
      </div>
      <div class="head-error-wrapper">
        <transition name="fade">
          <div class="head-error" v-if="hasError">
            <span class="text-danger">There was an unexpected error. Please try again.</span>
          </div>
        </transition>
      </div>
      <form @submit.prevent="handleSubmit">
        <app-text-input
          label="Name"
          placeholder="Name"
          input-name="name"
          :max-length="250"
          :display-validations="false"
          :touch-validation="touchValidation"
          @output="registration.name = $event"
          @valid="validStatus.name = $event"
        ></app-text-input>
        <app-email-input
          label="Email (this can be fake, just a demo site)"
          placeholder="Email Address"
          :display-validations="false"
          :touch-validation="touchValidation"
          @output="registration.email = $event"
          @valid="validStatus.email = $event"
        ></app-email-input>
        <app-url-input
          label="Create a Quirky Username"
          placeholder="Username"
          input-name="username"
          auto-complete="off"
          :min-length="4"
          :max-length="30"
          :display-validations="false"
          :urlValid="usernameAvail"
          :touch-validation="touchValidation"
          @output="checkUsername"
          @valid="validStatus.username = $event"
        ></app-url-input>
        <app-password-input
          label="Password"
          placeholder="Password"
          :touch-validation="touchValidation"
          @output="registration.password = $event"
          @valid="validStatus.password = $event"
        ></app-password-input>
        <div class="form-group btn-register">
          <button type="submit" class="btn btn-default">Register</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
  import TextInput from '../../components/form/TextInput.vue';
  import EmailInput from '../../components/form/EmailInput.vue';
  import PasswordInput from '../../components/form/PasswordInput.vue';
  import UrlInput from '../../components/form/UrlInput.vue';
  import _ from 'lodash';
  import { mapGetters } from 'vuex';

  export default {
    meta: {
      publicOnly: true
    },
    data() {
      return {
        title: 'Register',
        description: 'This is the register page description.',
        registration: {
          name: '',
          email: '',
          password: '',
          username: ''
        },
        validStatus: {
          name: false,
          email: false,
          password: false,
          username: false
        },
        touchValidation: false,
        usernameAvail: null
      };
    },
    head() {
      return {
        title: this.title,
        meta: [
          {
            hid: 'description',
            name: 'description',
            content: this.description
          }
        ]
      };
    },
    components: {
      appTextInput: TextInput,
      appEmailInput: EmailInput,
      appUrlInput: UrlInput,
      appPasswordInput: PasswordInput
    },
    computed: {
      ...mapGetters({
        preventSubmit: 'preventAuthSubmit',
        hasError: 'registerError'
      })
    },
    methods: {
      checkUsername(input) {
        this.usernameAvail = null;
        this.isNameUnique(input);
      },
      isNameUnique: _.debounce(function(input) {
        if (this.validStatus.username) {
          this.$axios
            .$get('account/username?username=' + input)
            .then(response => {
              if (response === true) {
                this.usernameAvail = true;
                this.registration.username = input;
              } else {
                this.usernameAvail = false;
                this.validStatus.username = false;
              }
            });
        }
      }, 500),
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
            this.$store.dispatch('registerUser', this.registration);
          } else {
            this.touchValidation = true;
          }
        }
      }
    }
  };
</script>

<style scoped>
  .btn-register {
    margin-top: 70px;
  }
</style>
