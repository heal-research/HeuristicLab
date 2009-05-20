#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.PluginInfrastructure;
using System.Net;
using System.ServiceModel;
using HeuristicLab.CEDMA.DB.Interfaces;
using HeuristicLab.CEDMA.DB;
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.GP.StructureIdentification;
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Modeling;

namespace HeuristicLab.CEDMA.Server {
  public abstract class DispatcherBase : IDispatcher {
    private IStore store;
    public DispatcherBase(IStore store) {
      this.store = store;
    }

    public Execution GetNextJob() {
      // find and select a dataset
      var dataSetVar = new HeuristicLab.CEDMA.DB.Interfaces.Variable("DataSet");
      var dataSetQuery = new Statement[] {
        new Statement(dataSetVar, Ontology.PredicateInstanceOf, Ontology.TypeDataSet)
      };

      Entity[] datasets = store.Query("?DataSet <" + Ontology.PredicateInstanceOf.Uri + "> <" + Ontology.TypeDataSet.Uri + "> .", 0, 100)
        .Select(x => (Entity)x.Get("DataSet"))
        .ToArray();

      // no datasets => do nothing
      if (datasets.Length == 0) return null;

      Entity dataSetEntity = SelectDataSet(datasets);
      DataSet dataSet = new DataSet(store, dataSetEntity);

      int targetVariable = SelectTargetVariable(dataSetEntity, dataSet.Problem.AllowedTargetVariables.ToArray());
      IAlgorithm selectedAlgorithm = SelectAlgorithm(dataSetEntity, targetVariable, dataSet.Problem.LearningTask);
      string targetVariableName = dataSet.Problem.GetVariableName(targetVariable);


      if (selectedAlgorithm != null) {
        Execution exec = CreateExecution(dataSet.Problem, targetVariable, selectedAlgorithm);
        exec.DataSetEntity = dataSetEntity;
        exec.TargetVariable = targetVariableName;
        return exec;
      } else return null;
    }

    public abstract Entity SelectDataSet(Entity[] datasets);
    public abstract int SelectTargetVariable(Entity dataSet, int[] targetVariables);
    public abstract IAlgorithm SelectAlgorithm(Entity dataSet, int targetVariable, LearningTask learningTask);

    private Execution CreateExecution(Problem problem, int targetVariable, IAlgorithm algorithm) {
      SetProblemParameters(algorithm, problem, targetVariable);
      Execution exec = new Execution(algorithm.Engine);
      exec.Description = algorithm.Name;
      return exec;
    }

    private void SetProblemParameters(IAlgorithm algo, Problem problem, int targetVariable) {
      algo.ProblemInjector.GetVariable("Dataset").Value = problem.DataSet;
      algo.ProblemInjector.GetVariable("TargetVariable").GetValue<IntData>().Data = targetVariable;
      algo.ProblemInjector.GetVariable("TrainingSamplesStart").GetValue<IntData>().Data = problem.TrainingSamplesStart;
      algo.ProblemInjector.GetVariable("TrainingSamplesEnd").GetValue<IntData>().Data = problem.TrainingSamplesEnd;
      algo.ProblemInjector.GetVariable("ValidationSamplesStart").GetValue<IntData>().Data = problem.ValidationSamplesStart;
      algo.ProblemInjector.GetVariable("ValidationSamplesEnd").GetValue<IntData>().Data = problem.ValidationSamplesEnd;
      algo.ProblemInjector.GetVariable("TestSamplesStart").GetValue<IntData>().Data = problem.TestSamplesStart;
      algo.ProblemInjector.GetVariable("TestSamplesEnd").GetValue<IntData>().Data = problem.TestSamplesEnd;
      ItemList<IntData> allowedFeatures = algo.ProblemInjector.GetVariable("AllowedFeatures").GetValue<ItemList<IntData>>();
      foreach (int allowedFeature in problem.AllowedInputVariables) allowedFeatures.Add(new IntData(allowedFeature));

      if (problem.LearningTask == LearningTask.TimeSeries) {
        algo.ProblemInjector.GetVariable("Autoregressive").GetValue<BoolData>().Data = problem.AutoRegressive;
        algo.ProblemInjector.GetVariable("MinTimeOffset").GetValue<IntData>().Data = problem.MinTimeOffset;
        algo.ProblemInjector.GetVariable("MaxTimeOffset").GetValue<IntData>().Data = problem.MaxTimeOffset;
      } else if (problem.LearningTask == LearningTask.Classification) {
        ItemList<DoubleData> classValues = algo.ProblemInjector.GetVariable("TargetClassValues").GetValue<ItemList<DoubleData>>();
        foreach (double classValue in GetDifferentClassValues(problem.DataSet, targetVariable)) classValues.Add(new DoubleData(classValue));
      }
    }

    private IEnumerable<double> GetDifferentClassValues(HeuristicLab.DataAnalysis.Dataset dataset, int targetVariable) {
      return Enumerable.Range(0, dataset.Rows).Select(x => dataset.GetValue(x, targetVariable)).Distinct();
    }
  }
}
