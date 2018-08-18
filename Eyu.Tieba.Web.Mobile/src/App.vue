<template>
  <div id="app">
    <mt-header :fixed="true" :title="header">
      <router-link to="/home/menu" slot="left" v-show="showBack">
        <mt-button icon="back">返回</mt-button>
      </router-link>
    </mt-header>
    <transition name="component-fade" mode="out-in">
      <router-view class="content" />
    </transition>
    <div v-if="isLogin">
      <mt-tabbar v-model="selected" @click.native="Select(selected)" :fixed="true">
        <mt-tab-item id="/home/index">
          <img slot="icon" src="./assets/home.png">主页
        </mt-tab-item>
        <mt-tab-item id="/home/menu">
          <img slot="icon" src="./assets/menu.png">功能菜单
        </mt-tab-item>
        <mt-tab-item id="/user/setting">
          <img slot="icon" src="./assets/user.png">我的
        </mt-tab-item>
      </mt-tabbar>
    </div>
    <br/>
    <br/>
    <br/>
    <footer class="footer">
      <span>© 2017 京ICP备17046207号-1</span>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'app',
  data () {
    return {
      selected: '/home/index',
      header: '云签到',
      showBack: false,
      isLogin: false
    }
  },
  methods: {
    Select (selected) {
      this.$router.replace(selected);
    }
  },
  watch: {
    // 如果路由有变化，会再次执行该方法
    "$route": function () {
      let path = this.$router.currentRoute.fullPath;
      console.log(path);
      if (path == '/home/bind' || path == '/home/ban' || path == '/home/reply') {
        this.showBack = true;
      } else {
        this.showBack = false;
      }
    }
  },
  created () {
    let token = localStorage.getItem('token');
    if (token != null) {
      this.isLogin = true;
    } else {
      this.isLogin = false;
    }
  },
  updated () {
    let token = localStorage.getItem('token');
    if (token != null) {
      this.isLogin = true;
    } else {
      this.isLogin = false;
    }
  }
}
</script>

<style>
body {
  margin: 0px;
  padding: 0px;
  background-color: #fafafa;
}

.footer {
  bottom: 0px;
  text-align: center;
  margin-bottom: 60px;
}
/* .mint-header {
  z-index: 199433;
} */
.mint-tabbar {
  background-color: #26a2ff;
  color: white;
}
.mint-tabbar > .mint-tab-item.is-selected {
  background-color: #26a2ff;
  color: black;
}
.mint-popup-right {
  top: 40px;
  right: 0px;
  transform: translate3d(0, 0%, 0);
  height: 100%;
  width: 35%;
  background: #fafafa;
}
.content {
  margin-top: 40px;
}
</style>
