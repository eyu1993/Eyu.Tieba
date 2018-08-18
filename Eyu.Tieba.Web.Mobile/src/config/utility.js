export default {
  install (Vue) {
    Vue.prototype.$login = function (username, password) {
      this.$loading.open();
      this.$axios({
        method: 'post',
        url: 'token',
        data: 'grant_type=password&password=' + password + '&username=' + username
      }).then(res => {
        this.$loading.close();
        if (res.data['access_token'] != null) {
          this.$toast('登陆成功');
          localStorage['token'] = res.data['access_token'];
          this.$router.replace(this.$route.query.redirect || '/home/index');
        } else {
          this.$toast('用户名或密码错误');
        }
      }).catch(err => {
        this.$loading.close();
        if (err.response.data['error'] == '验证失败') {
          this.$toast(err.response.data['error_description']);
        } else {
          this.$toast('服务器繁忙，请稍后再试。');
        }
      });
    }
    Vue.prototype.$logout = function () {
      window.sessionStorage.clear();
      delete localStorage['token'];
      this.$router.replace('/user/login');
    }
    Vue.prototype.$trim = function (str) {
      return str.replace(/(^\s*)|(\s*$)/g, "");
    }
  }
}
