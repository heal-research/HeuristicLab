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
using HeuristicLab.DataAnalysis;


namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class ROCAnalyser : OperatorBase {

    public override string Description {
      get { return @"Calculate TPR & FPR for various treshholds on dataset"; }
    }

    public ROCAnalyser()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Item list holding the estimated and orignial values for the ROCAnalyser", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("ROCValues", "The values of the ROCAnalyzer, namely TPR & FPR", typeof(ItemList), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);
      ItemList rocValues = GetVariableValue<ItemList>("ROCValues", scope, false, false);
      if (rocValues == null) {
        rocValues = new ItemList();
        IVariableInfo info = GetVariableInfo("ROCValues");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, rocValues));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), rocValues));
      } else
        rocValues.Clear();

      //ROC Curve starts at 0,0
      ItemList row = new ItemList();
      row.Add(new DoubleData(0));
      row.Add(new DoubleData(0));
      rocValues.Add(row);

      //calculate new ROC Values
      double estimated;
      double original;
      double positiveClassKey;
      double negativeClassKey;
      double truePositiveRate;
      double falsePositiveRate;

      //initialize classes dictionary
      Dictionary<double, List<double>> classes = new Dictionary<double, List<double>>();
      foreach (ItemList value in values) {
        estimated = ((DoubleData)value[0]).Data;
        original = ((DoubleData)value[1]).Data;
        if (!classes.ContainsKey(original))
          classes[original] = new List<double>();
        classes[original].Add(estimated);
      }

      //check for 2 classes classification problem
      if (classes.Keys.Count != 2)
        throw new Exception("ROCAnalyser only handles  2 class classification problems");

      //sort estimated values in classes dictionary
      foreach (List<double> estimatedValues in classes.Values)
        estimatedValues.Sort();

      //calculate truePosivite- & falsePositiveRate
      positiveClassKey = classes.Keys.Min<double>();
      negativeClassKey = classes.Keys.Max<double>();
      for (int i = 0; i < classes[negativeClassKey].Count; i++) {
        truePositiveRate = classes[positiveClassKey].Count<double>(value => value < classes[negativeClassKey][i]) / classes[positiveClassKey].Count;
        //stop calculation if truePositiveRate = 1; save runtime
        if (truePositiveRate == 1) 
          break;
        falsePositiveRate = (i) / classes[negativeClassKey].Count;
        row = new ItemList();
        row.Add(new DoubleData(falsePositiveRate));
        row.Add(new DoubleData(truePositiveRate));
        rocValues.Add(row);
      }

      //ROC ends at 1,1
      row = new ItemList();
      row.Add(new DoubleData(1));
      row.Add(new DoubleData(1));
      rocValues.Add(row);

      return null;
    }
  }
}
