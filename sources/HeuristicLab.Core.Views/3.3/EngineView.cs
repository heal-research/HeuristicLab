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
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Base class for editors of engines.
  /// </summary>
  [Content(typeof(Engine), true)]
  [Content(typeof(IEngine), false)]
  public partial class EngineView : ItemView {
    private int executionTimeCounter;

    /// <summary>
    /// Gets or sets the current engine.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>.</remarks>
    public new IEngine Content {
      get { return (IEngine)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBaseEditor"/>.
    /// </summary>
    public EngineView() {
      InitializeComponent();
    }
    public EngineView(IEngine content)
      : this() {
      Content = content;
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.Prepared -= new EventHandler(Content_Prepared);
      Content.Started -= new EventHandler(Content_Started);
      Content.Stopped -= new EventHandler(Content_Stopped);
      Content.ExecutionTimeChanged -= new EventHandler(Content_ExecutionTimeChanged);
      Content.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds event handlers to the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Prepared += new EventHandler(Content_Prepared);
      Content.Started += new EventHandler(Content_Started);
      Content.Stopped += new EventHandler(Content_Stopped);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="EditorBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void OnContentChanged() {
      base.OnContentChanged();
      stopButton.Enabled = false;
      logTextBox.Clear();
      if (Content == null) {
        logTextBox.Enabled = false;
        startButton.Enabled = false;
        executionTimeTextBox.Enabled = false;
      } else {
        logTextBox.Enabled = true;
        startButton.Enabled = !Content.Finished;
        UpdateExecutionTimeTextBox();
        executionTimeTextBox.Enabled = true;
      }
    }

    #region Content Events
    protected virtual void Content_Prepared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Prepared), sender, e);
      else {
        startButton.Enabled = !Content.Finished;
        stopButton.Enabled = false;
        UpdateExecutionTimeTextBox();
        logTextBox.Clear();
        Log("Engine prepared");
      }
    }
    protected virtual void Content_Started(object sender, EventArgs e) {
      executionTimeCounter = 0;
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Started), sender, e);
      else {
        startButton.Enabled = false;
        stopButton.Enabled = true;
        UpdateExecutionTimeTextBox();
        Log("Engine started");
      }
    }
    protected virtual void Content_Stopped(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Stopped), sender, e);
      else {
        startButton.Enabled = !Content.Finished;
        stopButton.Enabled = false;
        UpdateExecutionTimeTextBox();
        if (Content.Finished) Log("Engine finished");
        else Log("Engine stopped");
      }
    }
    protected virtual void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      executionTimeCounter++;
      if ((executionTimeCounter == 100) || !Content.Running) {
        executionTimeCounter = 0;
        UpdateExecutionTimeTextBox();
      }
    }
    protected virtual void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        Log(Auxiliary.BuildErrorMessage(e.Value));
    }
    #endregion

    #region Button events
    protected virtual void startButton_Click(object sender, EventArgs e) {
      Content.Start();
    }
    protected virtual void stopButton_Click(object sender, EventArgs e) {
      Content.Stop();
    }
    #endregion

    #region Helpers
    protected virtual void UpdateExecutionTimeTextBox() {
      if (InvokeRequired)
        Invoke(new Action(UpdateExecutionTimeTextBox));
      else
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
    }
    protected virtual void Log(string message) {
      if (InvokeRequired)
        Invoke(new Action<string>(Log), message);
      else {
        message = DateTime.Now.ToString() + "\t" + message;
        string[] newLines = message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        string[] lines = new string[logTextBox.Lines.Length + newLines.Length];
        logTextBox.Lines.CopyTo(lines, 0);
        newLines.CopyTo(lines, logTextBox.Lines.Length);
        logTextBox.Lines = lines;
      }
    }
    #endregion
  }
}
