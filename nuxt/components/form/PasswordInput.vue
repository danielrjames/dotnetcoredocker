<template>
  <div :class="['form-group', {'has-error': passwordError}]">
    <label :for="inputName" class="control-label" v-if="showLabel">{{ label }}</label>
    <input
      type="password"
      :name="inputName"
      class="form-control"
      :placeholder="placeholder"
      :ref="inputName"
      :maxlength="250"
      v-model.trim="password"
      :disabled="disabled"
      @input="validatePass"
      @blur="checkBlur"
      :autocomplete="autoComplete"
    >
    <div class="validation-messages-wrapper" v-if="displayValidations && !suppressErrors">
      <div class="validation-messages">
        <div v-if="!matchPassword">
          <div
            class="text-success"
            v-if="passwordCriteria.met && !passwordError"
          >Your password is secure</div>
          <div
            class="text-danger"
            v-if="maxLengthError"
          >{{ label }} exceeds limit of {{maxLength}} characters</div>
          <div v-if="passwordCriteria.met == false">
            <div class="row">
              <div class="col-md-12">
                <div class="row">
                  <div class="col-md-6" :class="{'criteria-met' : passwordCriteria.lowercase}">
                    <span>One lowercase letter</span>
                  </div>
                  <div class="col-md-6" :class="{'criteria-met' : passwordCriteria.special}">
                    <span>One special character</span>
                  </div>
                </div>
                <div class="row">
                  <div class="col-md-6" :class="{'criteria-met' : passwordCriteria.uppercase}">
                    <span>One uppercase letter</span>
                  </div>
                  <div class="col-md-6" :class="{'criteria-met' : passwordCriteria.min}">
                    <span>8 characters minimum</span>
                  </div>
                </div>
                <div class="row">
                  <div class="col-md-6" :class="{'criteria-met' : passwordCriteria.number}">
                    <span>One number</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div v-else>
          <div class="text-danger" v-if="passwordMatchError">
            <span>Passwords do not match</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    props: {
      label: {
        type: String,
        default: 'Password'
      },
      showLabel: {
        type: Boolean,
        default: true
      },
      placeholder: {
        type: String,
        default: 'Password'
      },
      inputName: {
        type: String,
        default: 'password'
      },
      inputValue: {
        type: String,
        default: ''
      },
      focus: {
        type: Boolean,
        default: false
      },
      disabled: {
        type: Boolean,
        default: false
      },
      autoComplete: {
        type: String,
        default: 'on'
      },
      displayValidations: {
        type: Boolean,
        default: true
      },
      required: {
        type: Boolean,
        default: true
      },
      maxLength: {
        type: Number,
        default: 128
      },
      matchPassword: {
        type: Boolean,
        default: false
      },
      passwordToMatch: {
        type: String,
        default: ''
      },
      suppressErrors: {
        type: Boolean,
        default: false
      },
      validateBlur: {
        type: Boolean,
        default: false
      },
      touchValidation: {
        type: Boolean,
        default: false
      }
    },
    data() {
      return {
        password: '',
        passwordCriteria: {
          lowercase: false,
          uppercase: false,
          number: false,
          special: false,
          min: false,
          max: false,
          met: false
        },
        passwordError: false,
        requiredError: false,
        maxLengthError: false,
        passwordMatchError: false
      };
    },
    created() {
      if (this.inputValue) {
        this.password = this.inputValue;
        this.validatePass();
      }
    },
    mounted() {
      if (this.focus) {
        this.$refs[this.inputName].focus();
      }
    },
    watch: {
      focus(newValue, oldValue) {
        if (newValue) {
          this.$refs[this.inputName].focus();
        }
      },
      inputValue(newValue, oldValue) {
        if (newValue !== this.password) {
          this.password = newValue;
          this.validatePass();
        }
      },
      touchValidation(newValue, oldValue) {
        if (newValue === true) {
          this.checkValidation();
        }
      }
    },
    methods: {
      checkRequired() {
        if (this.required) {
          if (this.password.length > 0) {
            if (this.requiredError) {
              this.requiredError = false;
            }
          } else {
            this.requiredError = true;
          }
        }
      },
      checkMaxLength() {
        if (this.password.length > 0) {
          if (this.password.length <= this.maxLength) {
            if (this.maxLengthError) {
              this.maxLengthError = false;
            }
          } else {
            this.maxLengthError = true;
          }
        }
      },
      validatePass: function() {
        if (!this.matchPassword) {
          if (this.password !== '') {
            if (this.password.length < 8) {
              this.passwordCriteria.min = false;
            } else {
              this.passwordCriteria.min = true;
            }

            if (this.password.length > 128) {
              this.passwordCriteria.max = false;
            } else {
              this.passwordCriteria.max = true;
            }

            if (this.password.search(/\d/) === -1) {
              this.passwordCriteria.number = false;
            } else {
              this.passwordCriteria.number = true;
            }

            if (this.password.search(/[a-z]/) === -1) {
              this.passwordCriteria.lowercase = false;
            } else {
              this.passwordCriteria.lowercase = true;
            }

            if (this.password.search(/[A-Z]/) === -1) {
              this.passwordCriteria.uppercase = false;
            } else {
              this.passwordCriteria.uppercase = true;
            }

            if (this.password.search(/\W+/) === -1) {
              this.passwordCriteria.special = false;
            } else {
              this.passwordCriteria.special = true;
            }

            if (
              (this.passwordCriteria.min === true &&
                this.passwordCriteria.max === true &&
                this.passwordCriteria.number === true &&
                this.passwordCriteria.lowercase === true &&
                this.passwordCriteria.uppercase === true &&
                this.passwordCriteria.special === true) ||
              this.suppressErrors
            ) {
              this.passwordCriteria.met = true;
              this.passwordError = false;
              this.$emit('output', this.password);
              this.$emit('valid', true);
            } else {
              this.passwordCriteria.met = false;
              this.$emit('valid', false);
            }
          } else {
            for (const key in this.passwordCriteria) {
              if (this.passwordCriteria.hasOwnProperty(key)) {
                if (this.passwordCriteria[key] !== false) {
                  this.passwordCriteria[key] = false;
                }
              }
            }
          }
        } else {
          // is password match
          if (this.password !== '') {
            // suppress error here when user starts typing again
            this.passwordError = false;
            this.passwordMatchError = false;
            if (this.passwordToMatch === this.password) {
              this.$emit('output', this.password);
              this.$emit('valid', true);
            } else {
              this.$emit('valid', false);
            }
          }
        }
      },
      checkBlur() {
        if (this.validateBlur) {
          this.checkValidation();
        }
      },
      checkValidation() {
        this.checkRequired();
        this.checkMaxLength();
        if (this.password !== '') {
          if (
            (!this.suppressErrors &&
              !this.matchPassword &&
              !this.requiredError &&
              !this.maxLengthError &&
              this.passwordCriteria.met) ||
            (!this.suppressErrors &&
              this.matchPassword &&
              this.passwordToMatch === this.password) ||
            (this.suppressErrors && !this.requiredError && !this.maxLengthError)
          ) {
            // everything is valid, get rid of errors
            if (this.passwordError === true) {
              this.passwordError = false;
              if (this.passwordMatchError === true) {
                this.passwordMatchError = false;
              }
            }
          } else {
            if (this.passwordError === false) {
              this.passwordError = true;
            }
            if (this.matchPassword) {
              this.passwordMatchError = true;
            }
            this.$emit('valid', false);
          }
        } else {
          this.passwordError = true;
          this.$emit('valid', false);
        }
      }
    }
  };
</script>

<style scoped>
  .criteria-met {
    opacity: 0.5;
    text-decoration: line-through;
  }
</style>
