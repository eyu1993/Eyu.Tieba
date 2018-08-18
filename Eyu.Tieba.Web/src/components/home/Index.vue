<template>
  <div id="index">
    <el-row type="flex" justify="center">
      <el-col :span="18">
        <el-card class="info-card">
          <div slot="header" class="clearfix">
            <span>程序信息</span>
          </div>
          <p>欢迎你 ：
            <a href="javascript:void(0)">{{username}}</a>
          </p>
          <p>百度贴吧云签到 V1.0</p>
          <p>点击上方功能导航可列出所有菜单</p>
          <p>此程序作者为 ：
            <a href="https://weibo.com/u/2074323574" target="_blank">
              <strong>鳄鱼大帝</strong>
              <img src="../../assets/weibo.png" alt="微博">
            </a>
          </p>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>
<script>

export default {
  name: 'index',
  data () {
    return {
      username: ''
    }
  },
  methods: {
    GetUserName () {
      this.$loading();
      this.$axios({
        type: 'get',
        url: '/api/user/GetUserName'
      }).then(res => {
        if (res.data['Error'] == 0) {
          this.username = res.data['Detail'];
        } else {
          this.$message.info('登陆信息已过期');
          window.sessionStorage.clear();
          delete localStorage['token'];
          this.$router.replace('/user/login');
        }
        this.$loading().close();
      }).catch(() => {
        this.$message.error('登陆信息已过期');
        window.sessionStorage.clear();
        delete localStorage['token'];
        this.$router.replace('/user/login');
        this.$loading().close();
      });
    }
  },
  created () {
    this.GetUserName();
  }
}
</script>
<style >
.info-card {
  margin: 20px;
  border: 1px solid dodgerblue;
}

.success-card {
  margin: 20px;
  border: 1px solid #42d885;
}

.info-card .el-card__header {
  background-color: dodgerblue;
}

.success-card .el-card__header {
  background-color: #42d885;
}

#index .info-card a {
  color: dodgerblue;
  text-decoration: none;
}
</style>



