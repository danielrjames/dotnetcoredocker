export default function({ redirect, route, store }) {
  if (store.state.auth.loginError) {
    store.dispatch('resetLoginError');
  }
  if (store.state.auth.registerError) {
    store.dispatch('resetRegisterError');
  }
  if (route.meta.some(record => record.requiresAuth)) {
    if (!store.state.auth.authenticated) {
      redirect({
        path: '/login',
        query: { returnUrl: route.path }
      });
    } else if (store.state.account.status !== 5) {
      redirect({ path: '/' });
    }
    if (route.meta.some(record => record.requiresType2)) {
      if (store.state.account.type !== 2) {
        redirect({ path: '/' });
      }
    }
    if (route.meta.some(record => record.requiresUnconfirmedEmail)) {
      if (store.state.account.emailConfirmed) {
        redirect({ path: '/' });
      }
    }
  } else if (route.meta.some(record => record.publicOnly)) {
    if (store.state.auth.authenticated) {
      redirect({ path: '/' });
    }
  }
}
