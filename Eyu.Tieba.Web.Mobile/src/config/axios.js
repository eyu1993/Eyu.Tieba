import axios from 'axios'
import { Toast } from 'mint-ui'

export default {
  install (Vue) {
    axios.defaults.baseURL = 'http://eyu1993.top:8088/';
    // axios.defaults.baseURL = 'http://localhost:12936/';

    axios.interceptors.request.use(
      config => {
        var token = localStorage['token'];
        if (token != null) {
          config.headers.Authorization = 'bearer ' + token;
        }
        return config;
      },
      err => {
        return Promise.reject(err);
      });

    axios.interceptors.response.use(res => {
      return res;
    }, err => {
      if (err.response.status == 401) {
        Toast('登陆信息已过期');
        setTimeout(() => {
          window.sessionStorage.clear();
          delete localStorage['token'];
          //$router.replace('/user/login');
          window.location.replace('');
          return new Promise(() => { });
        }, 1000);
      }
      return Promise.reject(err);
    });

    Vue.prototype.$axios = axios
  }
}
