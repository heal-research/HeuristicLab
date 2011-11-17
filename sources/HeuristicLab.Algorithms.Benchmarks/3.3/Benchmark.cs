#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.Benchmarks {
  /// <summary>
  /// A base class for benchmarks.
  /// </summary>
  [Item("Benchmark", "A wrapper for benchmark algorithms.")]
  [Creatable("Algorithms")]
  [StorableClass]
  public class Benchmark : IAlgorithm {
    private Random random = new Random();

    [Storable]
    private DateTime lastUpdateTime;

    [Storable]
    private IBenchmark benchmarkAlgorithm;
    public IBenchmark BenchmarkAlgorithm {
      get { return benchmarkAlgorithm; }
      set {
        if (value == null) throw new ArgumentNullException();
        benchmarkAlgorithm = value;
      }
    }

    private CancellationTokenSource cancellationTokenSource;

    [Storable]
    private ExecutionState executionState;
    public ExecutionState ExecutionState {
      get { return executionState; }
      private set {
        if (executionState != value) {
          executionState = value;
          OnExecutionStateChanged();
          OnItemImageChanged();
        }
      }
    }

    [Storable]
    private TimeSpan executionTime;
    public TimeSpan ExecutionTime {
      get { return executionTime; }
      protected set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    [Storable]
    private bool storeAlgorithmInEachRun;
    public bool StoreAlgorithmInEachRun {
      get { return storeAlgorithmInEachRun; }
      set {
        if (storeAlgorithmInEachRun != value) {
          storeAlgorithmInEachRun = value;
          OnStoreAlgorithmInEachRunChanged();
        }
      }
    }

    [Storable]
    protected int runsCounter;

    [Storable]
    private RunCollection runs = new RunCollection();
    public RunCollection Runs {
      get { return runs; }
      protected set {
        if (value == null) throw new ArgumentNullException();
        if (runs != value) {
          if (runs != null) DeregisterRunsEvents();
          runs = value;
          if (runs != null) RegisterRunsEvents();
        }
      }
    }

    [Storable]
    private ResultCollection results;
    public ResultCollection Results {
      get { return results; }
    }

    [Storable]
    private IProblem problem;
    public IProblem Problem {
      get { return problem; }
      set {
        if (problem != value) {
          if ((value != null) && !ProblemType.IsInstanceOfType(value)) throw new ArgumentException("Invalid problem type.");
          if (problem != null) DeregisterProblemEvents();
          problem = value;
          if (problem != null) RegisterProblemEvents();
          OnProblemChanged();
          Prepare();
        }
      }
    }

    public Type ProblemType {
      get { return typeof(IProblem); }
    }

    [Storable]
    protected string name;

    public string Name {
      get { return name; }
      set {
        if (!CanChangeName) throw new NotSupportedException("Name cannot be changed.");
        if (!(name.Equals(value) || (value == null) && (name == string.Empty))) {
          CancelEventArgs<string> e = value == null ? new CancelEventArgs<string>(string.Empty) : new CancelEventArgs<string>(value);
          OnNameChanging(e);
          if (!e.Cancel) {
            name = value == null ? string.Empty : value;
            OnNameChanged();
          }
        }
      }
    }

    public bool CanChangeName {
      get { return false; }
    }

    [Storable]
    protected string description;
    public string Description {
      get { return description; }
      set {
        if (!CanChangeDescription) throw new NotSupportedException("Description cannot be changed.");
        if (!(description.Equals(value) || (value == null) && (description == string.Empty))) {
          description = value == null ? string.Empty : value;
          OnDescriptionChanged();
        }
      }
    }

    public bool CanChangeDescription {
      get { return false; }
    }

    public string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }

    public string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }

    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(this.GetType()); }
    }

    public Image ItemImage {
      get {
        if (ExecutionState == ExecutionState.Prepared) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePrepared;
        else if (ExecutionState == ExecutionState.Started) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStarted;
        else if (ExecutionState == ExecutionState.Paused) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePaused;
        else if (ExecutionState == ExecutionState.Stopped) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStopped;
        else return HeuristicLab.Common.Resources.VSImageLibrary.Event;
      }
    }

    [Storable]
    private ParameterCollection parameters = new ParameterCollection();

    public IKeyedItemCollection<string, IParameter> Parameters {
      get { return parameters; }
    }

    private ReadOnlyKeyedItemCollection<string, IParameter> readOnlyParameters;

    IKeyedItemCollection<string, IParameter> IParameterizedItem.Parameters {
      get {
        if (readOnlyParameters == null) readOnlyParameters = parameters.AsReadOnly();
        return readOnlyParameters;
      }
    }

    public IEnumerable<IOptimizer> NestedOptimizers {
      get { return Enumerable.Empty<IOptimizer>(); }
    }

    #region Parameter Properties

    public ConstrainedValueParameter<IBenchmark> BenchmarkAlgorithmParameter {
      get { return (ConstrainedValueParameter<IBenchmark>)Parameters["BenchmarkAlgorithm"]; }
    }

    private ValueParameter<IntValue> ChunkSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["ChunkSize"]; }
    }

    private ValueParameter<DoubleValue> TimeLimitParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["TimeLimit"]; }
    }

    #endregion

    #region Constructors

    [StorableConstructor]
    public Benchmark(bool deserializing) { }

    public Benchmark() {
      name = ItemName;
      description = ItemDescription;
      parameters = new ParameterCollection();
      readOnlyParameters = null;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      storeAlgorithmInEachRun = false;
      runsCounter = 0;
      Runs = new RunCollection();
      results = new ResultCollection();
      CreateParameters();
      DiscoverBenchmarks();
      Prepare();
    }

    public Benchmark(Benchmark original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      name = original.name;
      description = original.description;
      parameters = cloner.Clone(original.parameters);
      readOnlyParameters = null;
      if (ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      executionState = original.executionState;
      executionTime = original.executionTime;
      storeAlgorithmInEachRun = original.storeAlgorithmInEachRun;
      runsCounter = original.runsCounter;
      runs = cloner.Clone(original.runs);
      Initialize();

      results = cloner.Clone(original.results);
      DiscoverBenchmarks();
      Prepare();
    }

    #endregion

    private void CreateParameters() {
      Parameters.Add(new ValueParameter<IntValue>("ChunkSize", "The size (MB) of the chunk array that gets generated", new IntValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("TimeLimit", "The time limit (in minutes) for a benchmark run. Zero means a fixed number of iterations", new DoubleValue(0)));
    }

    private void DiscoverBenchmarks() {
      var benchmarks = from t in ApplicationManager.Manager.GetTypes(typeof(IBenchmark))
                       select t;
      ItemSet<IBenchmark> values = new ItemSet<IBenchmark>();
      foreach (var benchmark in benchmarks) {
        IBenchmark b = (IBenchmark)Activator.CreateInstance(benchmark);
        values.Add(b);
      }
      string paramName = "BenchmarkAlgorithm";
      if (!Parameters.ContainsKey(paramName)) {
        if (values.Count > 0) {
          Parameters.Add(new ConstrainedValueParameter<IBenchmark>(paramName, values, values.First(a => a is IBenchmark)));
        } else {
          Parameters.Add(new ConstrainedValueParameter<IBenchmark>(paramName, values));
        }
      }
    }

    private void Initialize() {
      if (problem != null) RegisterProblemEvents();
      if (runs != null) RegisterRunsEvents();
    }

    public virtual void Prepare() {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      results.Clear();
      OnPrepared();
    }

    public void Prepare(bool clearRuns) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      if (clearRuns) runs.Clear();
      Prepare();
    }

    public virtual void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
    }
    public virtual void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
      cancellationTokenSource.Cancel();
    }

    public virtual void Start() {
      cancellationTokenSource = new CancellationTokenSource();
      OnStarted();
      Task task = Task.Factory.StartNew(Run, cancellationTokenSource.Token, cancellationTokenSource.Token);
      task.ContinueWith(t => {
        try {
          t.Wait();
        }
        catch (AggregateException ex) {
          try {
            ex.Flatten().Handle(x => x is OperationCanceledException);
          }
          catch (AggregateException remaining) {
            if (remaining.InnerExceptions.Count == 1) OnExceptionOccurred(remaining.InnerExceptions[0]);
            else OnExceptionOccurred(remaining);
          }
        }

        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
        OnStopped();
      });
    }

    protected virtual void Run(object state) {
      CancellationToken cancellationToken = (CancellationToken)state;
      lastUpdateTime = DateTime.Now;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        BenchmarkAlgorithm = (IBenchmark)BenchmarkAlgorithmParameter.ActualValue;
        int chunkSize = ((IntValue)ChunkSizeParameter.ActualValue).Value;
        if (chunkSize > 0) {
          BenchmarkAlgorithm.ChunkData = CreateDataChuck(chunkSize);
        } else if (chunkSize < 0) {
          throw new ArgumentException("ChunkSize must not be negativ.");
        }
        TimeSpan timelimit = TimeSpan.FromMinutes(((DoubleValue)TimeLimitParameter.ActualValue).Value);
        if (timelimit.TotalMilliseconds < 0) {
          throw new ArgumentException("TimeLimit must not be negativ. ");
        }
        BenchmarkAlgorithm.TimeLimit = timelimit;
        BenchmarkAlgorithm.Run(cancellationToken, results);
      }
      catch (OperationCanceledException) {
      }
      finally {
        timer.Elapsed -= new System.Timers.ElapsedEventHandler(timer_Elapsed);
        timer.Stop();
        ExecutionTime += DateTime.Now - lastUpdateTime;
      }
    }

    private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
      System.Timers.Timer timer = (System.Timers.Timer)sender;
      timer.Enabled = false;
      DateTime now = DateTime.Now;
      ExecutionTime += now - lastUpdateTime;
      lastUpdateTime = now;
      timer.Enabled = true;
    }

    public virtual void CollectResultValues(IDictionary<string, IItem> values) {
      values.Add("Execution Time", new TimeSpanValue(ExecutionTime));
      CollectResultsRecursively("", Results, values);
    }

    private void CollectResultsRecursively(string path, ResultCollection results, IDictionary<string, IItem> values) {
      foreach (IResult result in results) {
        values.Add(path + result.Name, result.Value);
        ResultCollection childCollection = result.Value as ResultCollection;
        if (childCollection != null) {
          CollectResultsRecursively(path + result.Name + ".", childCollection, values);
        }
      }
    }

    public virtual void CollectParameterValues(IDictionary<string, IItem> values) {
      foreach (IValueParameter param in parameters.OfType<IValueParameter>()) {
        if (param.GetsCollected && param.Value != null) values.Add(param.Name, param.Value);
        if (param.Value is IParameterizedItem) {
          Dictionary<string, IItem> children = new Dictionary<string, IItem>();
          ((IParameterizedItem)param.Value).CollectParameterValues(children);
          foreach (string key in children.Keys)
            values.Add(param.Name + "." + key, children[key]);
        }
      }
    }

    private byte[][] CreateDataChuck(int megaBytes) {
      if (megaBytes <= 0) {
        throw new ArgumentException("MegaBytes must be greater than zero", "megaBytes");
      }
      byte[][] chunk = new byte[megaBytes][];
      for (int i = 0; i < chunk.Length; i++) {
        chunk[i] = new byte[1024 * 1024];
        random.NextBytes(chunk[i]);
      }
      return chunk;
    }

    #region Events

    public event EventHandler ExecutionStateChanged;
    protected virtual void OnExecutionStateChanged() {
      EventHandler handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    protected virtual void OnExecutionTimeChanged() {
      EventHandler handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ProblemChanged;
    protected virtual void OnProblemChanged() {
      EventHandler handler = ProblemChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler StoreAlgorithmInEachRunChanged;
    protected virtual void OnStoreAlgorithmInEachRunChanged() {
      EventHandler handler = StoreAlgorithmInEachRunChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    protected virtual void OnPrepared() {
      ExecutionState = ExecutionState.Prepared;
      ExecutionTime = TimeSpan.Zero;
      foreach (IStatefulItem statefulObject in this.GetObjectGraphObjects().OfType<IStatefulItem>()) {
        statefulObject.InitializeState();
      }
      EventHandler handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    protected virtual void OnStarted() {
      ExecutionState = ExecutionState.Started;
      EventHandler handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    protected virtual void OnPaused() {
      ExecutionState = ExecutionState.Paused;
      EventHandler handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    protected virtual void OnStopped() {
      ExecutionState = ExecutionState.Stopped;
      foreach (IStatefulItem statefulObject in this.GetObjectGraphObjects().OfType<IStatefulItem>()) {
        statefulObject.ClearState();
      }
      runsCounter++;
      runs.Add(new Run(string.Format("{0} Run {1}", Name, runsCounter), this));
      EventHandler handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    protected virtual void OnExceptionOccurred(Exception exception) {
      EventHandler<EventArgs<Exception>> handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    public event EventHandler<CancelEventArgs<string>> NameChanging;
    protected virtual void OnNameChanging(CancelEventArgs<string> e) {
      var handler = NameChanging;
      if (handler != null) handler(this, e);
    }

    public event EventHandler NameChanged;
    protected virtual void OnNameChanged() {
      var handler = NameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnToStringChanged();
    }

    public event EventHandler DescriptionChanged;
    protected virtual void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ItemImageChanged;
    protected virtual void OnItemImageChanged() {
      EventHandler handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    protected virtual void OnToStringChanged() {
      EventHandler handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    protected virtual void DeregisterProblemEvents() {
      problem.OperatorsChanged -= new EventHandler(Problem_OperatorsChanged);
      problem.Reset -= new EventHandler(Problem_Reset);
    }
    protected virtual void RegisterProblemEvents() {
      problem.OperatorsChanged += new EventHandler(Problem_OperatorsChanged);
      problem.Reset += new EventHandler(Problem_Reset);
    }
    protected virtual void Problem_OperatorsChanged(object sender, EventArgs e) { }
    protected virtual void Problem_Reset(object sender, EventArgs e) {
      Prepare();
    }

    protected virtual void DeregisterRunsEvents() {
      runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
    }
    protected virtual void RegisterRunsEvents() {
      runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
    }
    protected virtual void Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      runsCounter = runs.Count;
    }

    #endregion

    #region Clone

    public IDeepCloneable Clone(Cloner cloner) {
      return new Benchmark(this, cloner);
    }

    public object Clone() {
      return Clone(new Cloner());
    }

    #endregion
  }
}
