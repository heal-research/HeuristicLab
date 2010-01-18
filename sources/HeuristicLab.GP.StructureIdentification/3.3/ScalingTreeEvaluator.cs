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
using System.Diagnostics;
using HeuristicLab.Common;
using HeuristicLab.DataAnalysis;
using HeuristicLab.GP.Interfaces;
using System.Collections.Generic; // double.IsAlmost extension
using System.Linq;
using System.Xml;
namespace HeuristicLab.GP.StructureIdentification {
  /// <summary>
  /// Evaluates FunctionTrees recursively by interpretation of the function symbols in each node.
  /// Scales the output of the function-tree to the desired output range of the target variable by linear transformation
  /// Not thread-safe!
  /// </summary>
  public class ScalingTreeEvaluator : HL3TreeEvaluator, ITreeEvaluator {
    public ScalingTreeEvaluator() : base() { } // for persistence
    public ScalingTreeEvaluator(double minValue, double maxValue) : base(minValue, maxValue) { }

    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
      set { targetVariable = value; }
    }

    public override IEnumerable<double> Evaluate(Dataset dataset, IFunctionTree tree, IEnumerable<int> rows) {
      double[] result = base.Evaluate(dataset, tree, rows).ToArray();
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      double tMean = dataset.GetMean(targetVariableIndex);
      double xMean = Statistics.Mean(result);
      double sumXT = 0;
      double sumXX = 0;
      for (int i = 0; i < result.Length; i++) {
        double x = result[i];
        double t = dataset.GetValue(rows.ElementAt(i), targetVariableIndex);
        sumXT += (x - xMean) * (t - tMean);
        sumXX += (x - xMean) * (x - xMean);
      }
      double b = sumXT / sumXX;
      double a = tMean - b * xMean;
      for (int i = 0; i < result.Length; i++) {
        double scaledResult = result[i] * b + a;
        scaledResult = Math.Min(Math.Max(scaledResult, LowerEvaluationLimit), UpperEvaluationLimit);
        if (double.IsNaN(scaledResult)) scaledResult = UpperEvaluationLimit;
        result[i] = scaledResult;
      }
      return result;
    }

    public override object Clone(IDictionary<Guid, object> clonedObjects) {
      ScalingTreeEvaluator clone = (ScalingTreeEvaluator)base.Clone(clonedObjects);
      clone.targetVariable = targetVariable;
      return clone;
    }

    public override System.Xml.XmlNode GetXmlNode(string name, System.Xml.XmlDocument document, IDictionary<Guid, HeuristicLab.Core.IStorable> persistedObjects) {
      XmlNode node = base.GetXmlNode(name, document, persistedObjects);
      XmlAttribute targetVariableAttribute = document.CreateAttribute("TargetVariable");
      targetVariableAttribute.Value = targetVariable;
      node.Attributes.Append(targetVariableAttribute);
      return node;
    }

    public override void Populate(XmlNode node, IDictionary<Guid, HeuristicLab.Core.IStorable> restoredObjects) {
      base.Populate(node, restoredObjects);
      targetVariable = node.Attributes["TargetVariable"].Value;
    }
  }
}
