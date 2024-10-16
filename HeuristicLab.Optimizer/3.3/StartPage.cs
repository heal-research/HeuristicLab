#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimizer {
  [View("Start Page")]
  public partial class StartPage : HeuristicLab.MainForm.WindowsForms.View {
    private const string StandardProblemsGroupName = "Standard Problems";
    private const string DataAnalysisGroupName = "Data Analysis";
    private const string ScriptsGroupName = "Scripts";
    private const string UncategorizedGroupName = "Uncategorized";
    private const string SampleNamePrefix = "HeuristicLab.Optimizer.Documents.";
    private const string SampleNameSuffix = ".hl";

    private readonly Dictionary<ListViewGroup, List<string>> groupLookup = new Dictionary<ListViewGroup, List<string>>();
    private readonly ListViewGroup standardProblemsGroup = new ListViewGroup(StandardProblemsGroupName);
    private readonly ListViewGroup dataAnalysisGroup = new ListViewGroup(DataAnalysisGroupName);
    private readonly ListViewGroup scriptsGroup = new ListViewGroup(ScriptsGroupName);
    private readonly ListViewGroup uncategorizedGroup = new ListViewGroup(UncategorizedGroupName);

    private IProgress progress;

    public StartPage() {
      InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e) {
      base.OnInitialized(e);
      Assembly assembly = Assembly.GetExecutingAssembly();
      AssemblyFileVersionAttribute version = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).
                                             Cast<AssemblyFileVersionAttribute>().FirstOrDefault();
      titleLabel.Text = "HeuristicLab Optimizer";
      if (version != null) titleLabel.Text += " " + version.Version;

      try {
        using (Stream stream = assembly.GetManifestResourceStream(typeof(StartPage), "Documents.FirstSteps.rtf"))
          firstStepsRichTextBox.LoadFile(stream, RichTextBoxStreamType.RichText);
      } catch (Exception) { }

      samplesListView.Enabled = false;
      samplesListView.Groups.Add(standardProblemsGroup);
      samplesListView.Groups.Add(dataAnalysisGroup);
      samplesListView.Groups.Add(scriptsGroup);
      samplesListView.Groups.Add(uncategorizedGroup);
      FillGroupLookup();

      showStartPageCheckBox.Checked = Properties.Settings.Default.ShowStartPage;

      ThreadPool.QueueUserWorkItem(new WaitCallback(LoadSamples));
    }

    protected override void OnClosing(FormClosingEventArgs e) {
      base.OnClosing(e);
      if (e.CloseReason == CloseReason.UserClosing) {
        e.Cancel = true;
        this.Hide();
      }
    }

    private void LoadSamples(object state) {
      progress = Progress.ShowOnControl(samplesListView, "Loading...");
      var samplesWithErrors = new List<(string SampleName, Exception Exception)>();
      try {
        var assembly = Assembly.GetExecutingAssembly();
        var samples = assembly.GetManifestResourceNames().Where(x => x.EndsWith(SampleNameSuffix));
        int count = samples.Count();

        foreach (var entry in groupLookup) {
          var group = entry.Key;
          var sampleList = entry.Value;
          foreach (var sampleName in sampleList) {
            string resourceName = SampleNamePrefix + sampleName + SampleNameSuffix;
            try {
              LoadSample(resourceName, assembly, group, count);
            } catch (Exception exc) {
              samplesWithErrors.Add((sampleName, exc));
            }
          }
        }

        var categorizedSamples = groupLookup.Select(x => x.Value).SelectMany(x => x).Select(x => SampleNamePrefix + x + SampleNameSuffix);
        var uncategorizedSamples = samples.Except(categorizedSamples);

        foreach (var resourceName in uncategorizedSamples) {
          try {
            LoadSample(resourceName, assembly, uncategorizedGroup, count);
          } catch (Exception exc) {
            string sampleName = resourceName;
            if (sampleName.StartsWith(SampleNamePrefix)) sampleName = sampleName.Substring(SampleNamePrefix.Length);
            if (sampleName.EndsWith(SampleNameSuffix)) sampleName = sampleName.Substring(0, sampleName.Length - SampleNameSuffix.Length);
            samplesWithErrors.Add((sampleName, exc));
          }
        }

        OnAllSamplesLoaded();
      } finally {
        Progress.HideFromControl(samplesListView);
      }
      if (samplesWithErrors.Any()) {
        string errorMessage = "The following samples could not be loaded:" 
                              + Environment.NewLine
                              + string.Join(Environment.NewLine, samplesWithErrors.Select(t => t.SampleName));
        var exc = new AggregateException(samplesWithErrors.Select(t => t.Exception));
        ErrorHandling.ShowErrorDialog(samplesListView, errorMessage, exc);
      }
    }

    private void LoadSample(string name, Assembly assembly, ListViewGroup group, int count) {
      using (var stream = assembly.GetManifestResourceStream(name)) {
        var serializer = new ProtoBufSerializer();
        var item = (NamedItem)serializer.Deserialize(stream, false);
        OnSampleLoaded(item, group, 1.0 / count);
      }
    }

    private void FillGroupLookup() {
      var standardProblems = new List<string> { "ALPSGA_TSP", "ES_Griewank", "OSES_Griewank", "GA_Grouping", "GA_TSP", "GA_VRP", "GE_ArtificialAnt",
                "IslandGA_TSP", "LS_Knapsack", "PSO_Rastrigin", "RAPGA_JSSP",
                "SA_Rastrigin", "SGP_SantaFe", "GP_Multiplexer", "SGP_Robocode", "SS_VRP", "TS_TSP", "TS_VRP", "VNS_OP", "VNS_TSP", "GA_BPP"
        };
      groupLookup[standardProblemsGroup] = standardProblems;
      var dataAnalysisProblems = new List<string> { "ALPSGP_SymReg", "SGP_SymbClass", "SGP_SymbReg", "OSGP_SymReg", "OSGP_TimeSeries", 
                                                    "GE_SymbReg", "GPR", "GP_Shape_Constrained_Regression", "GP_Structure_Template_Regression" };
      groupLookup[dataAnalysisGroup] = dataAnalysisProblems;
      var scripts = new List<string> { "GA_QAP_Script", "GUI_Automation_Script", "OSGA_Rastrigin_Script",
                                       "GridSearch_RF_Classification_Script", "GridSearch_RF_Regression_Script",
                                       "GridSearch_SVM_Classification_Script", "GridSearch_SVM_Regression_Script" };
      groupLookup[scriptsGroup] = scripts;
    }

    private void OnSampleLoaded(INamedItem sample, ListViewGroup group, double progress) {
      if (InvokeRequired)
        Invoke(new Action<INamedItem, ListViewGroup, double>(OnSampleLoaded), sample, group, progress);
      else {
        ListViewItem item = new ListViewItem(new string[] { sample.Name, sample.Description }, group);
        item.ToolTipText = sample.ItemName + ": " + sample.ItemDescription;
        samplesListView.SmallImageList.Images.Add(sample.ItemImage);
        item.ImageIndex = samplesListView.SmallImageList.Images.Count - 1;
        item.Tag = sample;
        samplesListView.Items.Add(item);
        this.progress.ProgressValue += progress;
      }
    }
    private void OnAllSamplesLoaded() {
      if (InvokeRequired)
        Invoke(new Action(OnAllSamplesLoaded));
      else {
        samplesListView.Enabled = samplesListView.Items.Count > 0;
        if (samplesListView.Items.Count > 0) {
          for (int i = 0; i < samplesListView.Columns.Count; i++)
            samplesListView.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
      }
    }

    private void firstStepsRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e) {
      System.Diagnostics.Process.Start(e.LinkText);
    }

    private void samplesListView_DoubleClick(object sender, EventArgs e) {
      if (samplesListView.SelectedItems.Count == 1) {
        var mainForm = MainFormManager.GetMainForm<MainForm.WindowsForms.MainForm>();
        try {
          mainForm.SetWaitCursor();
          mainForm.ShowContent((IContent)((IItem)samplesListView.SelectedItems[0].Tag).Clone());
        } finally {
          mainForm.ResetWaitCursor();
        }

      }
    }
    private void samplesListView_ItemDrag(object sender, ItemDragEventArgs e) {
      ListViewItem listViewItem = (ListViewItem)e.Item;
      IItem item = (IItem)listViewItem.Tag;
      DataObject data = new DataObject();
      data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, item);
      DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy);
    }

    private void showStartPageCheckBox_CheckedChanged(object sender, EventArgs e) {
      Properties.Settings.Default.ShowStartPage = showStartPageCheckBox.Checked;
      Properties.Settings.Default.Save();
    }
  }
}
