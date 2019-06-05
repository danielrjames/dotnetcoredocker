const getters = {
  authChecked: state => {
    return state.auth.checked;
  },
  isAuth: state => {
    return state.auth.authenticated;
  },
  isAuthHeaderSet: state => {
    return state.auth.authHeaderSet;
  },
  initSetTimer: state => {
    return state.auth.initRenewTime;
  },
  preventAuthSubmit: state => {
    return state.auth.preventSubmit;
  },
  loginError: state => {
    return state.auth.loginError;
  },
  registerError: state => {
    return state.auth.registerError;
  },
  name: state => {
    return state.account.name;
  },
  submitStatus: state => {
    return state.account.submitStatus;
  }
};

export default getters;
