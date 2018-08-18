using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eyu.Tieba.WebAPI.Models
{
    public class RsaKey
    {
        [JsonProperty("errno")]
        public string Errno { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("pubkey")]
        public string Pubkey { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}