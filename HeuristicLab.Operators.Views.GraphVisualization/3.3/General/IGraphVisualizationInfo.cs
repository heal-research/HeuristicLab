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
using HeuristicLab.Collections;
using HeuristicLab.Common;

namespace HeuristicLab.Operators.Views.GraphVisualization {
  public interface IGraphVisualizationInfo : IDeepCloneable, IContent {
    IShapeInfo InitialShape { get; set; }
    event EventHandler InitialShapeChanged;

    INotifyObservableCollectionItemsChanged<IShapeInfo> ObserveableShapeInfos { get; }
    IEnumerable<IShapeInfo> ShapeInfos { get; }

    INotifyObservableCollectionItemsChanged<IConnectionInfo> ObservableConnectionInfos { get; }
    IEnumerable<IConnectionInfo> ConnectionInfos { get; }

    void AddShapeInfo(IShapeInfo shapeInfo);
    void RemoveShapeInfo(IShapeInfo shapeInfo);
    void AddConnectionInfo(IConnectionInfo connectionInfo);
    void RemoveConnectionInfo(IConnectionInfo connectionInfo);

    IEnumerable<IConnectionInfo> GetConnectionInfos(IShapeInfo shapeInfo);
    IEnumerable<IConnectionInfo> GetConnectionInfos(IShapeInfo shapeInfo, string connector);
  }
}
