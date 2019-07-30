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
using HeuristicLab.Common;
using HEAL.Attic;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [StorableType("F1BC4885-753B-4E47-9169-EFC2E744782C")]
  public abstract class OptimizationSupportScript<T> : Script
    where T : class {

    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }

    [StorableConstructor]
    protected OptimizationSupportScript(StorableConstructorFlag _) : base(_) { }
    protected OptimizationSupportScript(OptimizationSupportScript<T> original, Cloner cloner)
      : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
    }

    protected OptimizationSupportScript()
      : base() {
      variableStore = new VariableStore();
    }

    protected OptimizationSupportScript(string code)
      : base(code) {
      variableStore = new VariableStore();
    }

    private readonly object compileLock = new object();
    private T compiledInstance;
    protected T CompiledInstance {
      get {
        if (compiledInstance == null) {
          lock (compileLock) {
            if (compiledInstance == null) {
              Compile();
            }
          }
        }
        return compiledInstance;
      }
      private set { compiledInstance = value; }
    }
    public dynamic Instance {
      get { return compiledInstance; }
    }

    public override Assembly Compile() {
      var assembly = base.Compile();
      var types = assembly.GetTypes();
      if (!types.Any(x => typeof(CompiledOptimizationSupport).IsAssignableFrom(x)))
        throw new OptimizationSupportException("The compiled code doesn't contain an optimization support." + Environment.NewLine + "The support class must be a subclass of CompiledOptimizationSupport.");
      if (types.Count(x => typeof(CompiledOptimizationSupport).IsAssignableFrom(x)) > 1)
        throw new OptimizationSupportException("The compiled code contains multiple support classes." + Environment.NewLine + "Only one subclass of CompiledOptimizationSupport is allowed.");

      CompiledOptimizationSupport inst;
      try {
        inst = (CompiledOptimizationSupport)Activator.CreateInstance(types.Single(x => typeof(CompiledOptimizationSupport).IsAssignableFrom(x)));
        inst.vars = new Variables(VariableStore);
      } catch (Exception e) {
        compiledInstance = null;
        throw new OptimizationSupportException("Instantiating the optimization support class failed." + Environment.NewLine + "Check your default constructor.", e);
      }

      var concreteInst = inst as T;
      if (concreteInst == null)
        throw new OptimizationSupportException("The optimization support class does not implement ISingleObjectiveOptimizationSupport." + Environment.NewLine + "Please implement that interface in the subclass of CompiledOptimizationSupport.");

      CompiledInstance = concreteInst;

      return assembly;
    }

    protected override void OnCodeChanged() {
      base.OnCodeChanged();
      compiledInstance = null;
    }
  }
}