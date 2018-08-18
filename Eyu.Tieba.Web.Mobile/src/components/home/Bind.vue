<template>
  <div id="bind">
    <mt-tab-container v-model="active">
      <!--首页开始-->
      <mt-tab-container-item id="home">
        <div v-if="!IsBind">
          <!-- 3 -->
          <p>
            <mt-cell title="通过账号登陆绑定" is-link @click.native="active='account'">
            </mt-cell>
          </p>
          <p>
            <mt-cell title="通过短信快捷登陆绑定" is-link @click.native="active='phone'">
            </mt-cell>
          </p>
          <p>
            <mt-cell title="手动绑定" is-link @click.native="active='manual'">
            </mt-cell>
          </p>
        </div>
        <div v-else>
          <div v-for="item in BaiduInfo">
            <p>
              <mt-cell title="贴吧ID:">{{item.Name}}</mt-cell>
            </p>
            <mt-cell title="BDUSS:"></mt-cell>
            <!-- <p> -->
            <!-- <mt-cell title="BDUSS:">
              </mt-cell> -->
            <!-- <span>BDUSS:</span> -->
            <div class="bduss">
              {{item.BDUSS}}
            </div>
            <!-- </p> -->
            <br/>
            <mt-cell title="备注：">
            </mt-cell>
            <mt-cell title="1.最多签到200个贴吧"></mt-cell>
            <mt-cell title="2.每天凌晨2点和下午2点各签到一次，若过了仍未签到，请联系我。"></mt-cell>
            <p>
              <div style="margin-top: 15px;">
                <mt-button plain size="small" type="danger" @click="CancelBind(item.Id)">解绑</mt-button>
              </div>
            </p>
          </div>
        </div>

      </mt-tab-container-item>
      <!--首页结束-->

      <!--账号绑定开始-->
      <mt-tab-container-item id="account">
        <p>
          <mt-field label="用户名" placeholder="请输入用户名" v-model="accountForm.username"></mt-field>
        </p>
        <p>
          <mt-field label="密码" placeholder="请输入密码" type="password" v-model="accountForm.password"></mt-field>
        </p>
        <div v-if="accountForm.imageVisible">
          <p>
            <mt-field label="验证码" v-model="accountForm.verifycode" placeholder="请输入验证码">
              <img :src="accountForm.imageCodeSrc" height="45px" width="100px" alt="验证码" @click="GetImageCode">
              <a href="javascript:void(0)" @click="GetImageCode">换一张</a>
            </mt-field>
          </p>
        </div>
        <p>
          <mt-button type="primary" size="large" @click="BeforeAccountLogin">登陆</mt-button>
        </p>
        <p>
          <mt-button @click="Cancel" size="large">返回</mt-button>
        </p>
      </mt-tab-container-item>
      <!--账号绑定结束-->

      <!--手机号绑定开始-->
      <mt-tab-container-item id="phone">
        <p>
          <mt-field label="手机号" placeholder="请输入手机号" v-model="phoneForm.phone"></mt-field>
        </p>
        <p>
          <mt-field label="验证码" placeholder="请输入验证码" v-model="phoneForm.password">
            <mt-button :disabled="phoneForm.buttonDisable" size="small" @click="CheckPhone">{{phoneForm.buttonMsg}}</mt-button>
          </mt-field>
        </p>
        <mt-button type="primary" size="large" @click="PhoneLogin">登陆</mt-button>
        <!-- <mt-popup v-model="phoneForm.popupVisible" popup-transition="popup-fade" closeOnClickModal="false" modal="true">
          <span>i am ok</span>
        </mt-popup> -->
      </mt-tab-container-item>
      <!--手机号绑定结束-->

      <!--手动绑定开始-->
      <mt-tab-container-item id="manual">
        <mt-field label="BDUSS:" type="textarea" placeholder="请输入BDUSS" v-model="manualForm.bduss"></mt-field>
        <mt-field label="STOKEN:" type="textarea" placeholder="请输入STOKEN" v-model="manualForm.stoken"></mt-field>
        <mt-field label="PTOKEN" type="textarea" placeholder="请输入PTOKEN" v-model="manualForm.ptoken"></mt-field>
        <p>
          <mt-button type="primary" size="large" @click="manuBind">确定</mt-button>
        </p>
        <p>
          <mt-button @click="Cancel" size="large">返回</mt-button>
        </p>
      </mt-tab-container-item>
      <!--手动绑定结束-->
    </mt-tab-container>
    <mt-popup v-model="phoneForm.imageVisible" popup-transition="popup-fade" modal="true" closeOnClickModal="false">
      <mt-header title="请输入验证码"></mt-header>
      <p>
        <img :src="phoneForm.imageCodeSrc" height="35px" width="130px" alt="图片" @click="GetImageCode">
        <a href="javascript:void(0)" @click="GetImageCode">换一张</a>
      </p>
      <p>
        <mt-field v-model="phoneForm.verifycode" placeholder="验证码">
        </mt-field>
        <mt-button type="danger" size="large" @click="CheckImageCode">确定</mt-button>
      </p>
    </mt-popup>
  </div>
</template>
<script>
export default {
  name: 'bind',
  data () {
    return {
      BaiduInfo: [],
      manualForm: {
        bduss: '',
        stoken: '',
        ptoken: ''
      },
      accountForm: {
        username: '',
        password: '',
        verifycode: '',
        imageVisible: false,
        imageCodeSrc: '',
      },
      phoneForm: {
        phone: '',
        password: '',
        buttonDisable: false,
        buttonMsg: '发送动态密码',
        imageVisible: false,
        verifycode: '',
        imageCodeSrc: ''
      },
      active: 'home',
      IsBind: false
    }
  },
  methods: {
    GetBaiduInfo () {
      this.$loading.open();
      this.$axios({
        method: 'get',
        url: '/api/Baidu/GetBaiduInfo'
      }).then(res => {
        this.BaiduInfo = res.data;
        if (this.BaiduInfo.length > 0) {
          this.IsBind = true;
        } else {
          this.IsBind = false;
        }
        this.$loading.close();
      }).catch(() => {
        this.$loading.close();
      });
    },
    Cancel () {
      this.manualForm.bduss = '';
      this.manualForm.stoken = '';
      this.manualForm.ptoken = '';

      this.accountForm.username = '';
      this.accountForm.password = '';
      this.accountForm.imageVisible = false;
      this.accountForm.imageCodeSrc = '';
      this.accountForm.verifycode = '';

      this.phoneForm.phone = '';
      this.phoneForm.password = '';
      this.phoneForm.buttonDisable = false;
      this.phoneForm.buttonMsg = '发送动态密码';
      this.phoneForm.verifycode = '';
      this.phoneForm.imageCodeSrc = '';
      this.phoneForm.imageVisible = false;

      this.active = 'home';
      window.sessionStorage.clear();
    },
    BeforeAccountLogin () {
      if (this.accountForm.imageVisible == true && this.accountForm.imageCodeSrc.length == 0) {
        this.$toast('请输入验证码');
        return;
      }
      if (this.accountForm.username.length == 0 || this.accountForm.password.length == 0) {
        this.$toast('参数错误');
        return;
      }
      this.$axios({
        method: 'get',
        url: '/api/Baidu/GetParameter'
      }).then(res => {
        if (res.data['Error'] == 0) {
          window.sessionStorage.setItem('token', res.data['Token']);
          window.sessionStorage.setItem('BAIDUID', res.data['BAIDUID']);
          window.sessionStorage.setItem('pubkey', res.data['PubKey']);
          window.sessionStorage.setItem('key', res.data['Key']);
          this.AccountLogin();
        } else {
          this.$toast('服务器繁忙，请稍后再试');
        }
      }).catch(() => {
        this.$toast('服务器繁忙，请稍后再试');
      });
    },
    AccountLogin () {
      var token = window.sessionStorage.getItem('token');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var pubkey = window.sessionStorage.getItem('pubkey');
      var key = window.sessionStorage.getItem('key');
      var codestring = window.sessionStorage.getItem('codestring') || '';
      var verifycode = this.accountForm.verifycode;
      var username = this.accountForm.username;
      var password = this.accountForm.password;
      this.$loading.open();
      this.$axios({
        method: 'post',
        url: '/api/Baidu/LoginByAccount',
        data: {
          UserName: username,
          Password: password,
          Token: token,
          BAIDUID: BAIDUID,
          PubKey: pubkey,
          Key: key,
          CodeString: codestring,
          VerifyCode: verifycode
        }
      }).then(res => {
        this.$loading.close();
        var err = res.data['Error'];
        switch (err) {
          case 0:
            this.Cancel();
            this.$toast('绑定成功');
            this.GetBaiduInfo();
            break;
          case 4:
            this.$toast(res.data['Detail']);
            break;
          case 6:
            this.$toast(res.data['Detail']);
            window.sessionStorage.setItem('codestring', res.data['CodeString']);
            this.GetImageCode();
            break;
          case 257:
            this.$toast(res.data['Detail']);
            window.sessionStorage.setItem('codestring', res.data['CodeString']);
            this.GetImageCode();
            break;
          default:
            this.$toast(res.data['Detail']);
            break;
        }
      }).catch(() => {
        this.$loading.close();
        this.$toast('服务器异常，请稍后再试');
      });
    },
    GetImageCode () {
      this.verifycode = '';
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var codestring = window.sessionStorage.getItem('codestring');
      this.$axios({
        method: 'get',
        url: '/api/Baidu/GetImageCode?codestring=' + codestring + '&BAIDUID=' + BAIDUID
      }).then(res => {
        if (this.active == 'account') {
          this.accountForm.imageCodeSrc = 'data:image/jpeg;base64,' + res.data;
          this.accountForm.imageVisible = true;
        } else if (this.active == 'phone') {
          this.phoneForm.imageCodeSrc = 'data:image/jpeg;base64,' + res.data;
          this.phoneForm.imageVisible = true;
        }
      }).catch(() => {
        this.$toast('获取图片验证码失败，请稍后再试');
      });
    },
    CheckImageCode () {
      var token = window.sessionStorage.getItem('token');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var codestring = window.sessionStorage.getItem('codestring');
      var verifycode = this.phoneForm.verifycode;
      this.$axios({
        method: 'get',
        url: '/api/Baidu/CheckImageCode?token=' + token + '&verifycode=' + verifycode + '&codestring=' + codestring + '&BAIDUID=' + BAIDUID
      }).then(res => {
        if (res.data['Error'] == 0) {
          this.phoneForm.imageVisible = false;
          this.SendCode();
        } else {
          this.$toast('验证码错误');
          this.GetImageCode();
        }
      }).catch(() => {
        this.$toast('服务器繁忙，请稍后再试');
      });
    },
    CancelBind (Id) {
      this.$confirm('删除当前绑定的百度账号, 是否继续?').then(() => {
        this.$axios({
          type: 'get',
          url: 'api/Baidu/CancelBinding?Id=' + Id
        }).then(res => {
          if (res.data['Error'] == 0) {
            this.GetBaiduInfo();
            this.$toast(res.data['Detail']);
          } else {
            this.$toast(res.data['Detail']);
          }
        }).catch(() => {
          this.$toast('服务器异常，解绑失败');
        });
      });
    },
    CheckPhone () {
      let reg = /^1\d{10}$/;
      if (reg.test(this.phoneForm.phone)) {
        this.$axios({
          method: 'get',
          url: '/api/Baidu/CheckPhone?phone=' + this.phoneForm.phone
        }).then(res => {
          if (res.data['Error'] == 0) {
            window.sessionStorage.setItem('token', res.data['Token']);
            window.sessionStorage.setItem('BAIDUID', res.data['BAIDUID']);
            this.SendCode();
          } else {
            this.$toast(res.data['Detiail']);
          }
        }).catch(() => {
          this.$toast('服务器繁忙，请稍后再试.');
        });
      } else {
        this.$toast('手机号为11位数字');
      }
    },
    SendCode () {
      var token = window.sessionStorage.getItem('token');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var codestring = window.sessionStorage.getItem('codestring') || '';
      var vcodesign = window.sessionStorage.getItem('vcodesign') || '';
      var verifycode = this.phoneForm.verifycode;
      this.$axios({
        method: 'post',
        url: '/api/Baidu/SendCode',
        data: {
          Phone: this.phoneForm.phone,
          BAIDUID: BAIDUID,
          Token: token,
          VCodeSign: vcodesign,
          CodeString: codestring,
          VerifyCode: verifycode
        }
      }).then(res => {
        if (res.data['Error'] == 0) {
          this.$toast('短信验证码已发送');
          this.phoneForm.buttonDisable = true;
          let countdown = 60;
          let timer1 = setInterval(() => {
            if (countdown == 0) {
              this.phoneForm.buttonDisable = false;
              this.phoneForm.buttonMsg = '发送动态密码';
              window.clearInterval(timer1);
            } else {
              this.phoneForm.buttonMsg = '重新发送(' + countdown + ')';
              countdown--;
            }
          }, 1000);
        } else if (res.data['Error'] == 18) {
          window.sessionStorage.setItem('codestring', res.data['CodeString']);
          window.sessionStorage.setItem('vcodesign', res.data['VCodeSign']);
          this.GetImageCode();
        } else {
          this.$toast(res.data['Detail']);
        }
      }).catch(() => {
        this.$toast('服务器繁忙，请稍后再试.');
      });
    },
    PhoneLogin () {
      var token = window.sessionStorage.getItem('token');
      var vcodesign = window.sessionStorage.getItem('vcodesign');
      var codestring = window.sessionStorage.getItem('codestring');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var phone = this.phoneForm.phone;
      var password = this.phoneForm.password;
      this.$loading.open();
      this.$axios({
        method: 'post',
        url: '/api/Baidu/LoginByPhone',
        data: {
          Phone: phone,
          Password: password,
          BAIDUID: BAIDUID,
          Token: token,
          VCodeSign: vcodesign,
          CodeString: codestring
        }
      }).then(res => {
        this.$loading.close();
        if (res.data['Error'] == 0) {
          this.$toast('成功绑定百度账号');
          this.Cancel();
          this.GetBaiduInfo();
        } else if (res.data['Error'] == 4) {
          this.$toast(res.data['Detail']);
        } else if (res.data['Error'] == 98) {
          this.$toast(res.data['Detail']);
        } else {
          this.$toast('未知错误，请稍后再试');
        }
      }).catch(() => {
        this.$loadingclose();
        this.$toast('服务器繁忙，请稍后再试');
      });
    },
    manuBind () {
      this.$loading.open('正在检查BDUSS是否可用');
      this.$axios({
        method: 'post',
        url: '/api/Baidu/ManualBind',
        data: {
          BDUSS: this.manualForm.bduss,
          STOKEN: this.manualForm.stoken,
          PTOKEN: this.manualForm.ptoken
        }
      }).then((res) => {
        this.$loading.close();
        if (res.data['Error'] == 0) {
          this.$toast('绑定成功');
          this.Cancel();
          this.GetBaiduInfo();
        } else {
          this.$toast(res.data['Detail']);
        }
      }).catch(() => {
        this.$loading.close();
        this.$toast('服务器繁忙，请稍后再试');
      });
    },
  },
  created () {
    this.GetBaiduInfo();
  }
}
</script>
<style>
.bduss {
  word-wrap: break-word;
  background-color: #fff;
  padding: 10px;
  color: #888;
}
#bind .mint-popup {
  height: 20%;
  width: 60%;
  background: #fafafa;
}
</style>
