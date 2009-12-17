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
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Core;
using System.Xml;
using System.Linq;
using System.Globalization;

namespace HeuristicLab.ArtificialNeuralNetworks {

  public class MultiLayerPerceptron : ItemBase {
    private alglib.mlpbase.multilayerperceptron perceptron;
    public alglib.mlpbase.multilayerperceptron Perceptron {
      get { return perceptron; }
      internal set { perceptron = value; }
    }

    private List<string> inputVariables;
    public IEnumerable<string> InputVariables {
      get {
        return inputVariables;
      }
    }
    
    private int minTimeOffset;
    public int MinTimeOffset {
      get { return minTimeOffset; }
    }
    private int maxTimeOffset;
    public int MaxTimeOffset {
      get { return maxTimeOffset; }
    }

    public MultiLayerPerceptron() : base() { } // for persistence;

    public MultiLayerPerceptron(alglib.mlpbase.multilayerperceptron perceptron, IEnumerable<string> inputVariables, 
      int minTimeOffset, int maxTimeOffset)
      : base() {
      this.perceptron = perceptron;
      this.minTimeOffset = minTimeOffset;
      this.maxTimeOffset = maxTimeOffset;
      this.inputVariables = new List<string>(inputVariables);
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      MultiLayerPerceptron clone = (MultiLayerPerceptron)base.Clone(clonedObjects);

      clone.inputVariables = new List<string>(inputVariables);

      double[] ra = null;
      int rlen = 0;
      alglib.mlpbase.mlpserialize(ref perceptron, ref ra, ref rlen); // output: ra, rlen
      alglib.mlpbase.mlpunserialize(ref ra, ref clone.perceptron); // output clone.perceptron
      return clone;
    }

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);

      XmlNode networkInformation = document.CreateElement("NetworkInformation");
      double[] ra = null;
      int rlen = 0;
      alglib.mlpbase.mlpserialize(ref perceptron, ref ra, ref rlen);
      networkInformation.InnerText = String.Join(";", ra.Select(x => x.ToString("r", CultureInfo.InvariantCulture)).ToArray()); // culture invariant & round trip
      node.AppendChild(networkInformation);

      XmlNode inputVariablesNode = document.CreateElement("InputVariables");
      inputVariablesNode.InnerText = String.Join(";", inputVariables.ToArray());
      node.AppendChild(inputVariablesNode);
      return node;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      double[] ra = (from s in node.SelectSingleNode("NetworkInformation").InnerText.Split(';')
                     select double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
      alglib.mlpbase.mlpunserialize(ref ra, ref perceptron);

      inputVariables = new List<string>(node.SelectSingleNode("InputVariables").InnerText.Split(';'));
    }
  }
}