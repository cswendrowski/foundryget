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

      <v-spacer></v-spacer>

      <v-btn
        href="https://github.com/vuetifyjs/vuetify/releases/latest"
        target="_blank"
        text
      >
        <span class="mr-2">Latest Release</span>
        <v-icon>mdi-open-in-new</v-icon>
      </v-btn>
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
