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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Optimization.Views {
  [View("ProblemInstanceProviderView")]
  [Content(typeof(IProblemInstanceProvider<>), IsDefaultView = true)]
  public partial class ProblemInstanceProviderView<T> : AsynchronousContentView {

    public new IProblemInstanceProvider<T> Content {
      get { return (IProblemInstanceProvider<T>)base.Content; }
      set { base.Content = value; }
    }

    public ProblemInstanceProviderView() {
      InitializeComponent();
      importButton.Text = String.Empty;
      importButton.Image = VSImageLibrary.Open;
      toolTip.SetToolTip(importButton, "Import a " + GetProblemType() + " instance from file.");
      loadButton.Text = String.Empty;
      loadButton.Image = VSImageLibrary.Checkout;
      toolTip.SetToolTip(loadButton, "Load the selected instance.");
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        instancesComboBox.DataSource = null;
      } else {
        instancesComboBox.DisplayMember = "Name";
        instancesComboBox.DataSource = Content.GetDataDescriptors().ToList();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      instancesComboBox.Enabled = !ReadOnly && !Locked && Content != null && Content.Consumer != null;
      loadButton.Enabled = !ReadOnly && !Locked && Content != null && Content.Consumer != null;
    }

    protected virtual void loadButton_Click(object sender, EventArgs e) {
      var descriptor = (IDataDescriptor)instancesComboBox.SelectedItem;
      var instance = Content.LoadData(descriptor);
      try {
        Content.Consumer.Load(instance);
      } catch (Exception ex) {
        MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", descriptor.Name, Environment.NewLine + ex.Message), "Cannot load instance");
      }
    }

    protected virtual void importButton_Click(object sender, EventArgs e) {
      openFileDialog.FileName = GetProblemType() + " instance";
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        T instance = default(T);
        try {
          instance = Content.LoadData(openFileDialog.FileName);
        } catch (Exception ex) {
          MessageBox.Show(String.Format("There was an error parsing the file: {0}", Environment.NewLine + ex.Message), "Error while parsing", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        try {
          Content.Consumer.Load(instance);
        } catch (Exception ex) {
          MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(openFileDialog.FileName), Environment.NewLine + ex.Message), "Cannot load instance");
        }
      }
    }

    private void comboBox_DataSourceChanged(object sender, EventArgs e) {
      var comboBox = (ComboBox)sender;
      if (comboBox.DataSource == null)
        comboBox.Items.Clear();
    }

    protected virtual string GetProblemType() {
      string dataTypeName = typeof(T).Name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
      if (dataTypeName.EndsWith("Data"))
        return dataTypeName.Substring(0, dataTypeName.Length - "Data".Length);
      else return dataTypeName;
    }
  }
}
