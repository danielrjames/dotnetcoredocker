<template>
  <div class="hv-navbar-auth-items">
    <div class="navbar-collapse collapse" :class="[ open ? 'expand' : 'collapse']">
      <div>
        <ul class="nav navbar-nav">
          <li class="nav-item">
            <nuxt-link class="nav-link" :to="{ name: 'index' }" exact>Dashboard</nuxt-link>
          </li>
        </ul>
      </div>
      <ul class="nav navbar-nav navbar-right">
        <li class="nav-item">
          <button class="logout" @click="onLogout">Log Out</button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
  import { mapGetters } from 'vuex';

  export default {
    props: {
      open: {
        type: Boolean,
        default: false
      }
    },
    computed: {
      ...mapGetters({
        name: 'name',
        accountUsername: 'username'
      })
    },
    methods: {
      onLogout() {
        this.$store.dispatch('logoutUser', '/');
      }
    }
  };
</script>

<style>
  .account-nav-item {
    display: flex;
    cursor: pointer;
    transition: opacity 0.25s;
    opacity: 0.85;
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
  }
  .account-nav-item:hover {
    opacity: 1;
  }
</style>
