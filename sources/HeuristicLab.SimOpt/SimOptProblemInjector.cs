using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.SimOpt {
  public class SimOptProblemInjector : OperatorBase {

    public override string Description {
      get { return @"Injects the parameters used for simulation parameter optimization"; }
    }

    private BoolData myMaximization;
    public BoolData Maximization {
      get { return myMaximization; }
      set { myMaximization = value; }
    }

    private StringData myGeneName;
    public StringData GeneName {
      get { return myGeneName; }
      set { myGeneName = value; }
    }

    private ConstrainedItemList myParameters;
    public ConstrainedItemList Parameters {
      get { return myParameters; }
      set { myParameters = value; }
    }

    public SimOptProblemInjector()
      : base() {
      myParameters = new ConstrainedItemList();
      myGeneName = new StringData("Items");
      myMaximization = new BoolData(false);
    }

    public override IOperation Apply(IScope scope) {
      scope.AddVariable(new Variable("Maximization", (BoolData)Maximization.Clone()));
      scope.AddVariable(new Variable(myGeneName.Data, (ConstrainedItemList)Parameters.Clone()));
      return null;
    }

    public override IView CreateView() {
      return new SimOptProblemInjectorView(this);
    }

    #region Clone & Persistence Methods
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      SimOptProblemInjector clone = new SimOptProblemInjector();
      clonedObjects.Add(Guid, clone);
      clone.Maximization = (BoolData)myMaximization.Clone(clonedObjects);
      clone.GeneName = (StringData)myGeneName.Clone(clonedObjects);
      clone.myParameters = (ConstrainedItemList)myParameters.Clone(clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid,IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      // variable infos should not be persisted
      XmlNode infosNode = node.SelectSingleNode("VariableInfos");
      infosNode.RemoveAll();
      XmlNode maxNode = PersistenceManager.Persist("Maximization", Maximization, document, persistedObjects);
      node.AppendChild(maxNode);
      XmlNode nameNode = PersistenceManager.Persist("GeneName", GeneName, document, persistedObjects);
      node.AppendChild(nameNode);
      XmlNode parametersNode = PersistenceManager.Persist("Items", Parameters, document, persistedObjects);
      node.AppendChild(parametersNode);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      if (node.SelectSingleNode("Maximization") != null) {
        myMaximization = (BoolData)PersistenceManager.Restore(node.SelectSingleNode("Maximization"), restoredObjects);
      }
      if (node.SelectSingleNode("GeneName") != null) {
        myGeneName = (StringData)PersistenceManager.Restore(node.SelectSingleNode("GeneName"), restoredObjects);
      }
      IStorable items = (ConstrainedItemList)PersistenceManager.Restore(node.SelectSingleNode("Items"), restoredObjects);
      try {
        myParameters = (ConstrainedItemList)items;
      } catch (InvalidCastException ice) {
        IVariable var = (Variable)items;
        myParameters = (ConstrainedItemList)var.Value;
      }
    }
    #endregion
  }
}