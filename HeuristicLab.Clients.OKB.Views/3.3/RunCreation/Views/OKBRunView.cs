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
using System.ComponentModel;
using System.Windows.Forms;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.RunCreation.Views {
  [View("OKBRun View")]
  [Content(typeof(OKBRun), true)]
  public sealed partial class OKBRunView : ItemView {
    public new OKBRun Content {
      get { return (OKBRun)base.Content; }
      set { base.Content = value; }
    }

    public OKBRunView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.PropertyChanged -= Content_PropertyChanged;
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PropertyChanged += Content_PropertyChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      viewHost.Content = Content == null ? null : Content.WrappedRun;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      storeButton.Enabled = (Content != null) && !ReadOnly && !Locked && !Content.Stored;
    }

    #region Content Events
    private void Content_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      if (e.PropertyName == "Stored")
        storeButton.Enabled = !Content.Stored;
    }
    #endregion

    #region Control Events
    private void storeButton_Click(object sender, System.EventArgs e) {
      try {
        Content.Store();
      } catch (MissingClientRegistrationException) {
        MessageBox.Show("You have to register your client to be able to store OKB runs." + Environment.NewLine
          + " Please click in the menu bar on Services -> Access -> Client Information and register your client. ", "Missing client registration", MessageBoxButtons.OK, MessageBoxIcon.Information);
      } catch (Exception ex) {
        ErrorHandling.ShowErrorDialog(this, "Store failed.", ex);
      }
    }
    #endregion
  }
}
