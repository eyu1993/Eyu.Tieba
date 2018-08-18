<template>
  <div id="bind">
    <el-row type="flex" justify="center">
      <el-col :span="18">
        <el-card class="info-card" v-if="!isBind">
          <div slot="header" class="clearfix">
            <span>贴吧信息</span>
          </div>
          <p>
            暂未绑定任何贴吧
          </p>
          <div style="margin-top: 15px;">
            <el-button type="text" @click="autoFormVisible = true">自动绑定</el-button>
            <el-button type="text" @click="manuFormVisible = true">手动绑定</el-button>
          </div>
        </el-card>
        <div v-else>
          <div v-for="item in bds">
            <el-card class="info-card">
              <div slot="header" class="clearfix">
                <span>贴吧信息</span>
              </div>
              <p>
                <strong>贴吧ID : </strong>
                <span>{{item.Name}}</span>
              </p>
              <p>
                <div style="word-wrap:break-word;">
                  <strong>BDUSS : </strong>{{item.BDUSS}}
                </div>
              </p>
              <p>
                <strong>备注 : </strong>
              </p>
              <p>
                <span>1.最多签到200个贴吧</span>
              </p>
              <p>
                <span>2.每天凌晨2点和下午2点各签到一次，若过了仍未签到，请联系我。</span>
              </p>
              <div style="margin-top: 15px;">
                <!-- <el-button type="text">更换</el-button> -->
                <el-button type="text" @click="CancelBind(item.Id)">解绑</el-button>
              </div>
            </el-card>
          </div>
        </div>
      </el-col>
    </el-row>

    <el-dialog title="自动绑定" :visible.sync="autoFormVisible" :before-close="autoClose">
      <el-tabs v-model="activeName">
        <el-tab-pane label="账号密码登陆" name="account">
          <el-form ref="accountForm" :model="accountForm" :rules="accountRules">
            <el-form-item prop="username" label="用户名">
              <el-input v-model="accountForm.username" placeholder="请输入用户名"></el-input>
            </el-form-item>
            <el-form-item prop="password" label="密码">
              <el-input v-model="accountForm.password" placeholder="请输入密码" type="password"></el-input>
            </el-form-item>
            <el-form-item style="text-align:right;margin-top:40px;">
              <el-button type="primary" @click="BeforeAccountLogin">登 陆</el-button>
              <el-button @click="resetForm('accountForm')">取 消</el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <el-tab-pane label="短信快捷登陆" name="phone">
          <el-form ref="phoneForm" :model="phoneForm" :rules="phoneRules">
            <el-form-item prop="phone" label="手机号">
              <el-input v-model="phoneForm.phone" placeholder="请输入手机号"></el-input>
            </el-form-item>
            <el-form-item prop="password" label="验证码">
              <el-input v-model="phoneForm.password" placeholder="请输入验证码">
                <el-button slot="append" @click="CheckPhone" :disabled="phoneForm.buttonDisable">{{phoneForm.buttonMsg}}</el-button>
              </el-input>
            </el-form-item>
            <el-form-item style="text-align:right;margin-top:40px;">
              <el-button type="primary" @click="PhoneLogin">登 陆</el-button>
              <el-button @click="resetForm('phoneForm')">取 消</el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-dialog>
    <el-dialog title="手动绑定" :visible.sync="manuFormVisible" :before-close="manualClose">
      <el-form ref="manualForm" :model="manualForm" :rules="manualRules">
        <el-form-item prop="bduss" label="BDUSS">
          <el-input v-model="manualForm.bduss" placeholder="请输入BDUSS"></el-input>
        </el-form-item>
        <el-form-item prop="stoken" label="STOKEN">
          <el-input v-model="manualForm.stoken" placeholder="请输入STOKEN"></el-input>
        </el-form-item>
        <el-form-item prop="ptoken" label="PTOKEN">
          <el-input v-model="manualForm.ptoken" placeholder="请输入PTOKEN"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="primary" @click="manuBind">确 定</el-button>
        <el-button @click="resetForm('manualForm')">取 消</el-button>
      </div>
    </el-dialog>
    <el-dialog title="请输入验证码" :visible.sync="imageVisible" size="tiny">
      <el-row>
        <el-col :span="12">
          <el-input v-model="verifycode"></el-input>
        </el-col>
        <el-col :span="12">
          <img :src="imageCodeSrc" alt="验证码">
          <a href="javascript:void(0)" @click="GetImageCode">换一张</a>
        </el-col>
      </el-row>
      <span slot="footer" class="dialog-footer">
        <el-button type="primary" @click="CheckImageCode">确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>
<script>

export default {
  name: 'bind',
  data () {
    var validatePhone = (rule, value, callback) => {
      let reg = /^1\d{10}$/;
      if (!reg.test(value)) {
        callback(new Error('请输入正确的手机号码'));
      } else {
        callback();
      }
    };
    return {
      isBind: false,
      activeName: 'account',
      imageCodeSrc: '',
      verifycode: '',
      imageVisible: false,
      autoFormVisible: false,
      manuFormVisible: false,
      bds: [],
      manualForm: {
        bduss: '',
        stoken: '',
        ptoken: ''
      },
      accountForm: {
        username: '',
        password: ''
      },
      phoneForm: {
        phone: '',
        password: '',
        buttonDisable: false,
        buttonMsg: '发送动态密码'
      },
      phoneRules: {
        phone: [
          { validator: validatePhone },
          { required: true, message: '请输入手机号码' }
        ],
        password: [
          { required: true, message: '请输入验证码' }
        ]
      },
      accountRules: {
        username: [
          { required: true, message: '请输入用户名' }
        ],
        password: [
          { required: true, message: '请输入密码' }
        ]
      },
      manualRules: {
        bduss: [
          { required: true, message: '请输入BDUSS' }
        ],
        stoken: [
          // { required: true, message: '请输入STOKEN', tigger: 'blur' }
        ],
        ptoken: [
          // { required: true, message: '请输入PTOKEN', tigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    GetBaiduInfo () {
      this.$loading();
      this.$axios({
        method: 'get',
        url: '/api/Baidu/GetBaiduInfo'
      }).then(res => {
        this.bds = res.data;
        if (this.bds.length > 0) {
          this.isBind = true;
        } else {
          this.isBind = false;
        }
        this.$loading().close();
      }).catch(() => {
        this.$loading().close();
        this.$message.error('服务器繁忙，请稍后再试');
      });
    },
    resetForm (formName) {
      this.$refs[formName].resetFields();
      this.autoFormVisible = false;
      this.manuFormVisible = false;
    },
    autoClose (done) {
      this.$refs['accountForm'].resetFields();
      this.$refs['phoneForm'].resetFields();
      done();
    },
    manualClose (done) {
      this.resetForm('manualForm');
      done();
    },
    CheckPhone () {
      this.$refs['phoneForm'].validateField('phone', (valid) => {
        if (valid.length == 0) {
          this.$axios({
            method: 'get',
            url: '/api/Baidu/CheckPhone?phone=' + this.phoneForm.phone
          }).then(res => {
            if (res.data['Error'] == 0) {
              window.sessionStorage.setItem('token', res.data['Token']);
              window.sessionStorage.setItem('BAIDUID', res.data['BAIDUID']);
              this.SendCode();
            } else {
              this.$message.error(res.data['Detiail']);
            }
          }).catch(() => {
            this.$message.error('服务器繁忙，请稍后再试.');
          });
        } else {
          this.$message.warning('请输入正确的手机号!');
        }
      });
    },
    SendCode () {
      var token = window.sessionStorage.getItem('token');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var codestring = window.sessionStorage.getItem('codestring') || '';
      var vcodesign = window.sessionStorage.getItem('vcodesign') || '';
      var verifycode = this.verifycode;
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
          this.$message.success('短信验证码已发送');
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
          this.$message.error(res.data['Detail']);
        }
      }).catch(() => {
        this.$message.error('服务器繁忙，请稍后再试.');
      });
    },
    PhoneLogin () {
      var token = window.sessionStorage.getItem('token');
      var vcodesign = window.sessionStorage.getItem('vcodesign');
      var codestring = window.sessionStorage.getItem('codestring');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var phone = this.phoneForm.phone;
      var password = this.phoneForm.password;
      this.$loading();
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
        this.$loading().close();
        if (res.data['Error'] == 0) {
          this.autoFormVisible = false;
          this.$message.success('成功绑定百度账号');
          this.GetBaiduInfo();
        } else if (res.data['Error'] == 4) {
          this.$message.error(res.data['Detail']);
        } else if (res.data['Error'] == 98) {
          this.$message.error(res.data['Detail']);
        } else {
          this.$message.error('未知错误，请稍后再试');
        }
      }).catch(() => {
        this.$loading().close();
        this.$message.error('服务器繁忙，请稍后再试');
      });
    },
    CheckImageCode () {
      var token = window.sessionStorage.getItem('token');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var codestring = window.sessionStorage.getItem('codestring');
      var verifycode = this.verifycode;
      this.$axios({
        method: 'get',
        url: '/api/Baidu/CheckImageCode?token=' + token + '&verifycode=' + verifycode + '&codestring=' + codestring + '&BAIDUID=' + BAIDUID
      }).then(res => {
        if (res.data['Error'] == 0) {
          this.imageVisible = false;
          if (this.activeName == 'account') {
            this.AccountLogin();
          } else if (this.activeName == 'phone') {
            this.SendCode();
          }
        } else {
          this.$message.error('验证码错误');
          this.GetImageCode();
        }
      }).catch(() => {
        this.$message.error('服务器繁忙，请稍后再试');
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
        this.imageCodeSrc = 'data:image/jpeg;base64,' + res.data;
        this.imageVisible = true;
      }).catch(() => {
        this.$message.error('获取图片验证码失败，请稍后再试');
      });
    },
    BeforeAccountLogin () {
      this.$refs['accountForm'].validate((valid) => {
        if (valid) {
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
              this.$message.error('服务器繁忙，请稍后再试');
            }
          }).catch(() => {
            this.$message.error('服务器繁忙，请稍后再试');
          });
        } else {
          this.$message.warning('参数有误');
        }
      });
    },
    AccountLogin () {
      var token = window.sessionStorage.getItem('token');
      var BAIDUID = window.sessionStorage.getItem('BAIDUID');
      var pubkey = window.sessionStorage.getItem('pubkey');
      var key = window.sessionStorage.getItem('key');
      var codestring = window.sessionStorage.getItem('codestring') || '';
      var verifycode = this.verifycode;
      var username = this.accountForm.username;
      var password = this.accountForm.password;
      this.$loading();
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
        this.$loading().close();
        var err = res.data['Error'];
        switch (err) {
          case 0:
            this.autoFormVisible = false;
            this.$message.success(res.data['Detail']);
            this.GetBaiduInfo();
            break;
          case 4:
            this.$message.error(res.data['Detail']);
            break;
          case 6:
            this.$message.error(res.data['Detail']);
            window.sessionStorage.setItem('codestring', res.data['CodeString']);
            this.GetImageCode();
            break;
          case 257:
            this.$message.error(res.data['Detail']);
            window.sessionStorage.setItem('codestring', res.data['CodeString']);
            this.GetImageCode();
            break;
          default:
            this.$message.error(res.data['Detail']);
            break;
        }
      }).catch(() => {
        this.$loading().close();
        this.$message.error('服务器异常，请稍后再试');
      });
    },
    manuBind () {
      this.$refs['manualForm'].validate((valid) => {
        if (valid) {
          let loading = this.$loading({
            text: '正在检查BDUSS是否可用'
          })
          this.$axios({
            method: 'post',
            url: '/api/Baidu/ManualBind',
            data: {
              BDUSS: this.manualForm.bduss,
              STOKEN: this.manualForm.stoken,
              PTOKEN: this.manualForm.ptoken
            }
          }).then((res) => {
            loading.close();
            if (res.data['Error'] == 0) {
              this.$message.success('绑定成功');
              this.manuFormVisible = false;
              this.GetBaiduInfo();
            } else {
              this.$message.error(res.data['Detail']);
            }
          }).catch(() => {
            loading.close();
            this.$message.error('服务器繁忙，请稍后再试');
          });
        } else {
          this.$message.warning('参数有误');
        }
      });
    },
    CancelBind (Id) {
      this.$confirm('删除当前绑定的百度账号, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      })
        .then(() => {
          this.$axios({
            type: 'get',
            url: 'api/Baidu/CancelBinding?Id=' + Id
          }).then(res => {
            if (res.data['Error'] == 0) {
              this.GetBaiduInfo();
              this.$message.success(res.data['Detail']);
            } else {
              this.$message.error(res.data['Detail']);
            }
          }).catch(() => {
            this.$message.error('服务器异常，解绑失败');
          });
        }).catch(() => {
          this.$message.warning('取消操作');
        });
    }
  },
  created () {
    this.GetBaiduInfo();
  }
}
</script>
<style>
.el-form {
  margin-bottom: 20px;
}

.info-card a {
  color: dodgerblue;
  text-decoration: none;
}
</style>
