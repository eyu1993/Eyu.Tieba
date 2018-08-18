<template>
  <div name="about">
    <el-row type="flex" justify="center">
      <el-col :span="18">
        <el-card class="info-card">
          <div slot="header" class="clearfix">
            <span>使用必读</span>
          </div>
          <p>
            1.先在本站注册账号
          </p>
          <p>
            2.绑定你的百度账号
          </p>
          <p>
            3.设置自动签到、封禁、回帖
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
  name: 'about'
}
</script>
