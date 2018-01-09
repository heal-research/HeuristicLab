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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Problems.Instances.Views;

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

    protected IEnumerable<IProblemInstanceProvider> problemInstanceProviders;
    public IEnumerable<IProblemInstanceProvider> ProblemInstanceProviders {
      get { return new List<IProblemInstanceProvider>(problemInstanceProviders); }
    }

    public IProblemInstanceProvider SelectedProvider { get; protected set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemBaseView"/>.
    /// </summary>
    public ProblemView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        problemInstanceProviders = null;
        problemInstanceProviderComboBox.DataSource = null;
        problemInstanceSplitContainer.Panel1Collapsed = true;
      } else {
        var consumer = Content as IProblemInstanceConsumer;
        if (consumer != null) {
          problemInstanceProviders = ProblemInstanceManager.GetProviders(Content);
          bool expand = problemInstanceProviders.Any();
          if (expand) {
            problemInstanceProviderComboBox.DisplayMember = "Name";
            problemInstanceProviderComboBox.DataSource = ProblemInstanceProviders.OrderBy(x => x.Name).ToList();
          }
          problemInstanceSplitContainer.Panel1Collapsed = !expand;
        } else
          problemInstanceSplitContainer.Panel1Collapsed = true;
      }
      SetEnabledStateOfControls();
    }

    protected virtual void problemInstanceProviderComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex >= 0) {
        SelectedProvider = (IProblemInstanceProvider)problemInstanceProviderComboBox.SelectedItem;
        problemInstanceProviderViewHost.Content = SelectedProvider;
        var view = (ProblemInstanceProviderView)problemInstanceProviderViewHost.ActiveView;
        var consumer = (IProblemInstanceConsumer)Content;
        view.Consumer = consumer;
        if (CheckForIProblemInstanceExporter(consumer))
          view.Exporter = (IProblemInstanceExporter)Content;
        else view.Exporter = null;
        SetTooltip();
      } else {
        SelectedProvider = null;
      }
      SetEnabledStateOfControls();
    }

    protected bool CheckForIProblemInstanceExporter(IProblemInstanceConsumer content) {
      return Content.GetType().GetInterfaces()
                    .Any(x => x == typeof(IProblemInstanceExporter));
    }

    #region ToolTip
    protected void SetTooltip() {
      toolTip.SetToolTip(problemInstanceProviderComboBox, GetProviderToolTip());
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
