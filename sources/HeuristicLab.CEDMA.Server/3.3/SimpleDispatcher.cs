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
  public class SimpleDispatcher : IDispatcher {
    private class AlgorithmConfiguration {
      public string name;
      public int targetVariable;
      public List<int> inputVariables;
    }

    private IModelingDatabase database;
    public IModelingDatabase Database {
      get {
        return database;
      }
    }

    private Problem problem;
    public Problem Problem {
      get {
        return problem;
      }
    }
    internal event EventHandler Changed;

    public IEnumerable<string> TargetVariables {
      get {
        return Enumerable.Range(0, problem.Dataset.Columns).Select(x => problem.Dataset.GetVariableName(x));
      }
    }

    public IEnumerable<string> InputVariables {
      get {
        return TargetVariables;
      }
    }

    private HeuristicLab.Modeling.IAlgorithm[] algorithms;
    public IEnumerable<HeuristicLab.Modeling.IAlgorithm> Algorithms {
      get {
        switch (Problem.LearningTask) {
          case LearningTask.Regression: {
              return algorithms.Where(a => (a as IClassificationAlgorithm) == null && (a as ITimeSeriesAlgorithm) == null);
            }
          case LearningTask.Classification: {
              return algorithms.Where(a => (a as IClassificationAlgorithm) != null);
            }
          case LearningTask.TimeSeries: {
              return algorithms.Where(a => (a as ITimeSeriesAlgorithm) != null);
            }
        }
        return new HeuristicLab.Modeling.IAlgorithm[] { };
      }
    }

    private List<HeuristicLab.Modeling.IAlgorithm> activeAlgorithms;
    public IEnumerable<HeuristicLab.Modeling.IAlgorithm> ActiveAlgorithms {
      get { return activeAlgorithms; }
    }

    private Random random;
    private List<int> allowedTargetVariables;
    private Dictionary<int, List<int>> activeInputVariables;
    private Dictionary<int, List<AlgorithmConfiguration>> finishedAndDispatchedRuns;
    private object locker = new object();

    public SimpleDispatcher(IModelingDatabase database, Problem problem) {
      this.problem = problem;
      this.database = database;
      this.random = new Random();

      this.finishedAndDispatchedRuns = new Dictionary<int, List<AlgorithmConfiguration>>();
      this.allowedTargetVariables = new List<int>();
      this.activeInputVariables = new Dictionary<int, List<int>>();
      this.activeAlgorithms = new List<HeuristicLab.Modeling.IAlgorithm>();
      DiscoveryService ds = new DiscoveryService();
      this.algorithms = ds.GetInstances<HeuristicLab.Modeling.IAlgorithm>();
      problem.Changed += (sender, args) => {
        lock (locker) {
          allowedTargetVariables.Clear();
          activeInputVariables.Clear();
          activeAlgorithms.Clear();
        }
        OnChanged();
      };

      PopulateFinishedRuns();
    }

    public HeuristicLab.Modeling.IAlgorithm GetNextJob() {
      lock (locker) {
        if (allowedTargetVariables.Count > 0) {
          int[] targetVariables = allowedTargetVariables.ToArray();
          int targetVariable = SelectTargetVariable(targetVariables);
          int[] inputVariables = activeInputVariables[targetVariable].ToArray();

          HeuristicLab.Modeling.IAlgorithm selectedAlgorithm = SelectAndConfigureAlgorithm(targetVariable, inputVariables, problem);

          return selectedAlgorithm;
        } else return null;
      }
    }

    public virtual int SelectTargetVariable(int[] targetVariables) {
      return targetVariables[random.Next(targetVariables.Length)];
    }

    public HeuristicLab.Modeling.IAlgorithm SelectAndConfigureAlgorithm(int targetVariable, int[] inputVariables, Problem problem) {
      HeuristicLab.Modeling.IAlgorithm selectedAlgorithm = null;
      DiscoveryService ds = new DiscoveryService();
      var allAlgorithms = ds.GetInstances<HeuristicLab.Modeling.IAlgorithm>();
      var allowedAlgorithmTypes = activeAlgorithms.Select(x => x.GetType());
      var possibleAlgos =
        allAlgorithms
        .Where(x => allowedAlgorithmTypes.Contains(x.GetType()) &&
          ((x is IStochasticAlgorithm) || !AlgorithmFinishedOrDispatched(targetVariable, inputVariables, x.Name)));
      if (possibleAlgos.Count() > 0) selectedAlgorithm = possibleAlgos.ElementAt(random.Next(possibleAlgos.Count()));
      if (selectedAlgorithm != null) {
        SetProblemParameters(selectedAlgorithm, problem, targetVariable, inputVariables);
        if (!(selectedAlgorithm is IStochasticAlgorithm))
          AddDispatchedRun(targetVariable, inputVariables, selectedAlgorithm.Name);
      }
      return selectedAlgorithm;
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
        ITimeSeriesAlgorithm timeSeriesAlgo = (ITimeSeriesAlgorithm)algo;
        timeSeriesAlgo.MinTimeOffset = problem.MinTimeOffset;
        timeSeriesAlgo.MaxTimeOffset = problem.MaxTimeOffset;
        if (problem.AutoRegressive) {
          allowedFeatures.Add(new IntData(targetVariable));
        }
      }
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

    public void EnableAlgorithm(HeuristicLab.Modeling.IAlgorithm algo) {
      lock (locker) {
        if (!activeAlgorithms.Contains(algo)) activeAlgorithms.Add(algo);
      }
    }

    public void DisableAlgorithm(HeuristicLab.Modeling.IAlgorithm algo) {
      lock (locker) {
        while (activeAlgorithms.Remove(algo)) ;
      }
    }

    internal void EnableTargetVariable(string name) {
      int varIndex = problem.Dataset.GetVariableIndex(name);
      lock (locker)
        if (!allowedTargetVariables.Contains(varIndex)) allowedTargetVariables.Add(varIndex);
    }

    internal void DisableTargetVariable(string name) {
      int varIndex = problem.Dataset.GetVariableIndex(name);
      lock (locker)
        while (allowedTargetVariables.Remove(varIndex)) ;
    }

    internal void EnableInputVariable(string target, string name) {
      int targetIndex = problem.Dataset.GetVariableIndex(target);
      int inputIndex = problem.Dataset.GetVariableIndex(name);
      lock (locker) {
        if (!activeInputVariables.ContainsKey(targetIndex)) activeInputVariables[targetIndex] = new List<int>();
        if (!activeInputVariables[targetIndex].Contains(inputIndex)) {
          activeInputVariables[targetIndex].Add(inputIndex);
        }
      }
    }

    internal void DisableInputVariable(string target, string name) {
      int targetIndex = problem.Dataset.GetVariableIndex(target);
      int inputIndex = problem.Dataset.GetVariableIndex(name);
      lock (locker) {
        if (!activeInputVariables.ContainsKey(targetIndex)) activeInputVariables[targetIndex] = new List<int>();
        while (activeInputVariables[targetIndex].Remove(inputIndex)) ;
      }
    }

    public void OnChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }

    internal IEnumerable<string> GetInputVariables(string target) {
      int targetIndex = problem.Dataset.GetVariableIndex(target);
      lock (locker) {
        if (!activeInputVariables.ContainsKey(targetIndex)) activeInputVariables[targetIndex] = new List<int>();
        return activeInputVariables[targetIndex]
          .Select(i => problem.Dataset.GetVariableName(i));
      }
    }


    #region IViewable Members

    public virtual IView CreateView() {
      return new DispatcherView(this);
    }

    #endregion


  }
}
