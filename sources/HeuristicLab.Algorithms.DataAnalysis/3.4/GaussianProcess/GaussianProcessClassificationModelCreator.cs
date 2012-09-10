#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "GaussianProcessClassificationModelCreator",
    Description = "Creates a Gaussian process model for least-squares classification given the data, the hyperparameters, a mean function, and a covariance function.")]
  public sealed class GaussianProcessClassificationModelCreator : GaussianProcessModelCreator {
    private const string ProblemDataParameterName = "ProblemData";

    #region Parameter Properties
    public ILookupParameter<IClassificationProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IClassificationProblemData>)Parameters[ProblemDataParameterName]; }
    }
    #endregion

    #region Properties
    private IClassificationProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    private GaussianProcessClassificationModelCreator(bool deserializing) : base(deserializing) { }
    private GaussianProcessClassificationModelCreator(GaussianProcessClassificationModelCreator original, Cloner cloner) : base(original, cloner) { }
    public GaussianProcessClassificationModelCreator()
      : base() {
      Parameters.Add(new LookupParameter<IClassificationProblemData>(ProblemDataParameterName, "The classification problem data for the Gaussian process model."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessClassificationModelCreator(this, cloner);
    }

    public override IOperation Apply() {
      try {
        var model = Create(ProblemData, Hyperparameter.ToArray(), MeanFunction, CovarianceFunction);
        ModelParameter.ActualValue = model;
        NegativeLogLikelihoodParameter.ActualValue = new DoubleValue(model.NegativeLogLikelihood);
        HyperparameterGradientsParameter.ActualValue = new RealVector(model.HyperparameterGradients);
        return base.Apply();
      }
      catch (ArgumentException) { }
      catch (alglib.alglibexception) { }
      NegativeLogLikelihoodParameter.ActualValue = new DoubleValue(1E300);
      HyperparameterGradientsParameter.ActualValue = new RealVector(Hyperparameter.Count());
      return base.Apply();
    }

    public static IGaussianProcessModel Create(IClassificationProblemData problemData, double[] hyperparameter, IMeanFunction meanFunction, ICovarianceFunction covarianceFunction) {
      return new GaussianProcessModel(problemData.Dataset, problemData.TargetVariable, problemData.AllowedInputVariables, problemData.TrainingIndices, hyperparameter, meanFunction, covarianceFunction);
    }
  }
}
