<template>
  <div id="login">
    <el-row type="flex" class="row-bg" justify="space-around">
      <el-card class="info-card login-card" :body-style="{ width: '380px' }">
        <div slot="header" class="clearfix">
          <strong>登陆</strong>
        </div>
        <el-form ref="form" :model="form" label-width="80px" :label-position="'left'" :rules="rules">
          <el-form-item label="用户名" prop="name">
            <el-input v-model="form.name" placeholder="用户名/手机号"></el-input>
          </el-form-item>
          <el-form-item label="密码" prop="password">
            <el-input v-model="form.password" type="password" placeholder="请输入密码"></el-input>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="Login">立即登陆</el-button>
            <router-link to="/user/findpassword">找回密码</router-link>
          </el-form-item>
        </el-form>
      </el-card>
    </el-row>
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
      this.$refs['form'].validate((valid) => {
        if (valid) {
          this.$login(this.form.name, this.form.password);
        } else {
          this.$message.error('参数有误');
        }
      });
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
.clearfix {
  text-align: center;
}
</style>

