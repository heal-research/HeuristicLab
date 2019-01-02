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
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [StorableType("32f2e082-9d02-4006-85d5-d2006cb47def")]
  public interface IDiscreteDoubleValueModifier : IOperator {
    ILookupParameter<DoubleValue> ValueParameter { get; }
    /// <summary>
    /// The start value of the parameter, will be assigned to <see cref="ValueParameter"/> as soon as <see cref="IndexParamter"/> equals <see cref="StartIndexParameter"/>.
    /// </summary>
    IValueLookupParameter<DoubleValue> StartValueParameter { get; }
    /// <summary>
    /// The end value of the parameter, will be assigned to <see cref="ValueParameter"/> as soon as <see cref="IndexParamter"/> equals <see cref="EndIndexParameter"/>.
    /// </summary>
    IValueLookupParameter<DoubleValue> EndValueParameter { get; }
    /// <summary>
    /// The index that denotes from which point in the function (relative to <see cref="StartIndexParameter"/> and <see cref="EndIndexParameter"/> the value should be assigned.
    /// </summary>
    ILookupParameter<IntValue> IndexParameter { get; }
    /// <summary>
    /// As soon as <see cref="IndexParameter"/> is &gt;= this parameter the value will start to be modified.
    /// </summary>
    IValueLookupParameter<IntValue> StartIndexParameter { get; }
    /// <summary>
    /// As long as <see cref="IndexParameter"/> is &lt;= this parameter the value will start to be modified.
    /// </summary>
    IValueLookupParameter<IntValue> EndIndexParameter { get; }
  }
}
