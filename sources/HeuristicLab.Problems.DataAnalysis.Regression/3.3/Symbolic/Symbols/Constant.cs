#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Core;
using HeuristicLab.Operators;
using HeuristicLab.Random;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Parameters;
namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Symbols {
  [StorableClass]
  [Item("Constant", "Represents a constant value.")]
  public sealed class Constant : Symbol {
    #region Parameter Properties
    public IValueParameter<DoubleValue> MinValueParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["MinValue"]; }
    }
    public IValueParameter<DoubleValue> MaxValueParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["MaxValue"]; }
    }
    #endregion
    #region Propeties
    public DoubleValue MinValue {
      get { return MinValueParameter.Value; }
      set { MinValueParameter.Value = value; }
    }
    public DoubleValue MaxValue {
      get { return MaxValueParameter.Value; }
      set { MaxValueParameter.Value = value; }
    }
    #endregion
    public Constant()
      : base() {
      Parameters.Add(new ValueParameter<DoubleValue>("MinValue", "The minimal value of the constant.", new DoubleValue(-20.0)));
      Parameters.Add(new ValueParameter<DoubleValue>("MaxValue", "The maximal value of the constant.", new DoubleValue(20.0)));
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new ConstantTreeNode(this);
    }
  }
}
