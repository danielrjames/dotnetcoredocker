export default function({ $axios, store }) {
  // IE 11 fix
  $axios.setHeader('Cache-Control', 'no-cache, no-store, must-revalidate');
  $axios.setHeader('Pragma', 'no-cache');
  $axios.setHeader('Expires', '0');

  // Auth Header status
  if (store.state.auth.authenticated && !store.state.auth.authHeaderSet) {
    store.dispatch('setAuthHeader');
  } else if (
    !store.state.auth.authenticated &&
    store.state.auth.authHeaderSet
  ) {
    store.dispatch('removeAuthHeader');
  }

  $axios.onRequest(config => {
    // showing for dotnetcoredocker.com
    console.log('Making request to: ' + config.url);
  });

  $axios.onError(error => {
    const code = parseInt(error.response && error.response.status);
    if (code === 401) {
      return store.dispatch('renewTokens');
    }
  });
}
