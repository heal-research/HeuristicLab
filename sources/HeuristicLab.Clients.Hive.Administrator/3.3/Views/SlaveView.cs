#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Clients.Access;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("SlaveView")]
  [Content(typeof(Resource), IsDefaultView = true)]
  public partial class SlaveView : ItemView {
    public new Resource Content {
      get { return (Resource)base.Content; }
      set { base.Content = value; }
    }

    public SlaveView() {
      InitializeComponent();
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ShowSlaveUI(true);
        txtName.Clear();
        txtCPU.Clear();
        txtDetailsDescription.Clear();
        txtMemory.Clear();
        txtOS.Clear();
        txtSlaveState.Clear();
        txtLastHeartbeat.Clear();
        txtFreeMemory.Clear();
        txtId.Clear();
        txtHbIntervall.Clear();
        cbxDisposable.Checked = false;
        cbxPublic.Checked = false;
      } else {
        if (Content.GetType() == typeof(Slave)) {
          ShowSlaveUI(true);
          Slave ct = (Slave)Content;
          bool authorized = HiveServiceLocator.Instance.CallHiveService<bool>(service => service.AuthorizesForResourceAdministration(ct.Id));
          txtName.Text = ct.Name;
          txtHbIntervall.Text = ct.HbInterval.ToString();
          cbxPublic.Enabled = authorized;
          cbxPublic.CheckedChanged -= new EventHandler(cbxPublic_CheckedChanged);
          cbxPublic.Checked = ct.OwnerUserId == null;
          cbxPublic.CheckedChanged += new EventHandler(cbxPublic_CheckedChanged);
          txtCPU.Text = string.Format("{0} Cores @ {1} Mhz, Arch.: {2}", ct.Cores.ToString(), ct.CpuSpeed.ToString(), ct.CpuArchitecture.ToString());
          txtDetailsDescription.Text = ct.Description;
          txtMemory.Text = ct.Memory.ToString();
          txtOS.Text = ct.OperatingSystem;
          txtSlaveState.Text = ct.SlaveState.ToString();
          txtLastHeartbeat.Text = ct.LastHeartbeat.ToString();
          txtFreeMemory.Text = ct.FreeMemory.ToString();
          txtId.Text = ct.Id.ToString();
          cbxDisposable.Enabled = authorized;
          cbxDisposable.Checked = ct.IsDisposable.GetValueOrDefault();
        } else if (Content.GetType() == typeof(SlaveGroup)) {
          SlaveGroup ct = (SlaveGroup)Content;
          txtName.Text = ct.Name;
          txtHbIntervall.Text = ct.HbInterval.ToString();
          cbxPublic.Enabled = ct.Name != "UNGROUPED" && HiveRoles.CheckAdminUserPermissions();
          cbxPublic.CheckedChanged -= new EventHandler(cbxPublic_CheckedChanged);
          cbxPublic.Checked = ct.OwnerUserId == null;
          cbxPublic.CheckedChanged += new EventHandler(cbxPublic_CheckedChanged);
          ShowSlaveUI(false);
        } else {
          throw new Exception("Unknown Resource in SlaveView");
        }
      }
    }

    private void ShowSlaveUI(bool show) {
      label1.Visible = show;
      label2.Visible = show;
      label4.Visible = show;
      label10.Visible = show;
      label11.Visible = show;
      label12.Visible = show;
      label13.Visible = show;
      label14.Visible = show;
      label15.Visible = show;
      txtCPU.Visible = show;
      txtDetailsDescription.Visible = show;
      txtMemory.Visible = show;
      txtOS.Visible = show;
      txtSlaveState.Visible = show;
      txtLastHeartbeat.Visible = show;
      txtFreeMemory.Visible = show;
      txtId.Visible = show;
      txtName.Enabled = !show;
      cbxDisposable.Visible = show;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    private void txtName_TextChanged(object sender, EventArgs e) {
      if (Content != null && Content is SlaveGroup) {
        Content.Name = txtName.Text;
      }
    }

    private void txtHbIntervall_TextChanged(object sender, EventArgs e) {
      if (Content != null) {
        if (txtHbIntervall.Text.Length > 0) {
          try {
            int interval = int.Parse(txtHbIntervall.Text);
            Content.HbInterval = interval;
          }
          catch (Exception) {
            MessageBox.Show("Please enter a numeric value for the Heartbeat Interval.", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtHbIntervall.Text = "10";
          }
        }
      }
    }

    private void cbxDisposable_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) {
        ((Slave)Content).IsDisposable = cbxDisposable.Checked;
      }
    }

    private void cbxPublic_CheckedChanged(object sender, EventArgs e) {
      if (Content != null) {
        Content.OwnerUserId = cbxPublic.Checked ? null : new Guid?(UserInformation.Instance.User.Id);
      }
    }
  }
}
