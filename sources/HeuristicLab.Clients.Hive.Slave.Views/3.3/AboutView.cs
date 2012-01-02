#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {
  [View("About View: Show some information about HeursticLab Hive")]
  public partial class AboutView : ItemView {
    public new Item Content {
      get { return (Item)base.Content; }
      set { base.Content = value; }
    }

    public AboutView() {
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
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    private void AboutView_Load(object sender, System.EventArgs e) {
      FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
      string version = versionInfo.FileVersion;

      versionTextBox.Text = version;
    }

    private void webLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      System.Diagnostics.Process.Start(webLinkLabel.Text);
    }

    private void mailLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
      System.Diagnostics.Process.Start("mailto:" + mailLinkLabel.Text);
    }
  }
}
