<template>
  <div id="register">
    <p>
      <mt-field label="用户名" placeholder="请输入用户名" type="text" v-model="form.name"></mt-field>
    </p>
    <p>
      <mt-field label="手机号" placeholder="请输入手机号" type="tel" v-model="form.phone"></mt-field>
    </p>
    <p>
      <mt-field label="验证码" placeholder="请输入验证码" type="text" v-model="form.code">
        <mt-button @click="SendSMS" :disabled="form.buttonDisable" size="small">{{form.buttonMsg}}</mt-button>
      </mt-field>
    </p>
    <p>
      <mt-field label="密码" placeholder="请输入密码" type="password" v-model="form.password">
      </mt-field>
    </p>
    <p>
      <mt-button type="primary" @click="Register" size="large">
        立即注册
      </mt-button>
    </p>
    <p>

      <mt-button type="danger" @click="Login" size="large">
        已有账号，立即登陆
      </mt-button>
    </p>
  </div>
</template>

<script>
export default {
  name: 'register',
  data () {
    var validPhone = (rule, value, callback) => {
      let reg = /^1\d{10}$/;
      if (reg.test(value)) {
        callback();
      } else {
        callback(new Error('手机号为11位数字'));
      }
    };
    return {
      form: {
        name: '',
        password: '',
        phone: '',
        buttonMsg: '获取验证码',
        buttonDisable: false
      },
      rules: {
        phone: [
          { required: true, message: '请输入手机号' },
          { validator: validPhone }
        ],
        code: [
          { required: true, message: '请输入验证码' },
          { min: 6, max: 6, message: '验证码为6位数字' }
        ]
      }
    };
  },
  methods: {
    Login () {
      this.$router.replace('/user/login');
    },
    Register () {
      let reg = /^1\d{10}$/;
      if (!reg.test(this.form.phone)) {
        this.$toast('手机号为11位数字');
        return;
      }
      if (this.form.name.length > 16 || this.form.length == 0) {
        this.$toast('用户名最长为16');
        return;
      }
      if (this.form.password.length < 6 || this.form.password.length > 16) {
        this.$toast('密码长度为6-16');
        return;
      }
      this.$loading.open();
      this.$axios({
        method: 'post',
        url: 'api/User/Register',
        data: {
          UserName: this.form.name,
          Password: this.form.password,
          Phone: this.form.phone,
          Code: this.form.code
        }
      }).then(res => {
        this.$loading.close();
        if (res.data['Error'] == 0) {
          this.$confirm('注册成功，是否直接登陆?').then(() => {
            this.$login(this.form.name, this.form.password);
          });
        } else {
          this.$toast(res.data['Detail']);
        }
      }).catch(() => {
        this.$toast('服务器繁忙，请稍后再试');
        this.$loading.close();
      });
    },
    SendSMS () {
      let reg = /^1\d{10}$/;
      if (reg.test(this.form.phone)) {
        this.$axios({
          methods: 'get',
          url: 'api/user/SendRegisterSMSCode?phone=' + this.form.phone
        }).then(res => {
          if (res.data['Error'] == 0) {
            this.$toast('短信验证码发送成功');
            this.form.buttonDisable = true;
            let countdown = 60;
            let timer1 = setInterval(() => {
              if (countdown == 0) {
                this.form.buttonDisable = false;
                this.form.buttonMsg = '获取验证码';
                window.clearInterval(timer1);
              } else {
                this.form.buttonMsg = '重新发送(' + countdown + ')';
                countdown--;
              }
            }, 1000);
          } else {
            this.$toast(res.data['Detail']);
          }
        }).catch(() => {
          this.$toast('服务器繁忙，请稍后再试');
        });
      } else {
        this.$toast('手机号为11位数字');
      }
    }
  },
  created () {
    let token = localStorage['token'];
    if (token != null) {
      this.$router.replace('/home/index');
    }
  }
};
</script>
<style scoped>
.clearfix {
  text-align: center;
}
</style>

