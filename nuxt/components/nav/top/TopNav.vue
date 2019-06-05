<template>
  <nav class="navbar navbar-top">
    <div class="container">
      <div class="navbar-header">
        <!-- <button class="navbar-toggle" type="button" @click="toggleNav">
          <span class="sr-only">Toggle navigation</span>
          <span class="icon-bar"></span>
          <span class="icon-bar"></span>
          <span class="icon-bar"></span>
        </button>-->
        <nuxt-link class="navbar-brand" :to="{ name: 'index' }" exact>.Net Core Docker!</nuxt-link>
      </div>
      <app-nav-items-public :open="openNav" v-if="!auth"></app-nav-items-public>
      <app-nav-items-auth :open="openNav" v-if="auth"></app-nav-items-auth>
    </div>
  </nav>
</template>

<script>
  import TopNavItemsPublic from './TopNavItemsPublic.vue';
  import TopNavItemsAuth from './TopNavItemsAuth.vue';
  import { mapGetters } from 'vuex';

  export default {
    data() {
      return {
        openNav: false
      };
    },
    components: {
      appNavItemsPublic: TopNavItemsPublic,
      appNavItemsAuth: TopNavItemsAuth
    },
    watch: {
      $route(to, from) {
        this.openNav = false;
      }
    },
    computed: {
      ...mapGetters({
        auth: 'isAuth'
      })
    },
    methods: {
      toggleNav() {
        this.openNav = !this.openNav;
      }
    }
  };
</script>

