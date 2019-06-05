<template>
  <div>
    <app-top-nav></app-top-nav>
    <div class="container body-content">
      <nuxt/>
    </div>
  </div>
</template>
<script>
  import { mapGetters } from 'vuex';
  import TopNav from '../components/nav/top/TopNav.vue';

  export default {
    head() {
      return {
        // if no subcomponents specify a metaInfo.title, this title will be used
        title: 'Default Title',
        // all titles will be injected into this template
        titleTemplate: '%s | Nuxtjs, .Net Core, & Docker'
      };
    },
    components: {
      appTopNav: TopNav
    },
    mounted() {
      // setting client-side token timer from nuxtServerInit results
      if (this.$store.getters.initSetTimer > 0) {
        this.$store.dispatch('renewTokenTimer', this.$store.getters.initSetTimer);
      }
    },
    computed: {
      ...mapGetters({
        auth: 'isAuth'
      })
    }
  };
</script>

<style>
  .dashboard {
    padding: 20px 15px;
  }
</style>

