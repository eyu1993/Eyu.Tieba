let app = getApp();
let util = app.globalData.util;
let https = app.globalData.https;

Page({
  data: {
    BDUSS: "",
    PTOKEN: "",
    STOKEN: "",
    btnLoading: false
  },
  onLoad: function(options) {
    // console.log(app);
    // console.log(util);
  },
  inputBDUSS: function(e) {
    this.setData({
      BDUSS: e.detail.value
    });
  },
  inputPTOKEN: function(e) {
    this.setData({
      PTOKEN: e.detail.value
    });
  },
  inputSTOKEN: function(e) {
    this.setData({
      STOKEN: e.detail.value
    });
  },
  bind: function() {
    if (this.data.BDUSS.length == 0) {
      wx.showToast({
        title: "BDUSS是必填项",
        icon: "none"
      });
    } else {
      this.setData({
        btnLoading: true
      })
      https
        .post("api/BaiduAPI/ManualBind", {
          BDUSS: this.data.BDUSS.trim(),
          PTOKEN: this.data.PTOKEN.trim(),
          STOKEN: this.data.STOKEN.trim()
        })
        .then(res => {
          this.setData({
            btnLoading: false
          })
          wx.showToast({
            title: '绑定成功',
            success: function() {
              wx.navigateTo({
                url: "/pages/bind/list/list"
              });
            }
          });
        })
        .catch(err => {
          this.setData({
            btnLoading: false
          })
          wx.showToast({
            title: err.message,
            icon: "none"
          });
        });
    }
  }
});