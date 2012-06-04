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
using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.Instances.Views {
  [View("ProblemInstanceProviderViewGeneric")]
  [Content(typeof(IProblemInstanceProvider<>), IsDefaultView = true)]
  public partial class ProblemInstanceProviderViewGeneric<T> : ProblemInstanceProviderView {

    public new IProblemInstanceProvider<T> Content {
      get { return (IProblemInstanceProvider<T>)base.Content; }
      set { base.Content = value; }
    }

    private IProblemInstanceConsumer<T> GenericConsumer { get { return Consumer as IProblemInstanceConsumer<T>; } }

    public IProblemInstanceConsumer consumer;
    public override IProblemInstanceConsumer Consumer {
      get { return consumer; }
      set {
        consumer = value;
        SetEnabledStateOfControls();
      }
    }

    public ProblemInstanceProviderViewGeneric() {
      InitializeComponent();
      loadButton.Text = String.Empty;
      loadButton.Image = VSImageLibrary.RefreshDocument;
      toolTip.SetToolTip(loadButton, "Load the selected problem.");
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        instancesComboBox.DataSource = null;
      } else {
        instancesComboBox.DisplayMember = "Name";
        IEnumerable<IDataDescriptor> dataDescriptors = Content.GetDataDescriptors().ToList();
        ShowInstanceLoad(dataDescriptors.Count() > 0);
        instancesComboBox.DataSource = dataDescriptors;
      }
    }

    protected void ShowInstanceLoad(bool show) {
      if (show) {
        instanceLabel.Show();
        instancesComboBox.Show();
        loadButton.Show();
      } else {
        instanceLabel.Hide();
        instancesComboBox.Hide();
        loadButton.Hide();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      instancesComboBox.Enabled = !ReadOnly && !Locked && Content != null && GenericConsumer != null;
      loadButton.Enabled = !ReadOnly && !Locked && Content != null && GenericConsumer != null;
    }

    protected virtual void loadButton_Click(object sender, EventArgs e) {
      var descriptor = (IDataDescriptor)instancesComboBox.SelectedItem;
      T instance = Content.LoadData(descriptor);
      try {
        GenericConsumer.Load(instance);
      }
      catch (Exception ex) {
        MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", descriptor.Name, Environment.NewLine + ex.Message), "Cannot load instance");
      }
    }

    private void instancesComboBox_DataSourceChanged(object sender, EventArgs e) {
      var comboBox = (ComboBox)sender;
      if (comboBox.DataSource == null)
        comboBox.Items.Clear();
    }
  }
}
