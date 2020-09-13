<template>
  <v-overlay :value="overlayState">
    <v-btn>
      <v-btn
        text
        v-bind:overlayState="overlayState"
        v-on:click="$emit('click', !overlayState)"
      >
							Close
      </v-btn>
    </v-btn>
  </v-overlay>
</template>

<script>
export default {
  name: "ModuleOverlay",
  components: {},
  props: {
    overlayState: Boolean
  },
  data: () => ({
  }),
  methods: {
  },
  mounted() {
    console.log("test")
  }
}
</script>

<style>

</style>
