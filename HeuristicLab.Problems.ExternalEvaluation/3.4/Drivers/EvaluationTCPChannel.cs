﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("EvaluationTCPChannel", "A channel that creates a TCP connection over a network.")]
  [StorableType("FECC0F5B-B22A-4117-888D-5B8B84332D24")]
  public class EvaluationTCPChannel : EvaluationChannel {

    public const int MAX_VARINT32_SIZE = 5;

    #region Fields & Properties
    [Storable]
    private string ipAddress;
    public string IpAddress {
      get { return ipAddress; }
      set {
        if (value == ipAddress) return;
        ipAddress = value;
        UpdateName();
        OnIpAddressChanged();
      }
    }
    [Storable]
    private int port;
    public int Port {
      get { return port; }
      set {
        if (value == port) return;
        port = value;
        UpdateName();
        OnPortChanged();
      }
    }
    private Socket socket;
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected EvaluationTCPChannel(StorableConstructorFlag _) : base(_) { }
    protected EvaluationTCPChannel(EvaluationTCPChannel original, Cloner cloner)
      : base(original, cloner) {
      ipAddress = original.ipAddress;
      port = original.port;
      UpdateName();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvaluationTCPChannel(this, cloner);
    }

    public EvaluationTCPChannel() : this(String.Empty, 0) { }
    public EvaluationTCPChannel(string ip, int port)
      : base() {
      this.ipAddress = ip;
      this.port = port;
      UpdateName();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      UpdateName();
    }
    #endregion



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
        throw;
      } catch (ObjectDisposedException) {
        socket = null;
        Close();
        throw;
      }
    }

    private byte[] EncodeDelimited(IMessage message) {
      int messageSize = message.CalculateSize();
      int headerSize = GetVarint32EncodedSize(messageSize);
      byte[] buffer = new byte[headerSize + messageSize];
      CodedOutputStream tmp = new CodedOutputStream(buffer);
      tmp.WriteInt32(messageSize);
      message.WriteTo(tmp);
      return buffer;
    }

    private int GetVarint32EncodedSize(int size) {
      // Varints in Protocol Buffers are encoded using the 7 lower order bits (the MSB indicates continuation (=1) or termination (=0))
      int sizeByteCount = 1;
      int limit = 128;
      while (size >= limit) {
        sizeByteCount++;
        limit *= 128;
      }
      return sizeByteCount;
    }

    public override IMessage Receive(IMessage builder, ExtensionRegistry extensions) {
      try {
        byte[] buffer = GetMessageBuffer();
        using (var stream = new MemoryStream(buffer))
          return QualityMessage.Parser.WithExtensionRegistry(extensions).ParseDelimitedFrom(stream);
      } catch (SocketException) {
        Close();
        throw;
      } catch (ObjectDisposedException) {
        socket = null;
        Close();
        throw;
      }
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
      CodedInputStream tmp = new CodedInputStream(buffer);
      return (int)tmp.ReadInt32();
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
        try {
          if (socket.Connected)
            socket.Disconnect(false);
          socket.Close();
        } catch { }
        socket = null;
      }
      bool wasInitialized = IsInitialized;
      base.Close();
      if (wasInitialized) OnDiconnected();
    }

    #endregion

    #region Auxiliary Methods
    private void UpdateName() {
      name = string.Format("TCPChannel {0}:{1}", ipAddress, port);
      OnNameChanged();
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
