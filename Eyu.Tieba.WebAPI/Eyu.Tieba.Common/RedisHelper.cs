using Microsoft.Win32;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.Common
{
    public class RedisHelper : IDisposable
    {
        private static RedisClient Redis;

        static RedisHelper()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("TIEBA").OpenSubKey("REDIS", false);
                string host = key.GetValue("host").ToString();
                int port = int.Parse(key.GetValue("port").ToString());
                string pwd = key.GetValue("pwd").ToString();
                key.Close();
                Redis = new RedisClient(host, port, pwd);
            }
            catch (Exception ex)
            {
                LogHelper.Error("Failed to read redis connection string in registry. " + ex.ToString());
                throw;
            }
        }

        /// <summary>  
        /// 设置缓存  
        /// </summary>  
        /// <typeparam name="T">值类型</typeparam>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <param name="timeout">过期时间，单位秒。小于0表示删除。</param>  
        /// <returns></returns>  
        public static bool Set<T>(string key, T value, int timeout)
        {
            if (timeout >= 0)
            {
                Redis.Expire(key, timeout);
                return Redis.Set<T>(key, value);
            }
            return Redis.Remove(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间。小于DateTime.Now表示删除</param>
        /// <returns></returns>
        public static bool Set<T>(string key, T value, DateTime expiry)
        {
            if (expiry <= DateTime.Now)
            {
                return Redis.Remove(key);
            }
            return Redis.Set<T>(key, value, expiry - DateTime.Now);
        }

        /// <summary>  
        /// 获取缓存值
        /// </summary>  
        /// <typeparam name="T">值类型</typeparam>  
        /// <param name="key">键</param>  
        /// <returns>值</returns>  
        public static T Get<T>(string key)
        {
            return Redis.Get<T>(key);
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string Get(string key)
        {
            return Redis.Get<string>(key);
        }

        /// <summary>  
        /// 删除缓存
        /// </summary>  
        /// <param name="key">键</param>  
        /// <returns></returns>  
        public static bool Remove(string key)
        {
            return Redis.Remove(key);
        }

        /// <summary>
        /// 模拟Redis Hash设置缓存
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="type">业务类型</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeout">过期时间</param>
        /// <returns></returns>
        public static bool SetEntryInMyHash<T>(Operation type, string key, T value, int timeout)
        {
            return Set<T>(type + key, value, timeout);
        }

        /// <summary>
        /// 模拟Redis Hash设置缓存
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="type">业务类型</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public static bool SetEntryInMyHash<T>(Operation type, string key, T value, DateTime expiry)
        {
            return Set<T>(type + key, value, expiry);
        }

        /// <summary>
        /// 从模拟的Hash链表中获取缓存值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="type">业务类型</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T GetValueFromMyHash<T>(Operation type, string key)
        {
            return Get<T>(type + key);
        }

        /// <summary>
        /// 从模拟的Hash链表中获取缓存值
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetValueFromMyHash(Operation type, string key)
        {
            return Get(type + key);
        }

        /// <summary>
        /// 从模拟的Hash链表中删除缓存
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool RemoveEntryFromMyHash(Operation type, string key)
        {
            return Remove(type + key);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Redis != null)
            {
                Redis.Dispose();
                Redis = null;
            }
            GC.Collect();
        }
    }
    public enum Operation
    {
        Register,
        Login,
        FindPassword,
        ChangePhone
    }
}