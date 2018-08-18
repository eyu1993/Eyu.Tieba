// import axios from 'axios'
// axios.defaults.baseURL = 'http://localhost:4629/';
export default {
  login (name, password) {
  },

  getToken () {
    return localStorage['token'];
  },
  getUserName () {
    return localStorage['username'];
  },
  logout (cb) {
    delete localStorage['token'];
    delete localStorage['username'];
    if (cb) {
      cb();
    }
    this.onChange(false);
  },

  loggedIn () {
    return !!localStorage['token'];
  },

  onChange () {

  }
}

// function Login (name, password, cb) {
//   axios.post('toke', 'grant_type=password&username=' + name + '&password=' + password)
//     .then(res => {
//       if (res.data['access_token'] != null) {
//         cb({
//           authenticated: true,
//           token: res.data['access_token'],
//           username: res.data['userName']
//         });
//       } else {
//         cb({
//           authenticated: false
//         });
//       }
//     }).catch(() => {
//       this.$message.error('服务器繁忙，请稍后再试');
//     });
//   axios.get('https://www.kancloud.cn/yunye/axios/234845')
//     .then(function (response) {
//       console.log(response);
//     })
//     .catch(function (error) {
//       console.log(error);
//     });
// }
