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

using System.Drawing;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.Programmable {
  [Item("Programmable Problem (multi-objective)", "Represents a multi-objective problem that can be programmed with a script.")]
  [StorableType("1AA24077-4E1E-4FAE-8EC8-B6008DFD30B9")]
  public abstract class MultiObjectiveProgrammableProblem<TEncoding, TEncodedSolution> : MultiObjectiveProblem<TEncoding, TEncodedSolution>, IProgrammableItem, IProgrammableProblem
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    protected static readonly string ENCODING_NAMESPACE = "ENCODING_NAMESPACE";
    protected static readonly string ENCODING_CLASS = "ENCODING_CLASS";
    protected static readonly string SOLUTION_CLASS = "SOLUTION_CLASS";

    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    private FixedValueParameter<MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>> MultiObjectiveProblemScriptParameter {
      get { return (FixedValueParameter<MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>>)Parameters["ProblemScript"]; }
    }

    Script IProgrammableProblem.ProblemScript {
      get { return ProblemScript; }
    }
    public MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution> ProblemScript {
      get { return MultiObjectiveProblemScriptParameter.Value; }
    }

    public IMultiObjectiveProblemDefinition<TEncoding, TEncodedSolution> ProblemDefinition {
      get { return MultiObjectiveProblemScriptParameter.Value; }
    }

    [StorableConstructor]
    protected MultiObjectiveProgrammableProblem(StorableConstructorFlag _) : base(_) { }
    protected MultiObjectiveProgrammableProblem(MultiObjectiveProgrammableProblem<TEncoding, TEncodedSolution> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }

    public MultiObjectiveProgrammableProblem(TEncoding encoding)
      : base(encoding) {
      Parameters.Add(new FixedValueParameter<MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>>("ProblemScript", "Defines the problem.",
        new MultiObjectiveProblemDefinitionScript<TEncoding, TEncodedSolution>() { Name = Name }));
      ProblemScript.Encoding = (TEncoding)encoding.Clone();

      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, typeof(TEncoding).Namespace);
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, typeof(TEncoding).Name);
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, typeof(TEncodedSolution).Name);
      ProblemScript.Code = codeTemplate;

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
      Parameters.Add(new ValueParameter<BoolArray>("Maximization", "Set to false if the problem should be minimized.", (BoolArray)new BoolArray(Maximization).AsReadOnly()) { Hidden = true });
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

    public override bool[] Maximization {
      get { return Parameters.ContainsKey("ProblemScript") ? ProblemDefinition.Maximization : new[] { false }; }
    }

    public override double[] Evaluate(TEncodedSolution individual, IRandom random) {
      return ProblemDefinition.Evaluate(individual, random);
    }

    public override void Analyze(TEncodedSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      ProblemDefinition.Analyze(individuals, qualities, results, random);
    }
  }
}
