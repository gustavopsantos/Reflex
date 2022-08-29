/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Unity.VisualStudio.Editor.Messaging
{
	internal class Messager : IDisposable
	{
		public event EventHandler<MessageEventArgs> ReceiveMessage;
		public event EventHandler<ExceptionEventArgs> MessagerException;

		private readonly UdpSocket _socket;
		private readonly object _disposeLock = new object();
		private bool _disposed;

#if UNITY_EDITOR_WIN
		[System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetHandleInformation(IntPtr hObject, HandleFlags dwMask, HandleFlags dwFlags);

		[Flags]
		private enum HandleFlags: uint
		{
			None = 0,
			Inherit = 1,
			ProtectFromClose = 2
		}
#endif

		protected Messager(int port)
		{
			_socket = new UdpSocket();
			_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);
			_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

#if UNITY_EDITOR_WIN
			// Explicitely disable inheritance for our UDP socket handle 
			// We found that Unity is creating a fork when importing new assets that can clone our socket
			SetHandleInformation(_socket.Handle, HandleFlags.Inherit, HandleFlags.None);
#endif

			_socket.Bind(IPAddress.Any, port);

			BeginReceiveMessage();
		}

		private void BeginReceiveMessage()
		{
			var buffer = new byte[UdpSocket.BufferSize];
			var any = UdpSocket.Any();

			try
			{
				lock (_disposeLock)
				{
					if (_disposed)
						return;

					_socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref any, ReceiveMessageCallback, buffer);
				}
			}
			catch (SocketException se)
			{
				MessagerException?.Invoke(this, new ExceptionEventArgs(se));

				BeginReceiveMessage();
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private void ReceiveMessageCallback(IAsyncResult result)
		{
			try
			{
				var endPoint = UdpSocket.Any();

				lock (_disposeLock)
				{
					if (_disposed)
						return;

					_socket.EndReceiveFrom(result, ref endPoint);
				}

				var message = DeserializeMessage(UdpSocket.BufferFor(result));
				if (message != null)
				{
					message.Origin = (IPEndPoint)endPoint;

					if (IsValidTcpMessage(message, out var port, out var bufferSize))
					{
						// switch to TCP mode to handle big messages
						TcpClient.Queue(message.Origin.Address, port, bufferSize, buffer =>
						{
							var originalMessage = DeserializeMessage(buffer);
							originalMessage.Origin = message.Origin;
							ReceiveMessage?.Invoke(this, new MessageEventArgs(originalMessage));
						});
					}
					else
					{
						ReceiveMessage?.Invoke(this, new MessageEventArgs(message));
					}
				}
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			catch (Exception e)
			{
				RaiseMessagerException(e);
			}

			BeginReceiveMessage();
		}

		private static bool IsValidTcpMessage(Message message, out int port, out int bufferSize)
		{
			port = 0;
			bufferSize = 0;
			if (message.Value == null)
				return false;
			if (message.Type != MessageType.Tcp)
				return false;
			var parts = message.Value.Split(':');
			if (parts.Length != 2)
				return false;
			if (!int.TryParse(parts[0], out port))
				return false;
			return int.TryParse(parts[1], out bufferSize);
		}

		private void RaiseMessagerException(Exception e)
		{
			MessagerException?.Invoke(this, new ExceptionEventArgs(e));
		}

		private static Message MessageFor(MessageType type, string value)
		{
			return new Message { Type = type, Value = value };
		}

		public void SendMessage(IPEndPoint target, MessageType type, string value = "")
		{
			var message = MessageFor(type, value);
			var buffer = SerializeMessage(message);

			try
			{
				lock (_disposeLock)
				{
					if (_disposed)
						return;

					if (buffer.Length >= UdpSocket.BufferSize)
					{
						// switch to TCP mode to handle big messages
						var port = TcpListener.Queue(buffer);
						if (port > 0)
						{
							// success, replace original message with "switch to tcp" marker + port information + buffer length
							message = MessageFor(MessageType.Tcp, string.Concat(port, ':', buffer.Length));
							buffer = SerializeMessage(message);
						}
					}

					_socket.BeginSendTo(buffer, 0, Math.Min(buffer.Length, UdpSocket.BufferSize), SocketFlags.None, target, SendMessageCallback, null);
				}
			}
			catch (SocketException se)
			{
				MessagerException?.Invoke(this, new ExceptionEventArgs(se));
			}
		}

		private void SendMessageCallback(IAsyncResult result)
		{
			try
			{
				lock (_disposeLock)
				{
					if (_disposed)
						return;

					_socket.EndSendTo(result);
				}
			}
			catch (SocketException se)
			{
				MessagerException?.Invoke(this, new ExceptionEventArgs(se));
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private static byte[] SerializeMessage(Message message)
		{
			var serializer = new Serializer();
			serializer.WriteInt32((int)message.Type);
			serializer.WriteString(message.Value);

			return serializer.Buffer();
		}

		private static Message DeserializeMessage(byte[] buffer)
		{
			if (buffer.Length < 4)
				return null;

			var deserializer = new Deserializer(buffer);
			var type = (MessageType)deserializer.ReadInt32();
			var value = deserializer.ReadString();

			return new Message { Type = type, Value = value };
		}

		public static Messager BindTo(int port)
		{
			return new Messager(port);
		}

		public void Dispose()
		{
			lock (_disposeLock)
			{
				_disposed = true;
				_socket.Close();
			}
		}
	}
}
