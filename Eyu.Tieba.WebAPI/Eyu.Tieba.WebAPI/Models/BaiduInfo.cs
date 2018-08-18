using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class BaiduInfo
    {
        public int Id { get; set; }
        public string BDUSS { get; set; }
        public string STOKEN { get; set; }
        public string PTOKEN { get; set; }
        public string Name { get; set; }
    }
}