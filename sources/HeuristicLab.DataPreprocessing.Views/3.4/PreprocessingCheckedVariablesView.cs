#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      return checkedItemList.Content.CheckedItems.Select(i => i.Value.Value).ToList();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;

      if (Content.VariableItemList == null) {
        Content.VariableItemList = Content.CreateVariableItemList();
      } else {
        var checkedNames = Content.VariableItemList.CheckedItems.Select(x => x.Value.Value);
        Content.VariableItemList = Content.CreateVariableItemList(checkedNames);
      }
      Content.VariableItemList.CheckedItemsChanged += CheckedItemsChanged;

      checkedItemList.Content = Content.VariableItemList;
      var target = Content.PreprocessingData.TargetVariable;
      var inputAndTarget = Content.PreprocessingData.InputVariables.Union(target != null ? new[] { target } : new string[] { });
      foreach (var col in Content.PreprocessingData.GetDoubleVariableNames().Except(inputAndTarget)) {
        var listViewItem = checkedItemList.ItemsListView.FindItemWithText(col);
        listViewItem.ForeColor = Color.LightGray;
      }
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.PreprocessingData.Changed += PreprocessingData_Changed;
      Content.PreprocessingData.SelectionChanged += PreprocessingData_SelctionChanged;
    }
    protected override void DeregisterContentEvents() {
      Content.PreprocessingData.Changed -= PreprocessingData_Changed;
      Content.PreprocessingData.SelectionChanged -= PreprocessingData_SelctionChanged;
      base.DeregisterContentEvents();
    }

    protected virtual void CheckedItemsChanged(object sender, CollectionItemsChangedEventArgs<IndexedItem<StringValue>> checkedItems) {
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
        var listViewItem = checkedItemList.ItemsListView.FindItemWithText(name);
        listViewItem.ForeColor = Color.LightGray;
      }
    }
    protected virtual void RemoveVariable(string name) {
      var stringValue = Content.VariableItemList.SingleOrDefault(n => n.Value == name);
      if (stringValue != null)
        Content.VariableItemList.Remove(stringValue);
    }
    protected virtual void UpdateVariable(string name) {
    }
    protected virtual void ResetAllVariables() {
    }

    protected virtual void PreprocessingData_SelctionChanged(object sender, EventArgs e) {
    }

    #region ContextMenu Events
    private void variablesListcontextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
      var data = Content.PreprocessingData;
      checkInputsTargetToolStripMenuItem.Text = "Check Inputs" + (data.TargetVariable != null ? "+Target" : "");
      checkOnlyInputsTargetToolStripMenuItem.Text = "Check only Inputs" + (data.TargetVariable != null ? "+Target" : "");
    }
    private void checkInputsTargetToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var name in checkedItemList.Content) {
        var isInputTarget = Content.PreprocessingData.InputVariables.Contains(name.Value) || Content.PreprocessingData.TargetVariable == name.Value;
        if (isInputTarget) {
          checkedItemList.Content.SetItemCheckedState(name, true);
        }
      }
    }
    private void checkOnlyInputsTargetToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var name in checkedItemList.Content) {
        var isInputTarget = Content.PreprocessingData.InputVariables.Contains(name.Value) || Content.PreprocessingData.TargetVariable == name.Value;
        checkedItemList.Content.SetItemCheckedState(name, isInputTarget);
      }
    }
    private void checkAllToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var name in checkedItemList.Content) {
        checkedItemList.Content.SetItemCheckedState(name, true);
      }
    }
    private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e) {
      foreach (var name in checkedItemList.Content) {
        checkedItemList.Content.SetItemCheckedState(name, false);
      }
    }
    #endregion
  }
}


