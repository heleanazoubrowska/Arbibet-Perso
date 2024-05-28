using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArbibetProgram.SignalR
{
	public class SignalR_Client
	{
		HubConnection prv_Connection;
		IHubProxy prv_HubProxy;

		public void Connect()
		{
			prv_Connection = new HubConnection("http://localhost:8281/");
			prv_HubProxy = prv_Connection.CreateHubProxy("ArbiBetHub");

			// --------------------------------------------------
			# region server-invokable methods
			// --------------------------------------------------
			prv_HubProxy.On<string>("TestFeedback", prm_Feedback =>
			{
				NLogger.Log(EventLevel.Alert, prm_Feedback);
			});

			//prv_HubProxy.On<Cambo88BTIAccount>("TestAccount", prm_Account =>
			//{
			//	// do something with prm_Account
			//});

			// --------------------------------------------------
			#endregion server-invokable methods
			// --------------------------------------------------

			// --------------------------------------------------
			// open the connection
			// --------------------------------------------------
			prv_Connection.Start().ContinueWith(prv_Task => 
			{
				if (prv_Task.IsFaulted)
				{
					NLogger.Log(EventLevel.Error, $"Error opening SignalR connection: {prv_Task.Exception.GetBaseException()}");
				}
				else
				{
					NLogger.Log(EventLevel.Info, "Connected to ArbiBetHub");
				}
			}).Wait();
		}

		public void Test()
		{
			prv_HubProxy.Invoke<string>("TestSignalR", "Hello World").ContinueWith(prv_Task => 
			{
				if (prv_Task.IsFaulted)
				{
					NLogger.Log(EventLevel.Error, $"Error invoking TestSignalR: {prv_Task.Exception.GetBaseException()}");
				}
				else
				{
					NLogger.Log(EventLevel.Info, $"Invoke TestSignalR result: {prv_Task.Result}");
				}
			});
		}

		public static void SetupSignalR()
		{
			// ----------------------------------------------------------------------
			// start SignalR server
			// * = bind to all addresses
			// when server is remote run the command below on cmd administrator
			// netsh http add urlacl url=http://*:6969/ user=Everyone
			// netsh http add urlacl url=http://localhost:6969/ user=Everyone
			// netsh http add urlacl url=http://192.168.0.5:6969/ user=Everyone
			// netsh http add urlacl url=https://*:6969/ user=Everyone
			// netsh http add urlacl url=https://localhost:6969/ user=Everyone
			// netsh http add urlacl url=https://192.168.0.5:6969/ user=Everyone
			// ----------------------------------------------------------------------
			//var prv_SignalR = WebApp.Start("http://localhost:6969");
			//var prv_SignalR = WebApp.Start("http://192.168.0.5:6969");
			//var prv_SignalR = WebApp.Start("http://*:6969");
			//Console.ReadLine();

			// ----------------------------------------------------------------------
			// test a SignalR client
			// ----------------------------------------------------------------------
			//var prv_SignalR_Client = new SignalR_Client();
			//prv_SignalR_Client.Connect();
			//prv_SignalR_Client.Test();
			//Console.ReadLine();
		}
	}
}
