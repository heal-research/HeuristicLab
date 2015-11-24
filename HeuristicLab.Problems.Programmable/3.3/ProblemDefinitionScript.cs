﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [StorableClass]
  public abstract class ProblemDefinitionScript : Script {
    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }

    [StorableConstructor]
    protected ProblemDefinitionScript(bool deserializing) : base(deserializing) { }
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
  [StorableClass]
  public abstract class ProblemDefinitionScript<TEncoding, TSolution> : ProblemDefinitionScript, IProblemDefinition<TEncoding, TSolution>
    where TEncoding : class, IEncoding<TSolution>
    where TSolution : class, ISolution {

    [Storable]
    private bool codeChanged;

    [Storable]
    private TEncoding encoding;
    internal TEncoding Encoding {
      get { return encoding; }
      set { encoding = value; }
    }

    TEncoding IProblemDefinition<TEncoding, TSolution>.Encoding {
      get { return Encoding; }
    }

    internal void Initialize() {
      CompiledProblemDefinition.Initialize();
    }

    [StorableConstructor]
    protected ProblemDefinitionScript(bool deserializing) : base(deserializing) { }
    protected ProblemDefinitionScript(ProblemDefinitionScript<TEncoding, TSolution> original, Cloner cloner)
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
    private volatile CompiledProblemDefinition<TEncoding, TSolution> compiledProblemDefinition;
    protected CompiledProblemDefinition<TEncoding, TSolution> CompiledProblemDefinition {
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
      if (!types.Any(x => typeof(CompiledProblemDefinition<TEncoding, TSolution>).IsAssignableFrom(x)))
        throw new ProblemDefinitionScriptException("The compiled code doesn't contain a problem definition." + Environment.NewLine + "The problem definition must be a subclass of CompiledProblemDefinition.");
      if (types.Count(x => typeof(CompiledProblemDefinition<TEncoding, TSolution>).IsAssignableFrom(x)) > 1)
        throw new ProblemDefinitionScriptException("The compiled code contains multiple problem definitions." + Environment.NewLine + "Only one subclass of CompiledProblemDefinition is allowed.");

      CompiledProblemDefinition<TEncoding, TSolution> inst;
      try {
        inst = (CompiledProblemDefinition<TEncoding, TSolution>)Activator.CreateInstance(types.Single(x => typeof(CompiledProblemDefinition<TEncoding, TSolution>).IsAssignableFrom(x)));
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
