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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization.Algorithms.SingleObjective {
  public abstract class HeuristicAlgorithm<TContext, TProblem, TEncoding, TSolution> : Algorithm
      where TContext : HeuristicAlgorithmContext<TProblem, TEncoding, TSolution>,
                       IStochasticContext, IEvaluatedSolutionsContext, IBestQualityContext,
                       IBestSolutionContext<TSolution>, new()
      where TProblem : class, ISingleObjectiveProblem<TEncoding, TSolution>, ISingleObjectiveProblemDefinition<TEncoding, TSolution>
      where TEncoding : class, IEncoding<TSolution>
      where TSolution : class, ISolution {
    private Type problemType;

    public override Type ProblemType {
      get { return typeof(SingleObjectiveProblem<TEncoding, TSolution>); }
    }

    public new TProblem Problem {
      get { return (TProblem)base.Problem; }
      set { base.Problem = value; }
    }

    private CancellationTokenSource cancellationTokenSource;
    protected CancellationTokenSource CancellationTokenSource {
      get { return cancellationTokenSource; }
      private set { cancellationTokenSource = value; }
    }


    public int? MaximumEvaluations {
      get {
        var val = ((OptionalValueParameter<IntValue>)Parameters["MaximumEvaluations"]).Value;
        return val != null ? val.Value : (int?)null;
      }
      set {
        var param = (OptionalValueParameter<IntValue>)Parameters["MaximumEvaluations"];
        param.Value = value.HasValue ? new IntValue(value.Value) : null;
      }
    }

    public TimeSpan? MaximumExecutionTime {
      get {
        var val = ((OptionalValueParameter<TimeSpanValue>)Parameters["MaximumExecutionTime"]).Value;
        return val != null ? val.Value : (TimeSpan?)null;
      }
      set {
        var param = (OptionalValueParameter<TimeSpanValue>)Parameters["MaximumExecutionTime"];
        param.Value = value.HasValue ? new TimeSpanValue(value.Value) : null;
      }
    }

    public double? TargetQuality {
      get {
        var val = ((OptionalValueParameter<DoubleValue>)Parameters["TargetQuality"]).Value;
        return val != null ? val.Value : (double?)null;
      }
      set {
        var param = (OptionalValueParameter<DoubleValue>)Parameters["TargetQuality"];
        param.Value = value.HasValue ? new DoubleValue(value.Value) : null;
      }
    }

    public bool SetSeedRandomly {
      get { return ((FixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]).Value.Value; }
      set { ((FixedValueParameter<BoolValue>)Parameters["SetSeedRandomly"]).Value.Value = value; }
    }

    public int Seed {
      get { return ((FixedValueParameter<IntValue>)Parameters["Seed"]).Value.Value; }
      set { ((FixedValueParameter<IntValue>)Parameters["Seed"]).Value.Value = value; }
    }

    public IMultiAnalyzer AlgorithmAnalyzer {
      get { return ((ValueParameter<IMultiAnalyzer>)Parameters["Analyzer.Algorithm"]).Value; }
      set { ((ValueParameter<IMultiAnalyzer>)Parameters["Analyzer.Algorithm"]).Value = value; }
    }

    public IMultiAnalyzer ProblemAnalyzer {
      get { return ((ValueParameter<IMultiAnalyzer>)Parameters["Analyzer.Problem"]).Value; }
      set { ((ValueParameter<IMultiAnalyzer>)Parameters["Analyzer.Problem"]).Value = value; }
    }

    [Storable]
    private ResultCollection results;
    public override ResultCollection Results {
      get { return results; }
    }

    [Storable]
    private TContext context;
    public TContext Context {
      get { return context; }
      protected set {
        if (context == value) return;
        context = value;
      }
    }

    [StorableConstructor]
    protected HeuristicAlgorithm(bool deserializing) : base(deserializing) { }
    protected HeuristicAlgorithm(HeuristicAlgorithm<TContext, TProblem, TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) { }
    protected HeuristicAlgorithm() {
      results = new ResultCollection();

      Parameters.Add(new ValueParameter<IMultiAnalyzer>("Analyzer.Algorithm", "The algorithm's analyzers to apply to the solution(s) (independent of the problem)."));
      Parameters.Add(new ValueParameter<IMultiAnalyzer>("Analyzer.Problem", "The problem's analyzer to apply to the solution(s)."));
      Parameters.Add(new OptionalValueParameter<IntValue>("MaximumEvaluations", "The maximum number of solution evaluations."));
      Parameters.Add(new OptionalValueParameter<TimeSpanValue>("MaximumExecutionTime", "The maximum runtime.", new TimeSpanValue(TimeSpan.FromMinutes(1))));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("TargetQuality", "The target quality at which the algorithm terminates."));
      Parameters.Add(new FixedValueParameter<BoolValue>("SetSeedRandomly", "Whether each run of the algorithm should be conducted with a new random seed.", new BoolValue(true)));
      Parameters.Add(new FixedValueParameter<IntValue>("Seed", "The random number seed that is used in case SetSeedRandomly is false.", new IntValue(0)));

    }

    protected override void OnProblemChanged() {
      base.OnProblemChanged();
      if (ProblemAnalyzer != null) {
        ProblemAnalyzer.Operators.Clear();
        if (Problem != null) {
          foreach (var analyzer in Problem.Operators.OfType<IAnalyzer>()) {
            foreach (var param in analyzer.Parameters.OfType<IScopeTreeLookupParameter>())
              param.Depth = 1;
            ProblemAnalyzer.Operators.Add(analyzer, analyzer.EnabledByDefault);
          }
        }
      }
    }

    public override void Prepare() {
      if (Problem == null) return;
      base.Prepare();
      results.Clear();
      Context = null;
      stopRequested = false;
      OnPrepared();
    }

    private bool stopRequested = false;
    public override void Start() {
      base.Start();
      CancellationTokenSource = new CancellationTokenSource();

      OnStarted();
      var task = Task.Run((Action)Run, cancellationTokenSource.Token);
      var continuation = new Task[3];
      continuation[0] = task.ContinueWith(t => {
        if (t.Exception != null) {
          OnExceptionOccurred(t.Exception.InnerExceptions.Count == 1 ? t.Exception.InnerExceptions[0] : t.Exception);
        }
        OnPaused();
      }, TaskContinuationOptions.OnlyOnFaulted);
      continuation[1] = task.ContinueWith(t => {
        OnStopped();
      }, TaskContinuationOptions.OnlyOnRanToCompletion);
      continuation[2] = task.ContinueWith(t => {
        if (stopRequested) OnStopped();
        else OnPaused();
      }, TaskContinuationOptions.OnlyOnCanceled);
      Task.WhenAny(continuation).ContinueWith(_ => {
        CancellationTokenSource.Dispose();
        CancellationTokenSource = null;
      });
    }

    public override void Pause() {
      base.Pause();
      CancellationTokenSource.Cancel();
    }

    public override void Stop() {
      // CancellationToken.ThrowIfCancellationRequested() must be called from within the Run method, otherwise stop does nothing
      // alternatively check the IsCancellationRequested property of the cancellation token
      base.Stop();
      stopRequested = true;
      if (CancellationTokenSource != null) {
        try {
          CancellationTokenSource.Cancel();
        } catch {
          OnStopped();
        }
      } else OnStopped();
    }

    private DateTime lastUpdateTime;
    private void Run() {
      var token = CancellationTokenSource.Token;
      lastUpdateTime = DateTime.UtcNow;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += timer_Elapsed;
      timer.Start();
      try {
        if (context == null) {
          context = CreateContext();

          IExecutionContext ctxt = null;
          foreach (var item in Problem.ExecutionContextItems)
            ctxt = new Core.ExecutionContext(ctxt, item, Context.Scope);
          ctxt = new Core.ExecutionContext(ctxt, this, Context.Scope);

          context.Parent = ctxt;

          if (SetSeedRandomly) Seed = new System.Random().Next();
          Context.Random.Reset(Seed);
          Context.Scope.Variables.Add(new Variable("Results", Results));
        } else context.CancellationToken = token;

        if (!Context.Initialized) {
          try {
            PerformInitialize(token);
          } catch {
            context = null;
            return;
          }
          Context.Initialized = true;
        }

        while (!CheckTerminate(token)) {
          PerformIterate(token);
          PerformAnalyze(token);
          token.ThrowIfCancellationRequested();
        }
      } finally {
        timer.Elapsed -= timer_Elapsed;
        timer.Stop();
        ExecutionTime += DateTime.UtcNow - lastUpdateTime;
      }
    }

    protected virtual TContext CreateContext() {
      return new TContext() { Problem = Problem, CancellationToken = CancellationTokenSource.Token };
    }

    protected virtual ISingleObjectiveSolutionScope<TSolution> CreateEmptySolutionScope() {
      return new SingleObjectiveSolutionScope<TSolution>(null, Problem.Encoding.Name, double.NaN, Problem.Evaluator.QualityParameter.ActualName);
    }

    protected static bool IsBetter(bool maximization, double a, double b) {
      return !double.IsNaN(a) && double.IsNaN(b) || maximization && a > b || !maximization && a < b;
    }

    protected virtual void Evaluate(ISingleObjectiveSolutionScope<TSolution> scope, CancellationToken token) {
      scope.Fitness = Problem.Evaluate(scope.Solution, Context.Random);
    }

    protected abstract void PerformInitialize(CancellationToken token);
    protected abstract void PerformIterate(CancellationToken token);
    protected virtual void PerformAnalyze(CancellationToken token) {
      IResult res;
      if (!Results.TryGetValue("EvaluatedSolutions", out res)) {
        res = new Result("EvaluatedSolutions", new IntValue(Context.EvaluatedSolutions));
        Results.Add(res);
      } else ((IntValue)res.Value).Value = Context.EvaluatedSolutions;
      if (!Results.TryGetValue("BestQuality", out res)) {
        res = new Result("BestQuality", new DoubleValue(Context.BestQuality));
        Results.Add(res);
      } else ((DoubleValue)res.Value).Value = Context.BestQuality;

      RunOperator(ProblemAnalyzer, Context.Scope, token);
      RunOperator(AlgorithmAnalyzer, Context.Scope, token);
    }

    protected virtual bool CheckTerminate(CancellationToken token) {
      return MaximumEvaluations.HasValue && Context.EvaluatedSolutions >= MaximumEvaluations.Value
        || MaximumExecutionTime.HasValue && ExecutionTime >= MaximumExecutionTime.Value
        || TargetQuality.HasValue && (Problem.Maximization && Context.BestQuality >= TargetQuality.Value
                                  || !Problem.Maximization && Context.BestQuality <= TargetQuality.Value);
    }

    #region Engine Helper
    protected void RunOperator(IOperator op, IScope scope, CancellationToken cancellationToken) {
      var stack = new Stack<IOperation>();
      stack.Push(Context.CreateChildOperation(op, scope));

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
    #endregion

    #region Events
    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      System.Timers.Timer timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.UtcNow;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }
    #endregion
  }
}
