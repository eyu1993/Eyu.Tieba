<template>
  <div id="login">
    <p>
      <mt-field label="用户名" placeholder="用户名/手机号" type="text" v-model="form.name"></mt-field>
    </p>
    <p>
      <mt-field label="密码" placeholder="请输入密码" type="password" v-model="form.password"></mt-field>
    </p>
    <p>
      <mt-button type="primary" size="large" @click="Login">立即登陆</mt-button>
    </p>
    <p>
      <mt-button type="danger" size="large" @click="Register">注册</mt-button>
    </p>
  </div>
</template>

<script>
export default {
  name: 'login',
  data () {
    return {
      form: {
        name: '',
        password: ''
      },
      rules: {
        name: [
          { required: true, message: '请输入用户名' }
        ],
        password: [
          { required: true, message: '请输入密码' },
          { min: 6, max: 16, message: '长度为6-16' }
        ]
      }
    }
  },
  methods: {
    Login () {
      if (this.form.name.length == 0) {
        this.$toast('用户名不能为空');
        return;
      }
      if (this.form.password.length < 6 || this.form.length > 16) {
        this.$toast('密码长度为6-16');
        return;
      }
      this.$login(this.form.name, this.form.password, () => {
        console.log('lalala');
      });
    },
    Register () {
      this.$router.replace('/user/register');
    }
  },
  created () {
    let token = localStorage['token'];
    if (token != null) {
      this.$router.replace('/home/index');
    }
  }
}
</script>
<style scoped>

</style>

