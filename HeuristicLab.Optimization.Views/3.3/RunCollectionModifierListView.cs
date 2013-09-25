#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {

  [View("Modifier View")]
  [Content(typeof(ICheckedItemList<IRunCollectionModifier>), false)]
  public partial class RunCollectionModifiersListView : CheckedItemListView<IRunCollectionModifier> {

    public Action Evaluator { get; set; }

    public RunCollectionModifiersListView() {
      InitializeComponent();
      itemsGroupBox.Text = "RunCollection Modifiers";
    }

    protected override IRunCollectionModifier CreateItem() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select RunCollection Modifiers";
        typeSelectorDialog.TypeSelector.Caption = "Available Modifiers";
        typeSelectorDialog.TypeSelector.Configure(typeof(IRunCollectionModifier), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          return (IRunCollectionModifier)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
      return null;
    }

    private void evaluateButton_Click(object sender, EventArgs e) {
      var evaluator = Evaluator;
      evaluateButton.Enabled = false;
      var worker = new BackgroundWorker();
      worker.DoWork += (s, a) => evaluator();
      worker.RunWorkerCompleted += (s, a) => {
        evaluateButton.Enabled = Content != null;
        if (a.Error != null)
          new ErrorDialog("Evaluation Failed", a.Error).ShowDialog(ParentForm);
      };
      worker.RunWorkerAsync();
    }
  }
}
