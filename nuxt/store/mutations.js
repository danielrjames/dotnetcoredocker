const mutations = {
  updateCheckStatus(state) {
    state.auth.checked = true;
  },
  authenticateUser(state, payload) {
    state.auth.authenticated = true;
    state.account.name = payload.name;
    state.account.submitStatus = payload.submitStatus;
  },
  updateLoginError(state, payload) {
    state.auth.loginError = payload;
  },
  updateRegisterError(state, payload) {
    state.auth.registerError = payload;
  },
  updateAuthSubmit(state, payload) {
    state.auth.preventSubmit = payload;
  },
  resetAuth(state) {
    state.auth.authenticated = false;
    state.auth.initRenewTime = 0;
    state.auth.loginError = false;
    state.auth.registerError = false;
    state.auth.preventSubmit = false;
    state.account.name = null;
    state.account.submitStatus = false;
  },
  updateAuthHeaderStatus(state, payload) {
    state.auth.authHeaderSet = payload;
  },
  setInitTokenTimer(state, payload) {
    state.auth.initRenewTime = payload;
  }
};

export default mutations;
