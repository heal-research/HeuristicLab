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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Model2 {
  [Item("Context-based Algorithm", "Algorithms that make use of contexts to facilitate applying operators.")]
  [StorableClass]
  public abstract class ContextAlgorithm<TContext> : BasicAlgorithm
    where TContext : class, IContext, new() {
    
    [Storable]
    private TContext context;
    public TContext Context {
      get { return context; }
    }

    [Storable]
    private ValueParameter<IAnalyzer> analyzerParameter;
    public IValueParameter<IAnalyzer> AnalyzerParameter {
      get { return analyzerParameter; }
    }
    [Storable]
    private FixedValueParameter<MultiTerminator> terminationParameter;
    public IFixedValueParameter<MultiTerminator> TerminationParameter {
      get { return terminationParameter; }
    }

    public IAnalyzer Analyzer {
      get { return analyzerParameter.Value; }
      set { analyzerParameter.Value = value; }
    }
    public MultiTerminator Termination {
      get { return terminationParameter.Value; }
    }

    [StorableConstructor]
    protected ContextAlgorithm(bool deserializing) : base(deserializing) { }
    protected ContextAlgorithm(ContextAlgorithm<TContext> original, Cloner cloner)
      : base(original, cloner) {
      context = cloner.Clone(original.context);
      analyzerParameter = cloner.Clone(original.analyzerParameter);
      terminationParameter = cloner.Clone(original.terminationParameter);
    }
    protected ContextAlgorithm()
      : base() {
      Parameters.Add(analyzerParameter = new ValueParameter<IAnalyzer>("Analyzer", "The analyzers that are used to perform an analysis of the solutions."));
      Parameters.Add(terminationParameter = new FixedValueParameter<MultiTerminator>("Termination", "The termination criteria that are being used.", new MultiTerminator()));

      var generationsTerminator = new ComparisonTerminator<IntValue>("Iterations", ComparisonType.Less, new IntValue(1000)) { Name = "Iterations" };
      var evaluationsTerminator = new ComparisonTerminator<IntValue>("EvaluatedSolutions", ComparisonType.Less, new IntValue(int.MaxValue)) { Name = "Evaluations" };
      var executionTimeTerminator = new ExecutionTimeTerminator(this, new TimeSpanValue(TimeSpan.FromMinutes(5)));

      Termination.AddOperator(generationsTerminator);
      Termination.AddOperator(evaluationsTerminator);
      Termination.AddOperator(executionTimeTerminator);

    }

    protected override void Initialize(CancellationToken cancellationToken) {
      base.Initialize(cancellationToken);
      context = new TContext();
      context.Scope.Variables.Add(new Variable("Results", Results));

      IExecutionContext ctxt = null;
      foreach (var item in Problem.ExecutionContextItems)
        ctxt = new Core.ExecutionContext(ctxt, item, Context.Scope);
      ctxt = new Core.ExecutionContext(ctxt, this, Context.Scope);
      context.Parent = ctxt;

      context.Iterations = 0;
      context.EvaluatedSolutions = 0;
      context.BestQuality = double.NaN;
    }

    public override void Prepare() {
      context = null;
      base.Prepare();
    }

    protected virtual bool StoppingCriterion() {
      Context.RunOperator(Termination, CancellationToken.None);
      return Context.Terminate;
    }
  }
}
