#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.Parameters.Views;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  [View("ResultParameter View")]
  [Content(typeof(ResultParameter<>), true)]
  [Content(typeof(IResultParameter<>), false)]
  public partial class ResultParameterView<T> : LookupParameterView<T> where T : class, IItem {
    protected TypeSelectorDialog typeSelectorDialog;

    public new IResultParameter<T> Content {
      get { return (IResultParameter<T>)base.Content; }
      set { base.Content = value; }
    }

    public ResultParameterView() {
      InitializeComponent();
      setDefaultValueButton.Text = string.Empty;
      setDefaultValueButton.Image = VSImageLibrary.Edit;
      removeDefaultValueButton.Text = string.Empty;
      removeDefaultValueButton.Image = VSImageLibrary.Remove;
      actualNameLabel.Text = "Result Name:";
      dataTypeLabel.Text = "Result Type:";
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    protected override void DeregisterContentEvents() {
      Content.ResultCollectionNameChanged -= ContentOnResultCollectionNameChanged;
      Content.DefaultValueChanged -= ContentOnDefaultValueChanged;
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ResultCollectionNameChanged += ContentOnResultCollectionNameChanged;
      Content.DefaultValueChanged += ContentOnDefaultValueChanged;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        resultCollectionNameTextBox.Text = "-";
        defaultValueViewHost.Content = null;
      } else {
        resultCollectionNameTextBox.Text = Content.ResultCollectionName;
        defaultValueViewHost.Content = Content.DefaultValue;
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      resultCollectionNameTextBox.Enabled = Content != null;
      resultCollectionNameTextBox.ReadOnly = ReadOnly;
      setDefaultValueButton.Enabled = Content != null && !ReadOnly && !Locked;
      removeDefaultValueButton.Enabled = Content != null && !ReadOnly && !Locked;
    }

    private void ContentOnDefaultValueChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)ContentOnDefaultValueChanged, sender, e);
      else defaultValueViewHost.Content = Content.DefaultValue;
    }

    private void ContentOnResultCollectionNameChanged(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)ContentOnResultCollectionNameChanged, sender, e);
      else resultCollectionNameTextBox.Text = Content.ResultCollectionName;
    }

    private void resultNameTextBox_Validated(object sender, EventArgs e) {
      if (InvokeRequired) Invoke((Action<object, EventArgs>)resultNameTextBox_Validated, sender, e);
      else Content.ResultCollectionName = resultCollectionNameTextBox.Text;
    }

    private void setDefaultValueButton_Click(object sender, EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Value";
        typeSelectorDialog.TypeSelector.Configure(Content.DataType, false, true);
      }
      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.DefaultValue = (T)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void removeDefaultValueButton_Click(object sender, EventArgs e) {
      Content.DefaultValue = null;
    }

    protected virtual void defaultValueGroupBox_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!ReadOnly && (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) != null) && Content.DataType.IsAssignableFrom(e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat).GetType())) {
        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    protected virtual void defaultValueGroupBox_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        T value = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as T;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) value = (T)value.Clone();
        Content.DefaultValue = value;
      }
    }
  }
}
