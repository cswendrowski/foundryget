import Vue from "vue";
import { func } from "./util/helpers"
import App from "./App.vue";
import vuetify from "./plugins/vuetify";
// import axios from "axios";



Vue.config.productionTip = false;
Vue.prototype.$func = func;

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
