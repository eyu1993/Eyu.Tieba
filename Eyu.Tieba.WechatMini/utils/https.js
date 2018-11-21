import config from "config.js";
let baseUrl = config.baseUrl;

class https {
  constructor() {}

  request(method, url, params) {
    return new Promise((resolve, reject) => {
      wx.request({
        url: baseUrl + url,
        header: {},
        method: method,
        data: params,
        success: function(res) {
          if (res.data.code == 0) {
            // 成功
            resolve(res.data);
          } else {
            //自定义异常（业务逻辑错误）
            reject(res.data);
          }
        },
        fail: function(err) {
          //服务器异常http code>400
          console.log(err);
          reject(err);
        }
      });
    });
  }
  get(url, params = {}) {
    return this.request("GET", url, params);
  }
  post(url, params = {}) {
    return this.request("POST", url, params);
  }
  put(url, params = {}) {
    return this.request("PUT", url, params);
  }
  patch(url, params = {}) {
    return this.request("PATCH", url, params);
  }
  delete(url, params = {}) {
    return this.request("DELETE", url, params);
  }
}
export default https;
