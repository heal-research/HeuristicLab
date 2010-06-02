#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Net;
using System.Net.Sockets;
using Google.ProtocolBuffers;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("EvaluationTCPChannel", "A channel that creates a TCP connection over a network.")]
  [StorableClass]
  public class EvaluationTCPChannel : EvaluationChannel {
    public const int MAX_VARINT32_SIZE = 5;

    [Storable]
    private string ipAddress;
    public string IpAddress {
      get { return ipAddress; }
      set {
        bool changed = !ipAddress.Equals(value);
        ipAddress = value;
        if (changed)
          OnIpAddressChanged();
      }
    }
    [Storable]
    private int port;
    public int Port {
      get { return port; }
      set {
        bool changed = port != value;
        port = value;
        if (changed)
          OnPortChanged();
      }
    }
    private Socket socket;

    public EvaluationTCPChannel() : this(String.Empty, 0) { }
    public EvaluationTCPChannel(string ip, int port)
      : base() {
      this.ipAddress = ip;
      this.port = port;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      EvaluationTCPChannel clone = (EvaluationTCPChannel)base.Clone(cloner);
      clone.ipAddress = ipAddress;
      clone.port = port;
      return clone;
    }

    #region IExternalEvaluationChannel Members

    public override void Open() {
      socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.Connect(IPAddress.Parse(ipAddress), port);
      if (socket.Connected) {
        base.Open();
        OnConnected();
      }
    }

    public override void Send(IMessage message) {
      try {
        byte[] buffer = EncodeDelimited(message);
        socket.Send(buffer);
      } catch (SocketException) {
        Close();
      } catch (ObjectDisposedException) {
        socket = null;
        Close();
      }
    }

    private byte[] EncodeDelimited(IMessage message) {
      int messageSize = message.SerializedSize;
      int headerSize = GetVarint32EncodedSize(messageSize);
      byte[] buffer = new byte[headerSize + messageSize];
      CodedOutputStream tmp = CodedOutputStream.CreateInstance(buffer);
      tmp.WriteRawVarint32((uint)messageSize);
      message.WriteTo(tmp);
      return buffer;
    }

    private int GetVarint32EncodedSize(int size) {
      // Varints in Protocol Buffers are encoded using the 7 lower order bits (the MSB indicates continuation (=1) or termination (=0))
      int sizeByteCount = 1;
      int limit = 128;
      while (size > limit) {
        sizeByteCount++;
        limit *= 128;
      }
      return sizeByteCount;
    }

    public override IMessage Receive(IBuilder builder) {
      try {
        byte[] buffer = GetMessageBuffer();
        return builder.WeakMergeFrom(ByteString.CopyFrom(buffer)).WeakBuild();
      } catch (SocketException) {
        Close();
      } catch (ObjectDisposedException) {
        socket = null;
        Close();
      }
      return null;
    }

    private byte[] GetMessageBuffer() {
      byte[] buffer;
      int messageSize = GetReceivedMessageSize(out buffer);
      int headerSize = GetVarint32EncodedSize(messageSize);
      int messageBytesInHeader = Math.Min(messageSize, buffer.Length - headerSize);
      byte[] messageBuffer = new byte[messageSize];
      Array.Copy(buffer, headerSize, messageBuffer, 0, messageBytesInHeader);
      ReceiveMessage(messageBuffer, messageBytesInHeader, messageSize - messageBytesInHeader);
      return messageBuffer;
    }

    private int GetReceivedMessageSize(out byte[] buffer) {
      buffer = new byte[MAX_VARINT32_SIZE];
      socket.Receive(buffer);
      CodedInputStream tmp = CodedInputStream.CreateInstance(buffer);
      return (int)tmp.ReadRawVarint32();
    }

    private void ReceiveMessage(byte[] buffer, int offset, int size) {
      while (size > 0) {
        int received = socket.Receive(buffer, offset, size, SocketFlags.None);
        offset += received;
        size -= received;
      }
    }

    public override void Close() {
      if (socket != null) {
        if (socket.Connected)
          socket.Disconnect(false);
        socket.Close();
        socket = null;
      }
      bool wasInitialized = IsInitialized;
      base.Close();
      if (wasInitialized) OnDiconnected();
    }

    #endregion

    #region Events
    public event EventHandler IpAddressChanged;
    protected void OnIpAddressChanged() {
      EventHandler handler = IpAddressChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler PortChanged;
    protected void OnPortChanged() {
      EventHandler handler = PortChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Connected;
    protected void OnConnected() {
      EventHandler handler = Connected;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Disconnected;
    protected void OnDiconnected() {
      EventHandler handler = Disconnected;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
