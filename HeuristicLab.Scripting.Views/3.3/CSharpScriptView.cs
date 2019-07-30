#region License Information
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
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Scripting.Views {

  [View("C# Script View")]
  [Content(typeof(CSharpScript), true)]
  public partial class CSharpScriptView : ExecutableScriptView {

    public new CSharpScript Content {
      get { return (CSharpScript)base.Content; }
      set { base.Content = value; }
    }

    public CSharpScriptView() {
      InitializeComponent();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ConsoleOutputChanged += ContentOnConsoleOutputChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.ConsoleOutputChanged -= ContentOnConsoleOutputChanged;
      base.DeregisterContentEvents();
    }

    #region Content event handlers
    protected virtual void ContentOnConsoleOutputChanged(object sender, EventArgs<string> e) {
      if (InvokeRequired)
        Invoke((Action<object, EventArgs<string>>)ContentOnConsoleOutputChanged, sender, e);
      else {
        outputTextBox.AppendText(e.Value);
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        variableStoreView.Content = null;
      } else {
        variableStoreView.Content = Content.VariableStore;
      }
    }
  }
}