using Eyu.Tieba.Common;
using Eyu.Tieba.WebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Eyu.Tieba.WebAPI
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            LogHelper.Error(actionExecutedContext.Exception.GetType().ToString() + ":" + actionExecutedContext.Exception.Message + "——Stack information：" +
                    actionExecutedContext.Exception.StackTrace + "\r\n");

            Result result = new Result();
            HttpResponseMessage resp = new HttpResponseMessage();
            if (actionExecutedContext.Exception is NotImplementedException)
            {
                resp.StatusCode = HttpStatusCode.NotImplemented;
                result.Error = 99;
                result.Detail = "NotImplemented";
            }
            else if (actionExecutedContext.Exception is TimeoutException)
            {
                resp.StatusCode = HttpStatusCode.RequestTimeout;
                result.Error = 99;
                result.Detail = "RequestTimeout";
            }
            //.....这里可以根据项目需要返回到客户端特定的状态码。如果找不到相应的异常，统一返回服务端错误500
            else
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                result.Error = 99;
                result.Detail = "InternalServerError";
            }
            resp.Content = new StringContent(JsonConvert.SerializeObject(result));
            actionExecutedContext.Response = resp;
            base.OnException(actionExecutedContext);
        }
    }
}