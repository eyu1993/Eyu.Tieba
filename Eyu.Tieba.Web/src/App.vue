<template>
  <div id="app">
    <img src="./assets/bg2.jpg" alt="背景图片" style="z-index:-1;position:fixed;width:1440px;overflow:hidden;">
    <el-menu mode="horizontal" :router="true" :default-active="$route.path" background-color="#0078d7" text-color="#fff" active-text-color="black">
      <el-row type="flex" justify="space-around">
        <div v-if="!IsLogin">
          <el-menu-item index="/user/login">登陆</el-menu-item>
          <el-menu-item index="/user/register">注册</el-menu-item>
        </div>
        <div v-else>
          <el-menu-item index="/home/index">首页</el-menu-item>
          <el-submenu index="2">
            <template slot="title">功能菜单</template>
            <el-menu-item index="/home/bind">绑定百度账号</el-menu-item>
            <el-menu-item index="/home/ban">循环封禁</el-menu-item>
            <el-menu-item index="/home/reply">自动回贴</el-menu-item>
          </el-submenu>
          <el-menu-item index="/user/setting">设置</el-menu-item>
          <el-submenu index="4">
            <template slot="title">关于</template>
            <el-menu-item index="/about">使用必读</el-menu-item>
            <el-menu-item index="/feedback">反馈</el-menu-item>
          </el-submenu>
        </div>
        <div v-show="IsLogin">
          <el-menu-item index="/logout" @click="Logout">退出</el-menu-item>
          <el-menu-item index="/tip">捐赠</el-menu-item>
        </div>
        <div v-show="!IsLogin">
          <el-menu-item index="/about">使用必读</el-menu-item>
        </div>
      </el-row>
    </el-menu>
    <transition name="component-fade" mode="out-in">
      <router-view/>
    </transition>
    <br/>
    <br/>
    <br/>
    <el-row type="flex" justify="center">
      <span>© 2017 京ICP备17046207号-1</span>
    </el-row>
  </div>
</template>

<script>
export default {
  name: 'app',
  data () {
    return {
      IsLogin: false
    }
  },
  methods: {
    Logout () {
      this.$logout();
    }
  },
  created () {
    let token = localStorage['token'];
    if (token != null) {
      this.IsLogin = true;
    } else {
      this.IsLogin = false;
    }
  },
  updated () {
    let token = localStorage['token'];
    if (token != null) {
      this.IsLogin = true;
    } else {
      this.IsLogin = false;
    }
  }
}
</script>
<style>
body {
  margin: 0px;
  padding: 0px;
}
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
</style>
