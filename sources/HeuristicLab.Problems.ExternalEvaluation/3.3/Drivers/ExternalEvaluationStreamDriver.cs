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

    private CodedInputStream inputStream;
    private Stream input;
    private CodedOutputStream outputStream;
    private Stream output;

    public ExternalEvaluationStreamDriver() : base() { }
    public ExternalEvaluationStreamDriver(Stream input, Stream output)
      : base() {
      if (!input.CanRead) throw new ArgumentException("Input stream cannot be read", "input");
      this.inputStream = CodedInputStream.CreateInstance(input);
      this.input = input;
      if (!output.CanWrite) throw new ArgumentException("Output stream cannot be written", "output");
      this.outputStream = CodedOutputStream.CreateInstance(output);
      this.output = output;
    }

    #region Overrides
    public override QualityMessage Evaluate(SolutionMessage solution) {
      solution.WriteTo(outputStream);
      outputStream.Flush();
      output.Flush();
      QualityMessage message = QualityMessage.ParseFrom(inputStream);
      return message;
    }

    public override void EvaluateAsync(SolutionMessage solution, Action<QualityMessage> callback) {
      solution.WriteTo(outputStream);
      outputStream.Flush();
      output.Flush();
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReceiveAsync), callback);
    }

    public override void Stop() {
      base.Stop();
      inputStream = null;
      input.Close();
      outputStream = null;
      output.Close();
    }
    #endregion

    private void ReceiveAsync(object callback) {
      QualityMessage message;
      lock (inputStream) { // only one thread can read from the stream at one time
        message = QualityMessage.ParseFrom(inputStream);
      }
      ((Action<QualityMessage>)callback).Invoke(message);
    }
  }
}
