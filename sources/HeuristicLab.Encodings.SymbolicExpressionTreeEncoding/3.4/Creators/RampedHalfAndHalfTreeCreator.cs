#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding {
  [NonDiscoverableType]
  [StorableClass]
  [Item("RampedHalfAndHalfTreeCreator", "An operator that creates new symbolic expression trees in an alternate way: half the trees are created usign the 'Grow' method while the other half are created using the 'Full' method")]
  public class RampedHalfAndHalfTreeCreator : SymbolicExpressionTreeCreator,
                                 ISymbolicExpressionTreeSizeConstraintOperator,
                                 ISymbolicExpressionTreeGrammarBasedOperator {
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";

    #region Parameter Properties
    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }

    public IValueLookupParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }

    #endregion
    #region Properties
    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.ActualValue; }
    }

    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.ActualValue; }
    }
    #endregion

    [StorableConstructor]
    protected RampedHalfAndHalfTreeCreator(bool deserializing) : base(deserializing) { }
    protected RampedHalfAndHalfTreeCreator(RampedHalfAndHalfTreeCreator original, Cloner cloner) : base(original, cloner) { }

    public RampedHalfAndHalfTreeCreator()
      : base() {
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName,
        "The maximal length (number of nodes) of the symbolic expression tree (this parameter is ignored)."));
      Parameters.Add(new ValueLookupParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName,
        "The maximal depth of the symbolic expression tree (a tree with one node has depth = 0)."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new RampedHalfAndHalfTreeCreator(this, cloner);
    }

    protected override ISymbolicExpressionTree Create(IRandom random) {
      return Create(random, ClonedSymbolicExpressionTreeGrammarParameter.ActualValue, MaximumSymbolicExpressionTreeLength.Value, MaximumSymbolicExpressionTreeDepth.Value);
    }

    public override ISymbolicExpressionTree CreateTree(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth) {
      return Create(random, grammar, maxTreeLength, maxTreeDepth);
    }

    /// <summary>
    /// Create a symbolic expression tree using 'RampedHalfAndHalf' strategy.
    /// Half the trees are created with the 'Grow' method, and the other half are created with the 'Full' method.
    /// </summary>
    /// <param name="random">Random generator</param>
    /// <param name="grammar">Available tree grammar</param>
    /// <param name="maxTreeLength">Maximum tree length (this parameter is ignored)</param>
    /// <param name="maxTreeDepth">Maximum tree depth</param>
    /// <returns></returns>
    public static ISymbolicExpressionTree Create(IRandom random, ISymbolicExpressionGrammar grammar, int maxTreeLength, int maxTreeDepth) {
      var tree = new SymbolicExpressionTree();
      var rootNode = (SymbolicExpressionTreeTopLevelNode)grammar.ProgramRootSymbol.CreateTreeNode();
      if (rootNode.HasLocalParameters) rootNode.ResetLocalParameters(random);
      rootNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      var startNode = (SymbolicExpressionTreeTopLevelNode)grammar.StartSymbol.CreateTreeNode();
      if (startNode.HasLocalParameters) startNode.ResetLocalParameters(random);
      startNode.SetGrammar(grammar.CreateExpressionTreeGrammar());

      rootNode.AddSubtree(startNode);

      double p = random.NextDouble();

      if (p < 0.5)
        GrowTreeCreator.Create(random, startNode, maxTreeDepth - 2);
      else
        FullTreeCreator.Create(random, startNode, maxTreeDepth - 2);

      tree.Root = rootNode;
      return tree;
    }
  }
}
