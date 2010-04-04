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
  [View("Engine View")]
  [Content(typeof(Engine), true)]
  [Content(typeof(IEngine), false)]
  public partial class EngineView : ItemView {
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
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
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
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.ExecutionTimeChanged += new EventHandler(Content_ExecutionTimeChanged);
      Content.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="EditorBase.UpdateControls"/> of base class <see cref="EditorBase"/>.</remarks>
    protected override void OnContentChanged() {
      base.OnContentChanged();
      logTextBox.Clear();
      if (Content == null) {
        logTextBox.Enabled = false;
        executionTimeTextBox.Text = "-";
        executionTimeTextBox.Enabled = false;
      } else {
        logTextBox.Enabled = true;
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
        executionTimeTextBox.Enabled = true;
      }
    }

    #region Content Events
    protected virtual void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionStateChanged), sender, e);
      else {
        if (Content.ExecutionState == ExecutionState.Prepared) Log("Engine prepared");
        else if (Content.ExecutionState == ExecutionState.Started) Log("Engine started");
        else if (Content.ExecutionState == ExecutionState.Paused) Log("Engine paused");
        else if (Content.ExecutionState == ExecutionState.Stopped) Log("Engine stopped");
      }
    }
    protected virtual void Content_ExecutionTimeChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ExecutionTimeChanged), sender, e);
      else
        executionTimeTextBox.Text = Content.ExecutionTime.ToString();
    }
    protected virtual void Content_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<Exception>>(Content_ExceptionOccurred), sender, e);
      else
        Log(Auxiliary.BuildErrorMessage(e.Value));
    }
    #endregion

    #region Helpers
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
