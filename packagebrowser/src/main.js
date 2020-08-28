import Vue from "vue";
import App from "./App.vue";
import vuetify from "./plugins/vuetify";
import './plugins/v-lazy-component';
// import axios from "axios";

Vue.config.productionTip = false;

new Vue({
  render: function(h) {
    return h(App);
  },

  data() {
    return {
      modules: null
    };
  },

  vuetify,

  mounted() {
    // console.log("Mounted");
    // axios
    //   .get(
    //     "https://raw.githubusercontent.com/ardittristan/FoundryAPI/api/modules.json"
    //   )
    //   .then(response => {
    //     console.log(response.data.modules);
    //     this.modules = response.data.modules;
    //   });
  }
}).$mount("#app");
