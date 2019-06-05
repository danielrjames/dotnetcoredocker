<template>
  <div :class="['form-group', {'has-error': inputError}]">
    <div v-if="showLabel">
      <label class="control-label">{{ label }}</label>
    </div>
    <div class="hv-checkbox-group" v-if="!inline">
      <div
        :class="['checkbox', { 'chrome-browser': chromeBrowser }]"
        v-for="(item, index) in options"
        :key="index"
      >
        <input
          type="checkbox"
          :id="inputName + '_' + index"
          :name="inputName + '_' + index"
          :value="item.value"
          v-model="checked"
          :disabled="disabled"
          @change="validateSelect"
        >
        <label :for="inputName + '_' + index">{{ item.name }}</label>
      </div>
      <div class="validation-messages" v-if="displayValidations && inputError">
        <span v-if="requiredError" class="text-danger">{{ label }} is required</span>
      </div>
    </div>
    <div class="hv-checkbox-inline-group" v-else>
      <span
        :class="['checkbox-inline', { 'chrome-browser': chromeBrowser }]"
        v-for="(item, index) in options"
        :key="index"
      >
        <input
          type="checkbox"
          :id="inputName + '_' + index"
          :name="inputName + '_' + index"
          :value="item.value"
          v-model="checked"
          :disabled="disabled"
          @change="validateSelect"
        >
        <label :for="inputName + '_' + index">{{ item.name }}</label>
      </span>
      <div class="validation-messages-wrapper" v-if="displayValidations && inputError">
        <div class="validation-messages">
          <span v-if="requiredError" class="text-danger">{{ label }} is required</span>
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
        default: 'Default Checkbox'
      },
      showLabel: {
        type: Boolean,
        default: true
      },
      inputName: {
        type: String,
        default: 'defaultCheckbox'
      },
      inputValue: {
        type: [Boolean, Array],
        default: null
      },
      options: {
        type: Array,
        default: () => [
          { name: 'Default Selection 1', value: 1 },
          { name: 'Default Selection 2', value: 2 }
        ]
      },
      disabled: {
        type: Boolean,
        default: false
      },
      inline: {
        type: Boolean,
        default: false
      },
      required: {
        type: Boolean,
        default: true
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
        checked: [],
        inputError: false,
        requiredError: false,
        chromeBrowser: false
      };
    },
    created() {
      if (this.inputValue && this.options.length > 0) {
        if (this.options.length === 1) {
          if (typeof this.inputValue === 'boolean') {
            if (this.inputValue) {
              this.checked.push(this.options[0].value);
            }
          }
        } else {
          if (Array.isArray(this.inputValue)) {
            for (let o = 0; o < this.inputValue.length; o++) {
              for (let i = 0; i < this.options.length; i++) {
                if (this.options[i].value === this.inputValue[o]) {
                  this.checked.push(this.options[i].value);
                  break;
                }
              }
            }
          }
        }
        this.checkValidation();
      }
    },
    mounted() {
      this.chromeBrowser =
        !!window.chrome && (!!window.chrome.webstore || !!window.chrome.runtime);
    },
    watch: {
      inputValue(newValue, oldValue) {
        if (newValue !== null && this.options.length > 0) {
          if (this.options.length === 1) {
            if (typeof newValue === 'boolean') {
              if (
                !(newValue && this.checked.length === 1) &&
                !(!newValue && this.checked.length === 0)
              ) {
                this.checked = []; //clearing for new update
                if (newValue) {
                  this.checked.push(this.options[0].value);
                }
              }
            }
          } else {
            if (Array.isArray(newValue)) {
              if (newValue !== this.checked) {
                this.checked = []; //clearing for new update
                for (let o = 0; o < newValue.length; o++) {
                  for (let i = 0; i < this.options.length; i++) {
                    if (this.options[i].value === newValue[o]) {
                      this.checked.push(this.options[i].value);
                      break;
                    }
                  }
                }
              }
            }
          }
          this.checkValidation();
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
          if (this.checked.length > 0) {
            if (this.requiredError) {
              this.requiredError = false;
            }
          } else {
            this.requiredError = true;
          }
        }
      },
      validateSelect() {
        if (!this.disabled) {
          this.checkRequired();
          if (!this.requiredError) {
            if (this.inputError) {
              this.inputError = false;
            }
            if (this.options.length > 1) {
              this.$emit('output', this.checked);
            } else {
              this.$emit('output', !!(this.checked.length > 0));
            }
            this.$emit('valid', true);
          } else {
            this.inputError = true;
            this.$emit('valid', false);
          }
        }
      },
      checkValidation() {
        if (!this.disabled) {
          this.checkRequired(this.selected);
          if (this.requiredError) {
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
    }
  };
</script>

<style>
  .checkbox {
    position: relative;
    display: block;
    margin-top: 0;
    margin-bottom: 15px;
  }
  .checkbox-inline {
    position: relative;
    display: inline-block;
    margin-left: 0px;
    margin-bottom: 0;
    padding: 0;
    font-weight: 400;
    vertical-align: middle;
    cursor: default;
  }

  .checkbox-inline + .checkbox-inline {
    margin-top: 0;
    margin-left: 10px;
  }

  .checkbox input[type='checkbox'],
  .checkbox-inline input[type='checkbox'] {
    position: absolute;
    z-index: -1;
    opacity: 0;
  }
  .checkbox label,
  .checkbox-inline label {
    position: relative;
    min-height: 20px;
    margin: 0 15px 0 0;
    padding: 0 0 0 30px;
    font-weight: 400;
    cursor: pointer;
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
  }
  .checkbox.chrome-browser label,
  .checkbox-inline.chrome-browser label {
    padding: 1px 0 0 30px !important;
  }
  .checkbox label::before,
  .checkbox-inline label::before {
    position: absolute;
    top: 0;
    left: 0;
    content: '';
    display: inline-block;
    height: 20px;
    width: 20px;
    background-color: #fff;
    border: 1px solid #e5e5e5;
    border-radius: 1px;
  }
  .checkbox label::after,
  .checkbox-inline label::after {
    position: absolute;
    top: 6px;
    left: 5px;
    display: inline-block;
    height: 5px;
    width: 10px;
    border-left: 2px solid #333;
    border-bottom: 2px solid #333;
    transform: rotate(-45deg);
  }
  .checkbox input[type='checkbox'] + label::after,
  .checkbox-inline input[type='checkbox'] + label::after {
    content: none;
  }
  .checkbox input[type='checkbox']:checked + label::after,
  .checkbox-inline input[type='checkbox']:checked + label::after {
    content: '';
  }
  .checkbox input[type='checkbox']:disabled + label,
  .checkbox-inline input[type='checkbox']:disabled + label {
    cursor: not-allowed;
    opacity: 0.65;
  }
  .hv-checkbox-group .validation-messages {
    margin-top: -11px;
  }
  .hv-checkbox-inline-group .validation-messages {
    margin-top: 4px;
  }
</style>

