using System;
using System.Collections.Generic;
using System.Linq;

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Random;
using static HeuristicLab.Problems.DataAnalysis.Symbolic.SymbolicExpressionHashExtensions;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [Item("DiversityCrossover", "Simple crossover operator prioritizing internal nodes according to the given probability.")]
  [StorableType("ED35B0D9-9704-4D32-B10B-8F9870E76781")]
  public sealed class SymbolicDataAnalysisExpressionDiversityPreservingCrossover<T> : SymbolicDataAnalysisExpressionCrossover<T> where T : class, IDataAnalysisProblemData {

    private const string InternalCrossoverPointProbabilityParameterName = "InternalCrossoverPointProbability";
    private const string WindowingParameterName = "Windowing";
    private const string ProportionalSamplingParameterName = "ProportionalSampling";

    private static readonly Func<byte[], ulong> hashFunction = HashUtil.JSHash;

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
    #endregion

    public SymbolicDataAnalysisExpressionDiversityPreservingCrossover() {
      name = "DiversityCrossover";
      Parameters.Add(new ValueLookupParameter<PercentValue>(InternalCrossoverPointProbabilityParameterName, "The probability to select an internal crossover point (instead of a leaf node).", new PercentValue(0.9)));
      Parameters.Add(new ValueLookupParameter<BoolValue>(WindowingParameterName, "Use proportional sampling with windowing for cutpoint selection.", new BoolValue(false)));
      Parameters.Add(new ValueLookupParameter<BoolValue>(ProportionalSamplingParameterName, "Select cutpoints proportionally using probabilities as weights instead of randomly.", new BoolValue(true)));
    }

    private SymbolicDataAnalysisExpressionDiversityPreservingCrossover(SymbolicDataAnalysisExpressionDiversityPreservingCrossover<T> original, Cloner cloner) : base(original, cloner) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicDataAnalysisExpressionDiversityPreservingCrossover<T>(this, cloner);
    }

    [StorableConstructor]
    private SymbolicDataAnalysisExpressionDiversityPreservingCrossover(StorableConstructorFlag _) : base(_) { }

    private static ISymbolicExpressionTreeNode ActualRoot(ISymbolicExpressionTree tree) {
      return tree.Root.GetSubtree(0).GetSubtree(0);
    }

    public static ISymbolicExpressionTree Cross(IRandom random, ISymbolicExpressionTree parent0, ISymbolicExpressionTree parent1, double internalCrossoverPointProbability, int maxLength, int maxDepth, bool windowing, bool proportionalSampling = false) {
      var leafCrossoverPointProbability = 1 - internalCrossoverPointProbability;

      var nodes0 = ActualRoot(parent0).MakeNodes().Sort(hashFunction);
      var nodes1 = ActualRoot(parent1).MakeNodes().Sort(hashFunction);

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

      return Cross(random, parent0, parent1, internalCrossoverPointProbability, maxLength, maxDepth, windowing, proportionalSampling);
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
