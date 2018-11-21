let app = getApp();
let util = app.globalData.util;
let https = app.globalData.https;

Page({
  data: {
    showMask: false,
    btnMenuText: "+"
  },
  onShow: function() {
    this.getBaiduList();
  },
  onLoad: function(options) {},
  //获取绑定的列表
  getBaiduList: function() {
    let page = this;
    wx.showLoading({
      title: "加载中..."
    });
    https
      .get("api/baidu")
      .then(res => {
        res.data.forEach(item => {
          item["BindDate"] = new Date(item["BindDate"]).Format(
            "yyyy-MM-dd hh:mm:ss"
          );
        });
        page.setData({
          baiduList: res.data
        });
        wx.hideLoading();
      })
      .catch(err => {
        console.log("错误");
        console.log(err);
        wx.hideLoading();
      });
  },
  //更新数据
  update: function() {},
  //取消绑定
  unbind: function(e) {
    let page = this;
    let id = e.target.dataset.id;
    wx.showModal({
      title: "解除绑定？",
      success: function(res) {
        if (res.confirm) {
          https
            .delete("api/baidu/" + id)
            .then(res => {
              wx.showToast({
                title: "解绑成功"
              });
              page.getBaiduList();
            })
            .catch(err => {
              console.log(err["message"]);
            });
        }
      }
    });
  },
  //显示操作菜单
  showMenu: function() {
    wx.showActionSheet({
      itemList: ['手动绑定', '账号密码登陆', '短信快捷登陆', '使用教程'],
      success: function(res) {
        if (res.tapIndex == 0) {
          wx.navigateTo({
            url: '/pages/bind/manualBind/manualBind'
          })
        } else if (res.tapIndex == 1) {
          wx.navigateTo({
            url: '/pages/bind/accountBind/accountBind'
          })
        } else if (res.tapIndex == 2) {
          wx.navigateTo({
            url: '/pages/bind/smsBind/smsBind'
          })
        } else {
          wx.navigateTo({
            url: '/pages/help/help'
          })
        }
      }
    });
  }
});