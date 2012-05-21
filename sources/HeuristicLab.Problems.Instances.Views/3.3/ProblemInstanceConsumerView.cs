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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.Instances.Views {
  [View("ProblemInstanceConsumerView")]
  [Content(typeof(IProblemInstanceConsumer), IsDefaultView = false)]
  public sealed partial class ProblemInstanceConsumerView : AsynchronousContentView {

    public new IProblemInstanceConsumer Content {
      get { return (IProblemInstanceConsumer)base.Content; }
      set { base.Content = value; }
    }

    public IProblemInstanceProvider SelectedProvider {
      get;
      private set;
    }

    public IEnumerable<IProblemInstanceProvider> ProblemInstanceProviders {
      get;
      private set;
    }

    public ProblemInstanceConsumerView() {
      InitializeComponent();
      libraryInfoButton.Text = String.Empty;
      libraryInfoButton.Image = VSImageLibrary.Help;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        ProblemInstanceProviders = null;
        problemInstanceProviderComboBox.DataSource = null;
      } else {
        problemInstanceProviderComboBox.DisplayMember = "Name";
        ProblemInstanceProviders = ProblemInstanceManager.GetProviders(Content);
        problemInstanceProviderComboBox.DataSource = ProblemInstanceProviders.OrderBy(x => x.Name).ToList();
      }
      SetEnabledStateOfControls();
    }

    public bool ContainsProviders() {
      return problemInstanceProviderComboBox.Items.Count > 0;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      problemInstanceProviderComboBox.Enabled = !ReadOnly && !Locked && Content != null && problemInstanceProviderComboBox.Items.Count > 0;
      libraryInfoButton.Enabled = SelectedProvider != null && SelectedProvider.WebLink != null;
    }

    private void problemInstanceProviderComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex >= 0) {
        SelectedProvider = (IProblemInstanceProvider)problemInstanceProviderComboBox.SelectedItem;
        problemInstanceProviderViewHost.Content = SelectedProvider;
        ProblemInstanceProviderView view = (ProblemInstanceProviderView)problemInstanceProviderViewHost.ActiveView;
        view.Consumer = Content;
        if (CheckForIProblemInstanceExporter(Content)) {
          view.Exporter = (IProblemInstanceExporter)Content;
        }
        SetTooltip();
      } else {
        SelectedProvider = null;
      }

      SetEnabledStateOfControls();
    }

    private bool CheckForIProblemInstanceExporter(IProblemInstanceConsumer content) {
      return Content.GetType().GetInterfaces()
                    .Any(x => x.Equals(typeof(IProblemInstanceExporter)));
    }

    private void libraryInfoButton_Click(object sender, EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex >= 0) {
        if (SelectedProvider != null && SelectedProvider.WebLink != null)
          Process.Start(SelectedProvider.WebLink.ToString());
      }
    }

    #region ToolTip
    private void SetTooltip() {
      toolTip.SetToolTip(problemInstanceProviderComboBox, GetProviderToolTip());
      if (SelectedProvider.WebLink != null)
        toolTip.SetToolTip(libraryInfoButton, "Browse to " + SelectedProvider.WebLink.ToString());
      else toolTip.SetToolTip(libraryInfoButton, "Library does not have a web reference.");
    }

    private string GetProviderToolTip() {
      var provider = SelectedProvider;
      string toolTip = provider.Name;

      if (!String.IsNullOrEmpty(provider.ReferencePublication)) {
        toolTip = toolTip
            + Environment.NewLine + Environment.NewLine
            + provider.ReferencePublication;
      }
      if (provider.WebLink != null) {
        toolTip = toolTip
            + Environment.NewLine
            + provider.WebLink.ToString();
      }

      return toolTip;
    }
    #endregion
  }
}
