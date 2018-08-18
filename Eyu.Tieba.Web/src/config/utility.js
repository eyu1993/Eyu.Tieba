export default {
  install (Vue) {
    Vue.prototype.$login = function (username, password) {
      let loading = this.$loading({
        lock: true,
        fullscreen: true,
        text: '登陆中...'
      });
      this.$axios({
        method: 'post',
        url: 'token',
        data: 'grant_type=password&password=' + password + '&username=' + username
      }).then(res => {
        loading.close();
        if (res.data['access_token'] != null) {
          this.$message.success('登陆成功');
          localStorage['token'] = res.data['access_token'];
          this.$router.replace(this.$route.query.redirect || '/home/index');
        } else {
          this.$message.error('用户名或密码错误');
        }
      }).catch(err => {
        loading.close();
        if (err.response.data['error'] == '验证失败') {
          this.$message.error(err.response.data['error_description']);
        } else {
          this.$message.error('服务器繁忙，请稍后再试。');
        }
      });
    }

    Vue.prototype.$logout = function () {
      this.$message.success('退出登录');
      window.sessionStorage.clear();
      delete localStorage['token'];
      this.$router.replace('/user/login');
    }
  }
}
