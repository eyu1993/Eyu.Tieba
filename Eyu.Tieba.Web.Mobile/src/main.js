// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.

import Vue from 'vue'
import mintUI from './config/mint-ui'
import axios from './config/axios'
import utility from './config/utility'

import App from './App'
import router from './config/router'

Vue.use(mintUI)
Vue.use(axios)
Vue.use(utility)

Vue.config.productionTip = false

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
