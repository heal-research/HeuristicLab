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
using System.IO;
using System.Windows.Forms;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  [View("Classification InstanceProvider View")]
  [Content(typeof(ClassificationInstanceProvider), IsDefaultView = true)]
  public partial class ClassificationInstanceProviderView : DataAnalysisInstanceProviderView<IClassificationProblemData> {

    public new ClassificationInstanceProvider Content {
      get { return (ClassificationInstanceProvider)base.Content; }
      set { base.Content = value; }
    }

    public ClassificationInstanceProviderView() {
      InitializeComponent();
    }

    protected override void importButton_Click(object sender, EventArgs e) {
      var importTypeDialog = new ClassificationImportTypeDialog();
      if (importTypeDialog.ShowDialog() == DialogResult.OK) {
        IClassificationProblemData instance = null;
        try {
          instance = Content.ImportData(importTypeDialog.Path, importTypeDialog.ImportType, importTypeDialog.CSVFormat);
        } catch (IOException ex) {
          ErrorWhileParsing(ex);
          return;
        }
        try {
          GenericConsumer.Load(instance);
          instancesComboBox.SelectedIndex = -1;
        } catch (IOException ex) {
          ErrorWhileLoading(ex, importTypeDialog.Path);
        }
      }
    }
  }
}
