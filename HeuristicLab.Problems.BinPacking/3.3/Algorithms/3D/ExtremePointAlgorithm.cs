#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinPacking3D {
  
  public enum SortingMethod { All, Given, VolumeHeight, HeightVolume, AreaHeight, HeightArea, ClusteredAreaHeight, ClusteredHeightArea }
  public enum FittingMethod { All, FirstFit, ResidualSpaceBestFit, FreeVolumeBestFit }

  [Item("Extreme-point-based Bin Packing (3d)", "An implementation of the extreme-point based packing described in Crainic, T. G., Perboli, G., & Tadei, R. (2008). Extreme point-based heuristics for three-dimensional bin packing. Informs Journal on computing, 20(3), 368-384.")]
  [StorableClass]
  [Creatable(CreatableAttribute.Categories.SingleSolutionAlgorithms, Priority = 180)]
  public sealed class ExtremePointAlgorithm : BasicAlgorithm {

    public override Type ProblemType {
      get { return typeof(PermutationProblem); }
    }

    public new PermutationProblem Problem {
      get { return (PermutationProblem)base.Problem; }
      set { base.Problem = value; }
    }

    public override bool SupportsPause {
      get { return false; }
    }

    [Storable]
    private readonly IValueParameter<EnumValue<SortingMethod>> sortingMethodParameter;
    public IValueParameter<EnumValue<SortingMethod>> SortingMethodParameter {
      get { return sortingMethodParameter; }
    }

    [Storable]
    private readonly IValueParameter<EnumValue<FittingMethod>> fittingMethodParameter;
    public IValueParameter<EnumValue<FittingMethod>> FittingMethodParameter {
      get { return fittingMethodParameter; }
    }

    [Storable]
    private readonly IValueParameter<PercentValue> deltaParameter;
    public IValueParameter<PercentValue> DeltaParameter {
      get { return deltaParameter; }
    }

    [StorableConstructor]
    private ExtremePointAlgorithm(bool deserializing) : base(deserializing) { }
    private ExtremePointAlgorithm(ExtremePointAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      sortingMethodParameter = cloner.Clone(original.sortingMethodParameter);
      fittingMethodParameter = cloner.Clone(original.fittingMethodParameter);
      deltaParameter = cloner.Clone(original.deltaParameter);
    }
    public ExtremePointAlgorithm() {
      Parameters.Add(sortingMethodParameter = new ValueParameter<EnumValue<SortingMethod>>("SortingMethod", "In which order the items should be packed.", new EnumValue<SortingMethod>(SortingMethod.All)));
      Parameters.Add(fittingMethodParameter = new ValueParameter<EnumValue<FittingMethod>>("FittingMethod", "Which method to fit should be used.", new EnumValue<FittingMethod>(FittingMethod.All)));
      Parameters.Add(deltaParameter = new ValueParameter<PercentValue>("Delta", "[1;100]% Clustered sorting methods use a delta parameter to determine the clusters.", new PercentValue(.1)));
      
      Problem = new PermutationProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtremePointAlgorithm(this, cloner);
    }
    
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    protected override void Run(CancellationToken token) {
      var items = Problem.Items;
      var bin = Problem.BinShape;
      var sorting = new[] { SortingMethodParameter.Value.Value };
      if (sorting[0] == SortingMethod.All) {
        sorting = Enum.GetValues(typeof(SortingMethod)).Cast<SortingMethod>().Where(x => x != SortingMethod.All).ToArray();
      }
      var fitting = new[] { fittingMethodParameter.Value.Value };
      if (fitting[0] == FittingMethod.All) {
        fitting = Enum.GetValues(typeof(FittingMethod)).Cast<FittingMethod>().Where(x => x != FittingMethod.All).ToArray();
      }
      var result = GetBest(bin, items, sorting, fitting, token);
      if (result == null) throw new InvalidOperationException("No result obtained!");

      Results.Add(new Result("Best Solution",
        "The best found solution",
        result.Item1));
      Results.Add(new Result("Best Solution Quality",
        "The quality of the best found solution according to the evaluator",
        new DoubleValue(result.Item2)));

      var binUtil = new BinUtilizationEvaluator();
      var packRatio = new PackingRatioEvaluator();
      Results.Add(new Result("Best Solution Bin Count",
        "The number of bins in the best found solution",
        new IntValue(result.Item1.NrOfBins)));
      Results.Add(new Result("Best Solution Bin Utilization",
        "The utilization given in percentage as calculated by the BinUtilizationEvaluator (total used space / total available space)",
        new PercentValue(Math.Round(binUtil.Evaluate(result.Item1), 3))));

      if (result.Item3.HasValue && sorting.Length > 1)
        Results.Add(new Result("Best Sorting Method",
          "The sorting method that found the best solution",
          new EnumValue<SortingMethod>(result.Item3.Value)));
      if (result.Item4.HasValue && fitting.Length > 1)
        Results.Add(new Result("Best Fitting Method",
          "The fitting method that found the best solution",
          new EnumValue<FittingMethod>(result.Item4.Value)));
    }

    private Tuple<Solution, double, SortingMethod?, FittingMethod?> GetBest(PackingShape bin, IList<PackingItem> items, SortingMethod[] sortings, FittingMethod[] fittings, CancellationToken token) {
      SortingMethod? bestSorting = null;
      FittingMethod? bestFitting = null;
      var best = double.NaN;
      Solution bestSolution = null;
      foreach (var fit in fittings) {
        foreach (var sort in sortings) {
          var result = Optimize(bin, items, sort, fit, DeltaParameter.Value.Value, Problem.UseStackingConstraints, Problem.SolutionEvaluator, token);
          if (double.IsNaN(result.Item2) || double.IsInfinity(result.Item2)) continue;
          if (double.IsNaN(best)
            || Problem.Maximization && result.Item2 > best
            || !Problem.Maximization && result.Item2 < best) {
            bestSolution = result.Item1;
            best = result.Item2;
            bestSorting = sort;
            bestFitting = fit;
          }
          if (token.IsCancellationRequested) return Tuple.Create(bestSolution, best, bestSorting, bestFitting);
        }
      }
      if (double.IsNaN(best)) return null;
      return Tuple.Create(bestSolution, best, bestSorting, bestFitting);
    }

    private static Tuple<Solution, double> Optimize(PackingShape bin, IList<PackingItem> items, SortingMethod sorting, FittingMethod fitting, double delta, bool stackingConstraints, IEvaluator evaluator, CancellationToken token) {
      Permutation sorted = null;
      switch (sorting) {
        case SortingMethod.Given:
          sorted = new Permutation(PermutationTypes.Absolute, Enumerable.Range(0, items.Count).ToArray());
          break;
        case SortingMethod.VolumeHeight:
          sorted = new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
          break;
        case SortingMethod.HeightVolume:
          sorted = new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width * x.Item.Height)
                         .Select(x => x.Index).ToArray());
          break;
        case SortingMethod.AreaHeight:
          sorted = new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Depth * x.Item.Width)
                         .ThenByDescending(x => x.Item.Height)
                         .Select(x => x.Index).ToArray());
          break;
        case SortingMethod.HeightArea:
          sorted = new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v })
                         .OrderByDescending(x => x.Item.Height)
                         .ThenByDescending(x => x.Item.Depth * x.Item.Width)
                         .Select(x => x.Index).ToArray());
          break;
        case SortingMethod.ClusteredAreaHeight:
          double clusterRange = bin.Width * bin.Depth * delta;
          sorted = new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Width * v.Depth / clusterRange)) })
                        .GroupBy(x => x.ClusterId)
                        .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(y => y.Item.Height).ToList() })
                        .OrderByDescending(x => x.Cluster)
                        .SelectMany(x => x.Items)
                        .Select(x => x.Index).ToArray());
          break;
        case SortingMethod.ClusteredHeightArea:
          double clusterRange2 = bin.Height * delta;
          sorted = new Permutation(PermutationTypes.Absolute,
                    items.Select((v, i) => new { Index = i, Item = v, ClusterId = (int)(Math.Ceiling(v.Height / clusterRange2)) })
                        .GroupBy(x => x.ClusterId)
                        .Select(x => new { Cluster = x.Key, Items = x.OrderByDescending(y => y.Item.Depth * y.Item.Width).ToList() })
                        .OrderByDescending(x => x.Cluster)
                        .SelectMany(x => x.Items)
                        .Select(x => x.Index).ToArray());
          break;
        default: throw new ArgumentException("Unknown sorting method: " + sorting);
      }
      
      ExtremePointPermutationDecoderBase decoder = null;
      switch (fitting) {
        case FittingMethod.FirstFit:
          decoder = new ExtremePointPermutationDecoder();
          break;
        case FittingMethod.FreeVolumeBestFit:
          decoder = new FreeVolumeBestFitExtremePointPermutationDecoder();
          break;
        case FittingMethod.ResidualSpaceBestFit:
          decoder = new ResidualSpaceBestFitExtremePointPermutationDecoder();
          break;
        default: throw new ArgumentException("Unknown fitting method: " + fitting);
      }

      var sol = decoder.Decode(sorted, bin, items, stackingConstraints);
      var fit = evaluator.Evaluate(sol);

      return Tuple.Create(sol, fit);
    }
  }
}
