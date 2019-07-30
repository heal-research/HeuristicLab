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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Random;
using static HeuristicLab.Problems.DataAnalysis.Symbolic.SymbolicExpressionHashExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("DiversityCrossover", "Simple crossover operator preventing swap between subtrees with the same hash value.")]
  [StorableType("ED35B0D9-9704-4D32-B10B-8F9870E76781")]
  public sealed class SymbolicDataAnalysisExpressionDiversityPreservingCrossover<T> : SymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {

    private const string InternalCrossoverPointProbabilityParameterName = "InternalCrossoverPointProbability";
    private const string WindowingParameterName = "Windowing";
    private const string ProportionalSamplingParameterName = "ProportionalSampling";
    private const string StrictHashingParameterName = "StrictHashing";

    private static readonly Func<byte[], ulong> hashFunction = HashUtil.DJBHash;

    #region Parameter Properties
    public IValueLookupParameter<PercentValue> InternalCrossoverPointProbabilityParameter {
      get { return (IValueLookupParameter<PercentValue>)Parameters[InternalCrossoverPointProbabilityParameterName]; }
    }

    public IValueLookupParameter<BoolValue> WindowingParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[WindowingParameterName]; }
    }

    public IValueLookupParameter<BoolValue> ProportionalSamplingParameter {
      get { return (IValueLookupParameter<BoolValue>)Parameters[ProportionalSamplingParameterName]; }
    }

    public IFixedValueParameter<BoolValue> StrictHashingParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[StrictHashingParameterName]; }
    }
    #endregion

    #region Properties
    public PercentValue InternalCrossoverPointProbability {
      get { return InternalCrossoverPointProbabilityParameter.ActualValue; }
    }

    public BoolValue Windowing {
      get { return WindowingParameter.ActualValue; }
    }

    public BoolValue ProportionalSampling {
      get { return ProportionalSamplingParameter.ActualValue; }
    }

    bool StrictHashing {
      get { return StrictHashingParameter.Value.Value; }
    }
    #endregion


    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(StrictHashingParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(StrictHashingParameterName, "Use strict hashing when calculating subtree hash values."));
      }
    }

    public SymbolicDataAnalysisExpressionDiversityPreservingCrossover() : base() {
      Parameters.Add(new ValueLookupParameter<PercentValue>(InternalCrossoverPointProbabilityParameterName, "The probability to select an internal crossover point (instead of a leaf node).", new PercentValue(0.9)));
      Parameters.Add(new ValueLookupParameter<BoolValue>(WindowingParameterName, "Use proportional sampling with windowing for cutpoint selection.", new BoolValue(false)));
      Parameters.Add(new ValueLookupParameter<BoolValue>(ProportionalSamplingParameterName, "Select cutpoints proportionally using probabilities as weights instead of randomly.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<BoolValue>(StrictHashingParameterName, "Use strict hashing when calculating subtree hash values."));
    }

    private SymbolicDataAnalysisExpressionDiversityPreservingCrossover(SymbolicDataAnalysisExpressionDiversityPreservingCrossover<T> original, Cloner cloner) : base(original, cloner) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionDiversityPreservingCrossover<T>(this, cloner);
    }

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionDiversityPreservingCrossover(StorableConstructorFlag _) : base(_) { }

    private static ISymbolicExpressionTreeNode ActualRoot(ISymbolicExpressionTree tree) {
      return tree.Root.GetSubtree(0).GetSubtree(0);
    }

    public static ISymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1, double internalCrossoverPointProbability, int maxLength, int maxDepth, bool windowing, bool proportionalSampling = false, bool strictHashing = false) {
      var nodes0 = ActualRoot(parent0).MakeNodes(strictHashing).Sort(hashFunction);
      var nodes1 = ActualRoot(parent1).MakeNodes(strictHashing).Sort(hashFunction);

      IList<HashNode<ISymbolicExpressionTreeNode>> sampled0;
      IList<HashNode<ISymbolicExpressionTreeNode>> sampled1;

      if (proportionalSampling) {
        var p = internalCrossoverPointProbability;
        var weights0 = nodes0.Select(x => x.IsLeaf ? 1 - p : p);
        sampled0 = nodes0.SampleProportionalWithoutRepetition(random, nodes0.Length, weights0, windowing: windowing).ToArray();

        var weights1 = nodes1.Select(x => x.IsLeaf ? 1 - p : p);
        sampled1 = nodes1.SampleProportionalWithoutRepetition(random, nodes1.Length, weights1, windowing: windowing).ToArray();
      } else {
        sampled0 = ChooseNodes(random, nodes0, internalCrossoverPointProbability).ShuffleInPlace(random);
        sampled1 = ChooseNodes(random, nodes1, internalCrossoverPointProbability).ShuffleInPlace(random);
      }

      foreach (var selected in sampled0) {
        var cutpoint = new CutPoint(selected.Data.Parent, selected.Data);

        var maxAllowedDepth = maxDepth - parent0.Root.GetBranchLevel(selected.Data);
        var maxAllowedLength = maxLength - (parent0.Length - selected.Data.GetLength());

        foreach (var candidate in sampled1) {
          if (candidate.CalculatedHashValue == selected.CalculatedHashValue
            || candidate.Data.GetDepth() > maxAllowedDepth
            || candidate.Data.GetLength() > maxAllowedLength
            || !cutpoint.IsMatchingPointType(candidate.Data)) {
            continue;
          }

          Swap(cutpoint, candidate.Data);
          return parent0;
        }
      }
      return parent0;
    }

    public override ISymbolicExpressionTree Crossover(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1) {
      if (this.ExecutionContext == null) {
        throw new InvalidOperationException("ExecutionContext not set.");
      }

      var maxDepth = MaximumSymbolicExpressionTreeDepth.Value;
      var maxLength = MaximumSymbolicExpressionTreeLength.Value;

      var internalCrossoverPointProbability = InternalCrossoverPointProbability.Value;
      var windowing = Windowing.Value;
      var proportionalSampling = ProportionalSampling.Value;

      return Cross(random, parent0, parent1, internalCrossoverPointProbability, maxLength, maxDepth, windowing, proportionalSampling, StrictHashing);
    }

    private static List<HashNode<ISymbolicExpressionTreeNode>> ChooseNodes(IRandom random, IEnumerable<HashNode<ISymbolicExpressionTreeNode>> nodes, double internalCrossoverPointProbability) {
      var list = new List<HashNode<ISymbolicExpressionTreeNode>>();

      var chooseInternal = random.NextDouble() < internalCrossoverPointProbability;

      if (chooseInternal) {
        list.AddRange(nodes.Where(x => !x.IsLeaf && x.Data.Parent != null));
      }
      if (!chooseInternal || list.Count == 0) {
        list.AddRange(nodes.Where(x => x.IsLeaf && x.Data.Parent != null));
      }

      return list;
    }
  }
}
