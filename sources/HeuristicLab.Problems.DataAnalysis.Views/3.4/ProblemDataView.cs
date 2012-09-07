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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.Data;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Problem Data View")]
  [Content(typeof(DataAnalysisProblemData), true)]
  public partial class ProblemDataView : ParameterizedNamedItemView {

    public new DataAnalysisProblemData Content {
      get { return (DataAnalysisProblemData)base.Content; }
      set { base.Content = value; }
    }

    public ProblemDataView() {
      InitializeComponent();
    }

    protected void FeatureCorrelationButton_Click(object sender, System.EventArgs e) {
      ViewHost viewHost = new ViewHost();
      viewHost.Content = (DataAnalysisProblemData)this.Content.Clone();
      viewHost.ViewType = typeof(FeatureCorrelationView);
      viewHost.Show();
    }

    protected void parameterCollectionView_DragDrop(object sender, DragEventArgs e) {
      if (e.Effect != DragDropEffects.None) {
        var stringValueList = (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IFixedValueParameter).Value as ICheckedItemList<StringValue>;
        SetInputVariables(stringValueList);
      }
    }

    private void SetInputVariables(ICheckedItemList<StringValue> stringValueList) {
      var inputVariables = Content.InputVariables;
      var stringValues = stringValueList.Select(x => x.Value);
      var notContainedVariables = inputVariables.Where(x => !stringValues.Contains(x.Value));

      if (notContainedVariables.Count() != 0) {
        StringBuilder strBuilder = new StringBuilder();
        foreach (var variable in notContainedVariables) {
          strBuilder.Append(variable.Value + ", ");
        }
        strBuilder.Remove(strBuilder.Length - 2, 2);
        MessageBox.Show(String.Format("There was an error while changing the input variables. The following input " +
          "variables have not been contained {0}", strBuilder.ToString()), "Error while changing the input variables",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
      } else {
        var checkedItems = stringValueList.CheckedItems.Select(x => x.Value.Value);
        var setChecked = inputVariables.Where(x => checkedItems.Contains(x.Value));
        foreach (var variable in inputVariables) {
          if (setChecked.Contains(variable) && !inputVariables.ItemChecked(variable)) {
            inputVariables.SetItemCheckedState(variable, true);
          } else if (!setChecked.Contains(variable) && inputVariables.ItemChecked(variable)) {
            inputVariables.SetItemCheckedState(variable, false);
          }
        }
      }
    }

    protected void parameterCollectionView_DragEnterOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (ReadOnly)
        return;
      if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) == null)
        return;

      var parameter = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IFixedValueParameter;
      if (parameter == null)
        return;

      var stringValueList = parameter.Value as ICheckedItemList<StringValue>;
      if (stringValueList == null)
        return;

      e.Effect = e.AllowedEffect;
    }
  }
}
