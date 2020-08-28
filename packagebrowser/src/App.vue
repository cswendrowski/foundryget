<template>
  <v-app>
    <v-app-bar app color="primary" dark>
      <div class="d-flex align-center">
        <v-img
          alt="The Forge Logo"
          class="shrink mr-2"
          contain
          src="https://forge-vtt.com/images/the-forge-banner-960x320.png"
          transition="scale-transition"
          width="150"
        />
      </div>
    </v-app-bar>
    
    <v-main>
      <Modules />
    </v-main>
  </v-app>
</template>
<style lang="scss">
  .v-main {
    background-color: var(--v-secondary-base);
  }
</style>
<script>
import Modules from "./components/Modules";

export default {
  name: "App",

  components: {
    Modules
  },

  data: () => ({
    //
  })
};
</script>
