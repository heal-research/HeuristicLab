#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 * and the BEACON Center for the Study of Evolution in Action.
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
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableType("1BF7BEFB-6739-48AA-89BC-B632E72D148C")]
  // this class is used as a surrogate for persistence of an actual GBT model 
  // since the actual GBT model would be very large when persisted we only store all necessary information to
  // recalculate the actual GBT model on demand
  [Item("Gradient boosted tree model", "")]
  public sealed class GradientBoostedTreesModelSurrogate : RegressionModel, IGradientBoostedTreesModel {
    // don't store the actual model!
    // the actual model is only recalculated when necessary
    private IGradientBoostedTreesModel fullModel;
    private readonly Lazy<IGradientBoostedTreesModel> actualModel;
    private IGradientBoostedTreesModel ActualModel {
      get { return actualModel.Value; }
    }

    [Storable]
    private readonly IRegressionProblemData trainingProblemData;
    [Storable]
    private readonly uint seed;
    [Storable]
    private readonly ILossFunction lossFunction;
    [Storable]
    private readonly double r;
    [Storable]
    private readonly double m;
    [Storable]
    private readonly double nu;
    [Storable]
    private readonly int iterations;
    [Storable]
    private readonly int maxSize;


    public override IEnumerable<string> VariablesUsedForPrediction {
      get {
        return ActualModel.Models.SelectMany(x => x.VariablesUsedForPrediction).Distinct().OrderBy(x => x);
      }
    }

    [StorableConstructor]
    private GradientBoostedTreesModelSurrogate(StorableConstructorFlag _) : base(_) {
      actualModel = CreateLazyInitFunc();
    }

    private GradientBoostedTreesModelSurrogate(GradientBoostedTreesModelSurrogate original, Cloner cloner)
      : base(original, cloner) {
      // clone data which is necessary to rebuild the model
      this.trainingProblemData = cloner.Clone(original.trainingProblemData);
      this.lossFunction = cloner.Clone(original.lossFunction);
      this.seed = original.seed;
      this.iterations = original.iterations;
      this.maxSize = original.maxSize;
      this.r = original.r;
      this.m = original.m;
      this.nu = original.nu;

      // clone full model if it has already been created
      if (original.fullModel != null) this.fullModel = cloner.Clone(original.fullModel);
      actualModel = CreateLazyInitFunc();
    }

    private Lazy<IGradientBoostedTreesModel> CreateLazyInitFunc() {
      return new Lazy<IGradientBoostedTreesModel>(() => {
        if (fullModel == null) fullModel = RecalculateModel();
        return fullModel;
      });
    }

    // create only the surrogate model without an actual model
    private GradientBoostedTreesModelSurrogate(IRegressionProblemData trainingProblemData, uint seed,
      ILossFunction lossFunction, int iterations, int maxSize, double r, double m, double nu)
      : base(trainingProblemData.TargetVariable, "Gradient boosted tree model", string.Empty) {
      this.trainingProblemData = trainingProblemData;
      this.seed = seed;
      this.lossFunction = lossFunction;
      this.iterations = iterations;
      this.maxSize = maxSize;
      this.r = r;
      this.m = m;
      this.nu = nu;

      actualModel = CreateLazyInitFunc();
    }

    // wrap an actual model in a surrogate
    public GradientBoostedTreesModelSurrogate(IGradientBoostedTreesModel model, IRegressionProblemData trainingProblemData, uint seed,
      ILossFunction lossFunction, int iterations, int maxSize, double r, double m, double nu)
      : this(trainingProblemData, seed, lossFunction, iterations, maxSize, r, m, nu) {
      fullModel = model;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GradientBoostedTreesModelSurrogate(this, cloner);
    }

    // forward message to actual model (recalculate model first if necessary)
    public override IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      return ActualModel.GetEstimatedValues(dataset, rows);
    }

    public override IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, (IRegressionProblemData)problemData.Clone());
    }

    private IGradientBoostedTreesModel RecalculateModel() {
      return GradientBoostedTreesAlgorithmStatic.TrainGbm(trainingProblemData, lossFunction, maxSize, nu, r, m, iterations, seed).Model;
    }

    public IEnumerable<IRegressionModel> Models {
      get { return ActualModel.Models; }
    }

    public IEnumerable<double> Weights {
      get { return ActualModel.Weights; }
    }
  }
}