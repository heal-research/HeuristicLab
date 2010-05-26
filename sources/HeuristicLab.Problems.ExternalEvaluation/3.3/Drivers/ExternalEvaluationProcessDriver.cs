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
using System.Text;
using System.Diagnostics;
using System.IO;
using Google.ProtocolBuffers;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ExternalEvaluationProcessDriver", "A driver for external evaluation problems that launches the external application in a new process.")]
  [StorableClass]
  public class LocalProcessDriver : ExternalEvaluationDriver {
    private Process process;
    [Storable]
    private string executable;
    [Storable]
    private string arguments;
    private ExternalEvaluationStreamDriver driver;

    public LocalProcessDriver(string executable, string arguments)
      : base() {
      this.executable = executable;
      this.arguments = arguments;
    }

    #region IExternalDriver Members

    public override void Start() {
      base.Start();
      process = new Process();
      process.StartInfo = new ProcessStartInfo(executable, arguments);
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.RedirectStandardInput = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.Start();
      Stream processStdOut = process.StandardOutput.BaseStream;
      Stream processStdIn = process.StandardInput.BaseStream;
      driver = new ExternalEvaluationStreamDriver(processStdOut, processStdIn);
      driver.Start();
    }

    public override QualityMessage Evaluate(SolutionMessage solution) {
      return driver.Evaluate(solution);
    }

    public override void EvaluateAsync(SolutionMessage solution, Action<QualityMessage> callback) {
      driver.EvaluateAsync(solution, callback);
    }

    public override void Stop() {
      base.Stop();
      if (!process.HasExited) {
        driver.Stop();
        if (!process.HasExited) {
          process.CloseMainWindow();
          process.WaitForExit(1000);
          process.Close();
        }
      }
      process = null;
    }

    #endregion
  }
}
