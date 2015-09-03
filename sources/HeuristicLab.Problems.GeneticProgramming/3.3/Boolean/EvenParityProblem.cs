#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;


namespace HeuristicLab.Problems.GeneticProgramming.Boolean {
  [Item("Even Parity Problem", "The Boolean even parity genetic programming problem. See Koza, 1992, page 529 section 20.2 Symbolic Regression of Even-Parity Functions")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 900)]
  [StorableClass]
  public sealed class EvenParityProblem : SymbolicExpressionTreeProblem {

    #region parameter names

    private const string NumberOfBitsParameterName = "NumberOfBits";

    #endregion

    #region Parameter Properties
    public IFixedValueParameter<IntValue> NumberOfBitsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[NumberOfBitsParameterName]; }
    }

    #endregion

    #region Properties

    public int NumberOfBits {
      get { return NumberOfBitsParameter.Value.Value; }
      set { NumberOfBitsParameter.Value.Value = value; }
    }


    #endregion

    public override bool Maximization {
      get { return true; }
    }

    public EvenParityProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumberOfBitsParameterName, "The number of bits for the input parameter for the even parity function", new IntValue(4)));

      var g = new SimpleSymbolicExpressionGrammar(); // will be replaced in update grammar
      Encoding = new SymbolicExpressionTreeEncoding(g, 100, 17);

      UpdateGrammar();
      RegisterEventHandlers();
    }

    private void UpdateGrammar() {
      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new[] { "AND", "OR", "NAND", "NOR" }, 2, 2); // see Koza, 1992, page 529 section 20.2 Symbolic Regression of Even-Parity Functions

      // add one terminal symbol for each bit
      for (int i = 0; i < NumberOfBits; i++)
        g.AddTerminalSymbol(string.Format("{0}", i));

      Encoding.Grammar = g;
    }


    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      if (NumberOfBits <= 0) throw new NotSupportedException("Number of bits must be larger than zero.");
      if (NumberOfBits > 10) throw new NotSupportedException("Even parity does not support problems with number of bits > 10.");
      var bs = Enumerable.Range(0, (int)Math.Pow(2, NumberOfBits));
      var targets = bs.Select(b => CalcTarget(b, NumberOfBits));
      var pred = Interpret(tree, bs);
      return targets.Zip(pred, (t, p) => t == p ? 1 : 0).Sum(); // count number of correct predictions
    }

    private static bool CalcTarget(int b, int numBits) {
      bool res = GetBits(b, 0);
      for (byte i = 1; i < numBits; i++)
        res = res ^ GetBits(b, i); // XOR
      return res;
    }

    private static IEnumerable<bool> Interpret(ISymbolicExpressionTree tree, IEnumerable<int> bs) {
      // skip programRoot and startSymbol
      return InterpretRec(tree.Root.GetSubtree(0).GetSubtree(0), bs);
    }


    private static IEnumerable<bool> InterpretRec(ISymbolicExpressionTreeNode node, IEnumerable<int> bs) {
      Func<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode, Func<bool, bool, bool>, IEnumerable<bool>> eval2 =
        (left, right, f) => InterpretRec(left, bs).Zip(InterpretRec(right, bs), f);

      switch (node.Symbol.Name) {
        case "AND": return eval2(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x & y);
        case "OR": return eval2(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x | y);
        case "NAND": return eval2(node.GetSubtree(0), node.GetSubtree(1), (x, y) => !(x & y));
        case "NOR": return eval2(node.GetSubtree(0), node.GetSubtree(1), (x, y) => !(x | y));
        default: {
            byte bitPos;
            if (byte.TryParse(node.Symbol.Name, out bitPos)) {
              return bs.Select(b => GetBits(b, bitPos));
            } else throw new NotSupportedException(string.Format("Found unexpected symbol {0}", node.Symbol.Name));
          }
      }
    }

    private static bool GetBits(int b, byte bitPos) {
      return (b & (1 << bitPos)) != 0;
    }

    #region item cloning and persistence
    // persistence
    [StorableConstructor]
    private EvenParityProblem(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    // cloning 
    private EvenParityProblem(EvenParityProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new EvenParityProblem(this, cloner);
    }
    #endregion

    #region events

    private void RegisterEventHandlers() {
      NumberOfBitsParameter.Value.ValueChanged += (sender, args) => UpdateGrammar();
    }
    #endregion
  }
}
