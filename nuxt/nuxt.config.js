const pkg = require('./package');

module.exports = {
  mode: 'universal',

  /*
  ** Headers of the page
  */
  head: {
    title: pkg.name,
    meta: [
      { charset: 'utf-8' },
      { name: 'viewport', content: 'width=device-width, initial-scale=1' },
      {
        name: 'robots',
        content: 'noindex,nofollow'
      },
      { name: 'google', content: 'notranslate' },
      { hid: 'description', name: 'description', content: pkg.description }
    ],
    script: [],
    link: [
      { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
      {
        rel: 'stylesheet',
        href:
          'https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.4.1/css/bootstrap.min.css'
      },
      {
        rel: 'stylesheet',
        href:
          'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css'
      }
    ]
  },

  /*
  ** Customize the progress-bar color
  */
  loading: false,

  /*
  ** Global CSS
  */
  css: ['@/assets/styles/main.css'],

  /*
  ** Plugins to load before mounting the App
  */
  plugins: ['~/plugins/axios'],

  /*
  ** Nuxt.js modules
  */
  modules: ['@nuxtjs/axios', 'cookie-universal-nuxt'],
  /*
  ** Router Config
  */
  router: {
    linkActiveClass: 'active',
    middleware: ['check-auth']
  },
  /*
  ** Axios module configuration
  */
  axios: {
    baseURL:
      process.env.NODE_ENV === 'production'
        ? 'https://api.dotnetcoredocker.com'
        : 'http://localhost:58038',
    https: process.env.NODE_ENV === 'production' ? true : false
  },

  /*
  ** Build configuration
  */
  build: {
    /*
    ** You can extend webpack config here
    */
    extend(config, ctx) {}
  }
};
