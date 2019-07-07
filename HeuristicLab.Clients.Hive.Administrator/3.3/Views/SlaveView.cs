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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("Slave View")]
  [Content(typeof(Slave), IsDefaultView = true)]
  public sealed partial class SlaveView : ResourceView {
    public new Slave Content {
      get { return (Slave)base.Content; }
      set { base.Content = value; }
    }

    public SlaveView() {
      InitializeComponent();
    }

    #region Overrides
    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        cpuTextBox.Clear();
        memoryTextBox.Clear();
        operatingSystemTextBox.Clear();
        stateTextBox.Clear();
        lastHeartbeatTextBox.Clear();
        disposableCheckBox.Checked = false;
      } else {
        cpuTextBox.Text = string.Format("{0} Cores @ {1} MHz, Arch.: {2}", Content.Cores, Content.CpuSpeed, Content.CpuArchitecture);
        memoryTextBox.Text = string.Format("{0} ({1} Free)", Content.Memory, Content.FreeMemory);
        operatingSystemTextBox.Text = Content.OperatingSystem;
        stateTextBox.Text = Content.SlaveState.ToString();
        lastHeartbeatTextBox.Text = Content.LastHeartbeat.ToString();
        disposableCheckBox.Checked = Content.IsDisposable.GetValueOrDefault();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      bool enabled = Content != null && !Locked;
      disposableCheckBox.Enabled = enabled;
    }
    #endregion

    #region Event Handlers
    private void disposableCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (Content != null)
        Content.IsDisposable = disposableCheckBox.Checked;
    }
    #endregion
  }
}
