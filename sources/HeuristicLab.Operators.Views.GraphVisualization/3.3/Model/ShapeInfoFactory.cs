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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using Netron.Diagramming.Core;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  public static class Factory {
    private static LinePenStyle connectionPenStyle;

    static Factory() {
      connectionPenStyle = new LinePenStyle();
      connectionPenStyle.EndCap = LineCap.ArrowAnchor;
    }

    public static IShapeInfo CreateShapeInfo(IOperator op) {
      IEnumerable<string> paramterNames = op.Parameters.Where(p => p is IValueParameter<IOperator> && p.Name != "Successor").Select(p => p.Name);
      OperatorShapeInfo operatorShapeInfo = new OperatorShapeInfo(paramterNames);
      operatorShapeInfo.Title = op.Name;
      operatorShapeInfo.Text = op.GetType().ToString();
      operatorShapeInfo.HeadColor = Color.LightBlue;

      return operatorShapeInfo;
    }

    public static IConnection CreateConnection(IConnector from, IConnector to) {
      Connection connection = new Connection(from.Point, to.Point);
      connection.From.AllowMove = false;
      connection.To.AllowMove = false;
      from.AttachConnector(connection.From);

      to.AttachConnector(connection.To);
      connection.PenStyle = connectionPenStyle;
      return connection;
    }
  }
}
