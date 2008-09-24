using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.SimOpt {
  public class NormalDoubleManipulator : SimOptManipulationOperatorBase {
    public override string Description {
      get { return @"Perturbs a DoubleData or ConstrainedDoubleData by a value normal distributed around 0"; }
    }
    /*private NormalDistributedRandom myRandom;
    public NormalDistributedRandom Random {
      get { return myRandom; }
      set { myRandom = value; }
    }

    private double myShakingFactor;
    public double ShakingFactor {
      get { return myShakingFactor; }
      set { myShakingFactor = value; }
    }*/

    public NormalDoubleManipulator()
      : base() {
      AddVariableInfo(new VariableInfo("ShakingFactor", "", typeof(DoubleData), VariableKind.In));
      /*myRandom = new NormalDistributedRandom(new MersenneTwister(), 0.0, 1.0);
      myShakingFactor = 1.0;*/
    }

    /*public override IView CreateView() {
      return new NormalDoubleManipulatorView(this);
    }*/

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      double shakingFactor = GetVariableValue<DoubleData>("ShakingFactor", scope, true).Data;
      NormalDistributedRandom normal = new NormalDistributedRandom(random, 0.0, shakingFactor);
      if (item is DoubleData) {
        ((DoubleData)item).Data += normal.NextDouble();
        return;
      } else if (item is ConstrainedDoubleData) {
        ConstrainedDoubleData data = (item as ConstrainedDoubleData);
        for (int tries = 100; tries >= 0; tries--) {
          double newValue = data.Data + normal.NextDouble();

          if (IsIntegerConstrained(data)) {
            newValue = Math.Round(newValue);
          }
          if (data.TrySetData(newValue)) {
            return;
          }
        }
        throw new InvalidProgramException("Coudn't find a valid value in 100 tries");
      } else throw new InvalidOperationException("ERROR: NormalDoubleManipulator does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }

    /*public override void Visit(DoubleData data) {
      data.Data += (myRandom.NextDouble() dat* myShakingFactor);
    }

    public override void Visit(ConstrainedDoubleData data) {
      for (int tries = 100; tries >= 0; tries--) {
        double newValue = data.Data + myRandom.NextDouble() * myShakingFactor;

        if (IsIntegerConstrained(data)) {
          newValue = Math.Round(newValue);
        }
        if (data.TrySetData(newValue)) {
          return;
        }
      }

      throw new InvalidProgramException("Coudn't find a valid value in 100 tries");
    }

    #region clone & persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      NormalDoubleManipulator clone = new NormalDoubleManipulator();
      clonedObjects.Add(Guid, clone);
      clone.Random = (NormalDistributedRandom)myRandom.Clone(clonedObjects);
      clone.ShakingFactor = myShakingFactor;
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode randomNode = PersistenceManager.Persist("Random", Random, document, persistedObjects);
      node.AppendChild(randomNode);

      XmlNode shakingFactorNode = document.CreateNode(XmlNodeType.Element, "ShakingFactor", null);
      shakingFactorNode.InnerText = myShakingFactor.ToString(CultureInfo.InvariantCulture);
      node.AppendChild(shakingFactorNode);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myRandom = (NormalDistributedRandom)PersistenceManager.Restore(node.SelectSingleNode("Random"), restoredObjects);
      myShakingFactor = double.Parse(node.SelectSingleNode("ShakingFactor").InnerText, CultureInfo.InvariantCulture);
    }
    #endregion*/
  }
}
