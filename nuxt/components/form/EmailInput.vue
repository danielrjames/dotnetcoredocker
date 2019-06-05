<template>
  <div :class="['form-group', {'has-error': emailError}]">
    <label :for="inputName" class="control-label" v-if="showLabel">{{ label }}</label>
    <input
      type="email"
      :name="inputName"
      :class="[customClass, 'form-control']"
      :placeholder="placeholder"
      :ref="inputName"
      :maxlength="maxLength"
      v-model.trim="email"
      :disabled="disabled"
      @input="validateEmail"
      @blur="checkBlur"
      :autocomplete="autoComplete"
    >
    <div class="validation-messages-wrapper" v-if="displayValidations && emailError">
      <div class="validation-messages">
        <span v-if="requiredError" class="text-danger">{{ label }} is required</span>
        <span
          v-if="validError && !maxLengthError"
          class="text-danger"
        >Please enter a valid email address</span>
        <!-- <span
          v-if="maxLengthError"
          class="text-danger"
        >{{ label }} exceeds limit of {{maxLength}} characters</span>-->
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    props: {
      label: {
        type: String,
        default: 'Email'
      },
      showLabel: {
        type: Boolean,
        default: true
      },
      customClass: {
        type: String,
        default: ''
      },
      placeholder: {
        type: String,
        default: 'Email Address'
      },
      inputName: {
        type: String,
        default: 'email'
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
      required: {
        type: Boolean,
        default: true
      },
      maxLength: {
        type: Number,
        default: 225
      },
      validateBlur: {
        type: Boolean,
        default: false
      },
      displayValidations: {
        type: Boolean,
        default: true
      },
      emailTaken: {
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
        email: '',
        emailError: false,
        validError: false,
        takenError: false,
        requiredError: false,
        maxLengthError: false
      };
    },
    created() {
      if (this.inputValue) {
        this.email = this.inputValue;
        this.validateEmail();
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
      emailTaken(newValue, oldValue) {
        if (newValue) {
          this.takenError = true;
        }
      },
      inputValue(newValue, oldValue) {
        if (newValue !== this.email) {
          this.email = newValue;
          this.validateEmail();
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
          if (this.email.length > 0) {
            if (this.requiredError) {
              this.requiredError = false;
            }
          } else {
            this.requiredError = true;
          }
        }
      },
      checkValid() {
        const emailRegex = /(^$|^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$)/;
        if (emailRegex.test(this.email)) {
          if (this.validError) {
            this.validError = false;
          }
        } else {
          this.validError = true;
        }
      },
      validateEmail() {
        this.checkRequired();
        this.checkValid();
        // this.checkMaxLength();
        if (!this.requiredError && !this.validError && !this.maxLengthError) {
          if (this.emailError) {
            this.emailError = false;
          }
          this.$emit('output', this.email);
          this.$emit('valid', true);
        } else {
          this.$emit('valid', false);
        }
      },
      checkBlur() {
        if (this.validateBlur) {
          this.checkValidation();
        }
      },
      checkValidation() {
        this.checkRequired();
        this.checkValid();
        // this.checkMaxLength();
        if (this.requiredError || this.validError || this.maxLengthError) {
          if (!this.emailError) {
            this.emailError = true;
          }
          this.$emit('valid', false);
        } else {
          // is valid, get rid of errors
          if (this.emailError) {
            this.emailError = false;
          }
        }
      }
    }
  };
</script>
