using Websocket.Server.Configuration;
using Websocket.Server.Handlers;
using Websocket.Server.Messaging;

namespace Websocket.Server
{
    public class Startup
    {
        private readonly MessageBroadcaster messageBroadcaster;

        public Startup()
        {
            messageBroadcaster = new MessageBroadcaster();
            // messageBroadcaster.StartSending(5); // Server starts broadcasting messages to all connected cliens every n seconds.
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            WebSocketConfigurator.Configure(app);
            WebSocketEndpointHandler.Configure(app);
        }
    }
}