<template>
  <div :class="['form-group', {'has-error': inputError}]">
    <div v-if="showLabel">
      <label :for="inputName" class="control-label">{{ label }}</label>
    </div>
    <div class="hv-dropdown-wrapper">
      <select
        :class="[customClass, 'form-control', 'hv-dropdown']"
        :name="inputName"
        :ref="inputName"
        v-model="selectedItem"
        :disabled="disabled"
        @change="validateSelect"
      >
        <option v-for="(item, index) in options" :value="item.value" :key="index">{{ item.name }}</option>
      </select>
      <i class="fa fa-angle-down text-right" aria-hidden="true"></i>
    </div>
    <div class="validation-messages-wrapper" v-if="displayValidations && inputError">
      <div class="validation-messages">
        <span v-if="requiredError" class="text-danger">{{ label }} is required</span>
      </div>
    </div>
  </div>
</template>

<script>
  export default {
    props: {
      label: {
        type: String,
        default: 'Default Dropdown'
      },
      showLabel: {
        type: Boolean,
        default: true
      },
      customClass: {
        type: String,
        default: ''
      },
      inputName: {
        type: String,
        default: 'defaultDropdown'
      },
      inputValue: {
        type: [String, Number],
        default: null
      },
      options: {
        type: Array,
        default: () => [
          { name: 'Default Selection 1', value: 1 },
          { name: 'Default Selection 2', value: 2 }
        ]
      },
      focus: {
        type: Boolean,
        default: false
      },
      disabled: {
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
        selectedItem: null,
        inputError: false,
        requiredError: false
      };
    },
    created() {
      if (this.inputValue) {
        if (this.options.length > 0) {
          for (let i = 0; i < this.options.length; i++) {
            if (this.options[i].value == this.inputValue) {
              this.selectedItem = this.inputValue;
              this.validateSelect();
              break;
            }
          }
        }
      }
    },
    watch: {
      focus(newValue, oldValue) {
        if (newValue) {
          this.$refs[this.inputName].focus();
        }
      },
      inputValue(newValue, oldValue) {
        if (newValue) {
          if (newValue !== this.selectedItem) {
            if (this.options.length > 0) {
              for (let i = 0; i < this.options.length; i++) {
                if (this.options[i].value === newValue) {
                  this.selectedItem = newValue;
                  this.validateSelect();
                  break;
                }
              }
            }
          }
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
          if (this.selectedItem !== null) {
            if (this.requiredError) {
              this.requiredError = false;
            }
          } else {
            this.requiredError = true;
            this.inputError = true;
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
            this.$emit('output', this.selectedItem);
            this.$emit('valid', true);
          } else {
            this.$emit('valid', false);
          }
        }
      },
      checkValidation() {
        if (!this.disabled) {
          this.checkRequired(this.selectedItem);
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

<style scoped>
  select {
    -webkit-appearance: none;
    -moz-appearance: none;
  }
  select::-ms-value {
    background-color: transparent;
    color: #333;
  }
  select::-ms-expand {
    display: none;
  }
  .hv-dropdown-wrapper {
    position: relative;
  }
  .hv-dropdown-wrapper .fa {
    position: absolute;
    top: 13px;
    right: 10px;
    pointer-events: none;
  }
</style>
