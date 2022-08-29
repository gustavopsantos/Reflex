/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
 using System;
using System.Net;
using System.Threading;

namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal class TcpListener
	{
		private const int ListenTimeoutMilliseconds = 5000;

		private class State
		{
			public System.Net.Sockets.TcpListener TcpListener;
			public byte[] Buffer;
		}

		public static int Queue(byte[] buffer)
		{
			var tcpListener = new System.Net.Sockets.TcpListener(IPAddress.Any, 0);
			var state = new State {Buffer = buffer, TcpListener = tcpListener};

			try
			{
				tcpListener.Start();

				int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;

				ThreadPool.QueueUserWorkItem(_ =>
				{
					bool listening = true;
					
					while (listening)
					{
						var handle = tcpListener.BeginAcceptTcpClient(OnIncomingConnection, state);
						listening = handle.AsyncWaitHandle.WaitOne(ListenTimeoutMilliseconds);
					}
					
					Cleanup(state);
				});

				return port;
			}
			catch (Exception)
			{
				Cleanup(state);
				return -1;
			}
		}

		private static void OnIncomingConnection(IAsyncResult result)
		{
			var state = (State)result.AsyncState;

			try
			{
				using (var client = state.TcpListener.EndAcceptTcpClient(result))
				{
					using (var stream = client.GetStream())
					{
						stream.Write(state.Buffer, 0, state.Buffer.Length);
					}
				}
			}
			catch (Exception)
			{
				// Ignore and cleanup
			}
		}

		private static void Cleanup(State state)
		{
			state.TcpListener?.Stop();

			state.TcpListener = null;
			state.Buffer = null;
		}
	}
}
