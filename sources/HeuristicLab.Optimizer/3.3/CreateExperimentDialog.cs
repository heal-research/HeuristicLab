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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Optimizer {
  public partial class CreateExperimentDialog : Form {
    private enum DialogMode { Normal = 1, DiscoveringInstances = 2, CreatingExperiment = 3 };

    private IOptimizer optimizer;
    public IOptimizer Optimizer {
      get { return optimizer; }
      set {
        optimizer = value;
        Experiment = null;
        okButton.Enabled = optimizer != null;
        SetTabControlVisibility();
        FillInstanceTreeViewAsync();
        FillParametersListView();
      }
    }

    public Experiment Experiment { get; private set; }

    private bool createBatchRun;
    private int repetitions;
    private Dictionary<IProblemInstanceProvider, HashSet<IDataDescriptor>> instances;
    private Dictionary<IValueParameter, Tuple<int, int, int>> intParameters;
    private Dictionary<IValueParameter, Tuple<double, double, double>> doubleParameters;
    private HashSet<IValueParameter> boolParameters;
    private Dictionary<IValueParameter, HashSet<INamedItem>> multipleChoiceParameters;

    private StringBuilder failedInstances;
    private EventWaitHandle backgroundWorkerWaitHandle = new ManualResetEvent(false);
    private bool suppressTreeViewEventHandling, suppressCheckAllNoneEventHandling;

    public CreateExperimentDialog() : this(null) { }
    public CreateExperimentDialog(IOptimizer optimizer) {
      InitializeComponent();
      instanceDiscoveryProgressLabel.BackColor = instancesTabPage.BackColor;
      createBatchRun = createBatchRunCheckBox.Checked;
      repetitions = (int)repetitionsNumericUpDown.Value;
      // do not set the Optimizer property here, because we want to delay instance discovery to the time when the form loads
      this.optimizer = optimizer;
      Experiment = null;
      okButton.Enabled = optimizer != null;

      instances = new Dictionary<IProblemInstanceProvider, HashSet<IDataDescriptor>>();
      intParameters = new Dictionary<IValueParameter, Tuple<int, int, int>>();
      doubleParameters = new Dictionary<IValueParameter, Tuple<double, double, double>>();
      boolParameters = new HashSet<IValueParameter>();
      multipleChoiceParameters = new Dictionary<IValueParameter, HashSet<INamedItem>>();
    }

    #region Event handlers
    private void CreateExperimentDialog_Load(object sender, EventArgs e) {
      SetTabControlVisibility();
      FillInstanceTreeViewAsync();
      FillParametersListView();
    }

    private void CreateExperimentDialog_FormClosing(object sender, FormClosingEventArgs e) {
      if (experimentCreationBackgroundWorker.IsBusy) {
        if (DialogResult != System.Windows.Forms.DialogResult.OK) {
          if (experimentCreationBackgroundWorker.IsBusy) experimentCreationBackgroundWorker.CancelAsync();
          if (instanceDiscoveryBackgroundWorker.IsBusy) instanceDiscoveryBackgroundWorker.CancelAsync();
        }
        e.Cancel = true;
      }
    }

    private void okButton_Click(object sender, EventArgs e) {
      SetMode(DialogMode.CreatingExperiment);
      experimentCreationBackgroundWorker.RunWorkerAsync();
      backgroundWorkerWaitHandle.WaitOne(); // make sure the background worker has started before exiting
    }

    #region Parameters variation
    private void parametersListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var parameter = (IValueParameter)e.Item.Tag;
      var isConstrainedValueParameter = typeof(OptionalConstrainedValueParameter<>).Equals(parameter.GetType().GetGenericTypeDefinition())
        || typeof(ConstrainedValueParameter<>).Equals(parameter.GetType().GetGenericTypeDefinition());

      if (!isConstrainedValueParameter && parameter.Value == null) {
        if (e.Item.Checked) e.Item.Checked = false;
        return;
      }

      if (isConstrainedValueParameter) {
        if (e.Item.Checked) {
          multipleChoiceParameters.Add(parameter, new HashSet<INamedItem>());
        } else {
          multipleChoiceParameters.Remove(parameter);
        }
      }

      var intValue = parameter.Value as ValueTypeValue<int>;
      if (intValue != null) {
        if (e.Item.Checked) {
          int minimum = intValue.Value;
          int maximum = intValue.Value;
          int step = 1;
          intParameters.Add(parameter, new Tuple<int, int, int>(minimum, maximum, step));
        } else intParameters.Remove(parameter);
      }

      var doubleValue = parameter.Value as ValueTypeValue<double>;
      if (doubleValue != null) {
        if (e.Item.Checked) {
          double minimum = doubleValue.Value;
          double maximum = doubleValue.Value;
          double step = 1;
          doubleParameters.Add(parameter, new Tuple<double, double, double>(minimum, maximum, step));
        } else doubleParameters.Remove(parameter);
      }

      var boolValue = parameter.Value as ValueTypeValue<bool>;
      if (boolValue != null) {
        if (e.Item.Checked) boolParameters.Add(parameter);
        else boolParameters.Remove(parameter);
      }

      UpdateVariationsLabel();
      if (e.Item.Selected) UpdateDetailsView(parameter);
      else e.Item.Selected = true;
    }

    private void parametersListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (parametersListView.SelectedItems.Count == 0) {
        ClearDetailsView();
      } else {
        var parameter = parametersListView.SelectedItems[0].Tag as IValueParameter;
        UpdateDetailsView(parameter);
      }
    }

    private void UpdateDetailsView(IValueParameter parameter) {
      ClearDetailsView();

      var isConstrainedValueParameter =
        typeof(OptionalConstrainedValueParameter<>).IsAssignableFrom(parameter.GetType().GetGenericTypeDefinition())
        || typeof(ConstrainedValueParameter<>).Equals(parameter.GetType().GetGenericTypeDefinition());

      if (isConstrainedValueParameter) {
        detailsTypeLabel.Text = "Choices:";
        choicesListView.Enabled = true;
        choicesListView.Visible = true;
        choicesListView.Tag = parameter;

        if (!multipleChoiceParameters.ContainsKey(parameter)) return;
        dynamic constrainedValuedParameter = parameter;
        dynamic validValues = constrainedValuedParameter.ValidValues;
        foreach (var choice in validValues) {
          choicesListView.Items.Add(new ListViewItem(choice.ToString()) {
            Tag = choice,
            Checked = multipleChoiceParameters[parameter].Contains((INamedItem)choice)
          });
        }
        return;
      }

      if (!(parameter.Value is ValueTypeValue<bool>)) {
        minimumLabel.Visible = true; minimumTextBox.Visible = true;
        maximumLabel.Visible = true; maximumTextBox.Visible = true;
        stepSizeLabel.Visible = true; stepSizeTextBox.Visible = true;
      } else detailsTypeLabel.Text = "Boolean parameter: True / False";

      var intValue = parameter.Value as ValueTypeValue<int>;
      if (intValue != null) {
        detailsTypeLabel.Text = "Integer parameter:";
        if (!intParameters.ContainsKey(parameter)) return;
        string min = intParameters[parameter].Item1.ToString();
        string max = intParameters[parameter].Item2.ToString();
        string step = intParameters[parameter].Item3.ToString();
        UpdateMinMaxStepSize(parameter, min, max, step);
        return;
      }

      var doubleValue = parameter.Value as ValueTypeValue<double>;
      if (doubleValue != null) {
        detailsTypeLabel.Text = "Double parameter:";
        if (!doubleParameters.ContainsKey(parameter)) return;
        string min = doubleParameters[parameter].Item1.ToString();
        string max = doubleParameters[parameter].Item2.ToString();
        string step = doubleParameters[parameter].Item3.ToString();
        UpdateMinMaxStepSize(parameter, min, max, step);
        return;
      }
    }

    #region Detail controls
    private void choiceListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      var parameter = (IValueParameter)choicesListView.Tag;
      if (e.Item.Checked) {
        multipleChoiceParameters[parameter].Add((INamedItem)e.Item.Tag);
      } else multipleChoiceParameters[parameter].Remove((INamedItem)e.Item.Tag);

      UpdateVariationsLabel();
    }

    private void detailsTextBox_Validating(object sender, CancelEventArgs e) {
      var parameter = (IValueParameter)((TextBox)sender).Tag;
      errorProvider.Clear();

      var intValue = parameter.Value as ValueTypeValue<int>;
      if (intValue != null) {
        int value;
        if (!int.TryParse(((TextBox)sender).Text, out value)) {
          errorProvider.SetError(((TextBox)sender), "Please enter a valid integer number.");
          e.Cancel = true;
        } else {
          var before = intParameters[parameter];
          var after = default(Tuple<int, int, int>);
          if (sender == minimumTextBox) after = new Tuple<int, int, int>(value, before.Item2, before.Item3);
          else if (sender == maximumTextBox) after = new Tuple<int, int, int>(before.Item1, value, before.Item3);
          else if (sender == stepSizeTextBox) after = new Tuple<int, int, int>(before.Item1, before.Item2, value);
          intParameters[parameter] = after;
        }
      }

      var doubleValue = parameter.Value as ValueTypeValue<double>;
      if (doubleValue != null) {
        double value;
        if (!double.TryParse(((TextBox)sender).Text, NumberStyles.Float, CultureInfo.CurrentCulture.NumberFormat, out value)) {
          errorProvider.SetError(((TextBox)sender), "Please enter a valid number.");
          e.Cancel = true;
        } else {
          var before = doubleParameters[parameter];
          var after = default(Tuple<double, double, double>);
          if (sender == minimumTextBox) after = new Tuple<double, double, double>(value, before.Item2, before.Item3);
          else if (sender == maximumTextBox) after = new Tuple<double, double, double>(before.Item1, value, before.Item3);
          else if (sender == stepSizeTextBox) after = new Tuple<double, double, double>(before.Item1, before.Item2, value);
          doubleParameters[parameter] = after;
        }
      }

      UpdateVariationsLabel();
    }
    #endregion
    #endregion

    #region Instances
    private void instancesTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (!suppressTreeViewEventHandling) {
        if (e.Node.Nodes.Count > 0) { // provider node was (un)checked
          SyncProviderNode(e.Node);
        } else { // descriptor node was (un)checked
          SyncInstanceNode(e.Node);
        }

        suppressCheckAllNoneEventHandling = true;
        try {
          var treeViewNodes = instancesTreeView.Nodes.OfType<TreeNode>().SelectMany(x => x.Nodes.OfType<TreeNode>());
          selectAllCheckBox.Checked = treeViewNodes.Count() == instances.SelectMany(x => x.Value).Count();
          selectNoneCheckBox.Checked = !treeViewNodes.Any(x => x.Checked);
        } finally { suppressCheckAllNoneEventHandling = false; }
        UpdateVariationsLabel();
      }
    }

    private void SyncProviderNode(TreeNode node) {
      suppressTreeViewEventHandling = true;
      try {
        foreach (TreeNode n in node.Nodes) {
          if (n.Checked != node.Checked) {
            n.Checked = node.Checked;
            SyncInstanceNode(n, false);
          }
        }
      } finally { suppressTreeViewEventHandling = false; }
    }

    private void SyncInstanceNode(TreeNode node, bool providerCheck = true) {
      var provider = (IProblemInstanceProvider)node.Parent.Tag;
      var descriptor = (IDataDescriptor)node.Tag;
      if (node.Checked) {
        if (!instances.ContainsKey(provider))
          instances.Add(provider, new HashSet<IDataDescriptor>());
        instances[provider].Add(descriptor);
      } else {
        if (instances.ContainsKey(provider)) {
          instances[provider].Remove(descriptor);
          if (instances[provider].Count == 0)
            instances.Remove(provider);
        }
      }
      if (providerCheck) {
        bool allChecked = node.Parent.Nodes.OfType<TreeNode>().All(x => x.Checked);
        suppressTreeViewEventHandling = true;
        try {
          node.Parent.Checked = allChecked;
        } finally { suppressTreeViewEventHandling = false; }
      }
    }

    private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (!suppressCheckAllNoneEventHandling) {
        if (selectAllCheckBox.Checked) {
          suppressCheckAllNoneEventHandling = true;
          try { selectNoneCheckBox.Checked = false; } finally { suppressCheckAllNoneEventHandling = false; }
          try {
            suppressTreeViewEventHandling = true;
            foreach (TreeNode node in instancesTreeView.Nodes) {
              if (!node.Checked) {
                node.Checked = true;
                SyncProviderNode(node);
              }
            }
          } finally { suppressTreeViewEventHandling = false; }
        }
      }
    }

    private void selectNoneCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (!suppressCheckAllNoneEventHandling) {
        if (selectNoneCheckBox.Checked) {
          suppressCheckAllNoneEventHandling = true;
          try { selectAllCheckBox.Checked = false; } finally { suppressCheckAllNoneEventHandling = false; }
          try {
            suppressTreeViewEventHandling = true;
            foreach (TreeNode node in instancesTreeView.Nodes) {
              if (node.Checked) {
                node.Checked = false;
                SyncProviderNode(node);
              }
            }
          } finally { suppressTreeViewEventHandling = false; }
        }
      }
    }
    #endregion

    private void createBatchRunCheckBox_CheckedChanged(object sender, EventArgs e) {
      repetitionsNumericUpDown.Enabled = createBatchRunCheckBox.Checked;
      createBatchRun = createBatchRunCheckBox.Checked;
    }

    private void repetitionsNumericUpDown_Validated(object sender, EventArgs e) {
      if (repetitionsNumericUpDown.Text == string.Empty)
        repetitionsNumericUpDown.Text = repetitionsNumericUpDown.Value.ToString();
      repetitions = (int)repetitionsNumericUpDown.Value;
    }

    private void experimentsLabel_TextChanged(object sender, EventArgs e) {
      long number;
      if (long.TryParse(variationsLabel.Text, NumberStyles.AllowThousands, CultureInfo.CurrentCulture.NumberFormat, out number)) {
        if (number > 1000) warningProvider.SetError(variationsLabel, "Consider reducing the number of variations!");
        else warningProvider.SetError(variationsLabel, null);
      }
    }
    #endregion

    #region Helpers
    private void SetTabControlVisibility() {
      bool isAlgorithm = optimizer != null && optimizer is IAlgorithm;
      bool instancesAvailable = isAlgorithm
        && ((IAlgorithm)optimizer).Problem != null
        && ProblemInstanceManager.GetProviders(((IAlgorithm)optimizer).Problem).Any();
      if (instancesAvailable && tabControl.TabCount == 1)
        tabControl.TabPages.Add(instancesTabPage);
      else if (!instancesAvailable && tabControl.TabCount == 2)
        tabControl.TabPages.Remove(instancesTabPage);
      tabControl.Visible = isAlgorithm;
      if (isAlgorithm) {
        variationsLabel.Visible = true;
        experimentsToCreateDescriptionLabel.Visible = true;
        Height = 430;
      } else {
        variationsLabel.Visible = false;
        experimentsToCreateDescriptionLabel.Visible = false;
        Height = 130;
      }
    }

    private void FillParametersListView() {
      parametersListView.Items.Clear();
      intParameters.Clear();
      doubleParameters.Clear();
      boolParameters.Clear();
      multipleChoiceParameters.Clear();

      if (Optimizer is IAlgorithm) {
        var parameters = ((IAlgorithm)optimizer).Parameters;
        foreach (var param in parameters) {
          var valueParam = param as IValueParameter;
          if (valueParam != null && (valueParam.Value is ValueTypeValue<bool>
              || valueParam.Value is ValueTypeValue<int>
              || valueParam.Value is ValueTypeValue<double>)
            || typeof(OptionalConstrainedValueParameter<>).IsAssignableFrom(param.GetType().GetGenericTypeDefinition())
            || typeof(ConstrainedValueParameter<>).IsAssignableFrom(param.GetType().GetGenericTypeDefinition()))
            parametersListView.Items.Add(new ListViewItem(param.Name) { Tag = param });
        }
      }
    }

    private void FillInstanceTreeViewAsync() {
      instances.Clear();
      instancesTreeView.Nodes.Clear();

      if (Optimizer is IAlgorithm && ((IAlgorithm)Optimizer).Problem != null) {
        SetMode(DialogMode.DiscoveringInstances);
        instanceDiscoveryBackgroundWorker.RunWorkerAsync();
      }
    }

    private void AddOptimizer(IOptimizer optimizer, Experiment experiment) {
      if (createBatchRun) {
        var batchRun = new BatchRun();
        batchRun.Repetitions = repetitions;
        batchRun.Optimizer = optimizer;
        experiment.Optimizers.Add(batchRun);
      } else {
        experiment.Optimizers.Add(optimizer);
      }
    }

    private int GetNumberOfVariations() {
      int instancesCount = 1;
      if (instances.Values.Any())
        instancesCount = Math.Max(instances.Values.SelectMany(x => x).Count(), 1);

      int intParameterVariations = 1;
      foreach (var intParam in intParameters.Values) {
        if (intParam.Item3 == 0) continue;
        intParameterVariations *= (intParam.Item2 - intParam.Item1) / intParam.Item3 + 1;
      }
      int doubleParameterVariations = 1;
      foreach (var doubleParam in doubleParameters.Values) {
        if (doubleParam.Item3 == 0) continue;
        doubleParameterVariations *= (int)Math.Floor((doubleParam.Item2 - doubleParam.Item1) / doubleParam.Item3) + 1;
      }
      int boolParameterVariations = 1;
      foreach (var boolParam in boolParameters) {
        boolParameterVariations *= 2;
      }
      int choiceParameterVariations = 1;
      foreach (var choiceParam in multipleChoiceParameters.Values) {
        choiceParameterVariations *= Math.Max(choiceParam.Count, 1);
      }

      return (instancesCount * intParameterVariations * doubleParameterVariations * boolParameterVariations * choiceParameterVariations);
    }

    private void SetMode(DialogMode mode) {
      createBatchRunCheckBox.Enabled = mode == DialogMode.Normal;
      repetitionsNumericUpDown.Enabled = mode == DialogMode.Normal;
      selectAllCheckBox.Enabled = mode == DialogMode.Normal;
      selectNoneCheckBox.Enabled = mode == DialogMode.Normal;
      instancesTreeView.Enabled = mode == DialogMode.Normal;
      instancesTreeView.Visible = mode == DialogMode.Normal || mode == DialogMode.CreatingExperiment;
      okButton.Enabled = mode == DialogMode.Normal;
      okButton.Visible = mode != DialogMode.CreatingExperiment;
      instanceDiscoveryProgressLabel.Visible = mode == DialogMode.DiscoveringInstances;
      instanceDiscoveryProgressBar.Visible = mode == DialogMode.DiscoveringInstances;
      experimentCreationProgressBar.Visible = mode == DialogMode.CreatingExperiment;
    }

    private void ClearDetailsView() {
      minimumLabel.Visible = false;
      minimumTextBox.Text = string.Empty;
      minimumTextBox.Enabled = false;
      minimumTextBox.Visible = false;
      maximumLabel.Visible = false;
      maximumTextBox.Text = string.Empty;
      maximumTextBox.Enabled = false;
      maximumTextBox.Visible = false;
      stepSizeLabel.Visible = false;
      stepSizeTextBox.Text = string.Empty;
      stepSizeTextBox.Enabled = false;
      stepSizeTextBox.Visible = false;
      choicesListView.Items.Clear();
      choicesListView.Enabled = false;
      choicesListView.Visible = false;
    }

    private void UpdateMinMaxStepSize(IValueParameter parameter, string min, string max, string step) {
      minimumLabel.Visible = true;
      minimumTextBox.Text = min;
      minimumTextBox.Enabled = true;
      minimumTextBox.Visible = true;
      minimumTextBox.Tag = parameter;
      maximumLabel.Visible = true;
      maximumTextBox.Text = max;
      maximumTextBox.Enabled = true;
      maximumTextBox.Visible = true;
      maximumTextBox.Tag = parameter;
      stepSizeLabel.Visible = true;
      stepSizeTextBox.Text = step;
      stepSizeTextBox.Enabled = true;
      stepSizeTextBox.Visible = true;
      stepSizeTextBox.Tag = parameter;
    }

    private void UpdateVariationsLabel() {
      variationsLabel.Text = GetNumberOfVariations().ToString("#,#", CultureInfo.CurrentCulture);
    }

    #region Retrieve parameter combinations
    private IEnumerable<Dictionary<IValueParameter, int>> GetIntParameterConfigurations() {
      var configuration = new Dictionary<IValueParameter, int>();
      var indices = new Dictionary<IValueParameter, int>();
      bool finished;
      do {
        foreach (var p in intParameters) {
          if (!indices.ContainsKey(p.Key)) indices.Add(p.Key, 0);
          var value = p.Value.Item1 + p.Value.Item3 * indices[p.Key];
          configuration[p.Key] = value;
        }
        yield return configuration;

        finished = true;
        foreach (var p in intParameters.Keys) {
          var newValue = intParameters[p].Item1 + intParameters[p].Item3 * (indices[p] + 1);
          if (newValue > intParameters[p].Item2 || intParameters[p].Item3 == 0)
            indices[p] = 0;
          else {
            indices[p]++;
            finished = false;
            break;
          }
        }
      } while (!finished);
    }

    private IEnumerable<Dictionary<IValueParameter, double>> GetDoubleParameterConfigurations() {
      var configuration = new Dictionary<IValueParameter, double>();
      var indices = new Dictionary<IValueParameter, int>();
      bool finished;
      do {
        foreach (var p in doubleParameters) {
          if (!indices.ContainsKey(p.Key)) indices.Add(p.Key, 0);
          var value = p.Value.Item1 + p.Value.Item3 * indices[p.Key];
          configuration[p.Key] = value;
        }
        yield return configuration;

        finished = true;
        foreach (var p in doubleParameters.Keys) {
          var newValue = doubleParameters[p].Item1 + doubleParameters[p].Item3 * (indices[p] + 1);
          if (newValue > doubleParameters[p].Item2 || doubleParameters[p].Item3 == 0)
            indices[p] = 0;
          else {
            indices[p]++;
            finished = false;
            break;
          }
        }
      } while (!finished);
    }

    private IEnumerable<Dictionary<IValueParameter, bool>> GetBoolParameterConfigurations() {
      var configuration = new Dictionary<IValueParameter, bool>();
      bool finished;
      do {
        finished = true;
        foreach (var p in boolParameters) {
          if (!configuration.ContainsKey(p)) configuration.Add(p, false);
          else {
            if (configuration[p]) {
              configuration[p] = false;
            } else {
              configuration[p] = true;
              finished = false;
              break;
            }
          }
        }
        yield return configuration;
      } while (!finished);
    }

    private IEnumerable<Dictionary<IValueParameter, INamedItem>> GetMultipleChoiceConfigurations() {
      var configuration = new Dictionary<IValueParameter, INamedItem>();
      var enumerators = new Dictionary<IValueParameter, IEnumerator<INamedItem>>();
      bool finished;
      do {
        foreach (var p in multipleChoiceParameters.Keys.ToArray()) {
          if (!enumerators.ContainsKey(p)) {
            enumerators.Add(p, multipleChoiceParameters[p].GetEnumerator());
            if (!enumerators[p].MoveNext()) {
              multipleChoiceParameters.Remove(p);
              continue;
            }
          }
          configuration[p] = enumerators[p].Current;
        }

        finished = true;
        foreach (var p in multipleChoiceParameters.Keys) {
          if (!enumerators[p].MoveNext()) {
            enumerators[p] = multipleChoiceParameters[p].GetEnumerator();
          } else {
            finished = false;
            break;
          }
        }
        yield return configuration;
      } while (!finished);
    }
    #endregion
    #endregion

    #region Background workers
    #region Instance discovery
    private void instanceDiscoveryBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      var instanceProviders = ProblemInstanceManager.GetProviders(((IAlgorithm)Optimizer).Problem).ToArray();
      var nodes = new TreeNode[instanceProviders.Length];
      for (int i = 0; i < instanceProviders.Length; i++) {
        var provider = instanceProviders[i];
        nodes[i] = new TreeNode(provider.Name) { Tag = provider };
      }
      e.Result = nodes;
      for (int i = 0; i < nodes.Length; i++) {
        var providerNode = nodes[i];
        var provider = providerNode.Tag as IProblemInstanceProvider;
        double progress = i / (double)nodes.Length;
        instanceDiscoveryBackgroundWorker.ReportProgress((int)(100 * progress), provider.Name);
        var descriptors = ProblemInstanceManager.GetDataDescriptors(provider).ToArray();
        for (int j = 0; j < descriptors.Length; j++) {
          #region Check cancellation request
          if (instanceDiscoveryBackgroundWorker.CancellationPending) {
            e.Cancel = true;
            return;
          }
          #endregion
          var node = new TreeNode(descriptors[j].Name) { Tag = descriptors[j] };
          providerNode.Nodes.Add(node);
        }
      }
      instanceDiscoveryBackgroundWorker.ReportProgress(100, string.Empty);
    }

    private void instanceDiscoveryBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      if (instanceDiscoveryProgressBar.Value != e.ProgressPercentage)
        instanceDiscoveryProgressBar.Value = e.ProgressPercentage;
      instanceDiscoveryProgressLabel.Text = (string)e.UserState;
      Application.DoEvents();
    }

    private void instanceDiscoveryBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      try {
        instancesTreeView.Nodes.AddRange((TreeNode[])e.Result);
        foreach (TreeNode node in instancesTreeView.Nodes)
          node.Collapse();
        selectNoneCheckBox.Checked = true;
      } catch { }
      try {
        SetMode(DialogMode.Normal);
        if (e.Error != null) MessageBox.Show(e.Error.Message, "Error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
      } catch { }
    }
    #endregion

    #region Experiment creation
    private void experimentCreationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
      backgroundWorkerWaitHandle.Set(); // notify the ok button that we're busy now
      failedInstances = new StringBuilder();
      var localExperiment = new Experiment();

      if (instances.Count == 0) {
        var variations = experimentCreationBackgroundWorker_CalculateParameterVariations(optimizer);
        foreach (var v in variations)
          AddOptimizer(v, localExperiment);
        experimentCreationBackgroundWorker.ReportProgress(100, string.Empty);

      } else {
        int counter = 0, totalVariations = GetNumberOfVariations();
        foreach (var provider in instances.Keys) {
          foreach (var descriptor in instances[provider]) {
            #region Check cancellation request
            if (experimentCreationBackgroundWorker.CancellationPending) {
              e.Cancel = true;
              localExperiment = null;
              return;
            }
            #endregion
            var algorithm = (IAlgorithm)Optimizer.Clone();
            bool failed = false;
            try {
              ProblemInstanceManager.LoadData(provider, descriptor, (IProblemInstanceConsumer)algorithm.Problem);
            } catch (Exception ex) {
              failedInstances.AppendLine(descriptor.Name + ": " + ex.Message);
              failed = true;
            }
            if (!failed) {
              var variations = experimentCreationBackgroundWorker_CalculateParameterVariations(algorithm);
              foreach (var v in variations) {
                AddOptimizer(v, localExperiment);
                counter++;
                experimentCreationBackgroundWorker.ReportProgress((int)Math.Round(100.0 * counter / totalVariations), descriptor.Name);
              }
            } else experimentCreationBackgroundWorker.ReportProgress((int)Math.Round(100.0 * counter / totalVariations), "Loading failed (" + descriptor.Name + ")");
          }
        }
      }
      if (localExperiment != null) localExperiment.Prepare(true);
      Experiment = localExperiment;
    }

    private IEnumerable<IOptimizer> experimentCreationBackgroundWorker_CalculateParameterVariations(IOptimizer optimizer) {
      if (!boolParameters.Any() && !intParameters.Any() && !doubleParameters.Any() && !multipleChoiceParameters.Any()) {
        yield return (IOptimizer)optimizer.Clone();
        yield break;
      }
      bool finished;
      var mcEnumerator = GetMultipleChoiceConfigurations().GetEnumerator();
      var boolEnumerator = GetBoolParameterConfigurations().GetEnumerator();
      var intEnumerator = GetIntParameterConfigurations().GetEnumerator();
      var doubleEnumerator = GetDoubleParameterConfigurations().GetEnumerator();
      mcEnumerator.MoveNext(); boolEnumerator.MoveNext(); intEnumerator.MoveNext(); doubleEnumerator.MoveNext();
      do {
        var variant = (IAlgorithm)optimizer.Clone();
        variant.Name += " {";
        finished = true;
        if (doubleParameters.Any()) {
          foreach (var d in doubleEnumerator.Current) {
            var value = (ValueTypeValue<double>)((IValueParameter)variant.Parameters[d.Key.Name]).Value;
            value.Value = d.Value;
            variant.Name += d.Key.Name + "=" + d.Value.ToString() + ", ";
          }
          if (finished) {
            if (doubleEnumerator.MoveNext()) {
              finished = false;
            } else {
              doubleEnumerator = GetDoubleParameterConfigurations().GetEnumerator();
              doubleEnumerator.MoveNext();
            }
          }
        }
        if (intParameters.Any()) {
          foreach (var i in intEnumerator.Current) {
            var value = (ValueTypeValue<int>)((IValueParameter)variant.Parameters[i.Key.Name]).Value;
            value.Value = i.Value;
            variant.Name += i.Key.Name + "=" + i.Value.ToString() + ", ";
          }
          if (finished) {
            if (intEnumerator.MoveNext()) {
              finished = false;
            } else {
              intEnumerator = GetIntParameterConfigurations().GetEnumerator();
              intEnumerator.MoveNext();
            }
          }
        }
        if (boolParameters.Any()) {
          foreach (var b in boolEnumerator.Current) {
            var value = (ValueTypeValue<bool>)((IValueParameter)variant.Parameters[b.Key.Name]).Value;
            value.Value = b.Value;
            variant.Name += b.Key.Name + "=" + b.Value.ToString() + ", ";
          }
          if (finished) {
            if (boolEnumerator.MoveNext()) {
              finished = false;
            } else {
              boolEnumerator = GetBoolParameterConfigurations().GetEnumerator();
              boolEnumerator.MoveNext();
            }
          }
        }
        if (multipleChoiceParameters.Any()) {
          foreach (var m in mcEnumerator.Current) {
            dynamic variantParam = variant.Parameters[m.Key.Name];
            var variantEnumerator = ((IEnumerable<object>)variantParam.ValidValues).GetEnumerator();
            var originalEnumerator = ((IEnumerable<object>)((dynamic)m.Key).ValidValues).GetEnumerator();
            while (variantEnumerator.MoveNext() && originalEnumerator.MoveNext()) {
              if (m.Value == (INamedItem)originalEnumerator.Current) {
                variantParam.Value = (dynamic)variantEnumerator.Current;
                variant.Name += m.Key.Name + "=" + m.Value.Name + ", ";
                break;
              }
            }
          }
          if (finished) {
            if (mcEnumerator.MoveNext()) {
              finished = false;
            } else {
              mcEnumerator = GetMultipleChoiceConfigurations().GetEnumerator();
              mcEnumerator.MoveNext();
            }
          }
        }
        variant.Name = variant.Name.Substring(0, variant.Name.Length - 2) + "}";
        yield return variant;
      } while (!finished);
    }

    private void experimentCreationBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
      experimentCreationProgressBar.Value = e.ProgressPercentage;
      Application.DoEvents();
    }

    private void experimentCreationBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      try {
        SetMode(DialogMode.Normal);
        if (e.Error != null) MessageBox.Show(e.Error.Message, "Error occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (failedInstances.Length > 0) MessageBox.Show("Some instances could not be loaded: " + Environment.NewLine + failedInstances.ToString(), "Some instances failed to load", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (!e.Cancelled && e.Error == null) {
          DialogResult = System.Windows.Forms.DialogResult.OK;
          Close();
        }
      } catch { }
    }
    #endregion
    #endregion
  }
}
