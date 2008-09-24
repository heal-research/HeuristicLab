using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators.Programmable;

namespace HeuristicLab.Communication.Data {
  public class StateTransition : ItemBase {
    private ProtocolState sourceState;
    public ProtocolState SourceState {
      get { return sourceState; }
      set {
        sourceState = value;
        OnChanged();
      }
    }
    private ProtocolState targetState;
    public ProtocolState TargetState {
      get { return targetState; }
      set {
        targetState = value;
        OnChanged();
      }
    }
    private ProgrammableOperator transitionCondition;
    public ProgrammableOperator TransitionCondition {
      get { return transitionCondition; }
      set {
        transitionCondition = value;
        OnChanged();
      }
    }

    public StateTransition()
      : base() {
      sourceState = null;
      targetState = null;
      transitionCondition = new ProgrammableOperator();
    }

    public override IView CreateView() {
      return new StateTransitionView(this);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      StateTransition clone = new StateTransition();
      clonedObjects.Add(Guid, clone);
      clone.sourceState = SourceState;
      clone.targetState = TargetState;
      clone.TransitionCondition = (ProgrammableOperator)Auxiliary.Clone(TransitionCondition, clonedObjects);
      return clone;
    }

    #region persistence
    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode sourceStateNode = PersistenceManager.Persist("SourceState", SourceState, document, persistedObjects);
      node.AppendChild(sourceStateNode);
      XmlNode targetStateNode = PersistenceManager.Persist("TargetState", TargetState, document, persistedObjects);
      node.AppendChild(targetStateNode);
      XmlNode transitionConditionNode = PersistenceManager.Persist("TransitionCondition", TransitionCondition, document, persistedObjects);
      node.AppendChild(transitionConditionNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      sourceState = (ProtocolState)PersistenceManager.Restore(node.SelectSingleNode("SourceState"), restoredObjects);
      targetState = (ProtocolState)PersistenceManager.Restore(node.SelectSingleNode("TargetState"), restoredObjects);
      transitionCondition = (ProgrammableOperator)PersistenceManager.Restore(node.SelectSingleNode("TransitionCondition"), restoredObjects);
    }
    #endregion persistence
  }
}
