<template>
  <div>
    <no-ssr>
      <app-dashboard></app-dashboard>
    </no-ssr>
  </div>
</template>

<script>
  import Dashboard from './dashboard/Dashboard.vue';

  export default {
    data() {
      return {};
    },
    components: {
      appDashboard: Dashboard
    }
  };
</script>
