#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Hive.Views {
  [View("StateLog View")]
  [Content(typeof(StateLog), IsDefaultView = true)]
  public sealed partial class StateLogView : ItemView {
    public new StateLog Content {
      get { return (StateLog)base.Content; }
      set { base.Content = value; }
    }

    public StateLogView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      // Deregister your event handlers here

      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      // Register your event handlers here

    }

    #region Event Handlers (Content)
    // Put event handlers of the content here

    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        // Add code when content has been changed and is null
        stateTextBox.Text = string.Empty;
        dateTextBox.Text = string.Empty;
        exceptionTextBox.Text = string.Empty;
        userIdTextBox.Text = string.Empty;
        slaveIdTextBox.Text = string.Empty;
      } else {
        // Add code when content has been changed and is not null
        stateTextBox.Text = Content.State.ToString();
        dateTextBox.Text = Content.DateTime.ToString();
        exceptionTextBox.Text = Content.Exception;
        userIdTextBox.Text = Content.UserId.ToString();
        slaveIdTextBox.Text = Content.SlaveId.ToString();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      // Enable or disable controls based on whether the content is null or the view is set readonly
      stateTextBox.ReadOnly = true;
      dateTextBox.ReadOnly = true;
      exceptionTextBox.ReadOnly = true;
      userIdTextBox.ReadOnly = true;
      slaveIdTextBox.ReadOnly = true;
    }

    #region Event Handlers (child controls)
    // Put event handlers of child controls here.

    #endregion
  }
}
