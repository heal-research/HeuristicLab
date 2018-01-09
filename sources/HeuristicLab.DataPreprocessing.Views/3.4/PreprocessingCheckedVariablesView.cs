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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Preprocessing Checked Variables View")]
  [Content(typeof(PreprocessingChartContent), false)]
  public abstract partial class PreprocessingCheckedVariablesView : ItemView {
    protected bool SuppressCheckedChangedUpdate = false;

    public new PreprocessingChartContent Content {
      get { return (PreprocessingChartContent)base.Content; }
      set { base.Content = value; }
    }

    protected PreprocessingCheckedVariablesView() {
      InitializeComponent();
    }

    protected bool IsVariableChecked(string name) {
      return Content.VariableItemList.CheckedItems.Any(x => x.Value.Value == name);
    }
    protected IList<string> GetCheckedVariables() {
      return Content.VariableItemList.CheckedItems.Select(i => i.Value.Value).ToList();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;

      variablesListView.ItemChecked -= variablesListView_ItemChecked;
      variablesListView.Items.Clear();
      foreach (var variable in Content.VariableItemList) {
        bool isInputTarget = Content.PreprocessingData.InputVariables.Contains(variable.Value)
          || Content.PreprocessingData.TargetVariable == variable.Value;
        variablesListView.Items.Add(new ListViewItem(variable.Value) {
          Tag = variable,
          Checked = IsVariableChecked(variable.Value),
          ForeColor = isInputTarget ? Color.Black : Color.Gray
        });
      }
      variablesListView.ItemChecked += variablesListView_ItemChecked;
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PreprocessingData.Changed += PreprocessingData_Changed;
      Content.VariableItemList.CheckedItemsChanged += CheckedItemsChanged;
    }

    protected override void DeregisterContentEvents() {
      Content.PreprocessingData.Changed -= PreprocessingData_Changed;
      Content.VariableItemList.CheckedItemsChanged -= CheckedItemsChanged;
      base.DeregisterContentEvents();
    }

    protected virtual void CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> checkedItems) {
      // sync listview
      foreach (var item in checkedItems.Items)
        variablesListView.Items[item.Index].Checked = Content.VariableItemList.ItemChecked(item.Value);
    }
    private void variablesListView_ItemChecked(object sender, ItemCheckedEventArgs e) {
      // sync checked item list
      var variable = (StringValue)e.Item.Tag;
      Content.VariableItemList.SetItemCheckedState(variable, e.Item.Checked);
    }

    private void PreprocessingData_Changed(object sender, DataPreprocessingChangedEventArgs e) {
      switch (e.Type) {
        case DataPreprocessingChangedEventType.DeleteColumn:
          RemoveVariable(Content.PreprocessingData.GetVariableName(e.Column));
          break;
        case DataPreprocessingChangedEventType.AddColumn:
          AddVariable(Content.PreprocessingData.GetVariableName(e.Column));
          break;
        case DataPreprocessingChangedEventType.ChangeColumn:
        case DataPreprocessingChangedEventType.ChangeItem:
          UpdateVariable(Content.PreprocessingData.GetVariableName(e.Column));
          break;
        default:
          ResetAllVariables();
          break;
      }
    }

    protected virtual void AddVariable(string name) {
      Content.VariableItemList.Add(new StringValue(name));
      if (!Content.PreprocessingData.InputVariables.Contains(name) && Content.PreprocessingData.TargetVariable != name) {
        var listViewItem = variablesListView.FindItemWithText(name, false, 0, false);
        listViewItem.ForeColor = Color.LightGray;
      }
    }
    protected virtual void RemoveVariable(string name) {
      var stringValue = Content.VariableItemList.SingleOrDefault(n => n.Value == name);
      if (stringValue != null)
        Content.VariableItemList.Remove(stringValue);
    }
    protected virtual void UpdateVariable(string name) { }
    protected virtual void ResetAllVariables() { }
    protected virtual void CheckedChangedUpdate() { }

    private void checkInputsTargetButton_Click(object sender, System.EventArgs e) {
      SuppressCheckedChangedUpdate = true;
      foreach (var name in Content.VariableItemList) {
        var isInputTarget = Content.PreprocessingData.InputVariables.Contains(name.Value) || Content.PreprocessingData.TargetVariable == name.Value;
        Content.VariableItemList.SetItemCheckedState(name, isInputTarget);
      }
      SuppressCheckedChangedUpdate = false;
      CheckedChangedUpdate();
    }

    private void checkAllButton_Click(object sender, System.EventArgs e) {
      SuppressCheckedChangedUpdate = true;
      foreach (var name in Content.VariableItemList) {
        Content.VariableItemList.SetItemCheckedState(name, true);
      }
      SuppressCheckedChangedUpdate = false;
      CheckedChangedUpdate();
    }

    private void uncheckAllButton_Click(object sender, System.EventArgs e) {
      SuppressCheckedChangedUpdate = true;
      foreach (var name in Content.VariableItemList) {
        Content.VariableItemList.SetItemCheckedState(name, false);
      }
      SuppressCheckedChangedUpdate = false;
      CheckedChangedUpdate();
    }
  }
}


