import getters from './getters';
import mutations from './mutations';
import actions from './actions';

const state = () => ({
  auth: {
    checked: false,
    authenticated: false,
    authHeaderSet: false,
    initRenewTime: 0,
    loginError: false,
    registerError: false,
    preventSubmit: false
  },
  account: {
    name: null,
    submitStatus: false
  }
});

export default {
  state,
  getters,
  mutations,
  actions
};
