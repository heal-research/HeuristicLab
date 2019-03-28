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
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.Programmable {
  [Item("Programmable Problem (multi-objective)", "Represents a multi-objective problem that can be programmed with a script.")]
  [Creatable(CreatableAttribute.Categories.Problems, Priority = 120)]
  [StorableType("1AA24077-4E1E-4FAE-8EC8-B6008DFD30B9")]
  public class MultiObjectiveProgrammableProblem<TEncoding, TSolution> : MultiObjectiveProblem<TEncoding, TSolution>, IProgrammableItem, IProgrammableProblem
    where TEncoding : class, IEncoding<TSolution>
    where TSolution : class, ISolution {
    protected static readonly string ENCODING_NAMESPACE = "ENCODING_NAMESPACE";
    protected static readonly string ENCODING_CLASS = "ENCODING_CLASS";
    protected static readonly string SOLUTION_CLASS = "SOLUTION_CLASS";

    public static new Image StaticItemImage {
      get { return VSImageLibrary.Script; }
    }

    private FixedValueParameter<MultiObjectiveProblemDefinitionScript<TEncoding, TSolution>> MultiObjectiveProblemScriptParameter {
      get { return (FixedValueParameter<MultiObjectiveProblemDefinitionScript<TEncoding, TSolution>>)Parameters["ProblemScript"]; }
    }

    Script IProgrammableProblem.ProblemScript {
      get { return ProblemScript; }
    }
    public MultiObjectiveProblemDefinitionScript<TEncoding, TSolution> ProblemScript {
      get { return MultiObjectiveProblemScriptParameter.Value; }
    }

    public IMultiObjectiveProblemDefinition<TEncoding, TSolution> ProblemDefinition {
      get { return MultiObjectiveProblemScriptParameter.Value; }
    }

    [StorableConstructor]
    private MultiObjectiveProgrammableProblem(StorableConstructorFlag _) : base(_) { }
    protected MultiObjectiveProgrammableProblem(MultiObjectiveProgrammableProblem<TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new MultiObjectiveProgrammableProblem<TEncoding, TSolution>(this, cloner);
    }

    public MultiObjectiveProgrammableProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<MultiObjectiveProblemDefinitionScript<TEncoding, TSolution>>("ProblemScript", "Defines the problem.",
        new MultiObjectiveProblemDefinitionScript<TEncoding, TSolution>() { Name = Name }));
      ProblemScript.Encoding = (TEncoding)Encoding.Clone();

      var codeTemplate = ScriptTemplates.MultiObjectiveProblem_Template;
      codeTemplate = codeTemplate.Replace(ENCODING_NAMESPACE, typeof(TEncoding).Namespace);
      codeTemplate = codeTemplate.Replace(ENCODING_CLASS, typeof(TEncoding).Name);
      codeTemplate = codeTemplate.Replace(SOLUTION_CLASS, typeof(TSolution).Name);
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

    public override double[] Evaluate(TSolution individual, IRandom random) {
      return ProblemDefinition.Evaluate(individual, random);
    }

    public override void Analyze(TSolution[] individuals, double[][] qualities, ResultCollection results, IRandom random) {
      ProblemDefinition.Analyze(individuals, qualities, results, random);
    }
  }
}
