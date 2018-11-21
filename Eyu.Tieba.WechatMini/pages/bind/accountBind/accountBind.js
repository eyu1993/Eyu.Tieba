let app = getApp();
let util = app.globalData.util;
let https = app.globalData.https;

Page({
  data: {
    username: "",
    password: ""
  },
  onLoad: function(options) {},
  inputName: function(e) {
    this.setData({
      username: e.detail.value
    });
  },
  inputPassword: function(e) {
    this.setData({
      password: e.detail.value
    });
  },
  bind: function() {
    let username = this.data.username.trim();
    let password = this.data.password.trim();
    if (username.length == 0 || password.length == 0) {
      wx.showToast({
        title: "用户或密码不能为空",
        icon: "none"
      });
    } else {
      wx.showLoading({
        title: "正在绑定",
        mask:true
      });
      https
        .post("api/BaiduAPI/AutoBind", {
          UserName: username,
          Password: password
        })
        .then(res => {
          wx.hideLoading();
          if (res.code == 0) {
          }
        })
        .catch(err => {
          wx.hideLoading();
        });
    }
  }
});
