using ArbibetProgram.Functions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbibetProgram.SignalR
{
    [HubName("ArbiBetHub")]
    public class ArbiBetHub : Hub
	{
		// get the ArbiBetHub context
		IHubContext prv_Context = GlobalHost.ConnectionManager.GetHubContext<ArbiBetHub>();

        public override Task OnConnected()
        {
            string prv_ConnectionID = Context.ConnectionId;
            NLogger.Log(EventLevel.Warn, $"SignalR client connected with ConnectionID {prv_ConnectionID}");

            object prv_Message = new
            {
	            ConnectionID = prv_ConnectionID,
	            Event = "Connection"
            };
            SendMessage(prv_ConnectionID, prv_Message);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool prm_StopCalled)
        {
	        string prv_ConnectionID = Context.ConnectionId;

            if (prm_StopCalled)
            {
                NLogger.Log(EventLevel.Warn, $"SignalR client {prv_ConnectionID} explicitly closed the connection.");
            }
            else
            {
                NLogger.Log(EventLevel.Warn, $"SignalR client {prv_ConnectionID} timed out.");
            }

            return base.OnDisconnected(prm_StopCalled);
        }

        public override Task OnReconnected()
        {
	        string prv_ConnectionID = Context.ConnectionId;


            NLogger.Log(EventLevel.Warn, $"SignalR client {prv_ConnectionID} reconnected.");

            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.

            object prv_Message = new
            {
	            ConnectionID = prv_ConnectionID,
	            Event = "Reconnection"
            };
            SendMessage(prv_ConnectionID, prv_Message);

            return base.OnReconnected();
        }

        /// <summary>
        /// Tests SignalR bidirectional connectivity. The client must have a TestFeedback function defined
        /// </summary>
        /// <param name="prm_SomeText"></param>
        [HubMethodName("TestSignalR")]
        public void TestSignalR(string prm_SomeText)
        {
            // get the client's ConnectionID from the calling context
            string prv_ConnectionID = Context.ConnectionId;

            // comment this out
            NLogger.Log(EventLevel.Emergency, $"SignalR received {prm_SomeText} from {prv_ConnectionID}");

            // invoke TestFeedback function defined on the client
            prv_Context.Clients.Client(prv_ConnectionID).TestFeedback($"SignalR noticed that client {prv_ConnectionID} sent: {prm_SomeText}");
        }

        private void SendMessage(string prm_ConnectionID, object prm_Message)
        {
	        prv_Context.Clients.Client(prm_ConnectionID).onReceivedMessage(prm_Message);
        }
    }
}
