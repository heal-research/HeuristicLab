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

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  [View("Classification InstanceProvider View")]
  [Content(typeof(IProblemInstanceConsumer<IClassificationProblemData>), IsDefaultView = true)]
  public partial class ClassificationInstanceConsumerView : DataAnalysisInstanceConsumerView<IClassificationProblemData> {
    public new IProblemInstanceConsumer<IClassificationProblemData> Content {
      get { return (IProblemInstanceConsumer<IClassificationProblemData>)base.Content; }
      set { base.Content = value; }
    }

    public ClassificationInstanceConsumerView() {
      InitializeComponent();
    }

    protected override void importButton_Click(object sender, EventArgs e) {
      var provider = SelectedProvider as ClassificationInstanceProvider;
      if (provider != null) {
        ClassificationImportTypeDialog importTypeDialog = new ClassificationImportTypeDialog();
        if (importTypeDialog.ShowDialog() == DialogResult.OK) {
          IClassificationProblemData instance = null;
          try {
            instance = provider.ImportData(importTypeDialog.Path, importTypeDialog.ImportType, importTypeDialog.CSVFormat);
          }
          catch (Exception ex) {
            MessageBox.Show(String.Format("There was an error parsing the file: {0}", Environment.NewLine + ex.Message), "Error while parsing", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
          try {
            GenericConsumer.Load(instance);
          }
          catch (Exception ex) {
            MessageBox.Show(String.Format("This problem does not support loading the instance {0}: {1}", Path.GetFileName(importTypeDialog.Path), Environment.NewLine + ex.Message), "Cannot load instance");
          }
        } else {
          return;
        }
      } else {
        base.importButton_Click(sender, e);
      }
    }
  }
}
