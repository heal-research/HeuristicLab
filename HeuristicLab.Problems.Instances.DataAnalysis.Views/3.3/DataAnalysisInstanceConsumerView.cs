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
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.Instances.Views;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  [View("DataAnalysis InstanceProvider View")]
  [Content(typeof(IProblemInstanceConsumer<IDataAnalysisProblemData>), IsDefaultView = true)]
  [Content(typeof(IProblemInstanceConsumer<IClusteringProblemData>), IsDefaultView = true)]
  public partial class DataAnalysisInstanceConsumerView<T> : ProblemInstanceConsumerViewGeneric<T>
    where T : class, IDataAnalysisProblemData {
    public new IProblemInstanceConsumer<T> Content {
      get { return (IProblemInstanceConsumer<T>)base.Content; }
      set { base.Content = value; }
    }

    public DataAnalysisInstanceConsumerView() {
      InitializeComponent();
    }

    protected override void importButton_Click(object sender, EventArgs e) {
      var provider = SelectedProvider as DataAnalysisInstanceProvider<T, DataAnalysisImportType>;
      if (provider != null) {
        DataAnalysisImportTypeDialog importTypeDialog = new DataAnalysisImportTypeDialog();
        if (importTypeDialog.ShowDialog() == DialogResult.OK) {
          T instance = default(T);
          try {
            instance = provider.ImportData(importTypeDialog.Path, importTypeDialog.ImportType, importTypeDialog.CSVFormat);
          }
          catch (IOException ex) {
            ErrorWhileParsing(ex);
            return;
          }
          try {
            GenericConsumer.Load(instance);
          }
          catch (IOException ex) {
            ErrorWhileLoading(ex, importTypeDialog.Path);
          }
        } else {
          return;
        }
      } else {
        base.importButton_Click(sender, e);
      }
    }

    protected void ErrorWhileParsing(Exception ex) {
      MessageBox.Show(String.Format("There was an error parsing the file: {0}", Environment.NewLine + ex.Message), "Error while parsing", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    protected void ErrorWhileLoading(Exception ex, string path) {
      MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(path), Environment.NewLine + ex.Message), "Cannot load instance");
    }
  }
}
