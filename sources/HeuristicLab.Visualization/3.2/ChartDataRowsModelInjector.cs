#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Drawing;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.Visualization {
  public class ChartDataRowsModelInjector : OperatorBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return @"Operator to test the new charting framework of the visualization project team."; }
    }

    public ChartDataRowsModelInjector() {
      AddVariableInfo(new VariableInfo("Model", "New data model which should be injected into a scope.", typeof(IChartDataRowsModel), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ChartDataRowsModel model = new ChartDataRowsModel();
      model.Title = "Solution Quality / Selection Pressure";
      model.XAxis.Label = "Generations";
      model.DefaultYAxis.Label = "Solution Quality";

      IDataRow dataRow;
      dataRow = new DataRow("Best Quality");
      dataRow.RowSettings.Color = Color.Green;
      dataRow.RowSettings.Thickness = 1;
      dataRow.RowSettings.Style = DrawingStyle.Solid;
      dataRow.RowSettings.ShowMarkers = true;
      model.AddDataRow(dataRow);

      dataRow = new DataRow("Average Quality");
      dataRow.RowSettings.Color = Color.Blue;
      dataRow.RowSettings.Thickness = 2;
      dataRow.RowSettings.Style = DrawingStyle.Dashed;
      dataRow.RowSettings.ShowMarkers = true;
      model.AddDataRow(dataRow);

      dataRow = new DataRow("Worst Quality");
      dataRow.RowSettings.Color = Color.Red;
      dataRow.RowSettings.Thickness = 1;
      dataRow.RowSettings.Style = DrawingStyle.Solid;
      dataRow.RowSettings.ShowMarkers = true;
      model.AddDataRow(dataRow);

      dataRow = new DataRow("Selection Pressure");
      dataRow.RowSettings.Color = Color.Gray;
      dataRow.RowSettings.Thickness = 1;
      dataRow.RowSettings.Style = DrawingStyle.Solid;
      dataRow.RowSettings.ShowMarkers = true;
      YAxisDescriptor yAxisDescriptor = new YAxisDescriptor();
      yAxisDescriptor.Label = "Selection Pressure";
      dataRow.YAxis = yAxisDescriptor;
      model.AddDataRow(dataRow);

      IVariableInfo modelInfo = GetVariableInfo("Model");
      if (modelInfo.Local) {
        this.AddVariable(new Variable(modelInfo.ActualName, model));
      } else {
        scope.AddVariable(new Variable(modelInfo.ActualName, model));
      }
      return null;
    }
  }
}
