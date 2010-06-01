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
    [Storable]
    private string ipAddress;
    [Storable]
    private int port;
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
      if (socket.Connected)
        base.Open();
    }

    public override void Send(IMessage message) {
      int size = message.SerializedSize;
      socket.Send(System.BitConverter.GetBytes(size));
      socket.Send(message.ToByteArray());
    }

    public override IMessage Receive(IBuilder builder) {
      byte[] sizeBuffer = new byte[4];
      socket.Receive(sizeBuffer);
      int size = System.BitConverter.ToInt32(sizeBuffer, 0);
      byte[] messageBuffer = new byte[size];
      int offset = 0;
      while (offset < size)
        offset += socket.Receive(messageBuffer, offset, size, SocketFlags.None);
      return builder.WeakMergeFrom(ByteString.CopyFrom(messageBuffer)).WeakBuild();
    }

    public override void Close() {
      socket.Disconnect(false);
      socket.Close();
      base.Close();
    }

    #endregion
  }
}
