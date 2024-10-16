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
using Google.Protobuf;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("EvaluationStreamChannel", "A channel that communicates via an input and an output stream.")]
  [StorableType("15DD5556-3C31-4D18-BE18-D438B45591D8")]
  public class EvaluationStreamChannel : EvaluationChannel {

    private Stream input;
    private Stream output;

    [StorableConstructor]
    protected EvaluationStreamChannel(StorableConstructorFlag _) : base(_) { }
    protected EvaluationStreamChannel(EvaluationStreamChannel original, Cloner cloner) : base(original, cloner) { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvaluationStreamChannel(this, cloner);
    }

    public EvaluationStreamChannel() : base() { }
    public EvaluationStreamChannel(Stream input, Stream output)
      : base() {
      if (!input.CanRead) throw new ArgumentException("Input stream cannot be read", "input");
      this.input = input;
      if (!output.CanWrite) throw new ArgumentException("Output stream cannot be written", "output");
      this.output = output;
    }

    #region IExternalEvaluationChannel Members

    public override void Send(IMessage solution) {
      lock (output) {
        solution.WriteDelimitedTo(output);
        output.Flush();
      }
    }

    public override IMessage Receive(IMessage builder, ExtensionRegistry extensions) {
      QualityMessage message;
      lock (input) { // only one thread can read from the stream at one time
        message = QualityMessage.Parser.WithExtensionRegistry(extensions).ParseDelimitedFrom(input);
      }
      return message;
    }

    public override void Close() {
      base.Close();
      input.Close();
      output.Close();
    }

    #endregion
  }
}
