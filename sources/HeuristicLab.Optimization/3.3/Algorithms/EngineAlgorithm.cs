#region License Information
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A base class for algorithms which use an engine for execution.
  /// </summary>
  [Item("EngineAlgorithm", "A base class for algorithms which use an engine for execution.")]
  [StorableClass]
  public abstract class EngineAlgorithm : Algorithm {
    [Storable]
    private OperatorGraph operatorGraph;
    public OperatorGraph OperatorGraph {
      get { return operatorGraph; }
      protected set {
        if (value == null) throw new ArgumentNullException();
        if (value != operatorGraph) {
          operatorGraph.InitialOperatorChanged -= new EventHandler(OperatorGraph_InitialOperatorChanged);
          operatorGraph = value;
          operatorGraph.InitialOperatorChanged += new EventHandler(OperatorGraph_InitialOperatorChanged);
          OnOperatorGraphChanged();
          Prepare();
        }
      }
    }

    [Storable]
    private IScope globalScope;
    protected IScope GlobalScope {
      get { return globalScope; }
    }

    [Storable]
    private IEngine engine;
    public IEngine Engine {
      get { return engine; }
      set {
        if (engine != value) {
          if (engine != null) DeregisterEngineEvents();
          engine = value;
          if (engine != null) RegisterEngineEvents();
          OnEngineChanged();
          Prepare();
        }
      }
    }

    public override ResultCollection Results {
      get {
        return (ResultCollection)globalScope.Variables["Results"].Value;
      }
    }

    protected EngineAlgorithm()
      : base() {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      operatorGraph = new OperatorGraph();
      Initialize();
    }
    protected EngineAlgorithm(string name)
      : base(name) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      operatorGraph = new OperatorGraph();
      Initialize();
    }
    protected EngineAlgorithm(string name, ParameterCollection parameters)
      : base(name, parameters) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      operatorGraph = new OperatorGraph();
      Initialize();
    }
    protected EngineAlgorithm(string name, string description)
      : base(name, description) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      operatorGraph = new OperatorGraph();
      Initialize();
    }
    protected EngineAlgorithm(string name, string description, ParameterCollection parameters)
      : base(name, description, parameters) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      operatorGraph = new OperatorGraph();
      Initialize();
    }
    [StorableConstructor]
    protected EngineAlgorithm(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();

      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      // clear global scope if it contains any sub-scopes or additional variables
      if ((ExecutionState == Core.ExecutionState.Stopped) && ((globalScope.SubScopes.Count > 0) || (globalScope.Variables.Count > 1))) {
        ResultCollection results = Results;
        globalScope.Clear();
        globalScope.Variables.Add(new Variable("Results", results));
      }
      #endregion
    }

    protected EngineAlgorithm(EngineAlgorithm original, Cloner cloner)
      : base(original, cloner) {
      globalScope = cloner.Clone(original.globalScope);
      engine = cloner.Clone(original.engine);
      operatorGraph = cloner.Clone(original.operatorGraph);
      Initialize();
    }

    private void Initialize() {
      operatorGraph.InitialOperatorChanged += new EventHandler(OperatorGraph_InitialOperatorChanged);
      if (engine == null) {
        var types = ApplicationManager.Manager.GetTypes(typeof(IEngine));
        Type t = types.FirstOrDefault(x => x.Name.Equals("SequentialEngine"));
        if (t == null) t = types.FirstOrDefault();
        if (t != null) engine = (IEngine)Activator.CreateInstance(t);
      }
      if (engine != null) RegisterEngineEvents();
    }

    public virtual IAlgorithm CreateUserDefinedAlgorithm() {
      return new UserDefinedAlgorithm(this, new Cloner());
    }

    public override void Prepare() {
      base.Prepare();
      globalScope.Clear();
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));

      if ((engine != null) && (operatorGraph.InitialOperator != null)) {
        ExecutionContext context = null;
        if (Problem != null) {
          foreach (var item in Problem.ExecutionContextItems)
            context = new ExecutionContext(context, item, globalScope);
        }
        context = new ExecutionContext(context, this, globalScope);
        context = new ExecutionContext(context, operatorGraph.InitialOperator, globalScope);
        engine.Prepare(context);
      }
    }
    public override void Start() {
      base.Start();
      if (engine != null) engine.Start();
    }
    public override void Pause() {
      base.Pause();
      if (engine != null) engine.Pause();
    }
    public override void Stop() {
      base.Stop();
      if (engine != null) engine.Stop();
    }

    #region Events
    public event EventHandler EngineChanged;
    protected virtual void OnEngineChanged() {
      EventHandler handler = EngineChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OperatorGraphChanged;
    protected virtual void OnOperatorGraphChanged() {
      EventHandler handler = OperatorGraphChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void RegisterEngineEvents() {
      Engine.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      Engine.ExecutionTimeChanged += new EventHandler(Engine_ExecutionTimeChanged);
      Engine.Paused += new EventHandler(Engine_Paused);
      Engine.Prepared += new EventHandler(Engine_Prepared);
      Engine.Started += new EventHandler(Engine_Started);
      Engine.Stopped += new EventHandler(Engine_Stopped);
    }
    private void DeregisterEngineEvents() {
      Engine.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      Engine.ExecutionTimeChanged -= new EventHandler(Engine_ExecutionTimeChanged);
      Engine.Paused -= new EventHandler(Engine_Paused);
      Engine.Prepared -= new EventHandler(Engine_Prepared);
      Engine.Started -= new EventHandler(Engine_Started);
      Engine.Stopped -= new EventHandler(Engine_Stopped);
    }
    private void Engine_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void Engine_ExecutionTimeChanged(object sender, EventArgs e) {
      ExecutionTime = Engine.ExecutionTime;
    }
    private void Engine_Paused(object sender, EventArgs e) {
      OnPaused();
    }
    private void Engine_Prepared(object sender, EventArgs e) {
      OnPrepared();
    }
    private void Engine_Started(object sender, EventArgs e) {
      OnStarted();
    }
    private void Engine_Stopped(object sender, EventArgs e) {
      ResultCollection results = Results;
      globalScope.Clear();
      globalScope.Variables.Add(new Variable("Results", results));
      OnStopped();
    }

    private void OperatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      Prepare();
    }
    #endregion
  }
}
