<template>
  <div class="navbar-collapse" :class="[ open ? 'expand' : 'collapse']" id="navbarNav">
    <ul class="nav navbar-nav">
      <li class="nav-item">
        <nuxt-link class="nav-link" :to="{ name: 'index'}" exact>Home</nuxt-link>
      </li>
      <!-- <li class="nav-item">
        <nuxt-link class="nav-link" :to="{ name: 'about'}">About</nuxt-link>
      </li>
      <li class="nav-item">
        <nuxt-link class="nav-link" :to="{ name: 'contact'}">Contact</nuxt-link>
      </li>-->
    </ul>
    <ul class="nav navbar-nav navbar-right">
      <li class="nav-item">
        <nuxt-link class="nav-link" :to="{ name: 'register'}">Register</nuxt-link>
      </li>
      <li class="nav-item">
        <nuxt-link class="nav-link" :to="{ name: 'login'}">Log In</nuxt-link>
      </li>
    </ul>
  </div>
</template>

<script>
  export default {
    props: {
      open: {
        type: Boolean,
        default: false
      }
    }
  };
</script>
