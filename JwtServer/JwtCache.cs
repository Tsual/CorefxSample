using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtServer
{
    public class JwtCache
    {
        private static Dictionary<string, DateTime> mem_cache = new Dictionary<string, DateTime>();

        public static bool CheckJwtInCache(string Uid)
        {
            return mem_cache.ContainsKey(Uid) && mem_cache[Uid] > DateTime.Now;
        }

        public static bool TryPushUid(string Uid, DateTime ExpireTime)
        {
            return ExpireTime != null && ExpireTime > DateTime.Now && mem_cache.TryAdd(Uid, ExpireTime);
        }
    }
}
