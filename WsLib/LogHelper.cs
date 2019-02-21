using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace log4net
{
    public class LogHelper
    {
        public static ILoggerRepository Repository { get; private set; }

        static LogHelper()
        {
            Repository = LogManager.CreateRepository("ws-lib");
            XmlConfigurator.Configure(Repository, new FileInfo("log4net.cfg.xml"));
        }

        public static ILog GetLog(Type type)
        {
            return LogManager.GetLogger(Repository.Name, type);
        }
    }
}
