import axios from 'axios'
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
    window.sessionStorage.clear();
    delete localStorage['token'];
    window.location.href = '';
    return new Promise(() => { });
  }
  return Promise.reject(err);
});

export default {
  install (Vue) {
    Vue.prototype.$axios = axios;
  }
}
