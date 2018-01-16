﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Problems.BinPacking3D.Packer;
using HeuristicLab.Problems.BinPacking3D.Encoding;
using HeuristicLab.Problems.BinPacking3D.Sorting;
using HeuristicLab.Problems.BinPacking3D.Evaluators;

namespace HeuristicLab.Problems.BinPacking3D {

  public enum SortingMethod { All, Given, VolumeHeight, HeightVolume, AreaHeight, HeightArea, ClusteredAreaHeight, ClusteredHeightArea }
  public enum FittingMethod { All, FirstFit, ResidualSpaceBestFit, FreeVolumeBestFit, MinimumResidualSpaceLeft }

  public enum ExtremePointCreationMethod { All, PointProjection, LineProjection }

  public enum ExtremePointPruningMethod { None, All, PruneBehind }

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

    [Storable]
    private readonly IValueParameter<BoolValue> sortByMaterialParameter;

    public IValueParameter<BoolValue> SortByMaterialParameter {
      get { return sortByMaterialParameter; }
    }

    [Storable]
    private readonly IValueParameter<EnumValue<ExtremePointCreationMethod>> extremePointCreationMethodParameter;
    public IValueParameter<EnumValue<ExtremePointCreationMethod>> ExtremePointCreationMethodParameter {
      get { return extremePointCreationMethodParameter; }
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
      Parameters.Add(sortingMethodParameter = new ValueParameter<EnumValue<SortingMethod>>(
        "SortingMethod", "In which order the items should be packed.", new EnumValue<SortingMethod>(SortingMethod.All)));

      Parameters.Add(fittingMethodParameter = new ValueParameter<EnumValue<FittingMethod>>(
        "FittingMethod", "Which method to fit should be used.", new EnumValue<FittingMethod>(FittingMethod.All)));

      Parameters.Add(extremePointCreationMethodParameter = new ValueParameter<EnumValue<ExtremePointCreationMethod>>(
        "ExtremePointCreationMethod", "Which method should be used for creatomg extreme points.", new EnumValue<ExtremePointCreationMethod>(ExtremePointCreationMethod.All)));

      Parameters.Add(deltaParameter = new ValueParameter<PercentValue>(
        "Delta", "[1;100]% Clustered sorting methods use a delta parameter to determine the clusters.", new PercentValue(.1)));

      Parameters.Add(sortByMaterialParameter = new ValueParameter<BoolValue>(
        "SortByMaterial", "If this parameter is set, the items will be sorted by their material first", new BoolValue(true)));

      Problem = new PermutationProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExtremePointAlgorithm(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
    }

    /// <summary>
    /// Runs the extreme point algorithm and adds the results to the property <see cref="Result"/>
    /// </summary>
    /// <param name="token"></param>
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

      var extremePointCreation = new[] { ExtremePointCreationMethodParameter.Value.Value };
      if (extremePointCreation[0] == ExtremePointCreationMethod.All) {
        extremePointCreation = Enum.GetValues(typeof(ExtremePointCreationMethod))
                                     .Cast<ExtremePointCreationMethod>()
                                     .Where(x => x != ExtremePointCreationMethod.All)
                                     .ToArray();
      }

      //
      var result = GetBest(bin, items, sorting, fitting, extremePointCreation, token);
      if (result == null) {
        throw new InvalidOperationException("No result obtained!");
      }

      Results.Add(new Result("Best Solution", "The best found solution", result.Item1));
      Results.Add(new Result("Best Solution Quality", "The quality of the best found solution according to the evaluator", new DoubleValue(result.Item2)));

      var binUtil = new BinUtilizationEvaluator();
      var packRatio = new PackingRatioEvaluator();
      Results.Add(new Result("Best Solution Bin Count",
        "The number of bins in the best found solution",
        new IntValue(result.Item1.NrOfBins)));
      Results.Add(new Result("Best Solution Bin Utilization",
        "The utilization given in percentage as calculated by the BinUtilizationEvaluator (total used space / total available space)",
        new PercentValue(Math.Round(binUtil.Evaluate(result.Item1), 3))));

      if (result.Item3.HasValue && sorting.Length > 1) {
        Results.Add(new Result("Best Sorting Method",
          "The sorting method that found the best solution",
          new EnumValue<SortingMethod>(result.Item3.Value)));
      }

      if (result.Item4.HasValue && fitting.Length > 1) {
        Results.Add(new Result("Best Fitting Method",
          "The fitting method that found the best solution",
          new EnumValue<FittingMethod>(result.Item4.Value)));
      }

      if (result.Item5.HasValue && extremePointCreation.Length > 1) {
        Results.Add(new Result("Best extreme point creation method",
          "The extreme point creation method that found the best solution",
          new EnumValue<ExtremePointCreationMethod>(result.Item5.Value)));
      }
    }

    /// <summary>
    /// Retunrs the best solution for the given parameters
    /// </summary>
    /// <param name="bin"></param>
    /// <param name="items"></param>
    /// <param name="sortings"></param>
    /// <param name="fittings"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private Tuple<Solution, double, SortingMethod?, FittingMethod?, ExtremePointCreationMethod?> 
          GetBest(PackingShape bin, 
                  IList<PackingItem> items, 
                  SortingMethod[] sortings, 
                  FittingMethod[] fittings,
                  ExtremePointCreationMethod[] epCreationMethods,
                  CancellationToken token) {
      SortingMethod? bestSorting = null;
      FittingMethod? bestFitting = null;
      ExtremePointCreationMethod? bestEPCreation = null;
      var best = double.NaN;
      Solution bestSolution = null;
      foreach (var fit in fittings) {
        foreach (var sort in sortings) {
          foreach (var epCreation in epCreationMethods) {
            IDecoder<Permutation> decoder = new ExtremePointPermutationDecoder() {
              FittingMethod = fit,
              ExtremePointCreationMethod = epCreation
            };
            Permutation sortedItems;
                    
            if (SortByMaterialParameter.Value.Value) {
              sortedItems = SortItemsByMaterialAndSortingMethod(bin, items, sort, DeltaParameter.Value.Value);
            } else {
              sortedItems = SortItemsBySortingMethod(bin, items, sort, DeltaParameter.Value.Value);
            }

            var result = Optimize(new OptimaizationParamters() {
              SortedItems = sortedItems,
              Bin = bin,
              Items = items,
              StackingConstraints = Problem.UseStackingConstraints,
              Decoder = decoder,
              Evaluator = Problem.SolutionEvaluator,
              ExtremePointGeneration = epCreation
            });
          
            if (double.IsNaN(result.Item2) || double.IsInfinity(result.Item2)) {
              continue;
            }

            if (double.IsNaN(best) || Problem.Maximization && result.Item2 > best || !Problem.Maximization && result.Item2 < best) {
              bestSolution = result.Item1;
              best = result.Item2;
              bestSorting = sort;
              bestFitting = fit;
              bestEPCreation = epCreation;
            }
            if (token.IsCancellationRequested) {
              return Tuple.Create(bestSolution, best, bestSorting, bestFitting, bestEPCreation);
            }
          }
        }
      }
      if (double.IsNaN(best)) {
        return null;
      }
      return Tuple.Create(bestSolution, best, bestSorting, bestFitting, bestEPCreation);
    }

    /// <summary>
    /// Returns a tuple with the solution and the packing ratio depending on the given parameters
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static Tuple<Solution, double> Optimize(OptimaizationParamters parameters) { 
      
      var sol = parameters.Decoder.Decode(parameters.SortedItems, parameters.Bin, parameters.Items, parameters.StackingConstraints);
      var fit = parameters.Evaluator.Evaluate(sol);

      return Tuple.Create(sol, fit);
    }

    private class OptimaizationParamters {
      public Permutation SortedItems { get; set; }
      public PackingShape Bin { get; set; }
      public IList<PackingItem> Items { get; set; }
      public bool StackingConstraints { get; set; }
      public IDecoder<Permutation> Decoder { get; set; }
      public IEvaluator Evaluator { get; set; }
      public ExtremePointCreationMethod ExtremePointGeneration { get; set; }
    }
    

    /// <summary>
    /// Returns a new permutation of the given items depending on the sorting method
    /// </summary>
    /// <param name="bin"></param>
    /// <param name="items"></param>
    /// <param name="sortingMethod"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    private Permutation SortItemsBySortingMethod(PackingShape bin, IList<PackingItem> items, SortingMethod sortingMethod, double delta) {
      Permutation sorted = null;
      
      switch (sortingMethod) {
        case SortingMethod.Given:
          sorted = new Permutation(PermutationTypes.Absolute, Enumerable.Range(0, items.Count).ToArray());
          break;
        case SortingMethod.VolumeHeight:
          sorted = items.SortByVolumeHeight();
          break;
        case SortingMethod.HeightVolume:
          sorted = items.SortByMaterialHeightVolume();
          break;
        case SortingMethod.AreaHeight:
          sorted = items.SortByMaterialAreaHeight();
          break;
        case SortingMethod.HeightArea:
          sorted = items.SortByMaterialHeightArea();
          break;
        case SortingMethod.ClusteredAreaHeight:
          sorted = items.SortByMaterialClusteredAreaHeight(bin, delta);
          break;
        case SortingMethod.ClusteredHeightArea:
          sorted = items.SortByMaterialClusteredHeightArea(bin, delta);
          break;
        default:
          throw new ArgumentException("Unknown sorting method: " + sortingMethod);
      }
      return sorted;
    }

    /// <summary>
    /// Returns a new permutation of the given items depending on the material and sorting method
    /// </summary>
    /// <param name="bin"></param>
    /// <param name="items"></param>
    /// <param name="sortingMethod"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    private Permutation SortItemsByMaterialAndSortingMethod(PackingShape bin, IList<PackingItem> items, SortingMethod sortingMethod, double delta) {
      Permutation sorted = null;

      switch (sortingMethod) {
        case SortingMethod.Given:
          sorted = new Permutation(PermutationTypes.Absolute, Enumerable.Range(0, items.Count).ToArray());
          break;
        case SortingMethod.VolumeHeight:
          sorted = items.SortByVolumeHeight();
          break;
        case SortingMethod.HeightVolume:
          sorted = items.SortByHeightVolume();
          break;
        case SortingMethod.AreaHeight:
          sorted = items.SortByAreaHeight();
          break;
        case SortingMethod.HeightArea:
          sorted = items.SortByHeightArea();
          break;
        case SortingMethod.ClusteredAreaHeight:
          sorted = items.SortByClusteredAreaHeight(bin, delta);
          break;
        case SortingMethod.ClusteredHeightArea:
          sorted = items.SortByClusteredHeightArea(bin, delta);
          break;
        default:
          throw new ArgumentException("Unknown sorting method: " + sortingMethod);
      }
      return sorted;
    }
  }
}