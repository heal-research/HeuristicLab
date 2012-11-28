#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("Benchmark Algorithm", "An algorithm to execute performance benchmarks (Linpack, Dhrystone, Whetstone, etc.).")]
  [Creatable("Algorithms")]
  [StorableClass]
  public sealed class BenchmarkAlgorithm : IAlgorithm {
    private CancellationTokenSource cancellationTokenSource;

    public string ItemName {
      get { return ItemAttribute.GetName(this.GetType()); }
    }
    public string ItemDescription {
      get { return ItemAttribute.GetDescription(this.GetType()); }
    }
    public Version ItemVersion {
      get { return ItemAttribute.GetVersion(this.GetType()); }
    }
    public static Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Event; }
    }
    public Image ItemImage {
      get {
        if (ExecutionState == ExecutionState.Prepared) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePrepared;
        else if (ExecutionState == ExecutionState.Started) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStarted;
        else if (ExecutionState == ExecutionState.Paused) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutablePaused;
        else if (ExecutionState == ExecutionState.Stopped) return HeuristicLab.Common.Resources.VSImageLibrary.ExecutableStopped;
        else return ItemAttribute.GetImage(this.GetType());
      }
    }

    [Storable]
    private DateTime lastUpdateTime;

    [Storable]
    private IBenchmark benchmark;
    public IBenchmark Benchmark {
      get { return benchmark; }
      set {
        if (value == null) throw new ArgumentNullException();
        benchmark = value;
      }
    }

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
      private set {
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
    private int runsCounter;

    [Storable]
    private RunCollection runs = new RunCollection();
    public RunCollection Runs {
      get { return runs; }
      private set {
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

    public Type ProblemType {
      get {
        // BenchmarkAlgorithm does not have a problem, so return a type which is no problem for sure
        return typeof(BenchmarkAlgorithm);
      }
    }

    public IProblem Problem {
      get { return null; }
      set { throw new NotImplementedException("BenchmarkAlgorithm does not have a problem."); }
    }

    [Storable]
    private string name;
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
            runs.OptimizerName = name;
          }
        }
      }
    }
    public bool CanChangeName {
      get { return true; }
    }

    [Storable]
    private string description;
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
      get { return true; }
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
    public IConstrainedValueParameter<IBenchmark> BenchmarkParameter {
      get { return (IConstrainedValueParameter<IBenchmark>)Parameters["Benchmark"]; }
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
    private BenchmarkAlgorithm(bool deserializing) { }
    private BenchmarkAlgorithm(BenchmarkAlgorithm original, Cloner cloner) {
      if (original.ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      cloner.RegisterClonedObject(original, this);
      name = original.name;
      description = original.description;
      parameters = cloner.Clone(original.parameters);
      readOnlyParameters = null;
      executionState = original.executionState;
      executionTime = original.executionTime;
      storeAlgorithmInEachRun = original.storeAlgorithmInEachRun;
      runsCounter = original.runsCounter;
      Runs = cloner.Clone(original.runs);
      results = cloner.Clone(original.results);
    }
    public BenchmarkAlgorithm() {
      name = ItemName;
      description = ItemDescription;
      parameters = new ParameterCollection();
      readOnlyParameters = null;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      storeAlgorithmInEachRun = false;
      runsCounter = 0;
      Runs = new RunCollection() { OptimizerName = name };
      results = new ResultCollection();
      CreateParameters();
      DiscoverBenchmarks();
      Prepare();
    }
    #endregion

    private void CreateParameters() {
      Parameters.Add(new ValueParameter<IntValue>("ChunkSize", "The size in MB of the chunk data array that is generated.", new IntValue(0)));
      Parameters.Add(new ValueParameter<DoubleValue>("TimeLimit", "The time limit in minutes for a benchmark run (zero means a fixed number of iterations).", new DoubleValue(0)));
    }
    private void DiscoverBenchmarks() {
      var benchmarks = from t in ApplicationManager.Manager.GetTypes(typeof(IBenchmark))
                       select t;
      ItemSet<IBenchmark> values = new ItemSet<IBenchmark>();
      foreach (var benchmark in benchmarks) {
        IBenchmark b = (IBenchmark)Activator.CreateInstance(benchmark);
        values.Add(b);
      }
      string paramName = "Benchmark";
      if (!Parameters.ContainsKey(paramName)) {
        if (values.Count > 0) {
          Parameters.Add(new ConstrainedValueParameter<IBenchmark>(paramName, "The benchmark which should be executed.", values, values.First(a => a is IBenchmark)));
        } else {
          Parameters.Add(new ConstrainedValueParameter<IBenchmark>(paramName, "The benchmark which should be executed.", values));
        }
      }
    }

    public IDeepCloneable Clone(Cloner cloner) {
      return new BenchmarkAlgorithm(this, cloner);
    }
    public object Clone() {
      return Clone(new Cloner());
    }

    public override string ToString() {
      return Name;
    }

    public void Prepare() {
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
    public void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
    }
    public void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
      cancellationTokenSource.Cancel();
    }
    public void Start() {
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

    private void Run(object state) {
      CancellationToken cancellationToken = (CancellationToken)state;
      lastUpdateTime = DateTime.Now;
      System.Timers.Timer timer = new System.Timers.Timer(250);
      timer.AutoReset = true;
      timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
      timer.Start();
      try {
        Benchmark = (IBenchmark)BenchmarkParameter.ActualValue;
        int chunkSize = ((IntValue)ChunkSizeParameter.ActualValue).Value;
        if (chunkSize > 0) {
          Benchmark.ChunkData = CreateChunkData(chunkSize);
        } else if (chunkSize < 0) {
          throw new ArgumentException("ChunkSize must not be negativ.");
        }
        TimeSpan timelimit = TimeSpan.FromMinutes(((DoubleValue)TimeLimitParameter.ActualValue).Value);
        if (timelimit.TotalMilliseconds < 0) {
          throw new ArgumentException("TimeLimit must not be negativ. ");
        }
        Benchmark.TimeLimit = timelimit;
        Benchmark.Run(cancellationToken, results);
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

    public void CollectResultValues(IDictionary<string, IItem> values) {
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
    public void CollectParameterValues(IDictionary<string, IItem> values) {
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

    private byte[][] CreateChunkData(int megaBytes) {
      if (megaBytes <= 0) {
        throw new ArgumentException("MegaBytes must be greater than zero", "megaBytes");
      }
      Random random = new Random();
      byte[][] chunk = new byte[megaBytes][];
      for (int i = 0; i < chunk.Length; i++) {
        chunk[i] = new byte[1024 * 1024];
        random.NextBytes(chunk[i]);
      }
      return chunk;
    }

    #region Events
    public event EventHandler ExecutionStateChanged;
    private void OnExecutionStateChanged() {
      EventHandler handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    private void OnExecutionTimeChanged() {
      EventHandler handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ProblemChanged { add { } remove { } }
    public event EventHandler StoreAlgorithmInEachRunChanged;
    private void OnStoreAlgorithmInEachRunChanged() {
      EventHandler handler = StoreAlgorithmInEachRunChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    private void OnPrepared() {
      ExecutionState = ExecutionState.Prepared;
      ExecutionTime = TimeSpan.Zero;
      foreach (IStatefulItem statefulObject in this.GetObjectGraphObjects().OfType<IStatefulItem>()) {
        statefulObject.InitializeState();
      }
      EventHandler handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      ExecutionState = ExecutionState.Started;
      EventHandler handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    private void OnPaused() {
      ExecutionState = ExecutionState.Paused;
      EventHandler handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
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
    private void OnExceptionOccurred(Exception exception) {
      EventHandler<EventArgs<Exception>> handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    public event EventHandler<CancelEventArgs<string>> NameChanging;
    private void OnNameChanging(CancelEventArgs<string> e) {
      var handler = NameChanging;
      if (handler != null) handler(this, e);
    }

    public event EventHandler NameChanged;
    private void OnNameChanged() {
      var handler = NameChanged;
      if (handler != null) handler(this, EventArgs.Empty);
      OnToStringChanged();
    }

    public event EventHandler DescriptionChanged;
    private void OnDescriptionChanged() {
      var handler = DescriptionChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    public event EventHandler ItemImageChanged;
    private void OnItemImageChanged() {
      EventHandler handler = ItemImageChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ToStringChanged;
    private void OnToStringChanged() {
      EventHandler handler = ToStringChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void DeregisterRunsEvents() {
      runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
    }
    private void RegisterRunsEvents() {
      runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Runs_CollectionReset);
    }
    private void Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      runsCounter = runs.Count;
    }
    #endregion
  }
}
