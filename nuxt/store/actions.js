import * as jwtDecode from 'jwt-decode';

const ACCESS_TOKEN_NAME = 'accessToken';
const REFRESH_TOKEN_NAME = 'refreshToken';
const COOKIE_OPTIONS = {
  path: '/',
  maxAge: 60 * 60 * 24 * 7,
  secure: process.env.NODE_ENV === 'production' ? true : false,
  sameSite: 'Strict'
};

let renewTimer;
let cookieModule;

const actions = {
  async nuxtServerInit({ commit, dispatch, state }, context) {
    if (!state.auth.checked) {
      cookieModule = context.app.$cookies;
      return await dispatch('INIT_CHECK_TOKENS').then(() => {
        commit('updateCheckStatus');
      });
    }
  },
  INIT_CHECK_TOKENS({ dispatch }) {
    const cookies = cookieModule.getAll();
    if (cookies) {
      if (cookies[ACCESS_TOKEN_NAME] && cookies[REFRESH_TOKEN_NAME]) {
        return dispatch('INIT_VALIDATE_TOKENS', {
          access_token: cookies[ACCESS_TOKEN_NAME],
          refresh_token: cookies[REFRESH_TOKEN_NAME],
          renewed: false
        });
      } else {
        cookieModule.removeAll();
      }
    }
    return;
  },
  INIT_VALIDATE_TOKENS({ commit, dispatch }, data) {
    const decodedToken = jwtDecode(data.access_token);
    const currentTime = Math.round(+new Date() / 1000);
    const refreshLife = decodedToken.rExp - currentTime;
    if (refreshLife - 15 > 0) {
      const accessLife = decodedToken.exp - currentTime;
      if (accessLife - 900 > 0) {
        if (data.renewed) {
          cookieModule.setAll([
            {
              name: ACCESS_TOKEN_NAME,
              value: data.access_token,
              opts: COOKIE_OPTIONS
            },
            {
              name: REFRESH_TOKEN_NAME,
              value: data.refresh_token,
              opts: COOKIE_OPTIONS
            }
          ]);
        }
        this.$axios.setToken(data.access_token, 'Bearer');
        commit('authenticateUser', {
          name: decodedToken.name,
          submitStatus: decodedToken.submitStatus
        });
        return commit('setInitTokenTimer', accessLife); // set renew token timer on client init
      }
      return dispatch('INIT_RENEW_TOKENS', data.refresh_token);
    }
    return cookieModule.removeAll(); // removes tokens
  },
  INIT_RENEW_TOKENS({ dispatch }, token) {
    return this.$axios
      .$post('/account/renew', {
        refreshToken: token,
        clientId: 1,
        grantType: 'refresh_token',
        scope: 'offline_access'
      })
      .then(response => {
        if (response.access_token && response.refresh_token) {
          return dispatch('INIT_VALIDATE_TOKENS', {
            access_token: response.access_token,
            refresh_token: response.refresh_token,
            renewed: true
          });
        } else {
          return cookieModule.removeAll();
        }
      })
      .catch(err => {
        return cookieModule.removeAll();
      });
  },
  async registerUser({ commit, dispatch }, componentData) {
    commit('updateAuthSubmit', true);
    const here = 'register';
    const data = {
      name: componentData.name,
      email: componentData.email,
      username: componentData.username,
      password: componentData.password,
      clientId: 1,
      grantType: 'password',
      scope: 'offline_access'
    };
    return await this.$axios
      .$post('/account/register', data)
      .then(response => {
        const data = {
          access_token: response.access_token,
          refresh_token: response.refresh_token,
          routeRedirect: '/',
          origin: here
        };
        return dispatch('handleResponse', data);
      })
      .catch(err => {
        return dispatch('handleError', { error: err, origin: here });
      });
  },
  async loginUser({ commit, dispatch }, componentData) {
    commit('updateAuthSubmit', true);
    const here = 'login';
    const data = {
      email: componentData.email,
      password: componentData.password,
      rememberMe: componentData.rememberMe,
      clientId: 1,
      grantType: 'password',
      scope: 'offline_access'
    };
    return await this.$axios
      .$post('/account/login', data)
      .then(response => {
        const responseData = {
          access_token: response.access_token,
          refresh_token: response.refresh_token,
          routeRedirect: componentData.routeRedirect,
          origin: here
        };
        return dispatch('handleResponse', responseData);
      })
      .catch(err => {
        return dispatch('handleError', { error: err, origin: here });
      });
  },
  handleResponse({ commit, dispatch, state }, data) {
    if (data.access_token && data.refresh_token) {
      if (data.origin === 'login') {
        if (state.auth.loginError) {
          commit('updateLoginError', false);
        }
      } else if (data.origin === 'register') {
        if (state.auth.registerError) {
          commit('updateRegisterError', false);
        }
      } else if (data.origin === 'renew') {
        // add logic here if needed
      }
      if (state.auth.preventSubmit) {
        commit('updateAuthSubmit', false);
      }
      return dispatch('validateTokens', data);
    }
    return dispatch('handleError', { error: true, origin: data.origin });
  },
  handleError({ commit, state }, data) {
    if (data.origin === 'login') {
      if (!state.auth.loginError) {
        commit('updateLoginError', true);
      }
    } else if (data.origin === 'register') {
      if (!state.auth.registerError) {
        commit('updateRegisterError', true);
      }
    } else if (data.origin === 'renew') {
      this.$router.replace({ path: '/login' });
    }
    if (state.auth.preventSubmit) {
      commit('updateAuthSubmit', false);
    }
    if (state.auth.authenticated) {
      commit('resetAuth');
    }
    const cookies = this.$cookies.getAll();
    if (cookies) {
      this.$cookies.removeAll();
    }
    return;
  },
  validateTokens({ commit, dispatch }, data) {
    const decodedToken = jwtDecode(data.access_token);
    const currentTime = Math.round(+new Date() / 1000);
    const refreshLife = decodedToken.rExp - currentTime;
    if (refreshLife - 15 > 0) {
      const accessLife = decodedToken.exp - currentTime;
      if (accessLife - 5 > 0) {
        this.$cookies.setAll([
          {
            name: ACCESS_TOKEN_NAME,
            value: data.access_token,
            opts: COOKIE_OPTIONS
          },
          {
            name: REFRESH_TOKEN_NAME,
            value: data.refresh_token,
            opts: COOKIE_OPTIONS
          }
        ]);
        commit('authenticateUser', {
          name: decodedToken.name,
          submitStatus: decodedToken.submitStatus
        });
        this.$axios.setToken(data.access_token, 'Bearer');
        commit('updateAuthHeaderStatus', true);
        if (data.routeRedirect) {
          this.$router.replace({ path: data.routeRedirect });
        }
        return dispatch('renewTokenTimer', accessLife);
      } else {
        // return dispatch('renewTokens');
      }
    }
    return dispatch('logoutUser', '/login');
  },
  renewTokenTimer({ dispatch, state }, expirationTime) {
    if (renewTimer) {
      clearTimeout(renewTimer);
    }
    renewTimer = setTimeout(() => {
      if (state.auth.authenticated) {
        dispatch('renewTokens');
      }
    }, expirationTime * 1000);
    return true;
  },
  async renewTokens({ dispatch, commit }) {
    const here = 'renew';
    const atk = this.$cookies.get(ACCESS_TOKEN_NAME);
    const r = this.$cookies.get(REFRESH_TOKEN_NAME);
    if (atk && r) {
      const data = {
        refreshToken: r,
        clientId: 1,
        grantType: 'refresh_token',
        scope: 'offline_access'
      };
      return await this.$axios
        .$post('/account/renew', data)
        .then(response => {
          if (response.access_token && response.refresh_token) {
            return dispatch('handleResponse', {
              access_token: response.access_token,
              refresh_token: response.refresh_token,
              origin: here
            });
          } else {
            return dispatch('handleError', { error: true, origin: here });
          }
        })
        .catch(err => {
          return dispatch('handleError', { error: err, origin: here });
        });
    }
    return dispatch('handleError', { error: true, origin: here });
  },
  logoutUser({ commit, dispatch }, route) {
    commit('resetAuth');
    if (renewTimer) {
      clearTimeout(renewTimer); // stopping renew timer
    }
    if (route !== '') {
      this.$router.replace({ path: route });
    } else {
      this.$router.replace({ path: '/' });
    }
    dispatch('removeAuthHeader');
    return dispatch('revokeAndRemove');
  },
  revokeAndRemove() {
    const rToken = this.$cookies.get(REFRESH_TOKEN_NAME);
    if (rToken) {
      this.$axios.$post('/account/revoke', {
        refreshToken: rToken
      });
    }
    const cookies = this.$cookies.getAll();
    if (cookies) {
      this.$cookies.removeAll();
    }
    return;
  },
  setAuthHeader({ commit }) {
    this.$axios.setToken(this.$cookies.get(ACCESS_TOKEN_NAME), 'Bearer');
    commit('updateAuthHeaderStatus', true);
  },
  removeAuthHeader({ commit }) {
    this.$axios.setToken(false);
    commit('updateAuthHeaderStatus', false);
  },
  resetRegisterError({ commit }) {
    commit('updateRegisterError', false);
  },
  resetLoginError({ commit }) {
    commit('updateLoginError', false);
  }
};

export default actions;
