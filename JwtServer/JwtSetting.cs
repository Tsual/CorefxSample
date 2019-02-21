using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtServer
{
    public class JwtSetting
    {
        //token是谁颁发的
        public static string Issuer { get; set; } = "sf.Tsual";
        //token可以给哪些客户端使用
        public static string Audience { get; set; } = "sf.Tsual";
        //加密的key
        public static SecurityKey SecretKey { get; set; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890sdfasgasd"));
    }
}
