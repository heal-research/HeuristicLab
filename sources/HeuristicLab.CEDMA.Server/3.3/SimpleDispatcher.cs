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
using System.ServiceModel.Description;
using System.Linq;
using HeuristicLab.CEDMA.Core;
using HeuristicLab.Data;
using HeuristicLab.Core;
using HeuristicLab.Modeling;
using HeuristicLab.Modeling.Database;

namespace HeuristicLab.CEDMA.Server {
  public class SimpleDispatcher : DispatcherBase {
    private class AlgorithmConfiguration {
      public string name;
      public int targetVariable;
      public List<int> inputVariables;
    }

    private Random random;
    private Dictionary<int, List<AlgorithmConfiguration>> finishedAndDispatchedRuns;

    public SimpleDispatcher(IModelingDatabase database, Problem problem)
      : base(database, problem) {
      random = new Random();
      finishedAndDispatchedRuns = new Dictionary<int, List<AlgorithmConfiguration>>();
      PopulateFinishedRuns();
    }

    public override HeuristicLab.Modeling.IAlgorithm SelectAndConfigureAlgorithm(int targetVariable, int[] inputVariables, Problem problem) {
      DiscoveryService ds = new DiscoveryService();
      HeuristicLab.Modeling.IAlgorithm[] algos = ds.GetInstances<HeuristicLab.Modeling.IAlgorithm>();
      HeuristicLab.Modeling.IAlgorithm selectedAlgorithm = null;
      switch (problem.LearningTask) {
        case LearningTask.Regression: {
            var regressionAlgos = algos.Where(a => (a as IClassificationAlgorithm) == null && (a as ITimeSeriesAlgorithm) == null);
            selectedAlgorithm = ChooseDeterministic(targetVariable, inputVariables, regressionAlgos) ?? ChooseStochastic(regressionAlgos);
            break;
          }
        case LearningTask.Classification: {
            var classificationAlgos = algos.Where(a => (a as IClassificationAlgorithm) != null);
            selectedAlgorithm = ChooseDeterministic(targetVariable, inputVariables, classificationAlgos) ?? ChooseStochastic(classificationAlgos);
            break;
          }
        case LearningTask.TimeSeries: {
            var timeSeriesAlgos = algos.Where(a => (a as ITimeSeriesAlgorithm) != null);
            selectedAlgorithm = ChooseDeterministic(targetVariable, inputVariables, timeSeriesAlgos) ?? ChooseStochastic(timeSeriesAlgos);
            break;
          }
      }


      if (selectedAlgorithm != null) {
        SetProblemParameters(selectedAlgorithm, problem, targetVariable, inputVariables);
        AddDispatchedRun(targetVariable, inputVariables, selectedAlgorithm.Name);
      }
      return selectedAlgorithm;
    }

    private HeuristicLab.Modeling.IAlgorithm ChooseDeterministic(int targetVariable, int[] inputVariables, IEnumerable<HeuristicLab.Modeling.IAlgorithm> algos) {
      var deterministicAlgos = algos
        .Where(a => (a as IStochasticAlgorithm) == null)
        .Where(a => AlgorithmFinishedOrDispatched(targetVariable, inputVariables, a.Name) == false);

      if (deterministicAlgos.Count() == 0) return null;
      return deterministicAlgos.ElementAt(random.Next(deterministicAlgos.Count()));
    }

    private HeuristicLab.Modeling.IAlgorithm ChooseStochastic(IEnumerable<HeuristicLab.Modeling.IAlgorithm> regressionAlgos) {
      var stochasticAlgos = regressionAlgos.Where(a => (a as IStochasticAlgorithm) != null);
      if (stochasticAlgos.Count() == 0) return null;
      return stochasticAlgos.ElementAt(random.Next(stochasticAlgos.Count()));
    }

    private void PopulateFinishedRuns() {
      var dispatchedAlgos = from model in Database.GetAllModels()
                            select new {
                              TargetVariable = model.TargetVariable.Name,
                              Algorithm = model.Algorithm.Name,
                              Inputvariables = Database.GetInputVariableResults(model).Select(x => x.Variable.Name).Distinct()
                            };
      foreach (var algo in dispatchedAlgos) {
        AddDispatchedRun(algo.TargetVariable, algo.Inputvariables, algo.Algorithm);
      }
    }

    private void SetProblemParameters(HeuristicLab.Modeling.IAlgorithm algo, Problem problem, int targetVariable, int[] inputVariables) {
      algo.Dataset = problem.Dataset;
      algo.TargetVariable = targetVariable;
      algo.ProblemInjector.GetVariable("TrainingSamplesStart").GetValue<IntData>().Data = problem.TrainingSamplesStart;
      algo.ProblemInjector.GetVariable("TrainingSamplesEnd").GetValue<IntData>().Data = problem.TrainingSamplesEnd;
      algo.ProblemInjector.GetVariable("ValidationSamplesStart").GetValue<IntData>().Data = problem.ValidationSamplesStart;
      algo.ProblemInjector.GetVariable("ValidationSamplesEnd").GetValue<IntData>().Data = problem.ValidationSamplesEnd;
      algo.ProblemInjector.GetVariable("TestSamplesStart").GetValue<IntData>().Data = problem.TestSamplesStart;
      algo.ProblemInjector.GetVariable("TestSamplesEnd").GetValue<IntData>().Data = problem.TestSamplesEnd;
      ItemList<IntData> allowedFeatures = algo.ProblemInjector.GetVariable("AllowedFeatures").GetValue<ItemList<IntData>>();
      foreach (int inputVariable in inputVariables) {
        if (inputVariable != targetVariable) {
          allowedFeatures.Add(new IntData(inputVariable));
        }
      }

      if (problem.LearningTask == LearningTask.TimeSeries) {
        algo.ProblemInjector.GetVariable("Autoregressive").GetValue<BoolData>().Data = problem.AutoRegressive;
        algo.ProblemInjector.GetVariable("MinTimeOffset").GetValue<IntData>().Data = problem.MinTimeOffset;
        algo.ProblemInjector.GetVariable("MaxTimeOffset").GetValue<IntData>().Data = problem.MaxTimeOffset;
        if (problem.AutoRegressive) {
          allowedFeatures.Add(new IntData(targetVariable));
        }
      } else if (problem.LearningTask == LearningTask.Classification) {
        ItemList<DoubleData> classValues = algo.ProblemInjector.GetVariable("TargetClassValues").GetValue<ItemList<DoubleData>>();
        foreach (double classValue in GetDifferentClassValues(problem.Dataset, targetVariable)) classValues.Add(new DoubleData(classValue));
      }
    }

    private IEnumerable<double> GetDifferentClassValues(HeuristicLab.DataAnalysis.Dataset dataset, int targetVariable) {
      return Enumerable.Range(0, dataset.Rows).Select(x => dataset.GetValue(x, targetVariable)).Distinct();
    }

    private void AddDispatchedRun(string targetVariable, IEnumerable<string> inputVariables, string algorithm) {
      AddDispatchedRun(
        Problem.Dataset.GetVariableIndex(targetVariable),
        inputVariables.Select(x => Problem.Dataset.GetVariableIndex(x)).ToArray(),
        algorithm);
    }

    private void AddDispatchedRun(int targetVariable, int[] inputVariables, string algoName) {
      if (!finishedAndDispatchedRuns.ContainsKey(targetVariable)) {
        finishedAndDispatchedRuns[targetVariable] = new List<AlgorithmConfiguration>();
      }
      AlgorithmConfiguration conf = new AlgorithmConfiguration();
      conf.name = algoName;
      conf.inputVariables = new List<int>(inputVariables);
      conf.targetVariable = targetVariable;
      finishedAndDispatchedRuns[targetVariable].Add(conf);
    }

    private bool AlgorithmFinishedOrDispatched(int targetVariable, int[] inputVariables, string algoName) {
      return
        finishedAndDispatchedRuns.ContainsKey(targetVariable) &&
        finishedAndDispatchedRuns[targetVariable].Any(x => targetVariable == x.targetVariable &&
                                                           algoName == x.name &&
                                                           inputVariables.Count() == x.inputVariables.Count() &&
                                                           inputVariables.All(v => x.inputVariables.Contains(v)));
    }
  }
}
