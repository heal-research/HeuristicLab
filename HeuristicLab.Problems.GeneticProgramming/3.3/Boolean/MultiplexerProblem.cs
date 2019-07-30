#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics.Contracts;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;


namespace HeuristicLab.Problems.GeneticProgramming.Boolean {
  [Item("Multiplexer Problem (MUX)",
    "The Boolean multiplexer genetic programming problem. See Koza 1992, page 171, section 7.4.1 11-multiplexer.")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 900)]
  [StorableType("6DFE64E4-3968-446F-AE3D-FAF13C18930C")]
  public sealed class MultiplexerProblem : SymbolicExpressionTreeProblem {

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

    #region item cloning and persistence
    // persistence
    [StorableConstructor]
    private MultiplexerProblem(StorableConstructorFlag _) : base(_) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    // cloning 
    private MultiplexerProblem(MultiplexerProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiplexerProblem(this, cloner);
    }
    #endregion


    public MultiplexerProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<IntValue>(NumberOfBitsParameterName,
        "The number of bits for the input parameter for the multiplexer function. This is the sum of the number of address bits and the number of input lines. E.g. the 11-MUX has 3 address bits and 8 input lines",
        new IntValue(11)));

      var g = new SimpleSymbolicExpressionGrammar(); // will be replaced in update grammar
      Encoding = new SymbolicExpressionTreeEncoding(g, 100, 17);
      Encoding.GrammarParameter.ReadOnly = true;

      UpdateGrammar();
      RegisterEventHandlers();
    }

    private void UpdateGrammar() {
      var g = new SimpleSymbolicExpressionGrammar();
      g.AddSymbols(new[] { "AND", "OR" }, 2, 2); // See Koza 1992, page 171, section 7.4.1 11-multiplexer
      g.AddSymbols(new[] { "NOT" }, 1, 1);
      g.AddSymbols(new[] { "IF" }, 3, 3);

      // find the number of address lines and input lines
      // e.g. 11-MUX: 3 addrBits + 8 input bits

      var addrBits = (int)Math.Log(NumberOfBits, 2); // largest power of two that fits into the number of bits
      var inputBits = NumberOfBits - addrBits;

      for (int i = 0; i < addrBits; i++)
        g.AddTerminalSymbol(string.Format("a{0}", i));
      for (int i = 0; i < inputBits; i++)
        g.AddTerminalSymbol(string.Format("d{0}", i));

      Encoding.GrammarParameter.ReadOnly = false;
      Encoding.Grammar = g;
      Encoding.GrammarParameter.ReadOnly = true;

      BestKnownQuality = Math.Pow(2, NumberOfBits); // this is a benchmark problem (the best achievable quality is known for a given number of bits)
    }


    public override double Evaluate(ISymbolicExpressionTree tree, IRandom random) {
      if (NumberOfBits <= 0) throw new NotSupportedException("Number of bits must be larger than zero.");
      if (NumberOfBits > 37) throw new NotSupportedException("Multiplexer does not support problems with number of bits > 37.");
      var bs = Enumerable.Range(0, (int)Math.Pow(2, NumberOfBits));
      var addrBits = (int)Math.Log(NumberOfBits, 2); // largest power of two that fits into the number of bits
      var inputBits = NumberOfBits - addrBits;
      var targets = bs.Select(b => CalcTarget(b, addrBits, inputBits));
      var pred = Interpret(tree, bs, (byte)addrBits);
      return targets.Zip(pred, (t, p) => t == p ? 1 : 0).Sum(); // count number of correct predictions
    }

    private bool CalcTarget(int b, int addrBits, int inputBits) {
      Contract.Assert(addrBits > 0);
      // calc addr
      int addr = 0;
      for (int i = addrBits - 1; i >= 0; i--) {
        addr = addr << 1;
        if (GetBits(b, (byte)i)) addr += 1;
      }
      if (addr <= inputBits)
        return GetBits(b, (byte)(addrBits + addr));
      else return false; // in case the number of bits is smaller then necessary we assume that the remaining lines are false
    }

    private static IEnumerable<bool> Interpret(ISymbolicExpressionTree tree, IEnumerable<int> bs, byte addrBits) {
      // skip programRoot and startSymbol
      return InterpretRec(tree.Root.GetSubtree(0).GetSubtree(0), bs, addrBits);
    }


    private static IEnumerable<bool> InterpretRec(ISymbolicExpressionTreeNode node, IEnumerable<int> bs, byte addrBits) {
      Func<ISymbolicExpressionTreeNode, Func<bool, bool>, IEnumerable<bool>> unaryEval =
        (child, f) => InterpretRec(child, bs, addrBits).Select(f);
      Func<ISymbolicExpressionTreeNode, ISymbolicExpressionTreeNode, Func<bool, bool, bool>, IEnumerable<bool>> binaryEval =
        (left, right, f) => InterpretRec(left, bs, addrBits).Zip(InterpretRec(right, bs, addrBits), f);

      switch (node.Symbol.Name) {
        case "AND": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x & y);
        case "OR": return binaryEval(node.GetSubtree(0), node.GetSubtree(1), (x, y) => x | y);
        case "NOT": return unaryEval(node.GetSubtree(0), (x) => !x);
        case "IF": return EvalIf(node.GetSubtree(0), node.GetSubtree(1), node.GetSubtree(2), bs, addrBits);
        default: {
            if (node.Symbol.Name[0] == 'a') {
              byte bitPos;
              if (byte.TryParse(node.Symbol.Name.Substring(1), out bitPos)) {
                return bs.Select(b => GetBits(b, bitPos));
              }
            } else if (node.Symbol.Name[0] == 'd') {
              byte bitPos;
              if (byte.TryParse(node.Symbol.Name.Substring(1), out bitPos)) {
                return bs.Select(b => GetBits(b, (byte)(bitPos + addrBits))); // offset of input line bits
              }
            }

            throw new NotSupportedException(string.Format("Found unexpected symbol {0}", node.Symbol.Name));
          }
      }
    }

    private static IEnumerable<bool> EvalIf(ISymbolicExpressionTreeNode pred, ISymbolicExpressionTreeNode trueBranch, ISymbolicExpressionTreeNode falseBranch, IEnumerable<int> bs, byte addrBits) {
      var preds = InterpretRec(pred, bs, addrBits).GetEnumerator();
      var tB = InterpretRec(trueBranch, bs, addrBits).GetEnumerator();
      var fB = InterpretRec(falseBranch, bs, addrBits).GetEnumerator();
      // start enumerators

      while (preds.MoveNext() & tB.MoveNext() & fB.MoveNext()) {
        if (preds.Current) yield return tB.Current;
        else yield return fB.Current;
      }
    }

    private static bool GetBits(int b, byte bitPos) {
      return (b & (1 << bitPos)) != 0;
    }

    #region events
    private void RegisterEventHandlers() {
      NumberOfBitsParameter.Value.ValueChanged += (sender, args) => UpdateGrammar();
    }
    #endregion
  }
}
