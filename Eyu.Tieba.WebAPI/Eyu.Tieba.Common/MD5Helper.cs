using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.Common
{
    public class MD5Helper
    {
        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStrMD5(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] b = Encoding.Default.GetBytes(str);
            byte[] result = md5.ComputeHash(b);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
