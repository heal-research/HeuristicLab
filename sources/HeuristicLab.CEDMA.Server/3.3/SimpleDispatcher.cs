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
  public class SimpleDispatcher : DispatcherBase {
    private class AlgorithmConfiguration {
      public string name;
      public int targetVariable;
      public List<int> inputVariables;
    }

    private Random random;
    private IStore store;
    private Dictionary<int, List<AlgorithmConfiguration>> finishedAndDispatchedRuns;

    public SimpleDispatcher(IStore store)
      : base(store) {
      this.store = store;
      random = new Random();
      finishedAndDispatchedRuns = new Dictionary<int, List<AlgorithmConfiguration>>();
      PopulateFinishedRuns();
    }

    public override IAlgorithm SelectAndConfigureAlgorithm(int[] targetVariables, int[] inputVariables, Problem problem) {
      int targetVariable = SelectTargetVariable(targetVariables);

      DiscoveryService ds = new DiscoveryService();
      IAlgorithm[] algos = ds.GetInstances<IAlgorithm>();
      IAlgorithm selectedAlgorithm = null;
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

    private IAlgorithm ChooseDeterministic(int targetVariable, int[] inputVariables, IEnumerable<IAlgorithm> algos) {
      var deterministicAlgos = algos
        .Where(a => (a as IStochasticAlgorithm) == null)
        .Where(a => AlgorithmFinishedOrDispatched(targetVariable, inputVariables, a.Name) == false);

      if (deterministicAlgos.Count() == 0) return null;
      return deterministicAlgos.ElementAt(random.Next(deterministicAlgos.Count()));
    }

    private IAlgorithm ChooseStochastic(IEnumerable<IAlgorithm> regressionAlgos) {
      var stochasticAlgos = regressionAlgos.Where(a => (a as IStochasticAlgorithm) != null);
      if (stochasticAlgos.Count() == 0) return null;
      return stochasticAlgos.ElementAt(random.Next(stochasticAlgos.Count()));
    }

    public int SelectTargetVariable(int[] targetVariables) {
      return targetVariables[random.Next(targetVariables.Length)];
    }

    private void PopulateFinishedRuns() {
      Dictionary<Entity, Entity> processedModels = new Dictionary<Entity, Entity>();
      var datasetBindings = store
        .Query(
        "?Dataset <" + Ontology.InstanceOf + "> <" + Ontology.TypeDataSet + "> .", 0, 1)
        .Select(x => (Entity)x.Get("Dataset"));

      if (datasetBindings.Count() > 0) {
        var datasetEntity = datasetBindings.ElementAt(0);

        DataSet ds = new DataSet(store, datasetEntity);
        var result = store
          .Query(
          "?Model <" + Ontology.TargetVariable + "> ?TargetVariable ." + Environment.NewLine +
          "?Model <" + Ontology.Name + "> ?AlgoName .",
          0, 1000)
          .Select(x => new Resource[] { (Literal)x.Get("TargetVariable"), (Literal)x.Get("AlgoName"), (Entity)x.Get("Model") });

        foreach (Resource[] row in result) {
          Entity model = ((Entity)row[2]);
          if (!processedModels.ContainsKey(model)) {
            processedModels.Add(model, model);

            string targetVariable = (string)((Literal)row[0]).Value;
            string algoName = (string)((Literal)row[1]).Value;
            int targetVariableIndex = ds.Problem.Dataset.GetVariableIndex(targetVariable);

            var inputVariableLiterals = store
              .Query(
                "<" + model.Uri + "> <" + Ontology.HasInputVariable + "> ?InputVariable ." + Environment.NewLine +
                "?InputVariable <" + Ontology.Name + "> ?Name .",
                0, 1000)
              .Select(x => (Literal)x.Get("Name"))
              .Select(l => (string)l.Value)
              .Distinct();

            List<int> inputVariables = new List<int>();
            foreach (string variableName in inputVariableLiterals) {
              int variableIndex = ds.Problem.Dataset.GetVariableIndex(variableName);
              inputVariables.Add(variableIndex);
            }
            if (!AlgorithmFinishedOrDispatched(targetVariableIndex, inputVariables.ToArray(), algoName)) {
              AddDispatchedRun(targetVariableIndex, inputVariables.ToArray(), algoName);
            }
          }
        }
      }
    }

    private void SetProblemParameters(IAlgorithm algo, Problem problem, int targetVariable, int[] inputVariables) {
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
