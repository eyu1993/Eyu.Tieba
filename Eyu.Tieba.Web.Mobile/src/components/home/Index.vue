<template>
  <div id="index">
    <p>
      <mt-cell title="欢迎你：">
        {{username}}
      </mt-cell>
    </p>
    <p>
      <mt-cell title="百度贴吧云签到 V1.0"></mt-cell>
    </p>
    <p>
      <mt-cell title="点击功能导航可列出所有菜单"></mt-cell>
    </p>
    <p>
      <mt-cell title="此程序作者为 ：">
        <a href="https://weibo.com/u/2074323574" target="_blank">
          <strong>鳄鱼大帝</strong>
          <img src="../../assets/weibo.png" alt="微博">
        </a>
      </mt-cell>
    </p>
  </div>
</template>

<script>
export default {
  name: 'index',
  data () {
    return {
      msg: 'i am index',
      username: ''
    }
  },
  methods: {
    GetUserName () {
      this.$loading.open();
      this.$axios({
        type: 'get',
        url: '/api/user/GetUserName'
      }).then(res => {
        this.$loading.close();
        if (res.data['Error'] == 0) {
          this.username = res.data['Detail'];
        }
      }).catch(() => {
        this.$loading.close();
      });
    }
  },
  created () {
    this.GetUserName();
  }
}
</script>

<style>

</style>
