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

using System;
using System.Linq;
using System.Reflection;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.Programmable {
  [Item("ProblemDefinitionScript", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableClass]
  public abstract class ProblemDefinitionScript : Script, IProblemDefinition {
    protected bool SuppressEvents { get; set; }

    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }

    [Storable]
    private bool codeChanged;

    [StorableConstructor]
    protected ProblemDefinitionScript(bool deserializing) : base(deserializing) { }
    protected ProblemDefinitionScript(ProblemDefinitionScript original, Cloner cloner)
      : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
      codeChanged = original.codeChanged;
    }
    protected ProblemDefinitionScript()
      : base() {
      variableStore = new VariableStore();
    }

    IEncoding IProblemDefinition.Encoding {
      get { return CompiledProblemDefinition.Encoding; }
    }

    private readonly object compileLock = new object();
    private volatile IProblemDefinition compiledProblemDefinition;
    protected IProblemDefinition CompiledProblemDefinition {
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

    public sealed override Assembly Compile() {
      return Compile(true);
    }

    private Assembly Compile(bool fireChanged) {
      var assembly = base.Compile();
      var types = assembly.GetTypes();
      if (!types.Any(x => typeof(CompiledProblemDefinition).IsAssignableFrom(x)))
        throw new ProblemDefinitionScriptException("The compiled code doesn't contain a problem definition." + Environment.NewLine + "The problem definition must be a subclass of CompiledProblemDefinition.");
      if (types.Count(x => typeof(CompiledProblemDefinition).IsAssignableFrom(x)) > 1)
        throw new ProblemDefinitionScriptException("The compiled code contains multiple problem definitions." + Environment.NewLine + "Only one subclass of CompiledProblemDefinition is allowed.");

      CompiledProblemDefinition inst;
      try {
        inst = (CompiledProblemDefinition)Activator.CreateInstance(types.Single(x => typeof(CompiledProblemDefinition).IsAssignableFrom(x)));
      } catch (Exception e) {
        compiledProblemDefinition = null;
        throw new ProblemDefinitionScriptException("Instantiating the problem definition failed." + Environment.NewLine + "Check your default constructor.", e);
      }

      try {
        inst.vars = new Variables(VariableStore);
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
