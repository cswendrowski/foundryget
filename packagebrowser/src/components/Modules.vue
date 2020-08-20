<template>
  <div class="hello">
    <v-container>


      <v-data-iterator
        :items="items"
        :items-per-page.sync="itemsPerPage"
        :page="page"
        :search="search"
        :sort-by="sortBy.toLowerCase()"
        :sort-desc="sortDesc"
      >

      <template v-slot:header>
        
          <v-toolbar
            dark
            color="blue darken-3"
            class="mb-1"
          >
            <v-text-field
              v-model="search"
              clearable
              flat
              solo-inverted
              hide-details
              label="Search"
            ></v-text-field>
            <template v-if="$vuetify.breakpoint.mdAndUp">
              <v-spacer></v-spacer>
              <v-select
                v-model="sortBy"
                flat
                solo-inverted
                hide-details
                :items="keys"
                label="Sort by"
              ></v-select>
              <v-spacer></v-spacer>
              <v-btn-toggle
                v-model="sortDesc"
                mandatory
              >
                <v-btn
                  large
                  depressed
                  color="blue"
                  :value="false"
                >
                  <v-icon>mdi-arrow-up</v-icon>
                </v-btn>
                <v-btn
                  large
                  depressed
                  color="blue"
                  :value="true"
                >
                  <v-icon>mdi-arrow-down</v-icon>
                </v-btn>
              </v-btn-toggle>
            </template>
          </v-toolbar>

                <!-- <v-chip-group
                    column
                    multiple
                >
                    <v-chip filter outlined>Systems</v-chip>
                    <v-chip filter outlined>Modules</v-chip>
                    <v-chip filter outlined>Libraries</v-chip>
                    <v-chip filter outlined>Content Packs</v-chip>
                </v-chip-group> -->
        </template>

        <template v-slot:default="props">
          <v-row>
            <v-col
              v-for="item in props.items"
              :key="item.name"
              cols="12"
              lg="4"
              md="6"
              sm="12"
            >
                <Module
                    v-bind:module="item"
                />
            </v-col>
          </v-row>
        </template>
<!--      
        <template v-slot:footer>
          <v-row class="mt-2" align="center" justify="center">
            <span class="grey--text">Items per page</span>
            <v-menu offset-y>
              <template v-slot:activator="{ on, attrs }">
                <v-btn
                  dark
                  text
                  color="primary"
                  class="ml-2"
                  v-bind="attrs"
                  v-on="on"
                >
                  {{ itemsPerPage }}
                  <v-icon>mdi-chevron-down</v-icon>
                </v-btn>
              </template>
              <v-list>
                <v-list-item
                  v-for="(number, index) in itemsPerPageArray"
                  :key="index"
                  @click="updateItemsPerPage(number)"
                >
                  <v-list-item-title>{{ number }}</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-menu>
  
            <v-spacer></v-spacer>
  
            <span
              class="mr-4
              grey--text"
            >
              Page {{ page }} of {{ numberOfPages }}
            </span>
            <v-btn
              fab
              dark
              color="blue darken-3"
              class="mr-1"
              @click="formerPage"
            >
              <v-icon>mdi-chevron-left</v-icon>
            </v-btn>
            <v-btn
              fab
              dark
              color="blue darken-3"
              class="ml-1"
              @click="nextPage"
            >
              <v-icon>mdi-chevron-right</v-icon>
            </v-btn>
          </v-row>
        </template>
       -->
      </v-data-iterator>

<!-- 
        <Module
            v-for="module in modules"
            v-bind:module="module"
            :key="module.name"
        /> -->
    </v-container>
  </div>
</template>

<script>
import axios from "axios";
import Module from "./Module.vue";

export default {
  name: "Modules",

  components: {
    Module
  },

  data() {
    return {
      modules: null,
      search: '',
      filter: {},
      sortDesc: false,
      sortBy: 'name',
      page: 1,
      itemsPerPage: 5,
      itemsPerPageArray: [6, 9, 12],
      keys: [
        'Name',
        'Type'
      ],
      items: [],
    };
  },

  mounted() {
    console.log("Mounted");
    axios
      .get(
        "https://raw.githubusercontent.com/ardittristan/FoundryAPI/api/modules.json"
      )
      .then(response => {
        console.log(response.data.modules);
        this.modules = response.data.modules;
        response.data.modules.forEach(x => x.type = "Module");
        this.items = this.items.concat(response.data.modules);
      });

    axios
      .get(
        "https://raw.githubusercontent.com/ardittristan/FoundryAPI/api/systems.json"
      )
      .then(response => {
        console.log(response.data.modules);
        this.systems = response.data.modules;
        response.data.modules.forEach(x => x.type = "System");
        this.items = this.items.concat(response.data.modules);
      });
    
  },

  computed: {
    filteredKeys () {
      return this.keys.filter(key => key !== `Name`)
    },
  },
};
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
