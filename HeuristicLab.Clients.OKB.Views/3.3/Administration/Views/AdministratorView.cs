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

using System;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.OKB.Administration {
  [View("OKB Administrator")]
  [Content(typeof(AdministrationClient), true)]
  public sealed partial class AdministratorView : AsynchronousContentView {
    public new AdministrationClient Content {
      get { return (AdministrationClient)base.Content; }
      set { base.Content = value; }
    }

    public AdministratorView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.Refreshing -= new EventHandler(Content_Refreshing);
      Content.Refreshed -= new EventHandler(Content_Refreshed);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.Refreshing += new EventHandler(Content_Refreshing);
      Content.Refreshed += new EventHandler(Content_Refreshed);
    }


    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        platformCollectionView.Content = null;
        algorithmClassCollectionView.Content = null;
        algorithmCollectionView.Content = null;
        problemClassCollectionView.Content = null;
        problemCollectionView.Content = null;
      } else {
        platformCollectionView.Content = Content.Platforms;
        algorithmClassCollectionView.Content = Content.AlgorithmClasses;
        algorithmCollectionView.Content = Content.Algorithms;
        problemClassCollectionView.Content = Content.ProblemClasses;
        problemCollectionView.Content = Content.Problems;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      refreshButton.Enabled = Content != null;
      platformCollectionView.Enabled = Content != null;
      algorithmClassCollectionView.Enabled = Content != null;
      algorithmCollectionView.Enabled = Content != null;
      problemClassCollectionView.Enabled = Content != null;
      problemCollectionView.Enabled = Content != null;
    }

    private void Content_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        refreshButton.Enabled = false;
        tabControl.Enabled = false;
      }
    }
    private void Content_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(Content_Refreshed), sender, e);
      } else {
        platformCollectionView.Content = Content.Platforms;
        algorithmClassCollectionView.Content = Content.AlgorithmClasses;
        algorithmCollectionView.Content = Content.Algorithms;
        problemClassCollectionView.Content = Content.ProblemClasses;
        problemCollectionView.Content = Content.Problems;
        refreshButton.Enabled = true;
        tabControl.Enabled = true;
        Cursor = Cursors.Default;
      }
    }

    private void refreshButton_Click(object sender, EventArgs e) {
      Content.RefreshAsync(new Action<Exception>((Exception ex) => ErrorHandling.ShowErrorDialog(this, "Refresh failed.", ex)));
    }
  }
}
