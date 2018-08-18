<template>
  <div id="feedback">
    <el-row type="flex" justify="center">
      <el-col :span="18">
        <el-card class="info-card">
          <div slot="header" class="clearfix">
            <span>反馈</span>
          </div>
          <p>
            有任何问题，或者发现bug，请直接联系我。
          </p>
          <br/>
          <p>
            <strong>邮箱：</strong>
            <span>eyu1993@otlook.com</span>
          </p>
          <p>
            <strong>微博：</strong>
            <a href="https://weibo.com/u/2074323574" target="_blank">
              <strong>鳄鱼大帝</strong>
              <img src="../assets/weibo.png" alt="微博">
            </a>
          </p>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>
<script>
export default {
  name: 'feedback',
  data () {
    return {
      msg: 'i am feedback'
    }
  }
}
</script>
<style>

</style>


