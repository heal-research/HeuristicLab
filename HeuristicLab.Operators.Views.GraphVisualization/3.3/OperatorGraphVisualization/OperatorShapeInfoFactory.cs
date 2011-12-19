#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  public static class OperatorShapeInfoFactory {
    public const string PredecessorConnector = "Predecessor";
    public const string SuccessorConnector = "Successor";
    public static IOperatorShapeInfo CreateOperatorShapeInfo(IOperator op) {
      IEnumerable<string> operatorParameterNames = op.Parameters.Where(p => p is IValueParameter && typeof(IOperator).IsAssignableFrom(p.DataType)).Select(p => p.Name);
      IEnumerable<string> paramaterNameValues = op.Parameters.Where(p => !(p is IValueParameter && typeof(IOperator).IsAssignableFrom(p.DataType))).Select(p => p.ToString());

      OperatorShapeInfo operatorShapeInfo = new OperatorShapeInfo(operatorParameterNames, paramaterNameValues);
      operatorShapeInfo.AddConnector(PredecessorConnector);
      operatorShapeInfo.Collapsed = true;
      operatorShapeInfo.Title = op.Name;
      operatorShapeInfo.TypeName = op.GetType().GetPrettyName();
      operatorShapeInfo.Color = Color.LightBlue;
      operatorShapeInfo.LineWidth = 1;
      operatorShapeInfo.LineColor = Color.Black;
      operatorShapeInfo.Icon = new Bitmap(op.ItemImage);

      return operatorShapeInfo;
    }
  }
}
