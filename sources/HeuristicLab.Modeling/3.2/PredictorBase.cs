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
using HeuristicLab.Core;
using HeuristicLab.DataAnalysis;
using System.Xml;

namespace HeuristicLab.Modeling {
  public abstract class PredictorBase : ItemBase, IPredictor {
    public double UpperPredictionLimit { get; set; }
    public double LowerPredictionLimit { get; set; }
    public abstract IEnumerable<double> Predict(Dataset dataset, int start, int end);
    public abstract IEnumerable<string> GetInputVariables();

    public PredictorBase() : this(double.MinValue, double.MaxValue) { } // for persistence
    public PredictorBase(double lowerPredictionLimit, double upperPredictionLimit)
      : base() {
      this.UpperPredictionLimit = upperPredictionLimit;
      this.LowerPredictionLimit = lowerPredictionLimit;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      PredictorBase clone = (PredictorBase)base.Clone(clonedObjects);
      clone.UpperPredictionLimit = UpperPredictionLimit;
      clone.LowerPredictionLimit = LowerPredictionLimit;
      return clone;
    }

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute minValue = document.CreateAttribute("LowerPredictionLimit");
      minValue.Value = XmlConvert.ToString(LowerPredictionLimit);
      XmlAttribute maxValue = document.CreateAttribute("UpperPredictionLimit");
      maxValue.Value = XmlConvert.ToString(UpperPredictionLimit);
      node.Attributes.Append(minValue); node.Attributes.Append(maxValue);
      return node;
    }

    public override void Populate(System.Xml.XmlNode node, IDictionary<Guid, IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      LowerPredictionLimit = XmlConvert.ToDouble(node.Attributes["LowerPredictionLimit"].Value);
      UpperPredictionLimit = XmlConvert.ToDouble(node.Attributes["UpperPredictionLimit"].Value);
    }
  } 
}
