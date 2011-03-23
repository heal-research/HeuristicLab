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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers {
  /// <summary>
  /// An operator that outputs the tree size of a symbolic expression tree.
  /// </summary>
  [Item("SymbolicExpressionTreeSizeCalculator", "An operator that outputs the tree size of a symbolic expression tree.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class SymbolicExpressionTreeSizeCalculator : SingleSuccessorOperator {
    private const string SymbolicExpressionTreeParameterName = "SymbolicExpressionTree";
    private const string SymbolicExpressionTreeSizeParameterName = "SymbolicExpressionTreeSize";

    #region parameter properties
    public ILookupParameter<SymbolicExpressionTree> SymbolicExpressionTreeParameter {
      get { return (ILookupParameter<SymbolicExpressionTree>)Parameters[SymbolicExpressionTreeParameterName]; }
    }
    public ILookupParameter<DoubleValue> SymbolicExpressionTreeSizeParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters[SymbolicExpressionTreeSizeParameterName]; }
    }
    #endregion

    #region properties
    public SymbolicExpressionTree SymbolicExpressionTree {
      get { return SymbolicExpressionTreeParameter.ActualValue; }
    }
    public DoubleValue SymbolicExpressionTreeSize {
      get { return SymbolicExpressionTreeSizeParameter.ActualValue; }
      set { SymbolicExpressionTreeSizeParameter.ActualValue = value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicExpressionTreeSizeCalculator(bool deserializing) : base(deserializing) { }
    private SymbolicExpressionTreeSizeCalculator(SymbolicExpressionTreeSizeCalculator original, Cloner cloner) : base(original, cloner) { }
    public SymbolicExpressionTreeSizeCalculator()
      : base() {
      Parameters.Add(new LookupParameter<SymbolicExpressionTree>(SymbolicExpressionTreeParameterName, "The symbolic expression tree whose size should be calculated."));
      Parameters.Add(new LookupParameter<DoubleValue>(SymbolicExpressionTreeSizeParameterName, "The tree size of the symbolic expression tree."));
    }

    public override IOperation Apply() {
      SymbolicExpressionTree tree = SymbolicExpressionTree;
      SymbolicExpressionTreeSize = new DoubleValue(tree.Size);
      return base.Apply();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeSizeCalculator(this, cloner);
    }
  }
}
