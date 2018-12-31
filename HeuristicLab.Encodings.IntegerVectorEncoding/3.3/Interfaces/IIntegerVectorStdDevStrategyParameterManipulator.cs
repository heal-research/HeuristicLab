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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  [StorableType("c2b76c64-9e5d-4ecb-b692-11cdf697ba02")]
  public interface IIntegerVectorStdDevStrategyParameterManipulator : IIntegerVectorStdDevStrategyParameterOperator, IStrategyParameterManipulator {
    ILookupParameter<DoubleArray> StrategyParameterParameter { get; }
    IValueLookupParameter<DoubleMatrix> BoundsParameter { get; }
  }
}
