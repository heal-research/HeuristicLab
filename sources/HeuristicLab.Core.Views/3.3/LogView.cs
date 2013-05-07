#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  /// <summary>
  /// Base class for editors of engines.
  /// </summary>
  [View("Log View")]
  [Content(typeof(Log), true)]
  [Content(typeof(ILog), false)]
  public partial class LogView : ItemView {
    /// <summary>
    /// Gets or sets the current engine.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="EditorBase"/>.</remarks>
    public new ILog Content {
      get { return (ILog)base.Content; }
      set { base.Content = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineBaseEditor"/>.
    /// </summary>
    public LogView() {
      InitializeComponent();
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void DeregisterContentEvents() {
      Content.Cleared -= new EventHandler(Content_Cleared);
      Content.MessageAdded -= new EventHandler<EventArgs<string>>(Content_MessageAdded);
      base.DeregisterContentEvents();
    }

    /// <summary>
    /// Adds event handlers to the underlying <see cref="IEngine"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Cleared += new EventHandler(Content_Cleared);
      Content.MessageAdded += new EventHandler<EventArgs<string>>(Content_MessageAdded);
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
      } else {
        logTextBox.Enabled = true;
        if (Content.Messages.FirstOrDefault() != null)
          logTextBox.Text = string.Join(Environment.NewLine, Content.Messages.ToArray());
      }
    }

    protected virtual void Content_MessageAdded(object sender, EventArgs<string> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<string>>(Content_MessageAdded), sender, e);
      else {
        logTextBox.Text = string.Join(Environment.NewLine, Content.Messages.ToArray());
        logTextBox.SelectionStart = logTextBox.Text.Length;
        logTextBox.ScrollToCaret();
      }
    }

    protected virtual void Content_Cleared(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Cleared), sender, e);
      else
        logTextBox.Clear();
    }

    protected virtual void clearButton_Click(object sender, EventArgs e) {
      Content.Clear();
    }
  }
}
