import Vue from 'vue'
import elementUI from './config/element-ui'
import axios from './config/axios'
import utility from './config/utility'
import App from './App'
import router from './config/router'

Vue.use(elementUI)
Vue.use(axios)
Vue.use(utility)

Vue.config.productionTip = false;
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
