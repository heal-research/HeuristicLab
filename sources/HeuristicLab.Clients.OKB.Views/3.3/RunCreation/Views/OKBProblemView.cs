#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [View("OKBProblem View")]
  [Content(typeof(SingleObjectiveOKBProblem), true)]
  [Content(typeof(MultiObjectiveOKBProblem), true)]
  public sealed partial class OKBProblemView : NamedItemView {
    private readonly CheckedItemList<ICharacteristicCalculator> calculatorList;

    public new OKBProblem Content {
      get { return (OKBProblem)base.Content; }
      set { base.Content = value; }
    }

    public OKBProblemView() {
      InitializeComponent();
      var calculatorListView = new CheckedItemListView<ICharacteristicCalculator>() {
        Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top,
        Location = new Point(flaSplitContainer.Padding.Left, calculateButton.Location.Y + calculateButton.Height + calculateButton.Padding.Bottom + 3),
      };
      calculatorListView.Size = new Size(flaSplitContainer.Panel1.Size.Width - flaSplitContainer.Panel1.Padding.Horizontal,
          flaSplitContainer.Panel1.Height - calculatorListView.Location.Y - flaSplitContainer.Panel1.Padding.Bottom);
      calculatorList = new CheckedItemList<ICharacteristicCalculator>();
      calculatorList.ItemsAdded += CalculatorListOnChanged;
      calculatorList.ItemsRemoved += CalculatorListOnChanged;
      calculatorList.ItemsReplaced += CalculatorListOnChanged;
      calculatorList.CollectionReset += CalculatorListOnChanged;
      calculatorList.CheckedItemsChanged += CalculatorListOnChanged;

      calculatorListView.Content = calculatorList.AsReadOnly();

      flaSplitContainer.Panel1.Controls.Add(calculatorListView);
      calculateButton.Text = string.Empty;
      calculateButton.Image = VSImageLibrary.Play;
      refreshButton.Text = string.Empty;
      refreshButton.Image = VSImageLibrary.Refresh;
      cloneProblemButton.Text = string.Empty;
      cloneProblemButton.Image = VSImageLibrary.Clone;
      downloadCharacteristicsButton.Text = string.Empty;
      downloadCharacteristicsButton.Image = VSImageLibrary.Refresh;
      uploadCharacteristicsButton.Text = string.Empty;
      uploadCharacteristicsButton.Image = VSImageLibrary.PublishToWeb;
    }

    private void CalculatorListOnChanged(object sender, EventArgs e) {
      SetEnabledStateOfControls();
    }

    protected override void OnInitialized(System.EventArgs e) {
      base.OnInitialized(e);
      RunCreationClient.Instance.Refreshing += new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed += new EventHandler(RunCreationClient_Refreshed);
      PopulateComboBox();
    }

    protected override void DeregisterContentEvents() {
      Content.ProblemChanged -= new EventHandler(Content_ProblemChanged);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ProblemChanged += new EventHandler(Content_ProblemChanged);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        problemComboBox.SelectedIndex = -1;
        parameterCollectionView.Content = null;
      } else {
        problemComboBox.SelectedItem = RunCreationClient.Instance.Problems.FirstOrDefault(x => x.Id == Content.ProblemId);
        parameterCollectionView.Content = Content.Parameters;
      }
      UpdateCharacteristicCalculators();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      problemComboBox.Enabled = (Content != null) && !ReadOnly && !Locked && (problemComboBox.Items.Count > 0);
      cloneProblemButton.Enabled = (Content != null) && (Content.ProblemId != -1) && !ReadOnly && !Locked;
      refreshButton.Enabled = (Content != null) && !ReadOnly && !Locked;
      parameterCollectionView.Enabled = Content != null;
      characteristicsMatrixView.Enabled = Content != null;
      downloadCharacteristicsButton.Enabled = Content != null && Content.ProblemId != -1 && !Locked;
      uploadCharacteristicsButton.Enabled = Content != null && Content.ProblemId != -1 && !Locked && !ReadOnly
        && characteristicsMatrixView.Content != null && characteristicsMatrixView.Content.Rows > 0;
      calculateButton.Enabled = Content != null && Content.ProblemId != -1 && !Locked && !ReadOnly && calculatorList.CheckedItems.Any();
    }

    protected override void OnClosed(FormClosedEventArgs e) {
      RunCreationClient.Instance.Refreshing -= new EventHandler(RunCreationClient_Refreshing);
      RunCreationClient.Instance.Refreshed -= new EventHandler(RunCreationClient_Refreshed);
      base.OnClosed(e);
    }

    private void UpdateCharacteristicCalculators() {
      calculatorList.Clear();
      if (Content == null || Content.ProblemId == -1) return;
      var problem = Content.CloneProblem();
      var calculators = ApplicationManager.Manager.GetInstances<ICharacteristicCalculator>().ToList();
      foreach (var calc in calculators) {
        calc.Problem = problem;
        if (!calc.CanCalculate()) continue;
        calculatorList.Add(calc, true);
      }
    }

    private void RunCreationClient_Refreshing(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshing), sender, e);
      } else {
        Cursor = Cursors.AppStarting;
        problemComboBox.Enabled = cloneProblemButton.Enabled = refreshButton.Enabled = parameterCollectionView.Enabled = false;
      }
    }
    private void RunCreationClient_Refreshed(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke(new EventHandler(RunCreationClient_Refreshed), sender, e);
      } else {
        PopulateComboBox();
        SetEnabledStateOfControls();
        Cursor = Cursors.Default;
      }
    }

    #region Content Events
    private void Content_ProblemChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_ProblemChanged), sender, e);
      else {
        OnContentChanged();
        SetEnabledStateOfControls();
      }
    }
    #endregion

    #region Control Events
    private void cloneProblemButton_Click(object sender, EventArgs e) {
      MainFormManager.MainForm.ShowContent(Content.CloneProblem());
    }
    private void refreshButton_Click(object sender, System.EventArgs e) {
      RunCreationClient.Instance.Refresh();
    }
    private void problemComboBox_SelectedValueChanged(object sender, System.EventArgs e) {
      Problem problem = problemComboBox.SelectedValue as Problem;
      if ((problem != null) && (Content != null)) {
        Content.Load(problem.Id);
        if (Content.ProblemId != problem.Id)  // reset selected item if load was not successful
          problemComboBox.SelectedItem = RunCreationClient.Instance.Problems.FirstOrDefault(x => x.Id == Content.ProblemId);
      }
    }
    private void downloadCharacteristicsButton_Click(object sender, EventArgs e) {
      var values = RunCreationClient.Instance.GetCharacteristicValues(Content.ProblemId).ToList();
      var content = new StringMatrix(values.Count, 3);
      for (var i = 0; i < values.Count; i++) {
        content[i, 0] = values[i].Name;
        content[i, 1] = values[i].GetValue();
        content[i, 2] = values[i].GetType().Name;
      }
      characteristicsMatrixView.Content = content;
      SetEnabledStateOfControls();
    }
    private void uploadCharacteristicsButton_Click(object sender, EventArgs e) {
      var matrix = characteristicsMatrixView.Content as StringMatrix;
      if (matrix == null) return;
      var values = new List<Value>(matrix.Rows);
      for (var i = 0; i < matrix.Rows; i++) {
        var name = matrix[i, 0];
        var strValue = matrix[i, 1];
        var type = matrix[i, 2];
        values.Add(Value.Create(name, strValue, type));
      }
      try {
        RunCreationClient.Instance.SetCharacteristicValues(Content.ProblemId, values);
      } catch (Exception ex) { ErrorHandling.ShowErrorDialog(ex); }
    }
    private void calculateButton_Click(object sender, EventArgs e) {
      var calculators = calculatorList.CheckedItems.Select(x => x.Value).Where(x => x.CanCalculate()).ToList();
      if (calculators.Count == 0) return;

      var results = new Dictionary<string, Value>();
      foreach (var calc in calculators) {
        foreach (var result in calc.Calculate())
          results[result.Name] = RunCreationClient.Instance.ConvertToValue(result.Value, result.Name);
      }
      var matrix = (characteristicsMatrixView.Content as StringMatrix) ?? (new StringMatrix(results.Count, 3));
      for (var i = 0; i < matrix.Rows; i++) {
        Value r;
        if (results.TryGetValue(matrix[i, 0], out r)) {
          matrix[i, 1] = r.GetValue();
          matrix[i, 2] = r.GetType().Name;
          results.Remove(matrix[i, 0]);
        }
      }
      if (results.Count == 0) return;
      var resultsList = results.ToList();
      var counter = resultsList.Count - 1;
      for (var i = 0; i < matrix.Rows; i++) {
        if (string.IsNullOrEmpty(matrix[i, 0])) {
          matrix[i, 0] = resultsList[counter].Key;
          matrix[i, 1] = resultsList[counter].Value.GetValue();
          matrix[i, 2] = resultsList[counter].Value.GetType().Name;
          resultsList.RemoveAt(counter);
          counter--;
          if (counter < 0) return;
        }
      }
      if (counter >= 0) {
        ((IStringConvertibleMatrix)matrix).Rows += counter + 1;
        for (var i = matrix.Rows - 1; counter >= 0; i--) {
          matrix[i, 0] = resultsList[0].Key;
          matrix[i, 1] = resultsList[0].Value.GetValue();
          matrix[i, 2] = resultsList[0].Value.GetType().Name;
          resultsList.RemoveAt(0);
          counter--;
        }
      }
      characteristicsMatrixView.Content = matrix;
      SetEnabledStateOfControls();
    }
    #endregion

    #region Helpers
    private void PopulateComboBox() {
      problemComboBox.DataSource = null;
      problemComboBox.DataSource = RunCreationClient.Instance.Problems.ToList();
      problemComboBox.DisplayMember = "Name";
    }
    #endregion

  }
}
