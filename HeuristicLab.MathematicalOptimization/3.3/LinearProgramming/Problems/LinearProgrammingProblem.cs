#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Drawing;
using Google.OrTools.LinearSolver;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.MathematicalOptimization.LinearProgramming.Problems {

  [Item("Linear/Mixed Integer Programming Problem (LP/MIP)", "")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems)]
  [StorableClass]
  public class LinearProgrammingProblem : Problem, IProgrammableItem {

    public LinearProgrammingProblem() {
      Parameters.Add(new FixedValueParameter<LinearProgrammingProblemDefinitionScript>("ProblemScript",
        "Defines the problem.", new LinearProgrammingProblemDefinitionScript { Name = Name }) { GetsCollected = false });
      RegisterEvents();
    }

    protected LinearProgrammingProblem(LinearProgrammingProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }

    [StorableConstructor]
    protected LinearProgrammingProblem(bool deserializing) : base(deserializing) { }

    public new static Image StaticItemImage => VSImageLibrary.Script;
    public ILinearProgrammingProblemDefinition ProblemDefinition => LinearProgrammingProblemScriptParameter.Value;
    public LinearProgrammingProblemDefinitionScript ProblemScript => LinearProgrammingProblemScriptParameter.Value;

    private FixedValueParameter<LinearProgrammingProblemDefinitionScript> LinearProgrammingProblemScriptParameter =>
      (FixedValueParameter<LinearProgrammingProblemDefinitionScript>)Parameters["ProblemScript"];

    public void BuildModel(Solver solver) => ProblemDefinition.BuildModel(solver);

    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearProgrammingProblem(this, cloner);
    }

    protected override void OnNameChanged() {
      base.OnNameChanged();
      ProblemScript.Name = Name;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void OnProblemDefinitionChanged() {
      OnOperatorsChanged();
      OnReset();
    }

    private void OnProblemScriptNameChanged() {
      Name = ProblemScript.Name;
    }

    private void RegisterEvents() {
      ProblemScript.ProblemDefinitionChanged += (o, e) => OnProblemDefinitionChanged();
      ProblemScript.NameChanged += (o, e) => OnProblemScriptNameChanged();
    }
  }
}
