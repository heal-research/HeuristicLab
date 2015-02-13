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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.ExternalEvaluation.Programmable;
using HeuristicLab.Scripting;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [Item("ProblemDefinitionScript", "Script that defines the parameter vector and evaluates the solution for a programmable problem.")]
  [StorableClass]
  public sealed class SingleObjectiveOptimizationSupportScript : Script, ISingleObjectiveOptimizationSupport {

    [Storable]
    private VariableStore variableStore;
    public VariableStore VariableStore {
      get { return variableStore; }
    }

    protected override string CodeTemplate {
      get { return Templates.CompiledSingleObjectiveOptimizationSupport; }
    }

    [StorableConstructor]
    private SingleObjectiveOptimizationSupportScript(bool deserializing) : base(deserializing) { }
    private SingleObjectiveOptimizationSupportScript(SingleObjectiveOptimizationSupportScript original, Cloner cloner)
      : base(original, cloner) {
      variableStore = cloner.Clone(original.variableStore);
    }
    public SingleObjectiveOptimizationSupportScript()
      : base() {
      variableStore = new VariableStore();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveOptimizationSupportScript(this, cloner);
    }

    private readonly object compileLock = new object();
    private volatile ISingleObjectiveOptimizationSupport compiledInstance;
    private ISingleObjectiveOptimizationSupport CompiledInstance {
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
      set { compiledInstance = value; }
    }

    public override Assembly Compile() {
      var assembly = base.Compile();
      var types = assembly.GetTypes();
      if (!types.Any(x => typeof(CompiledOptimizationSupport).IsAssignableFrom(x)))
        throw new SingleObjectiveOptimizationSupportException("The compiled code doesn't contain an optimization support." + Environment.NewLine + "The support class must be a subclass of CompiledOptimizationSupport.");
      if (types.Count(x => typeof(CompiledOptimizationSupport).IsAssignableFrom(x)) > 1)
        throw new SingleObjectiveOptimizationSupportException("The compiled code contains multiple support classes." + Environment.NewLine + "Only one subclass of CompiledOptimizationSupport is allowed.");

      CompiledOptimizationSupport inst;
      try {
        inst = (CompiledOptimizationSupport)Activator.CreateInstance(types.Single(x => typeof(CompiledOptimizationSupport).IsAssignableFrom(x)));
        inst.vars = new Variables(VariableStore);
      } catch (Exception e) {
        compiledInstance = null;
        throw new SingleObjectiveOptimizationSupportException("Instantiating the optimization support class failed." + Environment.NewLine + "Check your default constructor.", e);
      }

      var soInst = inst as ISingleObjectiveOptimizationSupport;
      if (soInst == null)
        throw new SingleObjectiveOptimizationSupportException("The optimization support class does not implement ISingleObjectiveOptimizationSupport." + Environment.NewLine + "Please implement that interface in the subclass of CompiledOptimizationSupport.");

      CompiledInstance = soInst;

      return assembly;
    }

    protected override void OnCodeChanged() {
      base.OnCodeChanged();
      compiledInstance = null;
    }

    void ISingleObjectiveOptimizationSupport.Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random) {
      CompiledInstance.Analyze(individuals, qualities, results, random);
    }

    IEnumerable<Individual> ISingleObjectiveOptimizationSupport.GetNeighbors(Individual individual, IRandom random) {
      return CompiledInstance.GetNeighbors(individual, random);
    }
  }
}
