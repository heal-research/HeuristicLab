#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;
using ExecutionContext = HeuristicLab.Core.ExecutionContext;

namespace HeuristicLab.Optimization.Algorithms.SingleObjective {
  [Item("AlgorithmContext", "")]
  [StorableClass]
  public abstract class HeuristicAlgorithmContext<TProblem, TEncoding, TSolution> : ParameterizedNamedItem,
      IProblemContext<TProblem, TEncoding, TSolution>, IStochasticContext,
      IEvaluatedSolutionsContext, IBestQualityContext, IBestSolutionContext<TSolution>,
      ILongRunningOperationContext
      where TProblem : class, ISingleObjectiveProblem<TEncoding, TSolution>, ISingleObjectiveProblemDefinition<TEncoding, TSolution>
      where TEncoding : class, IEncoding<TSolution>
      where TSolution : class, ISolution {

    private IExecutionContext parent;
    public IExecutionContext Parent {
      get { return parent; }
      set { parent = value; }
    }

    IKeyedItemCollection<string, IParameter> IExecutionContext.Parameters {
      get { return Parameters; }
    }

    public CancellationToken CancellationToken { get; set; }

    [Storable]
    private IValueParameter<TProblem> problem;
    public TProblem Problem {
      get { return problem.Value; }
      set { problem.Value = value; }
    }

    [Storable]
    private IScope scope;
    public IScope Scope {
      get { return scope; }
    }

    [Storable]
    private IValueParameter<BoolValue> initialized;
    public bool Initialized {
      get { return initialized.Value.Value; }
      set { initialized.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<IRandom> random;
    public IRandom Random {
      get { return random.Value; }
      set { random.Value = value; }
    }

    [Storable]
    private IValueParameter<IntValue> evaluatedSolutions;
    public int EvaluatedSolutions {
      get { return evaluatedSolutions.Value.Value; }
      set { evaluatedSolutions.Value.Value = value; }
    }

    public void IncEvaluatedSolutions(int inc) {
      EvaluatedSolutions += inc;
    }

    [Storable]
    private IValueParameter<DoubleValue> bestQuality;
    public double BestQuality {
      get { return bestQuality.Value.Value; }
      set { bestQuality.Value.Value = value; }
    }

    [Storable]
    private IValueParameter<TSolution> bestSolution;
    public TSolution BestSolution {
      get { return bestSolution.Value; }
      set { bestSolution.Value = value; }
    }

    [StorableConstructor]
    protected HeuristicAlgorithmContext(bool deserializing) : base(deserializing) { }
    protected HeuristicAlgorithmContext(HeuristicAlgorithmContext<TProblem, TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) {
      problem = cloner.Clone(original.problem);
      scope = cloner.Clone(original.scope);
      initialized = cloner.Clone(original.initialized);
      random = cloner.Clone(original.random);
      evaluatedSolutions = cloner.Clone(original.evaluatedSolutions);
      bestQuality = cloner.Clone(original.bestQuality);
      bestSolution = cloner.Clone(original.bestSolution);
    }
    protected HeuristicAlgorithmContext() : this(new Scope("Scope"), CancellationToken.None) { }
    protected HeuristicAlgorithmContext(IScope s, CancellationToken token) {
      scope = s;
      CancellationToken = token;
      Parameters.Add(problem = new ValueParameter<TProblem>("Problem", "The problem that is to be solved."));
      Parameters.Add(initialized = new ValueParameter<BoolValue>("Initialized", "Whether the algorithm has finished the initialization phase.", new BoolValue(false)));
      Parameters.Add(random = new ValueParameter<IRandom>("Random", "The random number generator.", new MersenneTwister()));
      Parameters.Add(evaluatedSolutions = new ValueParameter<IntValue>("EvaluatedSolutions", "The number of evaluated solutions.", new IntValue(0)));
      Parameters.Add(bestQuality = new ValueParameter<DoubleValue>("BestQuality", "The best quality found so far.", new DoubleValue(double.NaN)));
      Parameters.Add(bestSolution = new OptionalValueParameter<TSolution>("BestSolution", "The best solution found so far."));

    }

    public IAtomicOperation CreateOperation(IOperator op) {
      return new ExecutionContext(parent, op, scope);
    }

    public IAtomicOperation CreateOperation(IOperator op, IScope s) {
      return new ExecutionContext(parent, op, s);
    }

    public IAtomicOperation CreateChildOperation(IOperator op) {
      return new ExecutionContext(this, op, scope);
    }

    public IAtomicOperation CreateChildOperation(IOperator op, IScope s) {
      return new ExecutionContext(this, op, s);
    }
  }
}
