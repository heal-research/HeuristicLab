#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using Netron.Diagramming.Core;

namespace HeuristicLab.Operators.Views.GraphVisualization.Views {
  internal static class ShapeInfoExtensions {
    public static IShape CreateShape(this IShapeInfo shapeInfo) {
      if (shapeInfo is IOperatorShapeInfo) return CreateShape((IOperatorShapeInfo)shapeInfo);
      throw new ArgumentException("Could not determine which shape object should be created.");
    }

    public static void UpdateShape(this IShapeInfo shapeInfo, IShape shape) {
      shape.Location = shapeInfo.Location;
      if (shapeInfo is IOperatorShapeInfo && shape is OperatorShape)
        UpdateShape((IOperatorShapeInfo)shapeInfo, (OperatorShape)shape);
    }

    public static void UpdateShapeInfo(this IShapeInfo shapeInfo, IShape shape) {
      shapeInfo.Location = shape.Location;
      if (shapeInfo is IOperatorShapeInfo && shape is OperatorShape)
        UpdateShapeInfo((IOperatorShapeInfo)shapeInfo, (OperatorShape)shape);
    }

    #region OperatorShapeInfo specific methos
    public static OperatorShape CreateShape(IOperatorShapeInfo shapeInfo) {
      OperatorShape shape = new OperatorShape();
      shape.Tag = shapeInfo;
      shape.Location = shapeInfo.Location;
      shape.Title = shapeInfo.Title;
      shape.Subtitle = shapeInfo.TypeName;
      shape.Color = shapeInfo.Color;
      shape.LineColor = shapeInfo.LineColor;
      shape.LineWidth = shapeInfo.LineWidth;
      shape.Icon = shapeInfo.Icon;
      shape.Collapsed = shapeInfo.Collapsed;
      foreach (string connectorName in shapeInfo.Connectors)
        if (connectorName != OperatorShapeInfoFactory.SuccessorConnector && connectorName != OperatorShapeInfoFactory.PredecessorConnector)
          shape.AddConnector(connectorName);

      shape.UpdateLabels(shapeInfo.Labels);
      return shape;
    }

    public static void UpdateShape(IOperatorShapeInfo operatorShapeInfo, OperatorShape operatorShape) {
      operatorShape.Title = operatorShapeInfo.Title;
      operatorShape.Subtitle = operatorShapeInfo.TypeName;
      operatorShape.Color = operatorShapeInfo.Color;
      operatorShape.LineColor = operatorShapeInfo.LineColor;
      operatorShape.LineWidth = operatorShapeInfo.LineWidth;
      operatorShape.Icon = operatorShapeInfo.Icon;
      operatorShape.Collapsed = operatorShapeInfo.Collapsed;

      int i = 0;
      int j = 0;
      //remove old connectors and skip correct connectors
      List<string> oldConnectorNames = operatorShape.AdditionalConnectorNames.ToList();
      while (i < operatorShapeInfo.Connectors.Count() && j < oldConnectorNames.Count) {
        if (operatorShapeInfo.Connectors.ElementAt(i) == OperatorShapeInfoFactory.SuccessorConnector ||
          operatorShapeInfo.Connectors.ElementAt(i) == OperatorShapeInfoFactory.PredecessorConnector)
          i++;
        else if (oldConnectorNames[j] == OperatorShapeInfoFactory.SuccessorConnector ||
          oldConnectorNames[j] == OperatorShapeInfoFactory.PredecessorConnector)
          j++;
        else if (operatorShapeInfo.Connectors.ElementAt(i) != oldConnectorNames[j]) {
          operatorShape.RemoveConnector(oldConnectorNames[j]);
          j++;
        } else {
          i++;
          j++;
        }
      }
      //remove remaining old connectors
      for (; j < oldConnectorNames.Count; j++)
        operatorShape.RemoveConnector(oldConnectorNames[j]);

      //add new connectors except successor and connector
      for (; i < operatorShapeInfo.Connectors.Count(); i++)
        if (operatorShapeInfo.Connectors.ElementAt(i) != OperatorShapeInfoFactory.SuccessorConnector &&
          operatorShapeInfo.Connectors.ElementAt(i) != OperatorShapeInfoFactory.PredecessorConnector)
          operatorShape.AddConnector(operatorShapeInfo.Connectors.ElementAt(i));

      operatorShape.UpdateLabels(operatorShapeInfo.Labels);
    }

    public static void UpdateShapeInfo(IOperatorShapeInfo operatorShapeInfo, OperatorShape operatorShape) {
      operatorShapeInfo.Title = operatorShape.Title;
      operatorShapeInfo.TypeName = operatorShape.Subtitle;
      operatorShapeInfo.Color = operatorShape.Color;
      operatorShapeInfo.LineColor = operatorShape.LineColor;
      operatorShapeInfo.LineWidth = operatorShape.LineWidth;
      operatorShapeInfo.Icon = operatorShape.Icon;
      operatorShapeInfo.Collapsed = operatorShape.Collapsed;
    }
    #endregion
  }
}
