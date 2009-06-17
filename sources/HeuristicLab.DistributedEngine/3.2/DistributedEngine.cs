#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Xml;
using System.Threading;
using HeuristicLab.Core;
using HeuristicLab.Grid;
using System.ServiceModel;
using System.IO;
using System.IO.Compression;
using HeuristicLab.PluginInfrastructure;
using System.Windows.Forms;
using System.Diagnostics;

namespace HeuristicLab.DistributedEngine {
  public class DistributedEngine : EngineBase, IEditable {
    private List<KeyValuePair<ProcessingEngine, AtomicOperation>> suspendedEngines = new List<KeyValuePair<ProcessingEngine, AtomicOperation>>();
    private JobManager jobManager;
    private string serverAddress;
    public string ServerAddress {
      get { return serverAddress; }
      set {
        if (value != serverAddress) {
          serverAddress = value;
        }
      }
    }
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      DistributedEngine clone = (DistributedEngine)base.Clone(clonedObjects);
      clone.ServerAddress = serverAddress;
      return clone;
    }

    public override IView CreateView() {
      return new DistributedEngineEditor(this);
    }
    public virtual IEditor CreateEditor() {
      return new DistributedEngineEditor(this);
    }

    public override bool Terminated {
      get {
        return base.Terminated && suspendedEngines.Count == 0;
      }
    }

    public override void Reset() {
      suspendedEngines.Clear();
      base.Reset();
    }

    public override void Execute() {
      if (jobManager == null) this.jobManager = new JobManager(serverAddress);
      jobManager.Reset();
      base.Execute();
    }

    public override void ExecuteSteps(int steps) {
      throw new InvalidOperationException("DistributedEngine doesn't support stepwise execution");
    }

    protected override void ProcessNextOperation() {
      if (suspendedEngines.Count > 0) {
        ProcessSuspendedEngines();
      } else {
        IOperation operation = myExecutionStack.Pop();
        ProcessOperation(operation);
      }
    }

    private void ProcessSuspendedEngines() {
      AsyncGridResult[] asyncResults = new AsyncGridResult[suspendedEngines.Count];
      int i = 0;
      foreach (KeyValuePair<ProcessingEngine, AtomicOperation> suspendedPair in suspendedEngines) {
        asyncResults[i++] = jobManager.BeginExecuteEngine(suspendedPair.Key);
      }
      WaitForAll(asyncResults);
      // collect results
      List<KeyValuePair<ProcessingEngine, AtomicOperation>> results = new List<KeyValuePair<ProcessingEngine, AtomicOperation>>();
      try {
        int resultIndex = 0;
        foreach (KeyValuePair<ProcessingEngine, AtomicOperation> suspendedPair in suspendedEngines) {
          KeyValuePair<ProcessingEngine, AtomicOperation> p = new KeyValuePair<ProcessingEngine, AtomicOperation>(
              (ProcessingEngine)jobManager.EndExecuteEngine(asyncResults[resultIndex++]),
              suspendedPair.Value);
          results.Add(p);
        }
      }
      catch (Exception e) {
        // this exception means there was a problem with the underlying communication infrastructure
        // -> show message dialog and abort engine
        Abort();
        ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(e); });
        return;
      }
      // got all result engines without an exception -> merge results
      ProcessResults(results);
    }

    private void ProcessOperation(IOperation operation) {
      if (operation is AtomicOperation) {
        AtomicOperation atomicOperation = (AtomicOperation)operation;
        IOperation next = null;
        try {
          next = atomicOperation.Operator.Execute(atomicOperation.Scope);
        }
        catch (Exception ex) {
          // push operation on stack again
          myExecutionStack.Push(atomicOperation);
          Abort();
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
        }
        if (next != null)
          myExecutionStack.Push(next);
        OnOperationExecuted(atomicOperation);
        if (atomicOperation.Operator.Breakpoint) Abort();
      } else if (operation is CompositeOperation) {
        CompositeOperation compositeOperation = (CompositeOperation)operation;
        if (compositeOperation.ExecuteInParallel) {
          ProcessParallelOperation(compositeOperation);
          OnOperationExecuted(compositeOperation);
        } else {
          for (int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
            myExecutionStack.Push(compositeOperation.Operations[i]);
        }
      }
    }

    private void ProcessParallelOperation(CompositeOperation compositeOperation) {
      // send operations to grid
      AsyncGridResult[] asyncResults = BeginExecuteOperations(compositeOperation);
      WaitForAll(asyncResults);
      // collect results
      List<KeyValuePair<ProcessingEngine, AtomicOperation>> results = new List<KeyValuePair<ProcessingEngine, AtomicOperation>>();
      try {
        int i = 0;
        foreach (AtomicOperation parOperation in compositeOperation.Operations) {
          results.Add(new KeyValuePair<ProcessingEngine, AtomicOperation>(
              (ProcessingEngine)jobManager.EndExecuteEngine(asyncResults[i++]), parOperation));
        }
      }
      catch (Exception e) {
        // this exception means there was a problem with the underlying communication infrastructure
        // -> show message dialog, abort engine, requeue the whole composite operation again and return
        myExecutionStack.Push(compositeOperation);
        Abort();
        ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(e); });
        return;
      }
      // got all result engines without an exception -> merge results
      ProcessResults(results);
    }

    private AsyncGridResult[] BeginExecuteOperations(CompositeOperation compositeOperation) {
      AsyncGridResult[] asyncResults = new AsyncGridResult[compositeOperation.Operations.Count];
      int i = 0;
      // HACK: assume that all atomicOperations have the same parent scope.
      // 1) find that parent scope
      // 2) remove all branches starting from the global scope that don't lead to the parentScope of the parallel operation
      // 3) keep the branches to 'repair' the scope-tree later
      // 4) for each parallel job attach only the sub-scope that this operation uses
      // 5) after starting all parallel jobs restore the whole scope-tree
      IScope parentScope = FindParentScope(GlobalScope, ((AtomicOperation)compositeOperation.Operations[0]).Scope);
      List<IList<IScope>> prunedScopes = new List<IList<IScope>>();
      PruneToParentScope(GlobalScope, parentScope, prunedScopes);
      List<IScope> subScopes = new List<IScope>(parentScope.SubScopes);
      foreach (IScope scope in subScopes) {
        parentScope.RemoveSubScope(scope);
      }
      // start all parallel jobs
      foreach (AtomicOperation parOperation in compositeOperation.Operations) {
        parentScope.AddSubScope(parOperation.Scope);
        asyncResults[i++] = jobManager.BeginExecuteEngine(new ProcessingEngine(GlobalScope, parOperation));
        parentScope.RemoveSubScope(parOperation.Scope);
      }
      foreach (IScope scope in subScopes) {
        parentScope.AddSubScope(scope);
      }
      prunedScopes.Reverse();
      RestoreFullTree(GlobalScope, prunedScopes);

      return asyncResults;
    }

    private void WaitForAll(AsyncGridResult[] asyncResults) {
      // wait until all jobs are finished
      // WaitAll works only with maximally 64 waithandles
      if (asyncResults.Length <= 64) {
        WaitHandle[] waitHandles = new WaitHandle[asyncResults.Length];
        for (int i = 0; i < asyncResults.Length; i++) {
          waitHandles[i] = asyncResults[i].WaitHandle;
        }
        WaitHandle.WaitAll(waitHandles);
      } else {
        int i;
        for (i = 0; i < asyncResults.Length; i++) {
          asyncResults[i].WaitHandle.WaitOne();
        }
      }
    }

    private void ProcessResults(List<KeyValuePair<ProcessingEngine, AtomicOperation>> results) {
      // create a new compositeOperation to hold canceled operations that should be restarted
      CompositeOperation canceledOperations = new CompositeOperation();
      canceledOperations.ExecuteInParallel = true;

      suspendedEngines.Clear();
      // retrieve results and merge into scope-tree
      foreach (KeyValuePair<ProcessingEngine, AtomicOperation> p in results) {
        ProcessingEngine resultEngine = p.Key;
        AtomicOperation parOperation = p.Value;
        if (resultEngine.Canceled && !resultEngine.Suspended) {
          // when an engine was canceled but not suspended this means there was a problem
          // show error message and queue the operation for restart (again parallel)
          // but don't merge the results of the aborted engine
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(new JobExecutionException(resultEngine.ErrorMessage)); });
          canceledOperations.AddOperation(parOperation);
        } else if (resultEngine.Suspended) {
          // when the engine was suspended it means it was stopped because of a breakpoint 
          // -> merge the partial results and queue the engine (which has operations remaining in the execution stack) to be resumed (parallel)
          MergeScope(parOperation.Scope, resultEngine.InitialOperation.Scope);
          resultEngine.InitialOperation = parOperation;
          suspendedEngines.Add(new KeyValuePair<ProcessingEngine, AtomicOperation>(resultEngine, parOperation));
        } else {
          // engine is finished ->
          // simply merge the results into our local scope-tree
          MergeScope(parOperation.Scope, resultEngine.InitialOperation.Scope);
        }
      }
      // if there were exceptions -> abort 
      if (canceledOperations.Operations.Count > 0) {
        // requeue the aborted operations 
        myExecutionStack.Push(canceledOperations);
        Abort();
      }
      // if there were breakpoints -> abort
      if (suspendedEngines.Count > 0) {
        Abort();
      }
    }

    private void RestoreFullTree(IScope currentScope, IList<IList<IScope>> savedScopes) {
      if (savedScopes.Count == 0) return;
      IScope remainingBranch = currentScope.SubScopes[0];
      currentScope.RemoveSubScope(remainingBranch);
      IList<IScope> savedScopesForCurrent = savedScopes[0];
      foreach (IScope savedScope in savedScopesForCurrent) {
        currentScope.AddSubScope(savedScope);
      }
      savedScopes.RemoveAt(0);
      RestoreFullTree(remainingBranch, savedScopes);
    }

    private IScope PruneToParentScope(IScope currentScope, IScope scope, IList<IList<IScope>> prunedScopes) {
      if (currentScope == scope) return currentScope;
      if (currentScope.SubScopes.Count == 0) return null;
      IScope foundScope = null;
      // try to find the searched scope in all my sub-scopes
      foreach (IScope subScope in currentScope.SubScopes) {
        foundScope = PruneToParentScope(subScope, scope, prunedScopes);
        if (foundScope != null) break; // we can stop as soon as we find the scope in a branch
      }
      if (foundScope != null) { // when we found the scopes in my sub-scopes
        List<IScope> subScopes = new List<IScope>(currentScope.SubScopes); // store the list of sub-scopes
        prunedScopes.Add(subScopes);
        // remove all my sub-scopes
        foreach (IScope subScope in subScopes) {
          currentScope.RemoveSubScope(subScope);
        }
        // add only the branch that leads to the scope that I search for
        currentScope.AddSubScope(foundScope);
        return currentScope; // return that this scope contains the branch that leads to the searched scopes
      } else {
        return null; // otherwise we didn't find the searched scope and we can return null
      }
    }

    private IScope FindParentScope(IScope currentScope, IScope childScope) {
      if (currentScope.SubScopes.Contains(childScope)) return currentScope;
      foreach (IScope subScope in currentScope.SubScopes) {
        IScope result = FindParentScope(subScope, childScope);
        if (result != null) return result;
      }
      return null;
    }

    private void MergeScope(IScope original, IScope result) {
      // merge the results
      original.Clear();
      foreach (IVariable variable in result.Variables) {
        original.AddVariable(variable);
      }
      foreach (IScope subScope in result.SubScopes) {
        original.AddSubScope(subScope);
      }
      foreach (KeyValuePair<string, string> alias in result.Aliases) {
        original.AddAlias(alias.Key, alias.Value);
      }
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute addressAttribute = document.CreateAttribute("ServerAddress");
      addressAttribute.Value = ServerAddress;
      node.Attributes.Append(addressAttribute);
      if (suspendedEngines.Count > 0) {
        XmlNode suspendedEnginesNode = document.CreateElement("SuspendedEngines");
        foreach (KeyValuePair<ProcessingEngine, AtomicOperation> p in suspendedEngines) {
          XmlNode n = document.CreateElement("Entry");
          n.AppendChild(PersistenceManager.Persist(p.Key, document, persistedObjects));
          n.AppendChild(PersistenceManager.Persist(p.Value, document, persistedObjects));
          suspendedEnginesNode.AppendChild(n);
        }
        node.AppendChild(suspendedEnginesNode);
      }
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      ServerAddress = node.Attributes["ServerAddress"].Value;
      XmlNode suspendedEnginesNode = node.SelectSingleNode("SuspendedEngines");
      if (suspendedEnginesNode != null) {
        foreach (XmlNode n in suspendedEnginesNode.ChildNodes) {
          KeyValuePair<ProcessingEngine, AtomicOperation> p = new KeyValuePair<ProcessingEngine, AtomicOperation>(
            (ProcessingEngine)PersistenceManager.Restore(n.ChildNodes[0], restoredObjects),
            (AtomicOperation)PersistenceManager.Restore(n.ChildNodes[1], restoredObjects));
          suspendedEngines.Add(p);
        }
      }
    }
    #endregion
  }
}