<template>
  <div :class="['form-group', {'has-error': inputError}]">
    <label :for="inputName" class="control-label" v-if="showLabel">{{ label }}</label>
    <input
      type="text"
      :name="inputName"
      class="form-control"
      :placeholder="placeholder"
      :ref="inputName"
      :maxlength="maxLength"
      v-model.trim="inputText"
      :disabled="disabled"
      @keypress="validateKeypress"
      @input="validateText"
      @blur="checkBlur"
      :autocomplete="autoComplete"
    >
    <div class="validation-messages-wrapper" v-if="displayValidations">
      <div class="validation-messages">
        <div v-if="inputError">
          <span v-if="requiredError" class="text-danger">{{ label }} is required</span>
          <span
            v-if="!requiredError && validError && !minLengthError && !maxLengthError"
            class="text-danger"
          >{{ label }} cannot begin or end with a hyphen, nor contain consecutive hyphens</span>
          <span
            v-if="!requiredError && minLengthError"
            class="text-danger"
          >{{ label }} must be at least {{minLength}} characters</span>
          <!-- <span
            v-if="!requiredError && maxLengthError"
            class="text-danger"
          >{{ label }} exceeds limit of {{maxLength}} characters</span>-->
        </div>
        <div v-if="!inputError && !validError && urlValid !== null">
          <span v-if="urlValid" class="text-success">{{inputText}} is available</span>
          <span v-if="!urlValid" class="text-danger">{{inputText}} is not available</span>
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
        default: 'Text'
      },
      showLabel: {
        type: Boolean,
        default: true
      },
      placeholder: {
        type: String,
        default: 'Text'
      },
      inputName: {
        type: String,
        default: 'urlText'
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
      minLength: {
        type: Number,
        default: 0
      },
      maxLength: {
        type: Number,
        default: 250
      },
      validateBlur: {
        type: Boolean,
        default: false
      },
      displayValidations: {
        type: Boolean,
        default: true
      },
      urlValid: {
        type: Boolean,
        default: null
      },
      touchValidation: {
        type: Boolean,
        default: false
      }
    },
    data() {
      return {
        inputText: this.inputValue,
        inputError: false,
        validError: false,
        requiredError: false,
        minLengthError: false,
        maxLengthError: false
      };
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
        if (newValue !== this.inputText) {
          this.inputText = newValue;
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
          if (this.inputText.length > 0) {
            if (this.requiredError) {
              this.requiredError = false;
            }
          } else {
            this.requiredError = true;
          }
        }
      },
      checkMinLength() {
        if (this.minLength > 0) {
          if (this.inputText.length > 0) {
            if (this.inputText.length >= this.minLength) {
              if (this.minLengthError) {
                this.minLengthError = false;
              }
            } else {
              this.minLengthError = true;
            }
          } else {
            this.minLengthError = false;
          }
        }
      },
      checkValid() {
        const slugRegex = /^[A-Za-z0-9]+(?:-[A-Za-z0-9]+)*$/;
        if (slugRegex.test(this.inputText)) {
          if (this.validError) {
            this.validError = false;
          }
        } else {
          this.validError = true;
        }
      },
      validateKeypress(event) {
        const slugRegex = /^[a-zA-Z0-9]+$/;
        if (event.charCode != 45) {
          if (!slugRegex.test(event.key)) {
            event.preventDefault();
          }
        }
      },
      checkBlur() {
        if (this.validateBlur) {
          this.checkValidation();
        }
      },
      validateText() {
        this.checkRequired();
        this.checkValid();
        this.checkMinLength();
        // this.checkMaxLength();
        if (
          !this.requiredError &&
          !this.validError &&
          !this.maxLengthError &&
          !this.minLengthError
        ) {
          if (this.inputError) {
            this.inputError = false;
          }
          this.$emit('output', this.inputText);
          this.$emit('valid', true);
        } else {
          this.$emit('valid', false);
        }
      },
      checkValidation() {
        this.checkRequired();
        this.checkValid();
        this.checkMinLength();
        // this.checkMaxLength();
        if (
          this.requiredError ||
          this.validError ||
          this.maxLengthError ||
          this.minLengthError
        ) {
          if (!this.inputError) {
            this.inputError = true;
          }
          this.$emit('valid', false);
        } else {
          // is valid, get rid of errors
          if (this.inputError) {
            this.inputError = false;
          }
        }
      }
    }
  };
</script>
