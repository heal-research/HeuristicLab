#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Views;
using HeuristicLab.Persistence.Default.Xml;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Views {

  [View("DataAnalysisSolution View")]
  [Content(typeof(DataAnalysisSolution), false)]
  public partial class DataAnalysisSolutionView : NamedItemCollectionView<IResult> {
    public DataAnalysisSolutionView() {
      InitializeComponent();
      viewHost.ViewsLabelVisible = false;
    }

    public new DataAnalysisSolution Content {
      get { return (DataAnalysisSolution)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      addButton.Enabled = false;
      removeButton.Enabled = false;
      if (Content == null) {
        exportButton.Enabled = false;
        loadProblemDataButton.Enabled = false;
      } else {
        exportButton.Enabled = !Locked;
        loadProblemDataButton.Enabled = !Locked;
      }
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ProblemDataChanged += new EventHandler(Content_ProblemDataChanged);
    }
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
      Content.ProblemDataChanged -= new EventHandler(Content_ProblemDataChanged);
    }
    private void Content_ProblemDataChanged(object sender, EventArgs e) {
      OnContentChanged();
    }

    protected override void OnContentChanged() {
      string selectedName = null;
      if ((itemsListView.SelectedItems.Count == 1) && (itemsListView.SelectedItems[0].Tag != null && itemsListView.SelectedItems[0].Tag is Type))
        selectedName = itemsListView.SelectedItems[0].Text;

      base.OnContentChanged();
      AddEvaluationViewTypes();

      //recover selection
      if (selectedName != null) {
        foreach (ListViewItem item in itemsListView.Items) {
          if (item.Tag != null && item.Tag is Type && item.Text == selectedName)
            item.Selected = true;
        }
      }
    }

    protected override IResult CreateItem() {
      return null;
    }

    protected virtual void AddEvaluationViewTypes() {
      if (Content != null && !Content.ProblemData.IsEmpty) {
        var viewTypes = MainFormManager.GetViewTypes(Content.GetType(), true)
          .Where(t => typeof(IDataAnalysisSolutionEvaluationView).IsAssignableFrom(t));
        foreach (var viewType in viewTypes)
          AddViewListViewItem(viewType, ((IDataAnalysisSolutionEvaluationView)Activator.CreateInstance(viewType)).ViewImage);
      }
    }

    protected override void itemsListView_DoubleClick(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count != 1) return;

      IResult result = itemsListView.SelectedItems[0].Tag as IResult;
      Type viewType = itemsListView.SelectedItems[0].Tag as Type;
      if (result != null) {
        IContentView view = MainFormManager.MainForm.ShowContent(result, typeof(ResultView));
        if (view != null) {
          view.ReadOnly = ReadOnly;
          view.Locked = Locked;
        }
      } else if (viewType != null) {
        MainFormManager.MainForm.ShowContent(Content, viewType);
      }
    }

    protected override void itemsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (itemsListView.SelectedItems.Count == 1 && itemsListView.SelectedItems[0].Tag is Type) {
        detailsGroupBox.Enabled = true;
        Type viewType = (Type)itemsListView.SelectedItems[0].Tag;
        viewHost.ViewType = viewType;
        viewHost.Content = Content;
      } else
        base.itemsListView_SelectedIndexChanged(sender, e);
    }

    protected virtual void loadProblemDataButton_Click(object sender, EventArgs e) {
      if (loadProblemDataFileDialog.ShowDialog(this) != DialogResult.OK) return;
      object hlFile = XmlParser.Deserialize(loadProblemDataFileDialog.FileName);

      IDataAnalysisProblemData problemData = null;
      if (hlFile is IDataAnalysisProblemData) {
        problemData = (IDataAnalysisProblemData)hlFile;
      } else if (hlFile is IDataAnalysisProblem) {
        problemData = ((IDataAnalysisProblem)hlFile).ProblemData;
      } else if (hlFile is IDataAnalysisSolution) {
        problemData = ((IDataAnalysisSolution)hlFile).ProblemData;
      }

      if (problemData == null) {
        ErrorHandling.ShowErrorDialog(this, new NullReferenceException("The problem data is null." + Environment.NewLine
                                      + "The .hl-file contains no DataAnalysisProblemData or DataAnylsisProblem."));
        return;
      }

      if (CheckCompatibilityOfProblemData(problemData)) {
        var solution = (IDataAnalysisSolution)Content.Clone();
        solution.ProblemData = problemData;
        solution.Name += " with loaded problem data (" + loadProblemDataFileDialog + ")";
        MainFormManager.MainForm.ShowContent(solution);
      }
    }

    private void exportButton_Click(object sender, EventArgs e) {
      var exporters = ApplicationManager.Manager.GetInstances<IDataAnalysisSolutionExporter>()
        .Where(exporter => exporter.Supports(Content)).ToArray();
      exportFileDialog.Filter = exporters.Skip(1)
        .Aggregate(exporters.First().FileTypeFilter, (s, exporter) => s + "|" + exporter.FileTypeFilter);
      var result = exportFileDialog.ShowDialog();
      if (result == DialogResult.OK) {

        var name = exportFileDialog.FileName;
        var selectedExporter = exporters.Single(exporter => exporter.FileTypeFilter == exportFileDialog.Filter);

        using (BackgroundWorker bg = new BackgroundWorker()) {
          MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, "Exportion solution to " + name + ".");
          bg.DoWork += (_, __) => selectedExporter.Export(Content, name);
          bg.RunWorkerCompleted += (_, __) => MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
          bg.RunWorkerAsync();
        }
      }
    }

    protected void AddViewListViewItem(Type viewType, Image image) {
      ListViewItem listViewItem = new ListViewItem();
      listViewItem.Text = ViewAttribute.GetViewName(viewType);
      itemsListView.SmallImageList.Images.Add(image);
      listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      listViewItem.Tag = viewType;
      itemsListView.Items.Add(listViewItem);

      AdjustListViewColumnSizes();
    }

    protected void RemoveViewListViewItem(Type viewType) {
      List<ListViewItem> itemsToRemove = itemsListView.Items.Cast<ListViewItem>().Where(item => item.Tag as Type == viewType).ToList();

      foreach (ListViewItem item in itemsToRemove)
        itemsListView.Items.Remove(item);
    }

    protected override void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (showDetailsCheckBox.Checked && itemsListView.SelectedItems.Count == 1 && itemsListView.SelectedItems[0].Tag is Type) {
        Type viewType = (Type)itemsListView.SelectedItems[0].Tag;
        viewHost.ViewType = viewType;
        viewHost.Content = Content;
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = true;
      } else base.showDetailsCheckBox_CheckedChanged(sender, e);
    }

    protected override void RebuildImageList() {
      itemsListView.SmallImageList.Images.Clear();
      foreach (ListViewItem listViewItem in itemsListView.Items) {
        IResult result = listViewItem.Tag as IResult;
        Type viewType = listViewItem.Tag as Type;
        if (result != null) itemsListView.SmallImageList.Images.Add(result.ItemImage);
        else if (viewType != null && typeof(IDataAnalysisSolutionEvaluationView).IsAssignableFrom(viewType))
          itemsListView.SmallImageList.Images.Add(((IDataAnalysisSolutionEvaluationView)Activator.CreateInstance(viewType)).ViewImage);
        else itemsListView.SmallImageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Nothing);

        listViewItem.ImageIndex = itemsListView.SmallImageList.Images.Count - 1;
      }
    }

    #region drag and drop
    protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (ReadOnly) return;

      var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      if (dropData is DataAnalysisProblemData) validDragOperation = true;
      else if (dropData is IDataAnalysisProblem) validDragOperation = true;
      else if (dropData is IValueParameter) {
        var param = (IValueParameter)dropData;
        if (param.Value is IDataAnalysisProblemData) validDragOperation = true;
      }
    }

    protected override void itemsListView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect == DragDropEffects.None) return;

      IDataAnalysisProblemData problemData = null;
      var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      if (dropData is IDataAnalysisProblemData)
        problemData = (IDataAnalysisProblemData)dropData;
      else if (dropData is IDataAnalysisProblem)
        problemData = ((IDataAnalysisProblem)dropData).ProblemData;
      else if (dropData is IValueParameter) {
        var param = (IValueParameter)dropData;
        problemData = param.Value as DataAnalysisProblemData;
      }
      if (problemData == null) return;
      CheckCompatibilityOfProblemData(problemData);
      Content.ProblemData = (IDataAnalysisProblemData)problemData.Clone();
    }
    #endregion

    #region load problem data
    protected virtual bool CheckCompatibilityOfProblemData(IDataAnalysisProblemData problemData) {
      StringBuilder message = new StringBuilder();
      List<string> variables = problemData.InputVariables.Select(x => x.Value).ToList();
      foreach (var item in Content.ProblemData.InputVariables.CheckedItems) {
        if (!variables.Contains(item.Value.Value))
          message.AppendLine("Input variable '" + item.Value.Value + "' is not in the new problem data.");
      }

      if (message.Length != 0) {
        ErrorHandling.ShowErrorDialog(this, new InvalidOperationException(message.ToString()));
        return false;
      }
      return true;
    }
    #endregion
  }
}
