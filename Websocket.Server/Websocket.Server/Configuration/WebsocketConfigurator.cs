﻿using Websocket.Configuration;

namespace Websocket.Server.Configuration
{
    internal class WebSocketConfigurator
    {
        public static void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            var wsOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(WebSocketConfiguration.KeepAliveIntervalSeconds)
            };

            app.UseWebSockets(wsOptions);
        }
    }
}
