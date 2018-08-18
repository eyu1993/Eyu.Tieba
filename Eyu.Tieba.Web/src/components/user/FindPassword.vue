<template>
  <div id="findPassword">
    <el-row type="flex" class="row-bg" justify="space-around">
      <el-card class="info-card findPwd-card" :body-style="{ width: '380px' }">
        <div slot="header" class="clearfix">
          <strong>找回密码</strong>
        </div>
        <el-form ref="form" :model="form" label-width="80px" :label-position="labelPosition" :rules="rules">
          <el-form-item label="手机号" prop="phone">
            <el-input v-model="form.phone"></el-input>
          </el-form-item>
          <el-form-item label="验证码" prop="code">
            <el-col :span="12">
              <el-input v-model="form.code"></el-input>
            </el-col>
            <el-col :span="10" style="margin-left:10px">
              <el-button @click="SendSMSCode" :disabled="form.buttonDisable">{{form.buttonMsg}}</el-button>
            </el-col>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="CheckCode">找回密码</el-button>
          </el-form-item>
        </el-form>
      </el-card>
    </el-row>
    <el-dialog title="新密码" :visible.sync="passwordVisible" :before-close="passwordClose">
      <el-form ref="passwordForm" :model="passwordForm" :rules="passwordRules">
        <el-form-item prop="password" label="新登陆密码">
          <el-input v-model="passwordForm.password" type="password"></el-input>
        </el-form-item>
        <el-form-item prop="confirmPassword" label="确认密码">
          <el-input v-model="passwordForm.confirmPassword" type="password"></el-input>
        </el-form-item>
        <el-form-item style="text-align:right;margin-top:40px;">
          <el-button type="primary" @click="FindPassword">确 认</el-button>
          <el-button @click="resetForm('passwordForm')">取 消</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>
  </div>
</template>

<script>

export default {
  name: 'findPassword',
  data () {
    var confirmPassword = (rule, value, callback) => {
      if (value == this.passwordForm.password) {
        callback();
      } else {
        callback(new Error('两次输入的密码不一致'));
      }
    };
    var validPhone = (rule, value, callback) => {
      let reg = /^1\d{10}$/;
      if (reg.test(value)) {
        callback();
      } else {
        callback(new Error('手机号为11位数字'));
      }
    };
    return {
      labelPosition: 'left',
      passwordVisible: false,
      form: {
        phone: '',
        code: '',
        buttonDisable: false,
        buttonMsg: '获取验证码'
      },
      passwordForm: {
        password: '',
        confirmPassword: ''
      },
      rules: {
        phone: [
          { required: true, message: '请输入手机号', trigger: 'blur' },
          { validator: validPhone, trigger: 'blur,change' }
        ],
        code: [
          { required: true, message: '请输入验证码', trigger: 'blur' },
          { max: 6, min: 6, message: '验证码为6位数字', trigger: 'blur,change' },
          { type: 'number', message: '验证码为6位数字' }
        ]
      },
      passwordRules: {
        password: [
          { required: true, message: '请输入密码', tigger: 'blur' },
          { min: 6, max: 16, message: '长度为6-16', tigger: 'blur,change' }
        ],
        confirmPassword: [
          { required: true, message: '请输入密码', tigger: 'blur' },
          { min: 6, max: 16, message: '长度为6-16', tigger: 'blur,change' },
          { validator: confirmPassword, trigger: 'blur,change' }
        ]
      }
    }
  },
  methods: {
    CheckCode () {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          this.$axios({
            method: 'get',
            url: 'api/user/CheckFindPwdCode?phone=' + this.form.phone + '&code=' + this.form.code
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.passwordVisible = true;
            } else {
              this.$message.error('验证码错误');
            }
          }).catch(() => {
            this.$message.error('服务器繁忙，请稍后再试');
          })
        } else {
          this.$message.error('参数错误');
        }
      });
    },
    SendSMSCode () {
      this.$refs['form'].validateField('phone', (valid) => {
        if (valid.length == 0) {
          this.$axios({
            method: 'get',
            url: 'api/user/SendFindPwdSMSCode?phone=' + this.form.phone
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.$message.success('验证码发送成功');
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
          this.$message.error('参数错误');
        }
      })
    },
    FindPassword () {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          this.$refs['passwordForm'].validate((valid1) => {
            if (valid1) {
              this.$axios({
                method: 'post',
                url: 'api/user/FindPassword',
                params: {
                  phone: this.form.phone,
                  code: this.form.code,
                  password: this.passwordForm.password
                }
              }).then(res => {
                if (res.data['Error'] == 0) {
                  this.$message.success('密码已重置');
                  setTimeout(() => {
                    this.AutoLogin(this.form.phone, this.passwordForm.password);
                  }, 2000)
                } else {
                  this.$message.error(res.data['Detail']);
                }
              }).catch(() => {
                this.$message.error('服务器繁忙，请稍后再试');
              });
            } else {
              this.$message.error('参数错误');
            }
          });
        } else {
          this.$message.error('参数错误');
        }
      });
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
    },
    passwordClose (done) {
      this.resetForm('passwordForm');
      done();
    },
    resetForm (formName) {
      this.$refs[formName].resetFields();
      this.passwordVisible = false;
    }
  }
}
</script>
<style scoped>
.clearfix {
  text-align: center;
}
</style>

