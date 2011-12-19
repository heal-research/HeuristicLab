#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Classification {
  [Item("Classification Problem", "Represents a classfication problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public abstract class SingleObjectiveClassificationProblem<T, U> : SingleObjectiveHeuristicOptimizationProblem<T, U>, ISingleObjectiveDataAnalysisProblem
    where T : class, ISingleObjectiveEvaluator
    where U : class, ISolutionCreator {
    private const string ClassificationProblemDataParameterName = "ClassificationProblemData";

    public IValueParameter<ClassificationProblemData> ClassificationProblemDataParameter {
      get { return (IValueParameter<ClassificationProblemData>)Parameters[ClassificationProblemDataParameterName]; }
    }
    public ClassificationProblemData ClassificationProblemData {
      get { return ClassificationProblemDataParameter.Value; }
      set { ClassificationProblemDataParameter.Value = value; }
    }

    DataAnalysisProblemData IDataAnalysisProblem.DataAnalysisProblemData {
      get { return ClassificationProblemData; }
    }

    [StorableConstructor]
    protected SingleObjectiveClassificationProblem(bool deserializing) : base(deserializing) { }
    protected SingleObjectiveClassificationProblem(SingleObjectiveClassificationProblem<T, U> original, Cloner cloner)
      : base(original, cloner) {
      Initialize();
    }
    public SingleObjectiveClassificationProblem()
      : base() {
      Parameters.Add(new ValueParameter<ClassificationProblemData>(ClassificationProblemDataParameterName, "The data set, target variable and input variables of the data analysis problem."));
      ClassificationProblemData = new ClassificationProblemData();
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }
    private void Initialize() {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    private void RegisterParameterEvents() {
      ClassificationProblemDataParameter.ValueChanged += new EventHandler(ClassificationProblemDataParameter_ValueChanged);
    }
    private void RegisterParameterValueEvents() {
      ClassificationProblemData.ProblemDataChanged += new EventHandler(ClassificationProblemData_ProblemDataChanged);
    }

    protected virtual void OnClassificationProblemDataChanged() {
      OnReset();
    }
    private void ClassificationProblemDataParameter_ValueChanged(object sender, System.EventArgs e) {
      RegisterParameterValueEvents();
      OnClassificationProblemDataChanged();
    }
    private void ClassificationProblemData_ProblemDataChanged(object sender, System.EventArgs e) {
      OnClassificationProblemDataChanged();
    }
  }
}