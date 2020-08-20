<template>

  <v-card class="mx-auto"  >
    <v-card-title :class="color">{{ module.title }}</v-card-title>
    <v-card-subtitle :class="color">{{module.type}} - v{{ module.version }}</v-card-subtitle>
    <v-card-text style="padding-top: 10px;">
        <h4>Author(s): {{ module.author }}</h4>
        <p v-html="module.description"></p>
    </v-card-text>
    <v-card-actions>
          <v-btn
            text
            color="primary"
            v-bind:href="module.foundryUrl"
            target="_blank"
          >Project</v-btn>

          <v-spacer></v-spacer>
          
          <v-btn
            text
            color="primary"
            v-bind:href="foundryPackageUrl"
            target="_blank"
          >Package</v-btn>

          <v-spacer></v-spacer>

            <v-btn
                icon
                @click="show = !show"
            >
                <v-icon>{{ show ? 'mdi-chevron-up' : 'mdi-chevron-down' }}</v-icon>
            </v-btn>
        </v-card-actions>

        <v-expand-transition>
            <div v-show="show">
                <v-divider></v-divider>

                <v-btn
                    text
                    v-bind:href="module.manifest"
                    target="_blank"
                >Manifest</v-btn>

                <!-- <v-btn
                    :loading="loading"
                    :disabled="loading"
                    color="blue-grey"
                    class="ma-2 white--text"
                    @click="loader = 'loading'"
                    >
                    ZIP
                    <v-icon right dark>mdi-cloud-download</v-icon>
                </v-btn> -->

                <v-card-text>

                <b>Compatible Foundry Versions: </b><span>v{{ module.minimumCoreVersion }} - v{{ module.compatibleCoreVersion }}</span>
                </v-card-text>
            </div>
        </v-expand-transition>
  </v-card>
</template>

<script>
export default {
  name: "Module",
  props: {
    module: Object
  },

  data: () => ({
    show: false,
    loader: null,
    loading: false
  }),

  watch: {
    loader () {
      const l = this.loader
      this[l] = !this[l]

      setTimeout(() => (this[l] = false), 3000)

      this.loader = null
    }
  },

  computed: {
      foundryPackageUrl() {
          return "https://foundryvtt.com" + this.module.foundryUrl;
      },
      color() {
        if (this.module.type == "Module") {
          return "orange";
        }
        else if (this.module.type == "System") {
          return "blue";
        }
        else {
          return "white";
        }
      }
  },

  
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
