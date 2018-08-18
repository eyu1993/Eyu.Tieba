<template>
  <div name="tip">
    <el-row type="flex" justify="center">
      <el-col :span="18">
        <el-card class="info-card">
          <div slot="header">
            <span>捐赠</span>
          </div>
          <div style="text-align:center">
            <img src="../assets/wechat.jpg" alt="wechat" class="pay">
            <br/>
            <p>
              微信扫码捐赠
            </p>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>
<script>
export default {
  name: 'tip'
}
</script>

<style scoped>
.pay {
  height: 450px;
  margin: auto 0;
}
</style>
