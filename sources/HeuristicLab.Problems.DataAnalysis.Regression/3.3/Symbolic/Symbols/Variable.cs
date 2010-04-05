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
  [Item("Variable", "Represents a variable value.")]
  public sealed class Variable : Symbol {
    #region Parameter Properties
    public IValueParameter<DoubleValue> WeightNuParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["WeightNu"]; }
    }
    public IValueParameter<DoubleValue> WeightSigmaParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["WeightSigma"]; }
    }
    public IValueParameter<ItemList<StringValue>> VariableNamesParameter {
      get { return (IValueParameter<ItemList<StringValue>>)Parameters["VariableNames"]; }
    }
    #endregion
    #region Properties
    public DoubleValue WeightNu {
      get { return WeightNuParameter.Value; }
      set { WeightNuParameter.Value = value; }
    }
    public DoubleValue WeightSigma {
      get { return WeightSigmaParameter.Value; }
      set { WeightSigmaParameter.Value = value; }
    }
    public ItemList<StringValue> VariableNames {
      get { return VariableNamesParameter.Value; }
      set { VariableNamesParameter.Value = value; }
    }
    #endregion
    public Variable()
      : base() {
      Parameters.Add(new ValueParameter<DoubleValue>("WeightNu", "The mean value for the initialization of weight ((N(nu, sigma)).", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<DoubleValue>("WeightSigma", "The sigma value for the initialization of weight (N(nu, sigma))", new DoubleValue(1.0)));
      Parameters.Add(new ValueParameter<ItemList<StringValue>>("VariableNames", "The list of possible variable names for initialization."));
    }

    public override SymbolicExpressionTreeNode CreateTreeNode() {
      return new VariableTreeNode(this);
    }
  }
}
