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
    public ScalingTreeEvaluator(double minValue, double maxValue)
      : base(minValue, maxValue) {
    }

    private string targetVariable;
    public string TargetVariable {
      get { return targetVariable; }
      set { targetVariable = value; }
    }


    public override double Evaluate(int sampleIndex) {
      PC = 0;
      this.sampleIndex = sampleIndex;
      double estimation = EvaluateBakedCode();
      //if (double.IsPositiveInfinity(estimation)) estimation = UpperEvaluationLimit;
      //else if (double.IsNegativeInfinity(estimation)) estimation = LowerEvaluationLimit;
      //else if (double.IsNaN(estimation)) estimation = UpperEvaluationLimit;
      return estimation;
    }

    public override IEnumerable<double> Evaluate(Dataset dataset, IFunctionTree tree, IEnumerable<int> rows) {
      int targetVariableIndex = dataset.GetVariableIndex(targetVariable);
      // evaluate for all rows
      PrepareForEvaluation(dataset, tree);
      var result = (from row in rows
                    let y = Evaluate(row)
                    let y_ = dataset.GetValue(row, targetVariableIndex)
                    select new { Row = row, Estimation = y, Target = y_ }).ToArray();

      // calculate alpha and beta on the subset of rows with valid values 
      var filteredResult = result.Where(x => IsValidValue(x.Target) && IsValidValue(x.Estimation));
      var target = filteredResult.Select(x => x.Target);
      var estimation = filteredResult.Select(x => x.Estimation);
      double a, b;
      if (filteredResult.Count() > 2) {
        double tMean = target.Sum() / target.Count();
        double xMean = estimation.Sum() / estimation.Count();
        double sumXT = 0;
        double sumXX = 0;
        foreach (var r in result) {
          double x = r.Estimation;
          double t = r.Target;
          sumXT += (x - xMean) * (t - tMean);
          sumXX += (x - xMean) * (x - xMean);
        }
        b = sumXT / sumXX;
        a = tMean - b * xMean;
      } else {
        b = 1.0;
        a = 0.0;
      }
      // return scaled results
      return from r in result
             let scaledR = Math.Min(Math.Max(r.Estimation * b + a, LowerEvaluationLimit), UpperEvaluationLimit)
             select double.IsNaN(scaledR) ? UpperEvaluationLimit : scaledR;
    }

    private bool IsValidValue(double d) {
      return !double.IsInfinity(d) && !double.IsNaN(d);
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
