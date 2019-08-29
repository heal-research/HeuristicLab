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
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.Programmable {
  [StorableType("5573B778-C60C-44BF-98FB-A8E189818C00")]
  public abstract class ProblemDefinitionScript : Script {
    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }

    [StorableConstructor]
    protected ProblemDefinitionScript(StorableConstructorFlag _) : base(_) { }
    protected ProblemDefinitionScript(ProblemDefinitionScript original, Cloner cloner)
      : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
    }
    protected ProblemDefinitionScript()
      : base() {
      variableStore = new VariableStore();
    }
    protected ProblemDefinitionScript(string code)
      : base(code) {
      variableStore = new VariableStore();
    }
  }

  [Item("ProblemDefinitionScript", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableType("0B3AF22C-4744-4860-BBCF-A92046000847")]
  public abstract class ProblemDefinitionScript<TEncoding, TEncodedSolution> : ProblemDefinitionScript, IProblemDefinition<TEncoding, TEncodedSolution>
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {

    [Storable]
    private bool codeChanged;

    [Storable]
    private TEncoding encoding;
    internal TEncoding Encoding {
      get { return encoding; }
      set { encoding = value; }
    }

    TEncoding IProblemDefinition<TEncoding, TEncodedSolution>.Encoding {
      get { return Encoding; }
    }

    internal void Initialize() {
      CompiledProblemDefinition.Initialize();
    }

    [StorableConstructor]
    protected ProblemDefinitionScript(StorableConstructorFlag _) : base(_) { }
    protected ProblemDefinitionScript(ProblemDefinitionScript<TEncoding, TEncodedSolution> original, Cloner cloner)
      : base(original, cloner) {
      encoding = cloner.Clone(original.encoding);
      codeChanged = original.codeChanged;
    }
    protected ProblemDefinitionScript()
      : base() {
    }
    protected ProblemDefinitionScript(string code)
      : base(code) {
    }

    private readonly object compileLock = new object();
    private volatile CompiledProblemDefinition<TEncoding, TEncodedSolution> compiledProblemDefinition;
    protected CompiledProblemDefinition<TEncoding, TEncodedSolution> CompiledProblemDefinition {
      get {
        // double checked locking pattern
        if (compiledProblemDefinition == null) {
          lock (compileLock) {
            if (compiledProblemDefinition == null) {
              if (codeChanged) throw new ProblemDefinitionScriptException("The code has been changed, but was not recompiled.");
              Compile(false);
            }
          }
        }
        return compiledProblemDefinition;
      }
    }
    public dynamic Instance {
      get { return compiledProblemDefinition; }
    }

    public sealed override Assembly Compile() {
      return Compile(true);
    }

    private Assembly Compile(bool fireChanged) {
      var assembly = base.Compile();
      var types = assembly.GetTypes();
      if (!types.Any(x => typeof(CompiledProblemDefinition<TEncoding, TEncodedSolution>).IsAssignableFrom(x)))
        throw new ProblemDefinitionScriptException("The compiled code doesn't contain a problem definition." + Environment.NewLine + "The problem definition must be a subclass of CompiledProblemDefinition.");
      if (types.Count(x => typeof(CompiledProblemDefinition<TEncoding, TEncodedSolution>).IsAssignableFrom(x)) > 1)
        throw new ProblemDefinitionScriptException("The compiled code contains multiple problem definitions." + Environment.NewLine + "Only one subclass of CompiledProblemDefinition is allowed.");

      CompiledProblemDefinition<TEncoding, TEncodedSolution> inst;
      try {
        inst = (CompiledProblemDefinition<TEncoding, TEncodedSolution>)Activator.CreateInstance(types.Single(x => typeof(CompiledProblemDefinition<TEncoding, TEncodedSolution>).IsAssignableFrom(x)));
      } catch (Exception e) {
        compiledProblemDefinition = null;
        throw new ProblemDefinitionScriptException("Instantiating the problem definition failed." + Environment.NewLine + "Check your default constructor.", e);
      }

      try {
        inst.vars = new Variables(VariableStore);
        inst.Encoding = encoding;
        inst.Initialize();
      } catch (Exception e) {
        compiledProblemDefinition = null;
        throw new ProblemDefinitionScriptException("Initializing the problem definition failed." + Environment.NewLine + "Check your Initialize() method.", e);
      }

      try {
        compiledProblemDefinition = inst;
        if (fireChanged) OnProblemDefinitionChanged();
      } catch (Exception e) {
        compiledProblemDefinition = null;
        throw new ProblemDefinitionScriptException("Using the problem definition in the problem failed." + Environment.NewLine + "Examine this error message carefully (often there is an issue with the defined encoding).", e);
      }

      codeChanged = false;
      return assembly;
    }

    protected override void OnCodeChanged() {
      base.OnCodeChanged();
      compiledProblemDefinition = null;
      codeChanged = true;
    }

    public event EventHandler ProblemDefinitionChanged;
    protected virtual void OnProblemDefinitionChanged() {
      var handler = ProblemDefinitionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
