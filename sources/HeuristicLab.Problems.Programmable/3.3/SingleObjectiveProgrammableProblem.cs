#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Programmable {
  [Item("Programmable Problem (single-objective)", "Represents a single-objective problem that can be programmed with a script.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class SingleObjectiveProgrammableProblem : SingleObjectiveBasicProblem<IEncoding> {
    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    private FixedValueParameter<SingleObjectiveProblemDefinitionScript> SingleObjectiveProblemScriptParameter {
      get { return (FixedValueParameter<SingleObjectiveProblemDefinitionScript>)Parameters["ProblemScript"]; }
    }

    public SingleObjectiveProblemDefinitionScript ProblemScript {
      get { return SingleObjectiveProblemScriptParameter.Value; }
    }

    public ISingleObjectiveProblemDefinition ProblemDefinition {
      get { return SingleObjectiveProblemScriptParameter.Value; }
    }

    private SingleObjectiveProgrammableProblem(SingleObjectiveProgrammableProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SingleObjectiveProgrammableProblem(this, cloner); }

    [StorableConstructor]
    private SingleObjectiveProgrammableProblem(bool deserializing) : base(deserializing) { }
    public SingleObjectiveProgrammableProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<SingleObjectiveProblemDefinitionScript>("ProblemScript", "Defines the problem.", new SingleObjectiveProblemDefinitionScript() { Name = Name }));
      Operators.Add(new BestScopeSolutionAnalyzer());
      RegisterEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      ProblemScript.ProblemDefinitionChanged += (o, e) => OnProblemDefinitionChanged();
    }

    private void OnProblemDefinitionChanged() {
      Encoding = ProblemDefinition.Encoding;
    }

    public override bool Maximization {
      get { return ProblemDefinition.Maximization; }
    }

    public override double Evaluate(Individual individual, IRandom random) {
      return ProblemDefinition.Evaluate(individual, random);
    }

    public override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      ProblemDefinition.Analyze(individuals, qualities, results, random);
    }
    public override IEnumerable<Individual> GetNeighbors(Individual individual, IRandom random) {
      return ProblemDefinition.GetNeighbors(individual, random);
    }
  }
}
