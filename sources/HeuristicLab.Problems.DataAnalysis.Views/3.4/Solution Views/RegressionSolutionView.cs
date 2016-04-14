#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Data.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("RegressionSolution View")]
  [Content(typeof(RegressionSolutionBase), false)]
  public partial class RegressionSolutionView : DataAnalysisSolutionView {
    public RegressionSolutionView() {
      InitializeComponent();
    }

    public new RegressionSolutionBase Content {
      get { return (RegressionSolutionBase)base.Content; }
      set { base.Content = value; }
    }

    protected virtual void btnImpactCalculation_Click(object sender, EventArgs e) {
      var mainForm = (MainForm.WindowsForms.MainForm)MainFormManager.MainForm;
      var view = new StringConvertibleArrayView();
      view.Caption = Content.Name + " Variable Impacts";
      view.Show();

      Task.Factory.StartNew(() => {
        try {
          mainForm.AddOperationProgressToView(view, "Calculating variable impacts for " + Content.Name);

          var impacts = RegressionSolutionVariableImpactsCalculator.CalculateImpacts(Content);
          var impactArray = new DoubleArray(impacts.Select(i => i.Item2).ToArray());
          impactArray.ElementNames = impacts.Select(i => i.Item1);
          view.Content = (DoubleArray)impactArray.AsReadOnly();
        }
        finally {
          mainForm.RemoveOperationProgressFromView(view);
        }
      });
    }

    #region drag and drop
    protected override void itemsListView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (ReadOnly) return;

      var dropData = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      if (dropData is IRegressionProblemData) validDragOperation = true;
      else if (dropData is IRegressionProblem) validDragOperation = true;
      else if (dropData is IValueParameter) {
        var param = (IValueParameter)dropData;
        if (param.Value is RegressionProblemData) validDragOperation = true;
      }
    }
    #endregion
  }
}
