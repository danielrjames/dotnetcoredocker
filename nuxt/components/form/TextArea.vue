<template>
  <div :class="['form-group', {'has-error': inputError}]">
    <label :for="inputName" class="control-label" v-if="showLabel">{{ label }}</label>
    <textarea
      :name="inputName"
      class="form-control"
      :ref="inputName"
      :rows="rows"
      v-model.trim="inputText"
      :disabled="disabled"
      @input="validateText"
      @blur="checkBlur"
      :autocomplete="autoComplete"
    ></textarea>
    <div class="validation-messages-wrapper" v-if="displayValidations">
      <div class="validation-messages">
        <span v-if="inputError">
          <span v-if="requiredError" class="text-danger">{{ label }} is required</span>
          <span
            v-if="minLengthError"
            class="text-danger"
          >{{ label }} must be at least {{minLength}} characters</span>
          <span
            v-if="maxLengthError"
            class="text-danger"
          >{{ label }} exceeds limit of {{maxLength}} characters</span>
        </span>
        <span
          :class="['pull-right', {'text-danger': inputError}]"
          v-show="inputText.length > 0"
        >{{inputText.length}}</span>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    props: {
      label: {
        type: String,
        default: 'Default Text Area'
      },
      showLabel: {
        type: Boolean,
        default: true
      },
      inputName: {
        type: String,
        default: 'textarea'
      },
      inputValue: {
        type: String,
        default: ''
      },
      rows: {
        type: Number,
        default: 4
      },
      cols: {
        type: Number,
        default: 50
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
        default: 'off'
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
      checkMaxLength() {
        if (this.inputText.length > 0) {
          if (this.inputText.length <= this.maxLength) {
            if (this.maxLengthError) {
              this.maxLengthError = false;
            }
          } else {
            this.maxLengthError = true;
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
        this.checkMaxLength();
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
        this.checkMaxLength();
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
