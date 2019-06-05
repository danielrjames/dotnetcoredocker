<template>
  <div>
    <app-home-public v-if="!auth"></app-home-public>
    <app-home-auth v-if="auth"></app-home-auth>
  </div>
</template>

<script>
  import HomePublic from '../components/home/HomePublic.vue';
  import HomeAuth from '../components/home/HomeAuth.vue';
  import { mapGetters } from 'vuex';

  export default {
    asyncData({ store }) {
      if (store.state.auth.authenticated) {
        return {
          title: 'Dashboard',
          titleTemplate: '%s | Nuxtjs, .Net Core, & Docker'
        };
      } else {
        return {
          title: 'Nuxtjs, .Net Core, & Docker',
          titleTemplate:
            "%s | If you're here, then you probably saw my medium guide!"
        };
      }
    },
    head() {
      return {
        title: this.title,
        titleTemplate: this.titleTemplate,
        meta: [
          {
            hid: 'description',
            name: 'description',
            content: 'Nuxtjs, .Net Core, & Docker demo.'
          }
        ]
      };
    },
    components: {
      appHomePublic: HomePublic,
      appHomeAuth: HomeAuth
    },
    computed: {
      ...mapGetters({
        auth: 'isAuth'
      })
    },
    watch: {
      auth(newValue, oldValue) {
        if (newValue) {
          this.title = 'Welcome';
          this.titleTemplate = '%s | Nuxtjs, .Net Core, & Docker';
        } else {
          this.title = 'Nuxtjs, .Net Core, & Docker';
          this.titleTemplate =
            "%s | If you're here, then you probably saw my medium guide!";
        }
      }
    }
  };
</script>


