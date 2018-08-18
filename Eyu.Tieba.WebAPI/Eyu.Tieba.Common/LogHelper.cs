using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eyu.Tieba.Common
{
    public class LogHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger(); //初始化日志类

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Debug(string msg)
        {
            logger.Debug(msg);
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Error(string msg)
        {
            logger.Error(msg);
        }

        /// <summary>
        /// 严重致命错误日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Fatal(string msg)
        {
            logger.Fatal(msg);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Warn(string msg)
        {
            logger.Warn(msg);
        }

        /// <summary>
        /// 追踪日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Trace(string msg)
        {
            logger.Trace(msg);
        }
    }
}
