#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
    protected OperatorGraph OperatorGraph {
      get { return operatorGraph; }
      set {
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

    public override IDeepCloneable Clone(Cloner cloner) {
      EngineAlgorithm clone = (EngineAlgorithm)base.Clone(cloner);
      clone.globalScope = (IScope)cloner.Clone(globalScope);
      clone.engine = (IEngine)cloner.Clone(engine);
      clone.operatorGraph = (OperatorGraph)cloner.Clone(operatorGraph);
      clone.Initialize();
      return clone;
    }
    protected override void Clone(IDeepCloneable clone, Cloner cloner) {
      base.Clone(clone, cloner);
      EngineAlgorithm algorithm = clone as EngineAlgorithm;
      if (algorithm != null) {
        algorithm.globalScope = (IScope)cloner.Clone(globalScope);
        algorithm.engine = (IEngine)cloner.Clone(engine);
        algorithm.operatorGraph = (OperatorGraph)cloner.Clone(operatorGraph);
        algorithm.Initialize();
      }
    }

    public UserDefinedAlgorithm CreateUserDefinedAlgorithm() {
      UserDefinedAlgorithm algorithm = new UserDefinedAlgorithm();
      Cloner cloner = new Cloner();
      cloner.RegisterClonedObject(this, algorithm);
      Clone(algorithm, cloner);
      return algorithm;
    }

    public override void Prepare() {
      base.Prepare();
      globalScope.Clear();
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));

      if (engine != null) {
        ExecutionContext context = null;
        if (operatorGraph.InitialOperator != null) {
          if (Problem != null) {
            context = new ExecutionContext(context, Problem, globalScope);
            foreach (IParameter param in Problem.Parameters)
              param.ExecutionContext = context;
          }
          context = new ExecutionContext(context, this, globalScope);
          foreach (IParameter param in this.Parameters)
            param.ExecutionContext = context;
          context = new ExecutionContext(context, operatorGraph.InitialOperator, globalScope);
        }
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
      if (EngineChanged != null)
        EngineChanged(this, EventArgs.Empty);
    }
    protected virtual void OnOperatorGraphChanged() { }
    protected override void OnStopped() {
      if (Problem != null) {
        foreach (IParameter param in Problem.Parameters)
          param.ExecutionContext = null;
      }
      foreach (IParameter param in Parameters)
        param.ExecutionContext = null;
      base.OnStopped();
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
      OnStopped();
    }

    private void OperatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      Prepare();
    }
    #endregion
  }
}
