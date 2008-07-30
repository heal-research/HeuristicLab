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
    private JobManager jobManager;
    private string serverAddress;
    public string ServerAddress {
      get { return serverAddress; }
      set {
        if(value != serverAddress) {
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

    public override void Execute() {
      if(jobManager == null) this.jobManager = new JobManager(serverAddress);
      jobManager.Reset();
      base.Execute();
    }

    public override void ExecuteSteps(int steps) {
      throw new InvalidOperationException("DistributedEngine doesn't support stepwise execution");
    }

    protected override void ProcessNextOperation() {
      IOperation operation = myExecutionStack.Pop();
      if(operation is AtomicOperation) {
        AtomicOperation atomicOperation = (AtomicOperation)operation;
        IOperation next = null;
        try {
          next = atomicOperation.Operator.Execute(atomicOperation.Scope);
        } catch(Exception ex) {
          // push operation on stack again
          myExecutionStack.Push(atomicOperation);
          Abort();
          ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(ex); });
        }
        if(next != null)
          myExecutionStack.Push(next);
        OnOperationExecuted(atomicOperation);
        if(atomicOperation.Operator.Breakpoint) Abort();
      } else if(operation is CompositeOperation) {
        CompositeOperation compositeOperation = (CompositeOperation)operation;
        if(compositeOperation.ExecuteInParallel) {
          try {
            WaitHandle[] waithandles = new WaitHandle[compositeOperation.Operations.Count];
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
            foreach(IScope scope in subScopes) {
              parentScope.RemoveSubScope(scope);
            }
            // start all parallel jobs
            foreach(AtomicOperation parOperation in compositeOperation.Operations) {
              parentScope.AddSubScope(parOperation.Scope);
              waithandles[i++] = jobManager.BeginExecuteOperation(GlobalScope, parOperation);
              parentScope.RemoveSubScope(parOperation.Scope);
            }
            foreach(IScope scope in subScopes) {
              parentScope.AddSubScope(scope);
            }
            prunedScopes.Reverse();
            RestoreFullTree(GlobalScope, prunedScopes);

            // wait until all jobs are finished
            // WaitAll works only with maximally 64 waithandles
            if(waithandles.Length <= 64) {
              WaitHandle.WaitAll(waithandles);
            } else {
              for(i = 0; i < waithandles.Length; i++) {
                waithandles[i].WaitOne();
                waithandles[i].Close();
              }
            }
            // retrieve results and merge into scope-tree
            foreach(AtomicOperation parOperation in compositeOperation.Operations) {
              ProcessingEngine resultEngine = jobManager.EndExecuteOperation(parOperation);
              if(resultEngine.Canceled) {
                // When the engine was canceled because of a problem at the client we can try to execute the steps locally.
                // If they also fail the (local) distributued-engine will be aborted and we will see an error-message.
                // so just push the original parallel operation back on the stack to force local execution.
                ExecutionStack.Push(parOperation);
              } else {
                // if everything went fine we can merge the results into our local scope-tree
                MergeScope(parOperation.Scope, resultEngine.InitialOperation.Scope);
              }
            }
          } catch(Exception e) {
            myExecutionStack.Push(compositeOperation);
            Abort();
            ThreadPool.QueueUserWorkItem(delegate(object state) { OnExceptionOccurred(e); });
          }
          OnOperationExecuted(compositeOperation);
        } else {
          for(int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
            myExecutionStack.Push(compositeOperation.Operations[i]);
        }
      }
    }

    private void RestoreFullTree(IScope currentScope, IList<IList<IScope>> savedScopes) {
      if(savedScopes.Count == 0) return;
      IScope remainingBranch = currentScope.SubScopes[0];
      currentScope.RemoveSubScope(remainingBranch);
      IList<IScope> savedScopesForCurrent = savedScopes[0];
      foreach(IScope savedScope in savedScopesForCurrent) {
        currentScope.AddSubScope(savedScope);
      }
      savedScopes.RemoveAt(0);
      RestoreFullTree(remainingBranch, savedScopes);
    }

    private IScope PruneToParentScope(IScope currentScope, IScope scope, IList<IList<IScope>> prunedScopes) {
      if(currentScope == scope) return currentScope;
      if(currentScope.SubScopes.Count == 0) return null;
      IScope foundScope = null;
      // try to find the searched scope in all my sub-scopes
      foreach(IScope subScope in currentScope.SubScopes) {
        foundScope = PruneToParentScope(subScope, scope, prunedScopes);
        if(foundScope != null) break; // we can stop as soon as we find the scope in a branch
      }
      if(foundScope != null) { // when we found the scopes in my sub-scopes
        List<IScope> subScopes = new List<IScope>(currentScope.SubScopes); // store the list of sub-scopes
        prunedScopes.Add(subScopes);
        // remove all my sub-scopes
        foreach(IScope subScope in subScopes) {
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
      if(currentScope.SubScopes.Contains(childScope)) return currentScope;
      foreach(IScope subScope in currentScope.SubScopes) {
        IScope result = FindParentScope(subScope, childScope);
        if(result != null) return result;
      }
      return null;
    }

    private void MergeScope(IScope original, IScope result) {
      // merge the results
      original.Clear();
      foreach(IVariable variable in result.Variables) {
        original.AddVariable(variable);
      }
      foreach(IScope subScope in result.SubScopes) {
        original.AddSubScope(subScope);
      }
      foreach(KeyValuePair<string, string> alias in result.Aliases) {
        original.AddAlias(alias.Key, alias.Value);
      }
    }

    #region Persistence Methods
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute addressAttribute = document.CreateAttribute("ServerAddress");
      addressAttribute.Value = ServerAddress;
      node.Attributes.Append(addressAttribute);
      return node;
    }
    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      ServerAddress = node.Attributes["ServerAddress"].Value;
    }
    #endregion
  }
}