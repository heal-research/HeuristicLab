#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.Programmable {
  [Item("Programmable Problem (single-objective)", "Represents a single-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 110)]
  [StorableType("44944E6B-E95E-4805-8F0A-0C0F7D761DB9")]
  public class SingleObjectiveProgrammableProblem<TEncoding, TEncodedSolution> : SingleObjectiveProblem<TEncoding, TEncodedSolution>, IProgrammableItem, IProgrammableProblem
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    protected static readonly string ENCODING_NAMESPACE = "ENCODING_NAMESPACE";
    protected static readonly string ENCODING_CLASS = "ENCODING_CLASS";
    protected static readonly string SOLUTION_CLASS = "SOLUTION_CLASS";

    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    private FixedValueParameter<SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>> SingleObjectiveProblemScriptParameter {
      get { return (FixedValueParameter<SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>>)Parameters["ProblemScript"]; }
    }

    Script IProgrammableProblem.ProblemScript {
      get { return ProblemScript; }
    }
    public SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution> ProblemScript {
      get { return SingleObjectiveProblemScriptParameter.Value; }
    }

    public ISingleObjectiveProblemDefinition<TEncoding, TEncodedSolution> ProblemDefinition {
      get { return SingleObjectiveProblemScriptParameter.Value; }
    }

    protected SingleObjectiveProgrammableProblem(SingleObjectiveProgrammableProblem<TEncoding, TEncodedSolution> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveProgrammableProblem<TEncoding, TEncodedSolution>(this, cloner);
    }

    [StorableConstructor]
    protected SingleObjectiveProgrammableProblem(StorableConstructorFlag _) : base(_) { }
    public SingleObjectiveProgrammableProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>>("ProblemScript", "Defines the problem.", new SingleObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>() { Name = Name }));
      ProblemScript.Encoding = (TEncoding)Encoding.Clone();

      var codeTemplate = ScriptTemplates.SingleObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, typeof(TEncoding).Namespace);
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, typeof(TEncoding).Name);
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, typeof(TEncodedSolution).Name);
      ProblemScript.Code = codeTemplate;

      Operators.Add(new BestScopeSolutionAnalyzer());
      RegisterEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }

    private void RegisterEvents() {
      ProblemScript.ProblemDefinitionChanged += (o, e) => OnProblemDefinitionChanged();
      ProblemScript.NameChanged += (o, e) => OnProblemScriptNameChanged();
    }

    private void OnProblemDefinitionChanged() {
      Parameters.Remove("Maximization");
      Parameters.Add(new FixedValueParameter<BoolValue>("Maximization", "Set to false if the problem should be minimized.", (BoolValue)new BoolValue(Maximization).AsReadOnly()) { Hidden = true });
      Encoding = (TEncoding)ProblemScript.Encoding.Clone();

      OnOperatorsChanged();
      OnReset();
    }
    protected override void OnNameChanged() {
      base.OnNameChanged();
      ProblemScript.Name = Name;
    }
    private void OnProblemScriptNameChanged() {
      Name = ProblemScript.Name;
    }

    public override bool Maximization {
      get { return Parameters.ContainsKey("ProblemScript") && ProblemDefinition.Maximization; }
    }

    public override double Evaluate(TEncodedSolution individual, IRandom random) {
      return ProblemDefinition.Evaluate(individual, random);
    }

    public override void Analyze(TEncodedSolution[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      ProblemDefinition.Analyze(individuals, qualities, results, random);
    }
    public override IEnumerable<TEncodedSolution> GetNeighbors(TEncodedSolution individual, IRandom random) {
      return ProblemDefinition.GetNeighbors(individual, random);
    }
  }
}
