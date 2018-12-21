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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.Instances.Views {
  [View("ProblemInstanceProviderViewGeneric")]
  [Content(typeof(IProblemInstanceProvider<>), IsDefaultView = true)]
  public partial class ProblemInstanceProviderView<T> : ProblemInstanceProviderView {

    public new IProblemInstanceProvider<T> Content {
      get { return (IProblemInstanceProvider<T>)base.Content; }
      set { base.Content = value; }
    }

    #region Importer & Exporter
    protected IProblemInstanceConsumer<T> GenericConsumer { get { return Consumer as IProblemInstanceConsumer<T>; } }
    protected IProblemInstanceConsumer consumer;
    public override IProblemInstanceConsumer Consumer {
      get { return consumer; }
      set {
        consumer = value;
        SetEnabledStateOfControls();
        SetTooltip();
      }
    }

    protected IProblemInstanceExporter<T> GenericExporter { get { return Exporter as IProblemInstanceExporter<T>; } }
    protected IProblemInstanceExporter exporter;
    public override IProblemInstanceExporter Exporter {
      get { return exporter; }
      set {
        exporter = value;
        SetEnabledStateOfControls();
      }
    }
    #endregion

    public ProblemInstanceProviderView() {
      InitializeComponent();
      importButton.Text = String.Empty;
      importButton.Image = VSImageLibrary.Open;
      exportButton.Text = String.Empty;
      exportButton.Image = VSImageLibrary.SaveAs;
      libraryInfoButton.Text = String.Empty;
      libraryInfoButton.Image = VSImageLibrary.Help;
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        instancesComboBox.DataSource = null;
      } else {
        instancesComboBox.DisplayMember = "Name";
        var dataDescriptors = Content.GetDataDescriptors().ToList();
        ShowInstanceLoad(dataDescriptors.Any());
        instancesComboBox.DataSource = dataDescriptors;
        instancesComboBox.SelectedIndex = -1;
      }
      SetTooltip();
    }

    protected void ShowInstanceLoad(bool show) {
      if (show) {
        instanceLabel.Show();
        instancesComboBox.Show();
      } else {
        instanceLabel.Hide();
        instancesComboBox.Hide();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      instancesComboBox.Enabled = !ReadOnly && !Locked && Content != null && GenericConsumer != null;
      libraryInfoButton.Enabled = Content != null && Content.WebLink != null;
      importButton.Enabled = !ReadOnly && !Locked && Content != null && GenericConsumer != null && Content.CanImportData;
      splitContainer1.Panel1Collapsed = !importButton.Enabled;
      exportButton.Enabled = !ReadOnly && !Locked && Content != null && GenericExporter != null && Content.CanExportData;
      splitContainer2.Panel1Collapsed = !exportButton.Enabled;
    }

    private void instancesComboBox_DataSourceChanged(object sender, EventArgs e) {
      var comboBox = (ComboBox)sender;
      if (comboBox.DataSource == null)
        comboBox.Items.Clear();
      toolTip.SetToolTip(comboBox, String.Empty);
    }

    protected virtual void instancesComboBox_SelectionChangeCommitted(object sender, EventArgs e) {
      toolTip.SetToolTip(instancesComboBox, String.Empty);
      if (instancesComboBox.SelectedIndex >= 0) {
        var descriptor = (IDataDescriptor)instancesComboBox.SelectedItem;

        IContentView activeView = (IContentView)MainFormManager.MainForm.ActiveView;
        var content = activeView.Content;
        // lock active view and show progress bar
        Progress.Show(content, "Loading problem instance.", ProgressMode.Indeterminate);

        Task.Factory.StartNew(() => {
          T data;
          try {
            data = Content.LoadData(descriptor);
          } catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(String.Format("Could not load the problem instance {0}", descriptor.Name), ex);
            Progress.Hide(content);
            return;
          }
          try {
            GenericConsumer.Load(data);
          } catch (Exception ex) {
            ErrorHandling.ShowErrorDialog(String.Format("This problem does not support loading the instance {0}", descriptor.Name), ex);
          } finally {
            Progress.Hide(content);
          }
        });
      }
    }

    private void libraryInfoButton_Click(object sender, EventArgs e) {
      Process.Start(Content.WebLink.ToString());
    }

    protected virtual void importButton_Click(object sender, EventArgs e) {
      openFileDialog.FileName = Content.Name + " instance";
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        T instance = default(T);
        try {
          instance = Content.ImportData(openFileDialog.FileName);
        } catch (Exception ex) {
          MessageBox.Show(String.Format("There was an error parsing the file: {0}", Environment.NewLine + ex.Message), "Error while parsing", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        try {
          GenericConsumer.Load(instance);
          instancesComboBox.SelectedIndex = -1;
        } catch (Exception ex) {
          MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(openFileDialog.FileName), Environment.NewLine + ex.Message), "Cannot load instance");
        }
      }
    }

    protected virtual void exportButton_Click(object sender, EventArgs e) {
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          Content.ExportData(GenericExporter.Export(), saveFileDialog.FileName);
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    protected override void SetTooltip() {
      toolTip.SetToolTip(importButton, "Import a " + GetProblemType() + " from a file in the " + GetProviderFormatInfo() + " format.");
      toolTip.SetToolTip(exportButton, "Export currently loaded " + GetProblemType() + " to a file in the " + GetProviderFormatInfo() + " format.");
      if (Content != null && Content.WebLink != null)
        toolTip.SetToolTip(libraryInfoButton, "Browse to " + Content.WebLink);
      else toolTip.SetToolTip(libraryInfoButton, "Library does not have a web reference.");
    }
  }
}
