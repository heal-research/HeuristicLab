#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Reflection;
using Google.OrTools.LinearSolver;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Problems.Programmable;
using HeuristicLab.Scripting;

namespace HeuristicLab.ExactOptimization.LinearProgramming {

  [Item("Programmable Linear Problem Definition (LP, MIP)",
    "Script that defines the model for a linear/mixed integer programming problem.")]
  [StorableType("830F82CF-9FF1-4619-A75E-E43E208565F0")]
  public sealed class ProgrammableLinearProblemDefinition : Script, ILinearProblemDefinition,
    IStorableContent {
    private readonly object compileLock = new object();

    [Storable]
    private readonly VariableStore variableStore;

    [Storable]
    private bool codeChanged;

    private volatile ILinearProblemDefinition compiledProblemDefinition;

    public ProgrammableLinearProblemDefinition()
      : base(Templates.ScriptTemplates.CompiledLinearProblemDefinition) {
      Name = "Programmable Linear Problem Definition";
      variableStore = new VariableStore();
    }

    [StorableConstructor]
    private ProgrammableLinearProblemDefinition(StorableConstructorFlag _) : base(_) { }

    private ProgrammableLinearProblemDefinition(ProgrammableLinearProblemDefinition original,
      Cloner cloner) : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
      codeChanged = original.codeChanged;
    }

    public event EventHandler ProblemDefinitionChanged;

    public string Filename { get; set; }
    public dynamic Instance => compiledProblemDefinition;
    public VariableStore VariableStore => variableStore;

    private ILinearProblemDefinition CompiledProblemDefinition {
      get {
        // double checked locking pattern
        if (compiledProblemDefinition == null) {
          lock (compileLock) {
            if (compiledProblemDefinition == null) {
              if (codeChanged)
                throw new ProblemDefinitionScriptException("The code has been changed, but was not recompiled.");
              Compile(false);
            }
          }
        }

        return compiledProblemDefinition;
      }
    }

    public void Analyze(Solver solver, ResultCollection results) => CompiledProblemDefinition.Analyze(solver, results);

    public void BuildModel(Solver solver) => CompiledProblemDefinition.BuildModel(solver);

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ProgrammableLinearProblemDefinition(this, cloner);
    }

    public override Assembly Compile() => Compile(true);

    protected override void OnCodeChanged() {
      base.OnCodeChanged();
      compiledProblemDefinition = null;
      codeChanged = true;
    }

    private Assembly Compile(bool fireChanged) {
      var assembly = base.Compile();
      var types = assembly.GetTypes();
      if (!types.Any(x => typeof(CompiledProblemDefinition).IsAssignableFrom(x)))
        throw new ProblemDefinitionScriptException("The compiled code doesn't contain a problem definition." + Environment.NewLine +
                                                   $"The problem definition must be a subclass of {nameof(CompiledProblemDefinition)} and implement {nameof(ILinearProblemDefinition)}.");
      if (types.Count(x => typeof(CompiledProblemDefinition).IsAssignableFrom(x)) > 1)
        throw new ProblemDefinitionScriptException("The compiled code contains multiple problem definitions." + Environment.NewLine +
                                                   $"Only one subclass of {nameof(CompiledProblemDefinition)} is allowed.");

      CompiledProblemDefinition inst;
      try {
        inst = (CompiledProblemDefinition)Activator.CreateInstance(types.Single(x =>
         typeof(CompiledProblemDefinition).IsAssignableFrom(x)));
      } catch (Exception e) {
        compiledProblemDefinition = null;
        throw new ProblemDefinitionScriptException(
          "Instantiating the problem definition failed." + Environment.NewLine + "Check your default constructor.", e);
      }

      try {
        inst.vars = new Variables(VariableStore);
        inst.Initialize();
      } catch (Exception e) {
        compiledProblemDefinition = null;
        throw new ProblemDefinitionScriptException(
          "Initializing the problem definition failed." + Environment.NewLine + "Check your Initialize() method.", e);
      }

      try {
        compiledProblemDefinition = (ILinearProblemDefinition)inst;
        if (fireChanged) OnProblemDefinitionChanged();
      } catch (Exception e) {
        compiledProblemDefinition = null;
        throw new ProblemDefinitionScriptException(
          "Using the problem definition in the problem failed." + Environment.NewLine +
          "Examine this error message carefully.", e);
      }

      codeChanged = false;
      return assembly;
    }

    private void OnProblemDefinitionChanged() => ProblemDefinitionChanged?.Invoke(this, EventArgs.Empty);
  }
}
