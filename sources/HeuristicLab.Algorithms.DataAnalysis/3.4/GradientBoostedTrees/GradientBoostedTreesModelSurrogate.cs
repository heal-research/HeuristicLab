#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  // this class is used as a surrogate for persistence of an actual GBT model 
  // since the actual GBT model would be very large when persisted we only store all necessary information to
  // recalculate the actual GBT model on demand
  [Item("Gradient boosted tree model", "")]
  public sealed class GradientBoostedTreesModelSurrogate : NamedItem, IRegressionModel {
    // don't store the actual model!
    private IRegressionModel actualModel; // the actual model is only recalculated when necessary

    [Storable]
    private readonly IRegressionProblemData trainingProblemData;
    [Storable]
    private readonly uint seed;
    [Storable]
    private ILossFunction lossFunction;
    [Storable]
    private double r;
    [Storable]
    private double m;
    [Storable]
    private double nu;
    [Storable]
    private int iterations;
    [Storable]
    private int maxSize;


    [StorableConstructor]
    private GradientBoostedTreesModelSurrogate(bool deserializing) : base(deserializing) { }

    private GradientBoostedTreesModelSurrogate(GradientBoostedTreesModelSurrogate original, Cloner cloner)
      : base(original, cloner) {
      if (original.actualModel != null) this.actualModel = cloner.Clone(original.actualModel);

      this.trainingProblemData = cloner.Clone(original.trainingProblemData);
      this.lossFunction = cloner.Clone(original.lossFunction);
      this.seed = original.seed;
      this.iterations = original.iterations;
      this.maxSize = original.maxSize;
      this.r = original.r;
      this.m = original.m;
      this.nu = original.nu;
    }

    // create only the surrogate model without an actual model
    public GradientBoostedTreesModelSurrogate(IRegressionProblemData trainingProblemData, uint seed, ILossFunction lossFunction, int iterations, int maxSize, double r, double m, double nu)
      : base("Gradient boosted tree model", string.Empty) {
      this.trainingProblemData = trainingProblemData;
      this.seed = seed;
      this.lossFunction = lossFunction;
      this.iterations = iterations;
      this.maxSize = maxSize;
      this.r = r;
      this.m = m;
      this.nu = nu;
    }

    // wrap an actual model in a surrograte
    public GradientBoostedTreesModelSurrogate(IRegressionProblemData trainingProblemData, uint seed, ILossFunction lossFunction, int iterations, int maxSize, double r, double m, double nu, IRegressionModel model)
      : this(trainingProblemData, seed, lossFunction, iterations, maxSize, r, m, nu) {
      this.actualModel = model;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GradientBoostedTreesModelSurrogate(this, cloner);
    }

    // forward message to actual model (recalculate model first if necessary)
    public IEnumerable<double> GetEstimatedValues(IDataset dataset, IEnumerable<int> rows) {
      if (actualModel == null) actualModel = RecalculateModel();
      return actualModel.GetEstimatedValues(dataset, rows);
    }

    public IRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData) {
      return new RegressionSolution(this, (IRegressionProblemData)problemData.Clone());
    }


    private IRegressionModel RecalculateModel() {
      return GradientBoostedTreesAlgorithmStatic.TrainGbm(trainingProblemData, lossFunction, maxSize, nu, r, m, iterations, seed).Model;
    }
  }
}
