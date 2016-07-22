#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  [Item("NcaSolutionCreator", "Creates an NCA solution with a given model and some given data.")]
  [StorableClass]
  public class NcaSolutionCreator : SingleSuccessorOperator, INcaSolutionCreator {

    public ILookupParameter<IClassificationProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IClassificationProblemData>)Parameters["ProblemData"]; }
    }

    public ILookupParameter<INcaModel> NcaModelParameter {
      get { return (ILookupParameter<INcaModel>)Parameters["NcaModel"]; }
    }

    public ILookupParameter<INcaClassificationSolution> NcaSolutionParameter {
      get { return (ILookupParameter<INcaClassificationSolution>)Parameters["NcaSolution"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    [StorableConstructor]
    protected NcaSolutionCreator(bool deserializing) : base(deserializing) { }
    protected NcaSolutionCreator(NcaSolutionCreator original, Cloner cloner) : base(original, cloner) { }
    public NcaSolutionCreator()
      : base() {
      Parameters.Add(new LookupParameter<IClassificationProblemData>("ProblemData", "The classification problem data."));
      Parameters.Add(new LookupParameter<INcaModel>("NcaModel", "The NCA model that should be created."));
      Parameters.Add(new LookupParameter<INcaClassificationSolution>("NcaSolution", "The created NCA solution."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The results collection to store the results."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NcaSolutionCreator(this, cloner);
    }

    public override IOperation Apply() {
      var problemData = ProblemDataParameter.ActualValue;
      var model = NcaModelParameter.ActualValue;
      var results = ResultsParameter.ActualValue;

      var s = model.CreateClassificationSolution(problemData);
      NcaSolutionParameter.ActualValue = s;

      if (!results.ContainsKey("Solution")) {
        results.Add(new Result("Solution", "The NCA classification solution", s));
        results.Add(new Result("Accuracy (training)",
                               "The accuracy of the NCA solution on the training partition.",
                               new DoubleValue(s.TrainingAccuracy)));
        results.Add(new Result("Accuracy (test)",
                               "The accuracy of the NCA solution on the test partition.",
                               new DoubleValue(s.TestAccuracy)));
      } else {
        results["Solution"].Value = s;
        results["Accuracy (training)"].Value = new DoubleValue(s.TrainingAccuracy);
        results["Accuracy (test)"].Value = new DoubleValue(s.TestAccuracy);
      }
      return base.Apply();
    }
  }
}
