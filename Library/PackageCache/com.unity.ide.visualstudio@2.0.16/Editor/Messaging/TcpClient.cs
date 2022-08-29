/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal class TcpClient
	{
		private const int ConnectOrReadTimeoutMilliseconds = 5000;

		private class State
		{
			public System.Net.Sockets.TcpClient TcpClient;
			public NetworkStream NetworkStream;
			public byte[] Buffer;
			public Action<byte[]> OnBufferAvailable;
		}

		public static void Queue(IPAddress address, int port, int bufferSize, Action<byte[]> onBufferAvailable)
		{
			var client = new System.Net.Sockets.TcpClient();
			var state = new State {OnBufferAvailable = onBufferAvailable, TcpClient = client, Buffer = new byte[bufferSize]};

			try
			{
				ThreadPool.QueueUserWorkItem(_ =>
				{
					var handle = client.BeginConnect(address, port, OnClientConnected, state);
					if (!handle.AsyncWaitHandle.WaitOne(ConnectOrReadTimeoutMilliseconds))
						Cleanup(state);
				});
			}
			catch (Exception)
			{
				Cleanup(state);
			}
		}

		private static void OnClientConnected(IAsyncResult result)
		{
			var state = (State)result.AsyncState;

			try
			{
				state.TcpClient.EndConnect(result);
				state.NetworkStream = state.TcpClient.GetStream();
				var handle = state.NetworkStream.BeginRead(state.Buffer, 0, state.Buffer.Length, OnEndRead, state);
				if (!handle.AsyncWaitHandle.WaitOne(ConnectOrReadTimeoutMilliseconds))
					Cleanup(state);
			}
			catch (Exception)
			{
				Cleanup(state);
			}
		}

		private static void OnEndRead(IAsyncResult result)
		{
			var state = (State)result.AsyncState;

			try
			{
				var count = state.NetworkStream.EndRead(result);
				if (count == state.Buffer.Length)
					state.OnBufferAvailable?.Invoke(state.Buffer);
			}
			catch (Exception)
			{
				// Ignore and cleanup
			}
			finally
			{
				Cleanup(state);
			}
		}

		private static void Cleanup(State state)
		{
			state.NetworkStream?.Dispose();
			state.TcpClient?.Close();

			state.NetworkStream = null;
			state.TcpClient = null;
			state.Buffer = null;
			state.OnBufferAvailable = null;
		}
	}
}
