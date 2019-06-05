<template>
  <div class="row">
    <div class="col-md-6 col-md-offset-3">
      <div class="row">
        <div class="col-md-12">
          <h2>Log In</h2>
        </div>
      </div>
      <div class="head-error-wrapper">
        <transition name="fade">
          <div class="head-error" v-if="hasError">
            <span class="text-danger">Invalid Email and/or Password. Please try again.</span>
          </div>
        </transition>
      </div>
      <form @submit.prevent="handleSubmit">
        <app-email-input
          label="Email"
          placeholder="Email Address"
          :display-validations="false"
          :touch-validation="touchValidation"
          @output="login.email = $event"
          @valid="validStatus.email = $event"
        ></app-email-input>
        <app-password-input
          label="Password"
          placeholder="Password"
          :input-value="login.password"
          :display-validations="false"
          :suppress-errors="true"
          :touch-validation="touchValidation"
          @output="login.password = $event"
          @valid="validStatus.password = $event"
        ></app-password-input>
        <app-checkbox
          input-name="rememberMe"
          :input-value="login.rememberMe"
          :show-label="false"
          :required="false"
          :options="checkData"
          :touch-validation="touchValidation"
          @output="login.rememberMe = $event"
        ></app-checkbox>
        <div class="form-group btn-login">
          <button type="submit" class="btn btn-default">Login</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
  import EmailInput from '../../components/form/EmailInput.vue';
  import PasswordInput from '../../components/form/PasswordInput.vue';
  import Checkbox from '../../components/form/Checkbox.vue';
  import { mapGetters } from 'vuex';

  export default {
    meta: {
      publicOnly: true
    },
    data() {
      return {
        title: 'Log In',
        description: 'This is the login page description!',
        checkData: [{ name: 'Remember Me', value: 1 }],
        login: {
          email: '',
          password: '',
          rememberMe: false
        },
        validStatus: {
          email: false,
          password: false,
          rememberMe: true
        },
        touchValidation: false
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
      appEmailInput: EmailInput,
      appPasswordInput: PasswordInput,
      appCheckbox: Checkbox
    },
    computed: {
      ...mapGetters({
        preventSubmit: 'preventAuthSubmit',
        hasError: 'loginError'
      })
    },
    watch: {
      hasError(newValue, oldValue) {
        if (newValue === true) {
          this.login.password = '';
        }
      }
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
            let route = '/';
            if (this.$route.query.returnUrl) {
              route = this.$route.query.returnUrl;
            }
            const loginData = {
              email: this.login.email,
              password: this.login.password,
              rememberMe: this.login.rememberMe,
              routeRedirect: route
            };
            this.$store.dispatch('loginUser', loginData);
          } else {
            this.touchValidation = true;
          }
        }
      }
    }
  };
</script>

<style scoped>
  .btn-login {
    margin-top: -10px;
  }
</style>
