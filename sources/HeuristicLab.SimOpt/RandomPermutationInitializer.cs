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
using HeuristicLab.Data;
using HeuristicLab.Random;

namespace HeuristicLab.SimOpt {
  public class RandomPermutationInitializer : SimOptInitializationOperatorBase {
    public override string Description {
      get { return @"Randomly initialize a permutation with values ranging from 0 to n-1"; }
    }

    /*private MersenneTwister myRandom;
    public MersenneTwister Random {
      get { return myRandom; }
      set { myRandom = value; }
    }*/

    public RandomPermutationInitializer()
      : base() {
      //myRandom = new MersenneTwister();
    }

    protected override void Apply(IScope scope, IRandom random, IItem item) {
      if (item is Permutation.Permutation || item is IntArrayData) {
        IntArrayData data = (item as IntArrayData);
        IList<int> number = new List<int>(data.Data.Length);
        for (int i = 0; i < data.Data.Length; i++)
          number.Add(i);
        for (int i = 0; i < data.Data.Length; i++) {
          int index = random.Next(number.Count);
          data.Data[i] = number[index];
          number.RemoveAt(index);
        }
      } else throw new InvalidOperationException("ERROR: RandomPermutationInitializer does not know how to work with " + ((item != null) ? (item.GetType().ToString()) : ("null")) + " data");
    }

    /*public override IView CreateView() {
      return new RandomPermutationInitializerView(this);
    }

    public override void Visit(IntArrayData data) {
      IList<int> number = new List<int>(data.Data.Length);
      for (int i = 0; i < data.Data.Length; i++)
        number.Add(i);
      for (int i = 0; i < data.Data.Length; i++) {
        int index = random.Next(number.Count);
        data.Data[i] = number[index];
        number.RemoveAt(index);
      }
    }

    public override void Visit(ObjectData objectData) {
      if (objectData is Permutation) {
        Visit(objectData as IntArrayData);
      } else throw new NotImplementedException();
    }

    #region clone & persistence
    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      RandomPermutationInitializer clone = new RandomPermutationInitializer();
      clonedObjects.Add(Guid, clone);
      clone.Random = (MersenneTwister)myRandom.Clone(clonedObjects);
      return clone;
    }

    public override XmlNode GetXmlNode(string name, XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlNode randomNode = PersistenceManager.Persist("Random", Random, document, persistedObjects);
      node.AppendChild(randomNode);

      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      myRandom = (MersenneTwister)PersistenceManager.Restore(node.SelectSingleNode("Random"), restoredObjects);
    }
    #endregion*/
  }
}
