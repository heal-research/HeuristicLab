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
using System.Diagnostics;
using System.IO;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ExternalEvaluationProcessDriver", "A driver for external evaluation problems that launches the external application in a new process.")]
  [StorableClass]
  public class ExternalEvaluationProcessDriver : ExternalEvaluationDriver {
    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    private Process process;
    [Storable]
    private string executable;
    public string Executable {
      get { return executable; }
      set {
        if (IsInitialized) throw new InvalidOperationException("ExternalEvaluationProcessDriver cannot change the executable path as it has already been started.");
        string oldExecutable = executable;
        executable = value;
        if (!oldExecutable.Equals(executable)) OnExecutableChanged();
      }
    }
    [Storable]
    private string arguments;
    public string Arguments {
      get { return arguments; }
      set {
        if (IsInitialized) throw new InvalidOperationException("ExternalEvaluationProcessDriver cannot change the arguments as it has already been started.");
        string oldArguments = arguments;
        arguments = value;
        if (!oldArguments.Equals(arguments)) OnArgumentsChanged();
      }
    }
    private ExternalEvaluationStreamDriver driver;

    public ExternalEvaluationProcessDriver() : this(String.Empty, String.Empty) { }
    public ExternalEvaluationProcessDriver(string executable, string arguments)
      : base() {
      this.executable = executable;
      this.arguments = arguments;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ExternalEvaluationProcessDriver clone = (ExternalEvaluationProcessDriver)base.Clone(cloner);
      clone.executable = executable;
      clone.arguments = arguments;
      return clone;
    }

    #region IExternalDriver Members

    public override void Start() {
      if (!String.IsNullOrEmpty(executable.Trim())) {
        base.Start();
        process = new Process();
        process.StartInfo = new ProcessStartInfo(executable, arguments);
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.EnableRaisingEvents = true; // required to be notified of exit
        process.Start();
        Stream processStdOut = process.StandardOutput.BaseStream;
        Stream processStdIn = process.StandardInput.BaseStream;
        OnProcessStarted();
        process.Exited += new EventHandler(process_Exited);
        driver = new ExternalEvaluationStreamDriver(processStdOut, processStdIn);
        driver.Start();
      } else throw new InvalidOperationException("Cannot start ExternalEvaluationProcessDriver because executable is not defined.");
    }

    public override QualityMessage Evaluate(SolutionMessage solution) {
      return driver.Evaluate(solution);
    }

    public override void EvaluateAsync(SolutionMessage solution, Action<QualityMessage> callback) {
      driver.EvaluateAsync(solution, callback);
    }

    public override void Stop() {
      base.Stop();
      if (process != null) {
        if (!process.HasExited) {
          driver.Stop();
          if (!process.HasExited) {
            process.CloseMainWindow();
            process.WaitForExit(1000);
            process.Close();
            // for some reasons the event process_Exited does not fire
            OnProcessExited();
          }
        }
        process = null;
      }
    }

    #endregion

    #region Event handlers (process)
    private void process_Exited(object sender, EventArgs e) {
      if (IsInitialized) {
        if (driver.IsInitialized) driver.Stop();
        IsInitialized = false;
        process = null;
      }
      OnProcessExited();
    }
    #endregion

    #region Events
    public event EventHandler ExecutableChanged;
    protected void OnExecutableChanged() {
      EventHandler handler = ExecutableChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ArgumentsChanged;
    protected void OnArgumentsChanged() {
      EventHandler handler = ArgumentsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ProcessStarted;
    private void OnProcessStarted() {
      EventHandler handler = ProcessStarted;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ProcessExited;
    private void OnProcessExited() {
      EventHandler handler = ProcessExited;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
