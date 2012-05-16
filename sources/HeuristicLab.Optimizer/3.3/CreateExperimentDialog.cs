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
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Optimizer {
  public partial class CreateExperimentDialog : Form {
    private IOptimizer optimizer;
    public IOptimizer Optimizer {
      get { return optimizer; }
      set {
        optimizer = value;
        experiment = null;
        okButton.Enabled = optimizer != null;
      }
    }

    private Experiment experiment;
    public Experiment Experiment {
      get { return experiment; }
    }

    public CreateExperimentDialog() : this(null) { }
    public CreateExperimentDialog(IOptimizer optimizer) {
      InitializeComponent();
      Optimizer = optimizer;
      experiment = null;
      instancesListView.Items.Clear();
      instancesListView.Groups.Clear();
      FillOrHideInstanceListView();
    }

    private void FillOrHideInstanceListView() {
      if (Optimizer != null && optimizer is IAlgorithm) {
        var algorithm = (IAlgorithm)Optimizer;
        if (algorithm.Problem != null) {
          var instanceProviders = GetProblemInstanceProviders(algorithm.Problem);
          if (instanceProviders.Any()) {
            foreach (var provider in instanceProviders) {
              var group = new ListViewGroup(provider.Name, provider.Name);
              group.Tag = provider;
              instancesListView.Groups.Add(group);
              IEnumerable<IDataDescriptor> descriptors = ((dynamic)provider).GetDataDescriptors();
              foreach (var d in descriptors) {
                var item = new ListViewItem(d.Name, group);
                item.Checked = true;
                item.Tag = d;
                instancesListView.Items.Add(item);
              }
            }
            instancesListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            if (instancesListView.Items.Count > 0) return;
          }
        }
      }
      instancesLabel.Visible = false;
      instancesListView.Visible = false;
      Height = 130;
    }

    private IEnumerable<IProblemInstanceProvider> GetProblemInstanceProviders(IProblem problem) {
      var consumerTypes = problem.GetType().GetInterfaces()
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

    private void createBatchRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      repetitionsNumericUpDown.Enabled = createBatchRunCheckBox.Checked;
    }
    private void repetitionsNumericUpDown_Validated(object sender, EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
    }
    private void okButton_Click(object sender, EventArgs e) {
      experiment = new Experiment();
      if (instancesListView.CheckedItems.Count == 0) {
        AddOptimizer((IOptimizer)Optimizer.Clone());
      } else {
        foreach (var item in instancesListView.CheckedItems.OfType<ListViewItem>()) {
          var descriptor = (IDataDescriptor)item.Tag;
          var provider = (IProblemInstanceProvider)item.Group.Tag;
          var algorithm = (IAlgorithm)Optimizer.Clone();
          ((dynamic)algorithm.Problem).Load(((dynamic)provider).LoadData(descriptor));
          AddOptimizer(algorithm);
        }
      }
      Experiment.Prepare(true);
    }

    private void AddOptimizer(IOptimizer optimizer) {
      if (createBatchRunCheckBox.Checked) {
        var batchRun = new BatchRun();
        batchRun.Repetitions = (int)repetitionsNumericUpDown.Value;
        batchRun.Optimizer = optimizer;
        experiment.Optimizers.Add(batchRun);
      } else {
        experiment.Optimizers.Add(optimizer);
      }
    }
  }
}
