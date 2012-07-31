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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [StorableClass]
  [Item(Name = "GaussianProcessRegressionModelCreator",
    Description = "Creates a Gaussian process model for regression given the data, the hyperparameters, a mean function, and a covariance function.")]
  public sealed class GaussianProcessRegressionModelCreator : GaussianProcessModelCreator {
    private const string ProblemDataParameterName = "ProblemData";

    #region Parameter Properties
    public ILookupParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    #endregion

    #region Properties
    public IRegressionProblemData ProblemData {
      get { return ProblemDataParameter.ActualValue; }
    }
    #endregion
    [StorableConstructor]
    private GaussianProcessRegressionModelCreator(bool deserializing) : base(deserializing) { }
    private GaussianProcessRegressionModelCreator(GaussianProcessRegressionModelCreator original, Cloner cloner) : base(original, cloner) { }
    public GaussianProcessRegressionModelCreator()
      : base() {
      Parameters.Add(new LookupParameter<IRegressionProblemData>(ProblemDataParameterName, "The regression problem data for the Gaussian process model."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new GaussianProcessRegressionModelCreator(this, cloner);
    }

    public override IOperation Apply() {
      var model = Create(ProblemData, Hyperparameter.ToArray(), MeanFunction, CovarianceFunction);
      ModelParameter.ActualValue = model;
      NegativeLogLikelihoodParameter.ActualValue = new DoubleValue(model.NegativeLogLikelihood);
      HyperparameterGradientsParameter.ActualValue = new DoubleArray(model.GetHyperparameterGradients());
      return base.Apply();
    }

    public static IGaussianProcessModel Create(IRegressionProblemData problemData, double[] hyperparameter, IMeanFunction meanFunction, ICovarianceFunction covarianceFunction) {
      return new GaussianProcessModel(problemData.Dataset, problemData.TargetVariable, problemData.AllowedInputVariables, problemData.TrainingIndices, hyperparameter, meanFunction, covarianceFunction);
    }
  }
}
