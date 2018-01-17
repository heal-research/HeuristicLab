#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Model2 {
  [Item("Basic Context", "A base class for algorithms' contexts.")]
  [StorableClass]
  public class BasicContext : ParameterizedNamedItem, IContext {

    private IExecutionContext parent;
    public IExecutionContext Parent {
      get { return parent; }
      set { parent = value; }
    }

    [Storable]
    private IScope scope;
    public IScope Scope {
      get { return scope; }
      private set { scope = value; }
    }

    IKeyedItemCollection<string, IParameter> IExecutionContext.Parameters {
      get { return Parameters; }
    }

    [Storable]
    private IFixedValueParameter<IntValue> iterations;
    public int Iterations {
      get { return iterations.Value.Value; }
      set { iterations.Value.Value = value; }
    }

    [Storable]
    private IFixedValueParameter<IntValue> evaluatedSolutions;
    public int EvaluatedSolutions {
      get { return evaluatedSolutions.Value.Value; }
      set { evaluatedSolutions.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<DoubleValue> bestQuality;
    public double BestQuality {
      get { return bestQuality.Value.Value; }
      set { bestQuality.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<BoolValue> terminate;
    public bool Terminate {
      get { return terminate.Value.Value; }
      set { terminate.Value.Value = value; }
    }

    [StorableConstructor]
    protected BasicContext(bool deserializing) : base(deserializing) { }
    protected BasicContext(BasicContext original, Cloner cloner)
    : base(original, cloner) {
      scope = cloner.Clone(original.scope);
      iterations = cloner.Clone(original.iterations);
      evaluatedSolutions = cloner.Clone(original.evaluatedSolutions);
      bestQuality = cloner.Clone(original.bestQuality);
      terminate = cloner.Clone(original.terminate);
    }
    protected BasicContext() : base() {
      scope = new Scope("Global");
      Parameters.Add(iterations = new FixedValueParameter<IntValue>("Iterations", new IntValue(0)));
      Parameters.Add(evaluatedSolutions = new FixedValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      Parameters.Add(bestQuality = new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(double.NaN)));
      Parameters.Add(terminate = new ValueParameter<BoolValue>("Terminate", new BoolValue(false)));
    }
    protected BasicContext(string name) : base(name) {
      scope = new Scope("Global");
      Parameters.Add(iterations = new FixedValueParameter<IntValue>("Iterations", new IntValue(0)));
      Parameters.Add(evaluatedSolutions = new FixedValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      Parameters.Add(bestQuality = new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(double.NaN)));
      Parameters.Add(terminate = new ValueParameter<BoolValue>("Terminate", new BoolValue(false)));
    }
    protected BasicContext(string name, ParameterCollection parameters) : base(name, parameters) {
      scope = new Scope("Global");
      Parameters.Add(iterations = new FixedValueParameter<IntValue>("Iterations", new IntValue(0)));
      Parameters.Add(evaluatedSolutions = new FixedValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      Parameters.Add(bestQuality = new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(double.NaN)));
      Parameters.Add(terminate = new ValueParameter<BoolValue>("Terminate", new BoolValue(false)));
    }
    protected BasicContext(string name, string description) : base(name, description) {
      scope = new Scope("Global");
      Parameters.Add(iterations = new FixedValueParameter<IntValue>("Iterations", new IntValue(0)));
      Parameters.Add(evaluatedSolutions = new FixedValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      Parameters.Add(bestQuality = new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(double.NaN)));
      Parameters.Add(terminate = new ValueParameter<BoolValue>("Terminate", new BoolValue(false)));
    }
    protected BasicContext(string name, string description, ParameterCollection parameters) : base(name, description, parameters) {
      scope = new Scope("Global");
      Parameters.Add(iterations = new FixedValueParameter<IntValue>("Iterations", new IntValue(0)));
      Parameters.Add(evaluatedSolutions = new FixedValueParameter<IntValue>("EvaluatedSolutions", new IntValue(0)));
      Parameters.Add(bestQuality = new ValueParameter<DoubleValue>("BestQuality", new DoubleValue(double.NaN)));
      Parameters.Add(terminate = new ValueParameter<BoolValue>("Terminate", new BoolValue(false)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BasicContext(this, cloner);
    }


    public void RunOperator(IOperator op, CancellationToken cancellationToken) {
      RunOperator(op, scope, cancellationToken);
    }

    public void RunOperator(IOperator op, IScope s, CancellationToken cancellationToken) {
      if (op == null) return;
      var stack = new Stack<IOperation>();
      stack.Push(((IExecutionContext)this).CreateChildOperation(op, s));

      while (stack.Count > 0) {
        cancellationToken.ThrowIfCancellationRequested();

        var next = stack.Pop();
        if (next is OperationCollection) {
          var coll = (OperationCollection)next;
          for (int i = coll.Count - 1; i >= 0; i--)
            if (coll[i] != null) stack.Push(coll[i]);
        } else if (next is IAtomicOperation) {
          var operation = (IAtomicOperation)next;
          try {
            next = operation.Operator.Execute((IExecutionContext)operation, cancellationToken);
          } catch (Exception ex) {
            stack.Push(operation);
            if (ex is OperationCanceledException) throw ex;
            else throw new OperatorExecutionException(operation.Operator, ex);
          }
          if (next != null) stack.Push(next);
        }
      }
    }

    #region IExecutionContext members
    IAtomicOperation IExecutionContext.CreateOperation(IOperator op) {
      return new Core.ExecutionContext(this, op, Scope);
    }
    IAtomicOperation IExecutionContext.CreateOperation(IOperator op, IScope s) {
      return new Core.ExecutionContext(this, op, s);
    }
    IAtomicOperation IExecutionContext.CreateChildOperation(IOperator op) {
      return new Core.ExecutionContext(this, op, Scope);
    }
    IAtomicOperation IExecutionContext.CreateChildOperation(IOperator op, IScope s) {
      return new Core.ExecutionContext(this, op, s);
    }
    #endregion
  }
}
