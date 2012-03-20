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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Optimization.Views {
  /// <summary>
  /// The base class for visual representations of items.
  /// </summary>
  [View("Problem View")]
  [Content(typeof(IProblem), true)]
  public partial class ProblemView : ParameterizedNamedItemView {
    public new IProblem Content {
      get { return (IProblem)base.Content; }
      set { base.Content = value; }
    }

    protected IProblemInstanceProvider SelectedProvider {
      get { return (problemInstanceProviderComboBox.SelectedIndex >= 0 ? (IProblemInstanceProvider)problemInstanceProviderComboBox.SelectedItem : null); }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public ProblemView() {
      InitializeComponent();
      libraryInfoButton.Text = String.Empty;
      libraryInfoButton.Image = VSImageLibrary.Internet;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        problemInstanceProviderComboBox.DataSource = null;
      } else {
        problemInstanceProviderComboBox.DisplayMember = "Name";
        problemInstanceProviderComboBox.DataSource = GetProblemInstanceProviders().ToList();
        problemInstanceSplitContainer.Panel1Collapsed = problemInstanceProviderComboBox.Items.Count <= 0;

      }
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      problemInstanceProviderComboBox.Enabled = !ReadOnly && !Locked && Content != null && problemInstanceProviderComboBox.Items.Count > 0;
      libraryInfoButton.Enabled = SelectedProvider != null && SelectedProvider.WebLink != null;
    }

    protected virtual void problemInstanceProviderComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex >= 0) {
        dynamic provider = SelectedProvider;
        dynamic consumer = Content;
        provider.Consumer = consumer;
        problemInstanceProviderViewHost.Content = SelectedProvider;
        SetTooltip();
      }
      SetEnabledStateOfControls();
    }

    private void comboBox_DataSourceChanged(object sender, EventArgs e) {
      var comboBox = (ComboBox)sender;
      if (comboBox.DataSource == null)
        comboBox.Items.Clear();
    }

    protected virtual void libraryInfoButton_Click(object sender, EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex >= 0) {
        if (SelectedProvider != null && SelectedProvider.WebLink != null)
          Process.Start(SelectedProvider.WebLink.ToString());
      }
    }

    protected virtual IEnumerable<IProblemInstanceProvider> GetProblemInstanceProviders() {
      var consumerTypes = Content.GetType().GetInterfaces()
        .Where(x => x.IsGenericType
          && x.GetGenericTypeDefinition() == typeof(IProblemInstanceConsumer<>));

      if (consumerTypes.Any()) {
        var instanceTypes = consumerTypes
          .Select(x => x.GetGenericArguments().First())
          .Select(x => typeof(IProblemInstanceProvider<>).MakeGenericType(x));

        foreach (var type in instanceTypes) {
          foreach (var provider in ApplicationManager.Manager.GetInstances(type))
            yield return (IProblemInstanceProvider)provider;
        }
      }
    }

    protected virtual void SetTooltip() {
      toolTip.SetToolTip(problemInstanceProviderComboBox, GetProviderToolTip());
      if (SelectedProvider.WebLink != null)
        toolTip.SetToolTip(libraryInfoButton, "Browse to " + SelectedProvider.WebLink.ToString());
      else toolTip.SetToolTip(libraryInfoButton, "Library does not have a web reference.");
    }

    protected virtual string GetProviderToolTip() {
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
  }
}
