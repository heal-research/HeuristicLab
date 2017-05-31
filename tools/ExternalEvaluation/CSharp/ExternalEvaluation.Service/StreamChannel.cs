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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Google.ProtocolBuffers;

namespace HeuristicLab.Problems.ExternalEvaluation.Service {
  public class StreamChannel : Channel {
    private object inputLock = new object();
    private Stream input;
    private object outputLock = new object();
    private Stream output;

    public StreamChannel(Stream input, Stream output)
      : base() {
      this.input = input;
      this.output = output;
    }

    public override void Open() {
      base.Open();
    }

    public override void Send(IMessage msg) {
      lock (outputLock) {
        msg.WriteDelimitedTo(output);
        output.Flush(); // very important!
      }
    }

    public override IMessage Receive(IBuilder builder) {
      lock (inputLock) {
        // hacky, there doesn't seem to be an IBuilder.WeakMergeDelimitedFrom
        CodedInputStream cIn = CodedInputStream.CreateInstance(input);
        uint length = cIn.ReadRawVarint32();
        byte[] message = cIn.ReadRawBytes((int)length);
        builder.WeakMergeFrom(ByteString.CopyFrom(message));
        if (builder.IsInitialized)
          return builder.WeakBuild();
        else throw new EndOfStreamException("EOF reached, but message is incomplete.");
      }
    }

    public override void Close() {
      base.Close();
      input.Close();
      output.Close();
    }
  }
}
