using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebSocket
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IHostingEnvironment env, IConfiguration config,
            ILoggerFactory loggerFactory)
        {
            _env = env;
            _config = config;
            _loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            

            app.UseWebSockets()
            .Use(async (ctx, next) =>
            {
                if (ctx.Request.Path == "/ws")
                {
                    if (ctx.WebSockets.IsWebSocketRequest)
                    {
                        var socket = await ctx.WebSockets.AcceptWebSocketAsync();
                        await wsp.WsP2.DoProcess(ctx, socket);
                    }
                    else
                    {
                        ctx.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            })
            .UseFileServer();
        }
    }
}
