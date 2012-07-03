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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.Instances.Views {
  [View("ProblemInstanceConsumerView")]
  [Content(typeof(IProblemInstanceConsumer<>), IsDefaultView = true)]
  public partial class ProblemInstanceConsumerViewGeneric<T> : ProblemInstanceConsumerView {

    public new IProblemInstanceConsumer<T> Content {
      get { return (IProblemInstanceConsumer<T>)base.Content; }
      set { base.Content = value; }
    }

    protected IProblemInstanceProvider<T> GenericSelectedProvider { get { return SelectedProvider as IProblemInstanceProvider<T>; } }
    public IProblemInstanceProvider SelectedProvider { get; protected set; }

    #region Importer & Exporter
    protected IProblemInstanceConsumer<T> GenericConsumer { get { return Consumer as IProblemInstanceConsumer<T>; } }
    protected IProblemInstanceConsumer consumer;
    public IProblemInstanceConsumer Consumer {
      get { return consumer; }
      set {
        consumer = value;
        SetEnabledStateOfControls();
      }
    }

    protected IProblemInstanceExporter<T> GenericExporter { get { return Exporter as IProblemInstanceExporter<T>; } }
    protected IProblemInstanceExporter exporter;
    public IProblemInstanceExporter Exporter {
      get { return exporter; }
      set {
        exporter = value;
        SetEnabledStateOfControls();
      }
    }
    #endregion

    public ProblemInstanceConsumerViewGeneric() {
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
        problemInstanceProviders = null;
        problemInstanceProviderComboBox.DataSource = null;
      } else {
        problemInstanceProviderComboBox.DisplayMember = "Name";
        problemInstanceProviders = ProblemInstanceManager.GetProviders(Content);
        problemInstanceProviderComboBox.DataSource = ProblemInstanceProviders.OrderBy(x => x.Name).ToList();
      }
      SetEnabledStateOfControls();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      problemInstanceProviderComboBox.Enabled = !ReadOnly && !Locked && Content != null && problemInstanceProviderComboBox.Items.Count > 0;
      libraryInfoButton.Enabled = SelectedProvider != null && SelectedProvider.WebLink != null;
      importButton.Enabled = !ReadOnly && !Locked && Content != null && Consumer != null &&
                             GenericSelectedProvider != null && GenericSelectedProvider.CanImportData;
      ProviderImportSplitContainer.Panel2Collapsed = !importButton.Enabled;
      exportButton.Enabled = !ReadOnly && !Locked && Content != null && Exporter != null &&
                             GenericSelectedProvider != null && GenericSelectedProvider.CanExportData;
      ProviderExportSplitContainer.Panel2Collapsed = !exportButton.Enabled;
    }

    protected virtual void problemInstanceProviderComboBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex >= 0) {
        SelectedProvider = (IProblemInstanceProvider)problemInstanceProviderComboBox.SelectedItem;
        problemInstanceProviderViewHost.Content = SelectedProvider;
        ProblemInstanceProviderView view = (ProblemInstanceProviderView)problemInstanceProviderViewHost.ActiveView;
        consumer = Content;
        view.Consumer = Content;
        if (CheckForIProblemInstanceExporter(Content)) {
          exporter = (IProblemInstanceExporter)Content;
        }
        SetTooltip();
      } else {
        SelectedProvider = null;
      }
      SetEnabledStateOfControls();
    }

    protected bool CheckForIProblemInstanceExporter(IProblemInstanceConsumer content) {
      return Content.GetType().GetInterfaces()
                    .Any(x => x.Equals(typeof(IProblemInstanceExporter)));
    }

    private void libraryInfoButton_Click(object sender, EventArgs e) {
      if (problemInstanceProviderComboBox.SelectedIndex >= 0) {
        if (SelectedProvider != null && SelectedProvider.WebLink != null)
          Process.Start(SelectedProvider.WebLink.ToString());
      }
    }

    protected virtual void importButton_Click(object sender, EventArgs e) {
      openFileDialog.FileName = GetProblemType() + " instance";
      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        T instance = default(T);
        try {
          instance = GenericSelectedProvider.ImportData(openFileDialog.FileName);
        }
        catch (Exception ex) {
          MessageBox.Show(String.Format("There was an error parsing the file: {0}", Environment.NewLine + ex.Message), "Error while parsing", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        try {
          GenericConsumer.Load(instance);
        }
        catch (Exception ex) {
          MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(openFileDialog.FileName), Environment.NewLine + ex.Message), "Cannot load instance");
        }
      }
    }

    protected virtual void exportButton_Click(object sender, EventArgs e) {
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          GenericSelectedProvider.ExportData(GenericExporter.Export(), saveFileDialog.FileName);
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    protected string GetProblemType() {
      return SelectedProvider.Name;
    }

    #region ToolTip
    protected void SetTooltip() {
      toolTip.SetToolTip(problemInstanceProviderComboBox, GetProviderToolTip());
      toolTip.SetToolTip(importButton, "Open a " + GetProblemType() + " problem from file.");
      toolTip.SetToolTip(exportButton, "Export currently loaded " + GetProblemType() + " problem to a file.");
      if (SelectedProvider.WebLink != null)
        toolTip.SetToolTip(libraryInfoButton, "Browse to " + SelectedProvider.WebLink.ToString());
      else toolTip.SetToolTip(libraryInfoButton, "Library does not have a web reference.");
    }

    private string GetProviderToolTip() {
      var provider = SelectedProvider;
      string toolTip = provider.Name;

      if (!String.IsNullOrEmpty(provider.ReferencePublication)) {
        toolTip = toolTip
            + Environment.NewLine + Environment.NewLine
            + provider.ReferencePublication;
      }
      if (provider.WebLink != null) {
        toolTip = toolTip
            + Environment.NewLine
            + provider.WebLink.ToString();
      }

      return toolTip;
    }
    #endregion
  }
}
