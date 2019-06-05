<template>
  <div :class="['form-group', {'has-error': inputError}]">
    <label :for="inputName" class="control-label" v-if="showLabel">{{ label }}</label>
    <div :class="{'hv-search-field-container': isSearch}">
      <i class="fa fa-search" aria-hidden="true" v-if="isSearch"></i>
      <input
        type="text"
        :name="inputName"
        :class="[customClass, 'form-control', {'hv-search-field': isSearch}]"
        :placeholder="placeholder"
        :ref="inputName"
        :maxlength="maxLength"
        v-model.trim="inputText"
        :disabled="disabled"
        @input="validateText"
        @blur="checkBlur"
        :autocomplete="autoComplete"
      >
    </div>
    <div class="validation-messages-wrapper" v-if="displayValidations && inputError">
      <div class="validation-messages">
        <span v-if="requiredError" class="text-danger">{{ label }} is required</span>
        <span
          v-if="minLengthError"
          class="text-danger"
        >{{ label }} must be at least {{minLength}} characters</span>
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
      customClass: {
        type: String,
        default: ''
      },
      placeholder: {
        type: String,
        default: 'Text'
      },
      inputName: {
        type: String,
        default: 'text'
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
      isSearch: {
        type: Boolean,
        default: false
      },
      displayValidations: {
        type: Boolean,
        default: true
      },
      touchValidation: {
        type: Boolean,
        default: false
      }
    },
    data() {
      return {
        inputText: '',
        inputError: false,
        requiredError: false,
        minLengthError: false,
        maxLengthError: false
      };
    },
    created() {
      if (this.inputValue) {
        this.inputText = this.inputValue;
        this.validateText();
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
        if (newValue !== this.inputText) {
          this.inputText = newValue;
          this.validateText();
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
      checkBlur() {
        if (this.validateBlur) {
          this.checkValidation();
        }
      },
      validateText() {
        this.checkRequired();
        this.checkMinLength();
        if (!this.requiredError && !this.maxLengthError && !this.minLengthError) {
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
        this.checkMinLength();
        if (this.requiredError || this.maxLengthError || this.minLengthError) {
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
