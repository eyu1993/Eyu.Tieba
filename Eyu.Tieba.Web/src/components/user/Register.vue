<template>
  <div id="register">
    <el-row type="flex" class="row-bg" justify="space-around">
      <el-card class="info-card register-card" :body-style="{ width: '380px' }">
        <div slot="header" class="clearfix">
          <strong>注册</strong>
        </div>
        <el-form ref="form" :model="form" label-width="80px" :label-position="'left'" :rules="rules">
          <el-form-item label="用户名" prop="name">
            <el-input v-model="form.name" placeholder="请输入用户名"></el-input>
          </el-form-item>
          <el-form-item label="手机号" prop="phone">
            <el-input v-model="form.phone" placeholder="请输入手机号"></el-input>
          </el-form-item>
          <el-row>
            <el-form-item label="验证码" prop="code">
              <el-col :span="12">
                <el-input placeholder="请输入验证码" v-model="form.code"></el-input>
              </el-col>
              <el-col :span="10" style="margin-left:10px">
                <el-button @click="SendSMS" :disabled="form.buttonDisable">{{form.buttonMsg}}</el-button>
              </el-col>
            </el-form-item>
          </el-row>

          <el-form-item label="密码" prop="password">
            <el-input v-model="form.password" type="password" placeholder="请输入密码"></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="Register">立即注册</el-button>
            <el-button @click="resetForm('form')">重 置</el-button>
          </el-form-item>
        </el-form>
      </el-card>
    </el-row>
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
        name: [
          { required: true, message: '请输入用户名' },
          { max: 16, message: '用户名最长为16' }
        ],
        password: [
          { required: true, message: '请输入密码' },
          { min: 6, max: 16, message: '长度为6-16' }
        ],
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
    Register () {
      this.$refs['form'].validate(valid => {
        if (valid) {
          this.$loading();
          this.$axios({
            method: 'post',
            url: 'api/User/Register',
            data: {
              UserName: this.form.name,
              Password: this.form.password,
              Phone: this.form.phone,
              Code: this.form.code
            }
          })
            .then(res => {
              this.$loading().close();
              if (res.data['Error'] == 0) {
                this.$message.success(res.data['Detail']);
                let loading = this.$loading({
                  text: '正在自动登陆'
                });
                setTimeout(() => {
                  loading.close();
                  this.AutoLogin(this.form.name, this.form.password);
                  // this.$login(this.form.name, this.form.password);
                }, 2000)
              } else {
                this.$message.error(res.data['Detail']);
              }
            })
            .catch(() => {
              this.$message.error('服务器繁忙，请稍后再试');
              this.$loading().close();
            });
        } else {
          this.$message.error('参数有误');
        }
      });
    },
    SendSMS () {
      this.$refs['form'].validateField('phone', valid => {
        if (valid.length == 0) {
          this.$axios({
            methods: 'get',
            url: 'api/user/SendRegisterSMSCode?phone=' + this.form.phone
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.$message.success('短信验证码发送成功');
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
              this.$message.error(res.data['Detail']);
            }
          }).catch(() => {
            this.$message.error('服务器繁忙，请稍后再试');
          });
        } else {
          this.$message.error(valid);
        }
      });
    },
    resetForm (formName) {
      this.$refs[formName].resetFields();
    },
    AutoLogin (username, password) {
      let loading = this.$loading({
        lock: true,
        fullscreen: true,
        text: '正在自动登陆...'
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
          this.$message.error('登陆失败，请手动登陆');
          setTimeout(() => {
            this.$router.replace('/user/login');
          }, 2000)
        }
      }).catch(() => {
        loading.close();
        this.$message.error('登陆失败，请手动登陆');
        setTimeout(() => {
          this.$router.replace('/user/login');
        }, 2000)
      });
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

