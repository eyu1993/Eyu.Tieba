<template>
  <div id="setting">
    <!--这是用户设置主页-->
    <mt-tab-container v-model="active">
      <mt-tab-container-item id="user">
        <p>
          <mt-cell title="用户名" :label="userInfo.name" is-link @click.native="active='passwordContainer'">
            <span style="color:#0a8cd2">修改密码</span>
          </mt-cell>
        </p>
        <p>
          <mt-cell title="手机号" :label="userInfo.phone" is-link @click.native="active='phoneContainer'">
            <span style="color:#0a8cd2">更换</span>
          </mt-cell>
        </p>
        <p>
          <mt-cell title="邮箱" :label="userInfo.email">
          </mt-cell>
        </p>
        <p>
          <mt-button type="danger" size="large" @click="Logout">
            退出登陆
          </mt-button>
        </p>
      </mt-tab-container-item>
      <!--设置页面结束-->

      <!--修改密码页面开始-->
      <mt-tab-container-item id="passwordContainer">
        <p>
          <mt-field label="原密码" placeholder="请输入原密码" type="password" v-model="passwordForm.password"></mt-field>
        </p>
        <p>
          <mt-field label="新密码" placeholder="请输入新密码" type="password" v-model="passwordForm.newPassword"></mt-field>
        </p>
        <p>
          <mt-field label="确认密码" placeholder="再次输入新密码" type="password" v-model="passwordForm.confirmPassword">
          </mt-field>
        </p>
        <p>
          <mt-button type="primary" @click="ChangePassword" size="large">
            确定
          </mt-button>
        </p>
        <p>
          <mt-button @click="Cancel" size="large">取消</mt-button>
        </p>
      </mt-tab-container-item>
      <!--修改密码页面结束-->

      <!--修改手机页面开始-->
      <mt-tab-container-item id="phoneContainer">
        <p>
          <mt-cell title="您的当前手机是：">{{userInfo.phone}}</mt-cell>
        </p>
        <p>
          <mt-field placeholder="请输入完整的手机号" v-model="phoneForm.phone" type="tel"></mt-field>
        </p>
        <p>
          <mt-field placeholder="请输入验证码" v-model="phoneForm.code">
            <mt-button :disabled="phoneForm.buttonDisable" size="small" @click="CheckPhone">{{phoneForm.buttonMsg}}</mt-button>
          </mt-field>
        </p>
        <p>
          <mt-button type="primary" size="large" @click="CheckCode">下一步</mt-button>
        </p>
        <p>
          <mt-button @click="Cancel" size="large">取消</mt-button>
        </p>
      </mt-tab-container-item>

      <!--修改手机页面结束-->

      <!--新手机页面开始-->
      <mt-tab-container-item id="newPhoneContainer">
        <p>
          <mt-field placeholder="请输入新的手机号" v-model="newPhoneForm.phone" type="tel"></mt-field>
        </p>
        <p>
          <mt-field placeholder="请输入验证码" v-model="newPhoneForm.code">
            <mt-button :disabled="newPhoneForm.buttonDisable" size="small" @click="SendChangePhoneCode">{{newPhoneForm.buttonMsg}}</mt-button>
          </mt-field>
        </p>
        <p>
          <mt-button type="primary" size="large" @click="ChangePhone">确认更换</mt-button>
        </p>
        <mt-button @click="Cancel" size="large">取消</mt-button>
        </p>
      </mt-tab-container-item>
      <!--新手机页面结束-->
    </mt-tab-container>
  </div>
</template>
<script>
export default {
  name: 'setting',
  data () {
    return {
      userInfo: {
        phone: '',
        name: '',
        email: ''
      },
      passwordForm: {
        password: '',
        newPassword: '',
        confirmPassword: ''
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
      active: 'user'
    }
  },
  methods: {
    GetUserInfo () {
      this.$loading.open();
      this.$axios({
        method: 'get',
        url: 'api/user/GetUserInfo'
      }).then(res => {
        this.$loading.close();
        if (res.data['Error'] == 0) {
          this.userInfo.phone = res.data['Phone'] || '*****';
          this.userInfo.name = res.data['Name'];
          this.userInfo.email = res.data['Email'] || '未绑定';
        } else {
          this.userInfo.phone = '获取失败';
          this.userInfo.name = '获取失败';
          this.userInfo.email = '获取失败';
        }
      }).catch(() => {
        this.$loading.close();
        this.userInfo.phone = '获取失败';
        this.userInfo.name = '获取失败';
        this.userInfo.email = '获取失败';
      });
    },
    ChangePassword () {
      let password = this.$trim(this.passwordForm.password);
      let newPassword = this.$trim(this.passwordForm.newPassword);
      let confirm = this.$trim(this.passwordForm.confirmPassword);
      if (password.length > 16 || password.length < 6) {
        this.$toast('密码长度为6-16');
        return;
      }
      if (newPassword.length > 16 || newPassword.length < 6) {
        this.$toast('新密码长度为6-16');
        return;
      }
      if (confirm != newPassword) {
        this.$toast('两次密码不一致');
        return;
      }
      this.$loading.open();
      this.$axios({
        method: 'post',
        url: 'api/user/ChangePassword',
        params: {
          password: this.passwordForm.password,
          newPassword: this.passwordForm.newPassword
        }
      }).then(res => {
        if (res.data['Error'] == 0) {
          this.$toast('密码修改成功，请重新登录');
          this.$logout();
        } else {
          this.$toast(res.data['Detail']);
        }
        this.$loading.close();
      }).catch(() => {
        this.$toast('服务器繁忙，请稍后再试');
        this.$loading.close();
      });
    },
    CheckPhone () {
      let reg = /^1\d{10}$/;
      if (reg.test(this.phoneForm.phone)) {
        this.$axios({
          method: 'get',
          url: 'api/user/CheckPhone?phone=' + this.phoneForm.phone
        }).then(res => {
          if (res.data['Error'] == 0) {
            this.$toast('短信发送成功');
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
            this.$toast(res.data['Detail']);
          }
        }).catch(() => {
          this.$toast('服务器繁忙，请稍后再试');
        });
      } else {
        this.$toast('手机号为11位数字');
      }
    },
    CheckCode () {
      let reg = /^1\d{10}$/;
      if (reg.test(this.phoneForm.phone)) {
        this.$axios({
          method: 'get',
          url: 'api/user/CheckCode?phone=' + this.phoneForm.phone + '&code=' + this.phoneForm.code
        }).then(res => {
          if (res.data['Error'] == 0) {
            this.active = 'newPhoneContainer';
          } else {
            this.$toast(res.data['Detail']);
          }
        }).catch(() => {
          this.$toast('服务器繁忙，请稍后再试');
        });
      } else {
        this.$toast('参数错误');
      }
    },
    SendChangePhoneCode () {
      let reg = /^1\d{10}$/;
      if (reg.test(this.newPhoneForm.phone)) {
        this.$axios({
          method: 'get',
          url: 'api/user/SendChangePhoneCode?phone=' + this.newPhoneForm.phone
        }).then(res => {
          if (res.data['Error'] == 0) {
            this.$toast('验证码已发送');
            this.newPhoneForm.buttonDisable = true;
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
            this.$toast(res.data['Detail']);
          }
        }).catch(() => {
          this.$toast('服务器繁忙，请稍后再试');
        });
      } else {
        this.$toast('手机号为11位数字');
      }
    },
    ChangePhone () {
      let reg = /^1\d{10}$/;
      let reg2 = /^\d{6}$/;
      if (reg.test(this.newPhoneForm.phone) && reg.test(this.phoneForm.phone) && reg2.test(this.newPhoneForm.code) && reg2.test(this.phoneForm.code)) {
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
            this.$toast('手机号已修改');
            this.Cancel();
            this.GetUserInfo();
          } else {
            this.$toast(res.data['Detail']);
          }
        }).catch(() => {
          this.$toast('服务器繁忙，请稍后再试');
        });
      } else {
        this.$toast('参数有误');
      }
    },
    Cancel () {
      this.passwordForm.password = '';
      this.passwordForm.newPassword = '';
      this.passwordForm.confirmPassword = '';

      this.phoneForm.phone = '';
      this.phoneForm.code = '';
      this.phoneForm.buttonDisable = false;

      this.newPhoneForm.phone = '';
      this.newPhoneForm.code = '';
      this.newPhoneForm.buttonDisable = false;

      this.active = 'user';
      this.$toast('取消操作');
    },
    Logout () {
      this.$logout();
    }
  },
  created () {
    this.GetUserInfo();
  }
}
</script>
<style scoped>
.myToast {
  z-index: 300000;
}
</style>
