#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.BinPacking3D {
  [StorableType("d442d459-ad09-46df-8b3e-6164db9fb0e2")]
  public interface IOperator<TSol> : IItem {
    ILookupParameter<ReadOnlyItemList<PackingItem>> ItemsParameter { get; }
    ILookupParameter<PackingShape> BinShapeParameter { get; }
    ILookupParameter<IDecoder<TSol>> DecoderParameter { get; }
    ILookupParameter<IEvaluator> SolutionEvaluatorParameter { get; }
    ILookupParameter<BoolValue> UseStackingConstraintsParameter { get; }
  }
}
