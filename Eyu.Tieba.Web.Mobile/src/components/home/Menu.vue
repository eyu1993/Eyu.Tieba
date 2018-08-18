<template>
  <div id="menu">
    <p>
      <mt-cell title="绑定百度账号" to="/home/bind" is-link>
        <img slot="icon" src="../../assets/bind.png" width="24" height="24">
      </mt-cell>
    </p>
    <p>
      <mt-cell title="循环封禁" to="/home/ban" is-link>
        <img slot="icon" src="../../assets/ban.png" width="24" height="24">
      </mt-cell>
    </p>
    <p>
      <mt-cell title="自动回贴" to="/home/reply" is-link>
        <img slot="icon" src="../../assets/reply.png" width="24" height="24">
      </mt-cell>
    </p>
  </div>
</template>
<script>
export default {
  name: 'menu',
  data () {
    return {
      //selected: 'home'
    }
  }
}
</script>