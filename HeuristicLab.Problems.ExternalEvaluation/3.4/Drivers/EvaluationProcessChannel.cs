#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using Google.ProtocolBuffers;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("EvaluationProcessChannel", "A channel that launches an external application in a new process and communicates with that process via stdin and stdout.")]
  [StorableType("ECC07782-60E0-483C-8632-476697A058D2")]
  public class EvaluationProcessChannel : EvaluationChannel {

    #region Fields & Properties
    private Process process;
    [Storable]
    private string executable;
    public string Executable {
      get { return executable; }
      set {
        if (IsInitialized) throw new InvalidOperationException(Name + ": Cannot change the executable path as the process has already been started.");
        if (value == executable) return;
        executable = value;
        UpdateName();
        OnExecutableChanged();
      }
    }
    [Storable]
    private string arguments;
    public string Arguments {
      get { return arguments; }
      set {
        if (IsInitialized) throw new InvalidOperationException(Name + ": Cannot change the arguments as the process has already been started.");
        if (value == arguments) return;
        arguments = value;
        UpdateName();
        OnArgumentsChanged();
      }
    }
    private EvaluationStreamChannel streamingChannel;
    #endregion

    #region Construction & Cloning
    [StorableConstructor]
    protected EvaluationProcessChannel(StorableConstructorFlag _) : base(_) { }
    protected EvaluationProcessChannel(EvaluationProcessChannel original, Cloner cloner)
      : base(original, cloner) {
      executable = original.executable;
      arguments = original.arguments;
      UpdateName();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvaluationProcessChannel(this, cloner);
    }

    public EvaluationProcessChannel() : this(String.Empty, String.Empty) { }
    public EvaluationProcessChannel(string executable, string arguments)
      : base() {
      this.executable = executable;
      this.arguments = arguments;
      UpdateName();
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      UpdateName();
    }
    #endregion

    #region IExternalEvaluationChannel Members
    public override void Open() {
      if (!String.IsNullOrEmpty(executable.Trim())) {
        base.Open();
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
        streamingChannel = new EvaluationStreamChannel(processStdOut, processStdIn);
        streamingChannel.Open();
      } else throw new InvalidOperationException(Name + ": Cannot open the process channel because the executable is not defined.");
    }

    public override void Send(IMessage message) {
      try {
        streamingChannel.Send(message);
      } catch {
        Close();
        throw;
      }
    }

    public override IMessage Receive(IBuilder builder, ExtensionRegistry extensions) {
      try {
        return streamingChannel.Receive(builder, extensions);
      } catch {
        Close();
        throw;
      }
    }

    public override void Close() {
      base.Close();
      if (process != null) {
        if (!process.HasExited) {
          streamingChannel.Close();
          if (!process.HasExited) {
            try {
              process.CloseMainWindow();
              process.WaitForExit(1000);
              process.Close();
            } catch { }
          }
          // for some reasons the event process_Exited does not fire
          OnProcessExited();
        }
        process = null;
      }
    }

    #endregion

    #region Event handlers (process)
    private void process_Exited(object sender, EventArgs e) {
      if (IsInitialized) {
        if (streamingChannel.IsInitialized) streamingChannel.Close();
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

    #region Auxiliary Methods
    private void UpdateName() {
      name = string.Format("ProcessChannel {0} {1}", Path.GetFileNameWithoutExtension(executable), arguments);
      OnNameChanged();
    }
    #endregion
  }
}
