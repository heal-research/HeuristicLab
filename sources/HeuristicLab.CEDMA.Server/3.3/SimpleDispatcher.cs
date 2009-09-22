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
using HeuristicLab.DataAnalysis;

namespace HeuristicLab.CEDMA.Server {
  public class SimpleDispatcher : IDispatcher, IViewable {
    private class AlgorithmConfiguration {
      public string name;
      public ProblemSpecification problemSpecification;
    }

    internal event EventHandler Changed;

    private IModelingDatabase database;
    public IModelingDatabase Database {
      get {
        return database;
      }
    }

    private Dataset dataset;
    public Dataset Dataset {
      get {
        return dataset;
      }
    }

    public IEnumerable<string> TargetVariables {
      get {
        return Enumerable.Range(0, Dataset.Columns).Select(x => Dataset.GetVariableName(x));
      }
    }

    public IEnumerable<string> Variables {
      get {
        return TargetVariables;
      }
    }

    private HeuristicLab.Modeling.IAlgorithm[] defaultAlgorithms;
    public IEnumerable<HeuristicLab.Modeling.IAlgorithm> GetAlgorithms(LearningTask task) {
      switch (task) {
        case LearningTask.Regression: {
            return defaultAlgorithms.Where(a => (a as IClassificationAlgorithm) == null && (a as ITimeSeriesAlgorithm) == null);
          }
        case LearningTask.Classification: {
            return defaultAlgorithms.Where(a => (a as IClassificationAlgorithm) != null);
          }
        case LearningTask.TimeSeries: {
            return defaultAlgorithms.Where(a => (a as ITimeSeriesAlgorithm) != null);
          }
        default: {
            return new HeuristicLab.Modeling.IAlgorithm[] { };
          }
      }
    }

    private Random random;
    private Dictionary<string, ProblemSpecification> problemSpecifications;
    private Dictionary<string, List<HeuristicLab.Modeling.IAlgorithm>> algorithms;
    public IEnumerable<HeuristicLab.Modeling.IAlgorithm> GetAllowedAlgorithms(string targetVariable) {
      if (algorithms.ContainsKey(targetVariable))
        return algorithms[targetVariable];
      else return new HeuristicLab.Modeling.IAlgorithm[] { };
    }
    private Dictionary<string, bool> activeVariables;
    public IEnumerable<string> AllowedTargetVariables {
      get { return activeVariables.Where(x => x.Value).Select(x => x.Key); }
    }
    private Dictionary<string, List<AlgorithmConfiguration>> finishedAndDispatchedRuns;
    private object locker = new object();

    public SimpleDispatcher(IModelingDatabase database, Dataset dataset) {
      this.dataset = dataset;
      this.database = database;
      dataset.Changed += (sender, args) => FireChanged();
      random = new Random();

      activeVariables = new Dictionary<string, bool>();
      problemSpecifications = new Dictionary<string, ProblemSpecification>();
      algorithms = new Dictionary<string, List<HeuristicLab.Modeling.IAlgorithm>>();
      finishedAndDispatchedRuns = new Dictionary<string, List<AlgorithmConfiguration>>();

      DiscoveryService ds = new DiscoveryService();
      defaultAlgorithms = ds.GetInstances<HeuristicLab.Modeling.IAlgorithm>();

      // PopulateFinishedRuns();
    }

    public HeuristicLab.Modeling.IAlgorithm GetNextJob() {
      lock (locker) {
        if (activeVariables.Count > 0) {
          string[] targetVariables = activeVariables.Keys.ToArray();
          string targetVariable = SelectTargetVariable(targetVariables);
          HeuristicLab.Modeling.IAlgorithm selectedAlgorithm = SelectAndConfigureAlgorithm(targetVariable);

          return selectedAlgorithm;
        } else return null;
      }
    }

    public virtual string SelectTargetVariable(string[] targetVariables) {
      return targetVariables[random.Next(targetVariables.Length)];
    }

    public HeuristicLab.Modeling.IAlgorithm SelectAndConfigureAlgorithm(string targetVariable) {
      HeuristicLab.Modeling.IAlgorithm selectedAlgorithm = null;
      var possibleAlgos =
        algorithms[targetVariable]
        .Where(x =>
          ((x is IStochasticAlgorithm) || !AlgorithmFinishedOrDispatched(problemSpecifications[targetVariable], x.Name)));
      if (possibleAlgos.Count() > 0) selectedAlgorithm = possibleAlgos.ElementAt(random.Next(possibleAlgos.Count()));
      if (selectedAlgorithm != null) {
        // create a clone of the algorithm template before setting the parameters
        selectedAlgorithm = (HeuristicLab.Modeling.IAlgorithm)selectedAlgorithm.Clone(); 
        SetProblemParameters(selectedAlgorithm, problemSpecifications[targetVariable]);
        if (!(selectedAlgorithm is IStochasticAlgorithm))
          AddDispatchedRun(problemSpecifications[targetVariable], selectedAlgorithm.Name);
      }
      return selectedAlgorithm;
    }

    //private void PopulateFinishedRuns() {
    //  var dispatchedAlgos = from model in Database.GetAllModels()
    //                        select new {
    //                          TargetVariable = model.TargetVariable.Name,
    //                          Algorithm = model.Algorithm.Name,
    //                          InputVariables = Database.GetInputVariableResults(model).Select(x => x.Variable.Name).Distinct(),
    //                        };
    //  foreach (var algo in dispatchedAlgos) {
    //    ProblemSpecification spec = new ProblemSpecification();
    //    spec.TargetVariable = algo.TargetVariable;
    //    foreach (string variable in algo.InputVariables) spec.AddInputVariable(variable);
    //    AddDispatchedRun(spec, algo.Algorithm);
    //  }
    //}

    private void SetProblemParameters(HeuristicLab.Modeling.IAlgorithm algo, ProblemSpecification spec) {
      algo.Dataset = spec.Dataset;
      algo.TargetVariable = spec.Dataset.GetVariableIndex(spec.TargetVariable);
      algo.TrainingSamplesStart = spec.TrainingSamplesStart;
      algo.TrainingSamplesEnd = spec.TrainingSamplesEnd;
      algo.ValidationSamplesStart = spec.ValidationSamplesStart;
      algo.ValidationSamplesEnd = spec.ValidationSamplesEnd;
      algo.TestSamplesStart = spec.TestSamplesStart;
      algo.TestSamplesEnd = spec.TestSamplesEnd;
      List<int> allowedFeatures = new List<int>();
      foreach (string inputVariable in spec.InputVariables) {
        if (inputVariable != spec.TargetVariable) {
          allowedFeatures.Add(spec.Dataset.GetVariableIndex(inputVariable));
        }
      }

      if (spec.LearningTask == LearningTask.TimeSeries) {
        ITimeSeriesAlgorithm timeSeriesAlgo = (ITimeSeriesAlgorithm)algo;
        timeSeriesAlgo.MinTimeOffset = spec.MinTimeOffset;
        timeSeriesAlgo.MaxTimeOffset = spec.MaxTimeOffset;
        if (spec.AutoRegressive) {
          allowedFeatures.Add(spec.Dataset.GetVariableIndex(spec.TargetVariable));
        }
      }
      algo.AllowedVariables = allowedFeatures;
    }


    private void AddDispatchedRun(ProblemSpecification specification, string algorithm) {
      AlgorithmConfiguration conf = new AlgorithmConfiguration();
      conf.name = algorithm;
      conf.problemSpecification = new ProblemSpecification(specification);
      if (!finishedAndDispatchedRuns.ContainsKey(specification.TargetVariable))
        finishedAndDispatchedRuns.Add(specification.TargetVariable, new List<AlgorithmConfiguration>());
      finishedAndDispatchedRuns[specification.TargetVariable].Add(conf);
    }

    private bool AlgorithmFinishedOrDispatched(ProblemSpecification specification, string algoName) {
      return
        finishedAndDispatchedRuns.ContainsKey(specification.TargetVariable) &&
        finishedAndDispatchedRuns[specification.TargetVariable].Any(x =>
                                                           algoName == x.name &&
                                                           specification.Equals(x.problemSpecification));
    }

    internal void EnableTargetVariable(string name) {
      activeVariables[name] = true;
    }

    internal void DisableTargetVariable(string name) {
      activeVariables[name] = false;
    }

    public void EnableAlgorithm(string targetVariable, HeuristicLab.Modeling.IAlgorithm algo) {
      if (!algorithms.ContainsKey(targetVariable)) algorithms.Add(targetVariable, new List<HeuristicLab.Modeling.IAlgorithm>());
      algorithms[targetVariable].Add(algo);
    }

    public void DisableAlgorithm(string targetVariable, HeuristicLab.Modeling.IAlgorithm algo) {
      algorithms[targetVariable].Remove(algo);
    }

    public ProblemSpecification GetProblemSpecification(string targetVariable) {
      if (!problemSpecifications.ContainsKey(targetVariable))
        problemSpecifications[targetVariable] = CreateDefaultProblemSpecification(targetVariable);

      return problemSpecifications[targetVariable];
    }

    //internal void EnableInputVariable(string target, string name) {
    //  problemSpecifications[target].AddInputVariable(name);
    //}

    //internal void DisableInputVariable(string target, string name) {
    //  problemSpecifications[target].RemoveInputVariable(name);
    //}

    //internal void SetLearningTask(string target, LearningTask task) {
    //  problemSpecifications[target].LearningTask = task;
    //}

    //internal void SetDatasetBoundaries(
    //  string target,
    //  int trainingStart, int trainingEnd,
    //  int validationStart, int validationEnd,
    //  int testStart, int testEnd) {
    //  problemSpecifications[target].TrainingSamplesStart = trainingStart;
    //  problemSpecifications[target].TrainingSamplesEnd = trainingEnd;
    //  problemSpecifications[target].ValidationSamplesStart = validationStart;
    //  problemSpecifications[target].ValidationSamplesEnd = validationEnd;
    //  problemSpecifications[target].TestSamplesStart = testStart;
    //  problemSpecifications[target].TestSamplesEnd = testEnd;
    //}

    private ProblemSpecification CreateDefaultProblemSpecification(string targetVariable) {
      ProblemSpecification spec = new ProblemSpecification();
      spec.Dataset = dataset;
      spec.TargetVariable = targetVariable;
      spec.LearningTask = LearningTask.Regression;
      int targetColumn = dataset.GetVariableIndex(targetVariable);
      // find index of first correct target value
      int firstValueIndex;
      for (firstValueIndex = 0; firstValueIndex < dataset.Rows; firstValueIndex++) {
        double x = dataset.GetValue(firstValueIndex, targetColumn);
        if (!(double.IsNaN(x) || double.IsInfinity(x))) break;
      }
      // find index of last correct target value
      int lastValueIndex;
      for (lastValueIndex = dataset.Rows - 1; lastValueIndex > firstValueIndex; lastValueIndex--) {
        double x = dataset.GetValue(lastValueIndex, targetColumn);
        if (!(double.IsNaN(x) || double.IsInfinity(x))) break;
      }

      int validTargetRange = lastValueIndex - firstValueIndex;
      spec.TrainingSamplesStart = firstValueIndex;
      spec.TrainingSamplesEnd = firstValueIndex + (int)Math.Floor(validTargetRange * 0.5);
      spec.ValidationSamplesStart = spec.TrainingSamplesEnd;
      spec.ValidationSamplesEnd = firstValueIndex + (int)Math.Floor(validTargetRange * 0.75);
      spec.TestSamplesStart = spec.ValidationSamplesEnd;
      spec.TestSamplesEnd = lastValueIndex;
      return spec;
    }

    public void FireChanged() {
      if (Changed != null) Changed(this, new EventArgs());
    }

    #region IViewable Members

    public virtual IView CreateView() {
      return new DispatcherView(this);
    }

    #endregion
  }
}
