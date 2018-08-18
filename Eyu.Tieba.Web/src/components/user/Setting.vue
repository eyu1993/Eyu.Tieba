<template>
  <div id="setting">
    <el-row type="flex" justify="center">
      <el-col :span="18">
        <el-card class="info-card" style="padding-bottom:30px;">
          <div slot="header" class="clearfix">
            <span>设置</span>
          </div>
          <div>
            <ul>
              <li>
                <el-col :span="8">登录名</el-col>
                <el-col :span="8">{{userInfo.name}}</el-col>
                <el-col :span="8">
                  <el-button type="text" @click="passwordVisible=true">
                    <!-- <a href="javascript:void(0)">修改密码</a> -->
                    修改密码
                  </el-button>
                </el-col>
              </li>
              <li>
                <el-col :span="8">手机号</el-col>
                <el-col :span="8">{{userInfo.phone}}</el-col>
                <el-col :span="8">
                  <el-button type="text" @click="phoneVisible=true">更换</el-button>
                </el-col>
              </li>
              <li>
                <el-col :span="8">邮箱</el-col>
                <el-col :span="8">{{userInfo.email}}</el-col>
                <el-col :span="8">
                  <el-button type="text" disabled="disabled">更换</el-button>
                </el-col>
              </li>
            </ul>
          </div>
        </el-card>
      </el-col>
    </el-row>
    <el-dialog title="修改密码" :visible.sync="passwordVisible" :before-close="passwordClose">
      <el-form ref="passwordForm" :model="passwordForm" :rules="passwordRules">
        <el-form-item prop="password" label="原密码">
          <el-input v-model="passwordForm.password" placeholder="请输入原密码" type="password"></el-input>
        </el-form-item>
        <el-form-item prop="newPassword" label="新密码">
          <el-input v-model="passwordForm.newPassword" placeholder="请输入新密码" type="password"></el-input>
        </el-form-item>
        <el-form-item prop="confirmPassword" label="确认密码">
          <el-input v-model="passwordForm.confirmPassword" placeholder="请再次输入密码" type="password"></el-input>
        </el-form-item>
        <el-form-item style="text-align:right;margin-top:40px;">
          <el-button type="primary" @click="ChangePassword">确 定</el-button>
          <el-button @click="resetForm('passwordForm')">取 消</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>

    <el-dialog title="手机" :visible.sync="phoneVisible" :before-close="phoneClose">
      <el-form ref="phoneForm" :model="phoneForm" :rules="phoneRules">
        <p>为了保障您的帐号安全，变更信息前需验证身份</p>
        <p>您的当前手机是：{{userInfo.phone}}</p>
        <el-form-item prop="phone">
          <el-input placeholder="请输入完整的手机号" v-model="phoneForm.phone"></el-input>
        </el-form-item>
        <el-form-item prop="code">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-input placeholder="请输入验证码" v-model="phoneForm.code"></el-input>
            </el-col>
            <el-col :span="12">
              <el-button @click="CheckPhone" :disabled="phoneForm.buttonDisable">{{phoneForm.buttonMsg}}</el-button>
            </el-col>
          </el-row>
        </el-form-item>
        <el-form-item style="text-align:right;margin-top:40px;">
          <el-button type="primary" @click="CheckCode">确 认</el-button>
          <el-button @click="resetForm('phoneForm')">取 消</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>

    <el-dialog title="手机" :visible.sync="newPhoneVisible" :before-close="newPhoneClose">
      <el-form ref="newPhoneForm" :model="newPhoneForm" :rules="newPhoneRules">
        <p>请输入新的手机号码</p>
        <el-form-item prop="phone">
          <el-input placeholder="请输入新手机号" v-model="newPhoneForm.phone"></el-input>
        </el-form-item>
        <el-form-item prop="code">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-input placeholder="请输入验证码" v-model="newPhoneForm.code"></el-input>
            </el-col>
            <el-col :span="12">
              <el-button @click="SendChangePhoneCode" :disabled="newPhoneForm.buttonDisable">{{newPhoneForm.buttonMsg}}</el-button>
            </el-col>
          </el-row>
        </el-form-item>
        <el-form-item style="text-align:right;margin-top:40px;">
          <el-button type="primary" @click="ChangePhone">确 认</el-button>
          <el-button @click="resetForm('newPhoneForm')">取 消</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>
  </div>
</template>
<script>
export default {
  name: 'setting',
  data () {
    var confirmPass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'));
      } else if (value !== this.passwordForm.newPassword) {
        callback(new Error('两次输入密码不一致!'));
      } else {
        callback();
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
      labelPosition: 'right',
      passwordVisible: false,
      phoneVisible: false,
      newPhoneVisible: false,
      userInfo: {
        phone: '',
        name: '',
        email: ''
      },
      phoneForm: {
        phone: '',
        code: '',
        buttonDisable: false,
        buttonMsg: '获取验证码'
      },
      newPhoneForm: {
        phone: '',
        code: '',
        buttonDisable: false,
        buttonMsg: '获取验证码'
      },
      passwordForm: {
        password: '',
        newPassword: '',
        confirmPassword: ''
      },
      passwordRules: {
        password: [
          { required: true, message: '请输入原密码' },
          { min: 6, max: 16, message: '长度为6-16' }
        ],
        newPassword: [
          { required: true, message: '请输入新密码' },
          { min: 6, max: 16, message: '长度为6-16' }
        ],
        confirmPassword: [
          { required: true, message: '请再次输入密码' },
          { validator: confirmPass }
        ]
      },
      phoneRules: {
        phone: [
          { required: true, message: '请输入手机号' },
          { validator: validPhone }
        ],
        code: [
          { required: true, message: '请输入验证码' },
          { min: 6, max: 6, message: '验证码为6位数字' }
        ]
      },
      newPhoneRules: {
        phone: [
          { required: true, message: '请输入手机号' },
          { validator: validPhone }
        ],
        code: [
          { required: true, message: '请输入验证码' },
          { min: 6, max: 6, message: '验证码为6位数字' }
        ]
      }
    }
  },
  methods: {
    resetForm (formName) {
      this.$refs[formName].resetFields();
      this.passwordVisible = false;
      this.phoneVisible = false;
      this.newPhoneVisible = false;
    },
    passwordClose (done) {
      this.resetForm('passwordForm');
      done();
    },
    phoneClose (done) {
      this.resetForm('phoneForm');
      done();
    },
    newPhoneClose (done) {
      this.resetForm('newPhoneForm');
      done();
    },
    ChangePassword () {
      this.$refs['passwordForm'].validate((valid) => {
        if (valid) {
          this.$loading();
          this.$axios({
            method: 'post',
            url: 'api/user/ChangePassword',
            params: {
              password: this.passwordForm.password,
              newPassword: this.passwordForm.newPassword
            }
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.$message.success('密码修改成功，请重新登录');
              window.sessionStorage.clear();
              delete localStorage['token'];
              this.$router.replace('/user/login');
            } else {
              this.$message.error(res.data['Detail']);
            }
            this.$loading().close();
          }).catch(() => {
            this.$message.error('服务器繁忙，请稍后再试');
            this.$loading().close();
          });
        } else {
          this.$message.warning('参数错误');
        }
      });
    },
    CheckPhone () {
      this.$refs['phoneForm'].validateField('phone', (valid) => {
        if (valid.length == 0) {
          this.$axios({
            method: 'get',
            url: 'api/user/CheckPhone?phone=' + this.phoneForm.phone
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.$message.success('短信发送成功');
              this.phoneForm.buttonDisable = true;
              let countdown = 60;
              let timer1 = setInterval(() => {
                if (countdown == 0) {
                  this.phoneForm.buttonDisable = false;
                  this.phoneForm.buttonMsg = '获取验证码';
                  window.clearInterval(timer1);
                } else {
                  this.phoneForm.buttonMsg = '重新发送(' + countdown + ')';
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
          this.$message.warning('手机号格式错误');
        }
      });
    },
    CheckCode () {
      this.$refs['phoneForm'].validate(valid => {
        if (valid) {
          this.$axios({
            method: 'get',
            url: 'api/user/CheckCode?phone=' + this.phoneForm.phone + '&code=' + this.phoneForm.code
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.phoneVisible = false;
              this.newPhoneVisible = true;
            } else {
              this.$message.error(res.data['Detail']);
            }
          }).catch(() => {
            this.$message.error('服务器繁忙，请稍后再试');
          });
        } else {
          this.$message.warning('参数有误');
        }
      });
    },
    SendChangePhoneCode () {
      this.$refs['newPhoneForm'].validateField('phone', valid => {
        if (valid.length == 0) {
          this.$axios({
            method: 'get',
            url: 'api/user/SendChangePhoneCode?phone=' + this.newPhoneForm.phone
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.newPhoneForm.buttonDisable = true;
              this.$message.success('验证码已发送');
              let countdown = 60;
              let timer1 = setInterval(() => {
                if (countdown == 0) {
                  this.newPhoneForm.buttonDisable = false;
                  this.newPhoneForm.buttonMsg = '获取验证码';
                  window.clearInterval(timer1);
                } else {
                  this.newPhoneForm.buttonMsg = '重新发送(' + countdown + ')';
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
          this.$message.warning(valid)
        }
      });
    },
    ChangePhone () {
      this.$refs['phoneForm'].validate(valid => {
        if (valid) {
          this.$refs['newPhoneForm'].validate(valid1 => {
            if (valid1) {
              this.$axios({
                method: 'post',
                url: 'api/user/ChangePhone',
                data: {
                  Phone: this.phoneForm.phone,
                  Code: this.phoneForm.code,
                  NewPhone: this.newPhoneForm.phone,
                  NewCode: this.newPhoneForm.code
                }
              }).then(res => {
                if (res.data['Error'] == 0) {
                  this.$message.success('手机号已修改');
                  this.resetForm('newPhoneForm');
                  this.GetUserInfo();
                } else {
                  this.$message.error(res.data['Detail']);
                }
              }).catch(() => {
                this.$message.error('服务器繁忙，请稍后再试');
              });
            } else {
              this.$message.warning('参数有误');
            }
          })
        } else {
          this.$message.warning('参数有误');
        }
      });
    },
    GetUserInfo () {
      this.$loading();
      this.$axios({
        method: 'get',
        url: 'api/user/GetUserInfo'
      }).then(res => {
        this.$loading().close();
        if (res.data['Error'] == 0) {
          this.userInfo.phone = res.data['Phone'] || '*****';
          this.userInfo.name = res.data['Name'];
          this.userInfo.email = res.data['Email'] || '未绑定';
        } else {
          this.$message.error('服务器繁忙，请稍后再试');
          this.userInfo.phone = '获取失败';
          this.userInfo.name = '获取失败';
          this.userInfo.email = '获取失败';
        }
      }).catch(() => {
        this.$loading().close();
        this.$message.error('服务器繁忙，请稍后再试');
        this.userInfo.phone = '获取失败';
        this.userInfo.name = '获取失败';
        this.userInfo.email = '获取失败';
      });
    }
  },
  created () {
    this.GetUserInfo();
  }
}
</script>
<style>
.el-table .warning-row {
  background: oldlace;
}

.el-table .success-row {
  background: #f0f9eb;
}
/* #setting .el-card__body {
  padding: 0px;
  padding-bottom: 0px;
  padding-top: 0px;
}
#setting .el-card__body .el-tabs__header {
  padding: 20px;
  background-color: dodgerblue;
  margin: 0px;
  padding-top: 10px;
  padding-bottom: 10px;
}
#setting .el-tabs__item {
  color: white;
}
#setting .el-tabs__item:hover {
  color: white;
}
#setting .el-tabs__item.is-active {
  color: black;
}
#setting .el-tab-pane {
  padding: 20px;
}
#setting .el-tabs__nav-wrap::after {
  background-color: dodgerblue;
} */
#setting ul {
  list-style: none;
  line-height: 40px;
}
#setting ul li {
  margin: 10px;
}
</style>


