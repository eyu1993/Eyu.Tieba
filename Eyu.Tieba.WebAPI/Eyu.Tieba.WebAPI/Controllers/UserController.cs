using Eyu.Tieba.Common;
using Eyu.Tieba.Model;
using Eyu.Tieba.UnitOfWork;
using Eyu.Tieba.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Eyu.Tieba.WebAPI.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        public IUnitOfWork UOW { get; set; }
        public UserController(IUnitOfWork unitOfWork)
        {
            this.UOW = unitOfWork;
        }

        //发送注册验证码
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult SendRegisterSMSCode(string phone)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            if (IsExistPhone(phone))
            {
                result.Error = 1;
                result.Detail = "手机号已被注册";
                return Ok(result);
            }
            string code = GenerateAuthenticode();
            RedisHelper.SetEntryInMyHash(Operation.Register, phone, code, DateTime.Now.AddMinutes(5));
            SMSResult err = SMSHelper.Send(phone, code);
            switch (err)
            {
                case SMSResult.OK:
                    result.Error = 0;
                    result.Detail = "发送成功";
                    LogHelper.Info(string.Format("Send register SMS code to {0} successfully. IP address is {1}.", phone, ip));
                    break;
                case SMSResult.Frequently:
                    result.Error = 1;
                    result.Detail = "短信发送过于频繁";
                    LogHelper.Info(string.Format("Send register SMS code to {0} failed. IP address is {1}. Reason: too frequent.", phone, ip));
                    break;
                case SMSResult.Exception:
                    result.Error = 1;
                    result.Detail = "短信系统出错";
                    LogHelper.Info(string.Format("Send register SMS code to {0} failed. IP address is {1}. Reason: SMS system error.", phone, ip));
                    break;
                default:
                    result.Error = 1;
                    result.Detail = "发送失败";
                    LogHelper.Info(string.Format("Send register SMS code to {0} failed. IP address is {1}. Reason: Other errors.", phone, ip));
                    break;
            }
            return Ok(result);
        }

        //注册
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> Register(Register model)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            if (!ModelState.IsValid || model == null)
            {
                result.Error = 1;
                result.Detail = "参数错误";
                return Ok(result);
            }
            if (IsExistPhone(model.Phone))
            {
                result.Error = 1;
                result.Detail = "此手机号已被注册";
                return Ok(result);
            }
            if (IsExistUser(model.UserName))
            {
                result.Error = 1;
                result.Detail = "用户名已被使用";
                return Ok(result);
            }
            string code = RedisHelper.GetValueFromMyHash(Operation.Register, model.Phone);
            if (code != model.Code)
            {
                result.Error = 1;
                result.Detail = "验证码错误";
                return Ok(result);
            }
            UOW.UserRepository.Add(new User()
            {
                UserId = Guid.NewGuid(),
                Name = model.UserName,
                Password = MD5Helper.GetStrMD5(model.Password),
                Phone = model.Phone,
                IsActive = true,
                //Email="",
                CreatedTime = DateTime.Now
            });
            if (await UOW.SaveChangesAsync() > 0)
            {
                result.Error = 0;
                result.Detail = "注册成功";
                LogHelper.Info(string.Format("User {0} registered. IP address is {1}.", model.UserName, ip));
                return Ok(result);
            }
            result.Error = 1;
            result.Detail = "注册失败";
            return Ok(result);
        }

        //发送找回密码验证码
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult SendFindPwdSMSCode(string phone)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            if (!IsExistPhone(phone))
            {
                result.Error = 1;
                result.Detail = "手机号不存在";
            }
            else
            {
                string code = GenerateAuthenticode();
                RedisHelper.SetEntryInMyHash(Operation.FindPassword, phone, code, DateTime.Now.AddMinutes(5));
                SMSResult err = SMSHelper.Send(phone, code);
                switch (err)
                {
                    case SMSResult.OK:
                        result.Error = 0;
                        result.Detail = "发送成功";
                        LogHelper.Info(string.Format("Send find password SMS code to {0} successfully. IP address is {1}.", phone, ip));
                        break;
                    case SMSResult.Frequently:
                        result.Error = 1;
                        result.Detail = "短信发送过于频繁";
                        LogHelper.Info(string.Format("Send find password SMS code to {0} failed. IP address is {1}. Reason: too frequent.", phone, ip));
                        break;
                    case SMSResult.Exception:
                        result.Error = 1;
                        result.Detail = "短信系统出错";
                        LogHelper.Info(string.Format("Send find password SMS code to {0} failed. IP address is {1}. Reason: SMS system error.", phone, ip));
                        break;
                    default:
                        result.Error = 1;
                        result.Detail = "发送失败";
                        LogHelper.Info(string.Format("Send find password SMS code to {0} failed. IP address is {1}. Reason: Other errors.", phone, ip));
                        break;
                }
            }
            return Ok(result);
        }

        //检查找回密码的验证码是否正确
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult CheckFindPwdCode(string phone, string code)
        {
            Result result = new Result();
            if (code == RedisHelper.GetValueFromMyHash(Operation.FindPassword, phone))
            {
                result.Error = 0;
                result.Detail = "短信验证码正确";
            }
            else
            {
                result.Error = 1;
                result.Detail = "短信验证码错误";
            }
            return Ok(result);
        }

        //找回密码
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> FindPassword(string phone, string code, string password)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            if (code != RedisHelper.GetValueFromMyHash(Operation.FindPassword, phone))
            {
                result.Error = 1;
                result.Detail = "验证码错误";
                return Ok(result);

            }
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Phone == phone && u.IsActive == true);
            if (user != null)
            {
                string newPwd = MD5Helper.GetStrMD5(password);
                user.Password = newPwd;
                UOW.UserRepository.Update(user);
                if (await UOW.SaveChangesAsync() > 0)
                {
                    result.Error = 0;
                    result.Detail = "密码已重置";
                    LogHelper.Info(string.Format("User {0} reset password. IP address is {1}.", user.Name, ip));
                    return Ok(result);
                }

            }
            result.Error = 1;
            result.Detail = "密码设置失败";
            return Ok(result);
        }

        //检查是否是当前用户的手机号码,并发送
        [HttpGet]
        public async Task<IHttpActionResult> CheckPhone(string phone)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            string username = User.Identity.Name;
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == username && u.Phone == phone);
            if (user != null)
            {
                string code = GenerateAuthenticode();
                RedisHelper.SetEntryInMyHash(Operation.ChangePhone, phone, code, DateTime.Now.AddMinutes(5));
                SMSResult res = SMSHelper.Send(phone, code);
                switch (res)
                {
                    case SMSResult.OK:
                        result.Error = 0;
                        result.Detail = "发送成功";
                        LogHelper.Info(string.Format("Send  change phone SMS code to {0} successfully. IP address is {1}.", phone, ip));
                        break;
                    case SMSResult.Frequently:
                        result.Error = 1;
                        result.Detail = "短信发送过于频繁";
                        LogHelper.Info(string.Format("Send change phone SMS code to {0} failed. IP address is {1}. Reason: too frequent.", phone, ip));
                        break;
                    case SMSResult.Exception:
                        result.Error = 1;
                        result.Detail = "短信系统出错";
                        LogHelper.Info(string.Format("Send change phone SMS code to {0} failed. IP address is {1}. Reason: SMS system error.", phone, ip));
                        break;
                    default:
                        result.Error = 1;
                        result.Detail = "发送失败";
                        LogHelper.Info(string.Format("Send change phone SMS code to {0} failed. IP address is {1}. Reason: Other errors.", phone, ip));
                        break;
                }
            }
            else
            {
                result.Error = 1;
                result.Detail = "手机号码错误";
            }
            return Ok(result);
        }

        //检查更换手机号短信验证码是否正确
        [HttpGet]
        public IHttpActionResult CheckCode(string phone, string code)
        {
            Result result = new Result();
            if (code == RedisHelper.GetValueFromMyHash(Operation.ChangePhone, phone))
            {
                result.Error = 0;
                result.Detail = "短信验证码正确";
            }
            else
            {
                result.Error = 1;
                result.Detail = "短信验证码错误";
            }
            return Ok(result);
        }

        //发送更换手机号短信验证码
        [HttpGet]
        public async Task<IHttpActionResult> SendChangePhoneCode(string phone)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            string username = User.Identity.Name;
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == username);
            if (user == null)
            {
                result.Error = 1;
                result.Detail = "发送失败";
                return Ok(result);
            }
            else
            {
                if (user.Phone == phone)
                {
                    result.Error = 1;
                    result.Detail = "请输入新的手机号";
                    return Ok(result);
                }
            }
            if (IsExistPhone(phone))
            {
                result.Error = 1;
                result.Detail = "手机号已被其他账号绑定，请解绑后再操作。";
                return Ok(result);
            }

            string code = GenerateAuthenticode();
            RedisHelper.SetEntryInMyHash(Operation.ChangePhone, phone, code, DateTime.Now.AddMinutes(5));
            SMSResult err = SMSHelper.Send(phone, code);
            switch (err)
            {
                case SMSResult.OK:
                    result.Error = 0;
                    result.Detail = "发送成功";
                    LogHelper.Info(string.Format("Send  change phone SMS code to {0} successfully. IP address is {1}.", phone, ip));
                    break;
                case SMSResult.Frequently:
                    result.Error = 1;
                    result.Detail = "短信发送过于频繁";
                    LogHelper.Info(string.Format("Send change phone SMS code to {0} failed. IP address is {1}. Reason: too frequent.", phone, ip));
                    break;
                case SMSResult.Exception:
                    result.Error = 1;
                    result.Detail = "短信系统出错";
                    LogHelper.Info(string.Format("Send change phone SMS code to {0} failed. IP address is {1}. Reason: SMS system error.", phone, ip));
                    break;
                default:
                    result.Error = 1;
                    result.Detail = "发送失败";
                    LogHelper.Info(string.Format("Send change phone SMS code to {0} failed. IP address is {1}. Reason: Other errors.", phone, ip));
                    break;
            }
            return Ok(result);
        }

        //更换手机号
        [HttpPost]
        public async Task<IHttpActionResult> ChangePhone(ChangePhone model)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            if (!ModelState.IsValid || model == null)
            {
                result.Error = 1;
                result.Detail = "参数有误";
                return Ok(result);
            }
            if (model.Phone == model.NewPhone)
            {
                result.Error = 1;
                result.Detail = "请输入新的手机号";
                return Ok(result);
            }
            result.Error = 1;
            result.Detail = "修改失败";
            string username = User.Identity.Name;
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == username && u.Phone == model.Phone);
            if (user != null)
            {
                if (IsExistPhone(model.NewPhone))
                {
                    result.Error = 1;
                    result.Detail = "手机号已被其他账号绑定，请解绑后再操作。";
                    return Ok(result);
                }

                if (RedisHelper.GetValueFromMyHash(Operation.ChangePhone, model.Phone) == model.Code && RedisHelper.GetValueFromMyHash(Operation.ChangePhone, model.NewPhone) == model.NewCode)
                {
                    user.Phone = model.NewPhone;
                    UOW.UserRepository.Update(user);
                    if (await UOW.SaveChangesAsync() > 0)
                    {
                        result.Error = 0;
                        result.Detail = "修改成功";
                        LogHelper.Info(string.Format("User {0} change phone. IP address is {1}.", user.Name, ip));
                        return Ok(result);
                    }
                }
                else
                {
                    result.Error = 1;
                    result.Detail = "验证码错误";
                }
            }
            return Ok(result);
        }

        //在首页获取当前用户名
        [HttpGet]
        public IHttpActionResult GetUserName()
        {
            Result result = new Result();
            result.Error = 0;
            result.Detail = User.Identity.Name;
            return Ok(result);
        }

        //获取用户的手机，邮箱等信息
        [HttpGet]
        public async Task<IHttpActionResult> GetUserInfo()
        {
            UserInfo info = new UserInfo();
            string username = User.Identity.Name;
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == username);
            if (user != null)
            {
                info.Error = 0;
                info.Detail = "获取成功";
                info.Name = user.Name;
                if (!string.IsNullOrEmpty(user.Phone))
                {
                    info.Phone = user.Phone.Substring(0, 3) + "*****" + user.Phone.Substring(8);
                }
                if (!string.IsNullOrEmpty(user.Email))
                {
                    info.Email = user.Email.Substring(0, 3) + "*****" + user.Email.Substring(user.Email.IndexOf("@") + 1);
                }
            }
            return Ok(info);
        }

        //修改密码
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(string password, string newPassword)
        {
            string ip = GetIPAddress();
            Result result = new Result();
            result.Error = 1;
            result.Detail = "修改失败，请稍后再试";
            string username = User.Identity.Name;
            var user = await UOW.UserRepository.GetSingleAsync(u => u.Name == username);
            if (user != null)
            {
                string pwdMD5 = MD5Helper.GetStrMD5(password);
                if (pwdMD5 != user.Password)
                {
                    result.Error = 1;
                    result.Detail = "原始密码错误";
                }
                else
                {
                    user.Password = MD5Helper.GetStrMD5(newPassword);
                    UOW.UserRepository.Update(user);
                    if (await UOW.SaveChangesAsync() > 0)
                    {
                        result.Error = 0;
                        result.Detail = "修改成功";
                        LogHelper.Info(string.Format("User {0} change password. IP address is {1}.", user.Name, ip));
                        return Ok(result);
                    }
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// 检查email是否存在
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool IsExistEmail(string email)
        {
            var user = UOW.UserRepository.GetSingle(u => u.Email == email.Trim().ToLower());
            return user == null ? false : true;
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool IsExistUser(string userName)
        {
            var user = UOW.UserRepository.GetSingle(u => u.Name == userName.Trim());
            return user == null ? false : true;
        }

        /// <summary>
        /// 检查手机号是否存在
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private bool IsExistPhone(string phone)
        {
            var user = UOW.UserRepository.GetSingle(u => u.Phone == phone);
            return user == null ? false : true;
        }

        /// <summary>
        /// 生成六位数字验证码
        /// </summary>
        /// <returns></returns>
        private string GenerateAuthenticode()
        {
            return new Random().Next(100000, 1000000).ToString();
        }

        /// <summary>
        /// 获取客户端的IP地址
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            return Request.GetOwinContext().Request.RemoteIpAddress;
        }

    }
}
