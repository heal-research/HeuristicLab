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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.Views;

namespace HeuristicLab.Optimization.Views {
  public partial class ProblemInstanceProvidersControl : UserControl {
    public ProblemInstanceProvidersControl() {
      InitializeComponent();
    }

    private IProblemInstanceConsumer consumer;
    public IProblemInstanceConsumer Consumer {
      get => consumer;
      set { consumer = value; OnConsumerChanged(); }
    }

    public bool ProvidersAvailable => problemInstanceProviderComboBox.DataSource != null;


    private void OnConsumerChanged() {
      if (Consumer == null) {
        problemInstanceProviderComboBox.DataSource = null;
        return;
      }

      var problemInstanceProviders = ProblemInstanceManager.GetProviders(Consumer);
      if (problemInstanceProviders.Any()) {
        problemInstanceProviderComboBox.DisplayMember = "Name";
        problemInstanceProviderComboBox.DataSource = problemInstanceProviders.OrderBy(x => x.Name).ToList();
      } else {
        problemInstanceProviderComboBox.DataSource = null;
      }

    }

    private void problemInstanceProviderComboBox_SelectedIndexChanged(object sender, EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex < 0) return;

      var provider = (IProblemInstanceProvider)problemInstanceProviderComboBox.SelectedItem;
      problemInstanceProviderViewHost.Content = provider;
      var view = (ProblemInstanceProviderView)problemInstanceProviderViewHost.ActiveView;
      view.Consumer = Consumer;
      if (CheckForIProblemInstanceExporter(consumer))
        view.Exporter = (IProblemInstanceExporter)consumer;
      else view.Exporter = null;
      SetTooltip();
    }

    private static bool CheckForIProblemInstanceExporter(IProblemInstanceConsumer consumer) {
      return consumer.GetType().GetInterfaces().Any(x => x == typeof(IProblemInstanceExporter));
    }


    private void SetTooltip() {
      var provider = (IProblemInstanceProvider)problemInstanceProviderComboBox.SelectedItem;
      var toolTipText = GetProviderToolTipText(provider);
      toolTip.SetToolTip(problemInstanceProviderComboBox, toolTipText);
    }

    private static string GetProviderToolTipText(IProblemInstanceProvider provider) {
      string toolTip = provider.Name;

      if (!string.IsNullOrEmpty(provider.ReferencePublication)) {
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
