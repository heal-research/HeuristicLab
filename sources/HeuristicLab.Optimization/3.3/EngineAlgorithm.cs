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
using HeuristicLab.Collections;
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
    private OperatorGraph operatorGraph;
    [Storable]
    private OperatorGraph OperatorGraphPersistence {
      get { return operatorGraph; }
      set {
        if (operatorGraph != null) operatorGraph.InitialOperatorChanged -= new EventHandler(OperatorGraph_InitialOperatorChanged);
        operatorGraph = value;
        if (operatorGraph != null) operatorGraph.InitialOperatorChanged += new EventHandler(OperatorGraph_InitialOperatorChanged);
      }
    }
    protected OperatorGraph OperatorGraph {
      get { return operatorGraph; }
      set {
        if (value == null) throw new ArgumentNullException();
        if (value != operatorGraph) {
          if (operatorGraph != null) operatorGraph.InitialOperatorChanged -= new EventHandler(OperatorGraph_InitialOperatorChanged);
          operatorGraph = value;
          if (operatorGraph != null) operatorGraph.InitialOperatorChanged += new EventHandler(OperatorGraph_InitialOperatorChanged);
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

    private IEngine engine;
    [Storable]
    private IEngine EnginePersistence {
      get { return engine; }
      set {
        if (engine != null) DeregisterEngineEvents();
        engine = value;
        if (engine != null) RegisterEngineEvents();
      }
    }
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

    public override TimeSpan ExecutionTime {
      get {
        if (engine == null) return TimeSpan.Zero;
        else return engine.ExecutionTime;
      }
    }

    public override bool Finished {
      get {
        if (engine == null) return true;
        else return engine.Finished;
      }
    }

    protected EngineAlgorithm()
      : base() {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      OperatorGraph = new OperatorGraph();
      InitializeEngine();
    }
    protected EngineAlgorithm(string name)
      : base(name) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      OperatorGraph = new OperatorGraph();
      InitializeEngine();
    }
    protected EngineAlgorithm(string name, ParameterCollection parameters)
      : base(name, parameters) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      OperatorGraph = new OperatorGraph();
      InitializeEngine();
    }
    protected EngineAlgorithm(string name, string description)
      : base(name, description) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      OperatorGraph = new OperatorGraph();
      InitializeEngine();
    }
    protected EngineAlgorithm(string name, string description, ParameterCollection parameters)
      : base(name, description, parameters) {
      globalScope = new Scope("Global Scope");
      globalScope.Variables.Add(new Variable("Results", new ResultCollection()));
      OperatorGraph = new OperatorGraph();
      InitializeEngine();
    }

    private void InitializeEngine() {
      if (ApplicationManager.Manager != null) {
        var types = ApplicationManager.Manager.GetTypes(typeof(IEngine));
        Type t = types.FirstOrDefault(x => x.Name.Equals("SequentialEngine"));
        if (t == null) t = types.FirstOrDefault();
        if (t != null) Engine = (IEngine)Activator.CreateInstance(t);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      EngineAlgorithm clone = (EngineAlgorithm)base.Clone(cloner);
      clone.globalScope = (IScope)cloner.Clone(globalScope);
      clone.Engine = (IEngine)cloner.Clone(engine);
      clone.OperatorGraph = (OperatorGraph)cloner.Clone(operatorGraph);
      return clone;
    }

    public UserDefinedAlgorithm CreateUserDefinedAlgorithm() {
      UserDefinedAlgorithm algorithm = new UserDefinedAlgorithm(Name, Description);
      Cloner cloner = new Cloner();
      foreach (IParameter param in Parameters)
        algorithm.Parameters.Add((IParameter)cloner.Clone(param));
      algorithm.Problem = (IProblem)cloner.Clone(Problem);
      algorithm.Engine = (IEngine)cloner.Clone(engine);
      algorithm.OperatorGraph = (OperatorGraph)cloner.Clone(operatorGraph);
      return algorithm;
    }

    protected override void OnCanceledChanged() {
      if (Canceled && (engine != null))
        engine.Stop();
    }
    protected override void OnPrepared() {
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
      base.OnPrepared();
    }
    protected override void OnStarted() {
      if (engine != null) engine.Start();
      base.OnStarted();
    }

    protected virtual void OnOperatorGraphChanged() { }

    public event EventHandler EngineChanged;
    protected virtual void OnEngineChanged() {
      if (EngineChanged != null)
        EngineChanged(this, EventArgs.Empty);
    }

    private void OperatorGraph_InitialOperatorChanged(object sender, EventArgs e) {
      Prepare();
    }
    private void RegisterEngineEvents() {
      Engine.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      Engine.ExecutionTimeChanged += new EventHandler(Engine_ExecutionTimeChanged);
      Engine.RunningChanged += new EventHandler(Engine_RunningChanged);
    }
    private void DeregisterEngineEvents() {
      Engine.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Engine_ExceptionOccurred);
      Engine.ExecutionTimeChanged -= new EventHandler(Engine_ExecutionTimeChanged);
      Engine.RunningChanged -= new EventHandler(Engine_RunningChanged);
    }

    private void Engine_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void Engine_ExecutionTimeChanged(object sender, EventArgs e) {
      OnExecutionTimeChanged();
    }
    private void Engine_RunningChanged(object sender, EventArgs e) {
      if (!Engine.Running) OnStopped();
    }
  }
}
