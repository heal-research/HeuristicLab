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
    private Random random;
    private IStore store;
    private Dictionary<Entity, Dictionary<int, List<string>>> finishedAndDispatchedRuns;

    public SimpleDispatcher(IStore store)
      : base(store) {
      this.store = store;
      random = new Random();
      finishedAndDispatchedRuns = new Dictionary<Entity, Dictionary<int, List<string>>>();
      PopulateFinishedRuns();
    }

    public override IAlgorithm SelectAlgorithm(Entity dataSetEntity, int targetVariable, LearningTask learningTask) {
      DiscoveryService ds = new DiscoveryService();
      IAlgorithm[] algos = ds.GetInstances<IAlgorithm>();
      IAlgorithm selectedAlgorithm = null;
      switch (learningTask) {
        case LearningTask.Regression: {
            var regressionAlgos = algos.Where(a => (a as IClassificationAlgorithm) == null && (a as ITimeSeriesAlgorithm) == null);
            selectedAlgorithm = ChooseDeterministic(dataSetEntity, targetVariable, regressionAlgos) ?? ChooseStochastic(regressionAlgos);
            break;
          }
        case LearningTask.Classification: {
            var classificationAlgos = algos.Where(a => (a as IClassificationAlgorithm) != null);
            selectedAlgorithm = ChooseDeterministic(dataSetEntity, targetVariable, classificationAlgos) ?? ChooseStochastic(classificationAlgos);
            break;
          }
        case LearningTask.TimeSeries: {
            var timeSeriesAlgos = algos.Where(a => (a as ITimeSeriesAlgorithm) != null);
            selectedAlgorithm = ChooseDeterministic(dataSetEntity, targetVariable, timeSeriesAlgos) ?? ChooseStochastic(timeSeriesAlgos);
            break;
          }
      }
      if (selectedAlgorithm != null) {
        AddDispatchedRun(dataSetEntity, targetVariable, selectedAlgorithm.Name);
      }
      return selectedAlgorithm;
    }

    private IAlgorithm ChooseDeterministic(Entity dataSetEntity, int targetVariable, IEnumerable<IAlgorithm> algos) {
      var deterministicAlgos = algos
        .Where(a => (a as IStochasticAlgorithm) == null)
        .Where(a => AlgorithmFinishedOrDispatched(dataSetEntity, targetVariable, a.Name) == false);

      if (deterministicAlgos.Count() == 0) return null;
      return deterministicAlgos.ElementAt(random.Next(deterministicAlgos.Count()));
    }

    private IAlgorithm ChooseStochastic(IEnumerable<IAlgorithm> regressionAlgos) {
      var stochasticAlgos = regressionAlgos.Where(a => (a as IStochasticAlgorithm) != null);
      if (stochasticAlgos.Count() == 0) return null;
      return stochasticAlgos.ElementAt(random.Next(stochasticAlgos.Count()));
    }

    public override Entity SelectDataSet(Entity[] datasets) {
      return datasets[random.Next(datasets.Length)];
    }

    public override int SelectTargetVariable(Entity dataSet, int[] targetVariables) {
      return targetVariables[random.Next(targetVariables.Length)];
    }

    private void PopulateFinishedRuns() {
      var result = store
        .Query("?DataSet <" + Ontology.PredicateInstanceOf.Uri + "> <" + Ontology.TypeDataSet.Uri + "> ." + Environment.NewLine +
        "?DataSet <" + Ontology.PredicateHasModel + "> ?Model ." + Environment.NewLine +
        "?Model <" + Ontology.TargetVariable + "> ?TargetVariable ." + Environment.NewLine +
        "?Model <" + Ontology.AlgorithmName + "> ?AlgoName .",
        0, 1000)
        .Select(x => new Resource[] { (Entity)x.Get("DataSet"), (Literal)x.Get("TargetVariable"), (Literal)x.Get("AlgoName") });

      foreach (Resource[] row in result) {
        Entity dataset = (Entity)row[0];
        int targetVariable = (int)((Literal)row[1]).Value;
        string algoName = (string)((Literal)row[2]).Value;
        if (!AlgorithmFinishedOrDispatched(dataset, targetVariable, algoName))
          AddDispatchedRun(dataset, targetVariable, algoName);
      }
    }

    private void AddDispatchedRun(Entity dataSetEntity, int targetVariable, string algoName) {
      if (!finishedAndDispatchedRuns.ContainsKey(dataSetEntity)) {
        finishedAndDispatchedRuns[dataSetEntity] = new Dictionary<int, List<string>>();
      }
      if (!finishedAndDispatchedRuns[dataSetEntity].ContainsKey(targetVariable)) {
        finishedAndDispatchedRuns[dataSetEntity][targetVariable] = new List<string>();
      }
      finishedAndDispatchedRuns[dataSetEntity][targetVariable].Add(algoName);
    }

    private bool AlgorithmFinishedOrDispatched(Entity dataSetEntity, int targetVariable, string algoName) {
      return
        finishedAndDispatchedRuns.ContainsKey(dataSetEntity) &&
        finishedAndDispatchedRuns[dataSetEntity].ContainsKey(targetVariable) &&
        finishedAndDispatchedRuns[dataSetEntity][targetVariable].Contains(algoName);
    }
  }
}
