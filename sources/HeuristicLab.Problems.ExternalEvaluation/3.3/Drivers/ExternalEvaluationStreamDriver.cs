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
using System.IO;
using Google.ProtocolBuffers;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ExternalEvaluationStreamDriver", "A driver for external evaluation problems that communicates via an input and an output stream.")]
  [StorableClass]
  public class ExternalEvaluationStreamDriver : ExternalEvaluationDriver {
    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    private Stream input;
    private Stream output;

    public ExternalEvaluationStreamDriver() : base() { }
    public ExternalEvaluationStreamDriver(Stream input, Stream output)
      : base() {
      if (!input.CanRead) throw new ArgumentException("Input stream cannot be read", "input");
      this.input = input;
      if (!output.CanWrite) throw new ArgumentException("Output stream cannot be written", "output");
      this.output = output;
    }

    #region Overrides
    public override QualityMessage Evaluate(SolutionMessage solution) {
      Send(solution);
      QualityMessage message = QualityMessage.ParseDelimitedFrom(input);
      return message;
    }

    public override void EvaluateAsync(SolutionMessage solution, Action<QualityMessage> callback) {
      Send(solution);
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveAsync), callback);
    }

    private void Send(SolutionMessage solution) {
      lock (output) {
        solution.WriteDelimitedTo(output);
        output.Flush();
      }
    }

    public override void Stop() {
      base.Stop();
      input.Close();
      output.Close();
    }
    #endregion

    private void ReceiveAsync(object callback) {
      QualityMessage message;
      lock (input) { // only one thread can read from the stream at one time
        message = QualityMessage.ParseDelimitedFrom(input);
      }
      ((Action<QualityMessage>)callback).Invoke(message);
    }
  }
}
