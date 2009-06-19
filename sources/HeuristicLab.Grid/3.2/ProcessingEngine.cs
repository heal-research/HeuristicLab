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
using System.Linq;
using System.Text;
using HeuristicLab.Core;
using System.Xml;
using System.Threading;

namespace HeuristicLab.Grid {
  public class ProcessingEngine : EngineBase {
    private AtomicOperation initialOperation;
    public AtomicOperation InitialOperation {
      get { return initialOperation; }
      set { initialOperation = value; }
    }

    private string errorMessage;
    public string ErrorMessage {
      get { return errorMessage; }
    }

    private bool suspended;
    public bool Suspended {
      get { return suspended; }
    }

    public ProcessingEngine()
      : base() {
    }

    public ProcessingEngine(IScope globalScope, AtomicOperation initialOperation)
      : base() {
      this.initialOperation = initialOperation;
      myGlobalScope = globalScope;
      myExecutionStack.Push(initialOperation);
    }

    private void Suspend() {
      Abort();
      suspended = true;
    }

    public override void Execute() {
      suspended = false;
      base.Execute();
    }

    public override void ExecuteSteps(int steps) {
      suspended = false;
      base.ExecuteSteps(steps);
    }

    protected override void ProcessNextOperation() {
      IOperation operation = myExecutionStack.Pop();
      if(operation is AtomicOperation) {
        AtomicOperation atomicOperation = (AtomicOperation)operation;
        IOperation next = null;
        try {
          next = atomicOperation.Operator.Execute(atomicOperation.Scope);
        } catch(Exception ex) {
          errorMessage = CreateErrorMessage(ex);
          HeuristicLab.Tracing.HiveLogger.Error(errorMessage);
          // push operation on stack again
          myExecutionStack.Push(atomicOperation);
          Abort();
        }
        if(next != null)
          myExecutionStack.Push(next);
        if(atomicOperation.Operator.Breakpoint) Suspend();
      } else if(operation is CompositeOperation) {
        CompositeOperation compositeOperation = (CompositeOperation)operation;
        for(int i = compositeOperation.Operations.Count - 1; i >= 0; i--)
          myExecutionStack.Push(compositeOperation.Operations[i]);
      }
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute canceledAttr = document.CreateAttribute("Canceled");
      canceledAttr.Value = Canceled.ToString();
      node.Attributes.Append(canceledAttr);
      XmlAttribute suspendedAttr = document.CreateAttribute("Suspended");
      suspendedAttr.Value = Suspended.ToString();
      node.Attributes.Append(suspendedAttr);
      if(errorMessage != null) {
        XmlAttribute errorMessageAttr = document.CreateAttribute("ErrorMessage");
        errorMessageAttr.Value = ErrorMessage;
        node.Attributes.Append(errorMessageAttr);
      }
      node.AppendChild(PersistenceManager.Persist("InitialOperation", initialOperation, document, persistedObjects));
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myCanceled = bool.Parse(node.Attributes["Canceled"].Value);
      suspended = bool.Parse(node.Attributes["Suspended"].Value);
      if(node.Attributes["ErrorMessage"] != null) errorMessage = node.Attributes["ErrorMessage"].Value;
      initialOperation = (AtomicOperation)PersistenceManager.Restore(node.SelectSingleNode("InitialOperation"), restoredObjects);
    }
    #endregion

    private string CreateErrorMessage(Exception ex) {
      StringBuilder sb = new StringBuilder();
      sb.Append("Sorry, but something went wrong!\n\n" + ex.Message + "\n\n" + ex.StackTrace);

      while(ex.InnerException != null) {
        ex = ex.InnerException;
        sb.Append("\n\n-----\n\n" + ex.Message + "\n\n" + ex.StackTrace);
      }
      return sb.ToString();
    }
  }
}
