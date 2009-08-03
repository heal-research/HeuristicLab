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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;


namespace HeuristicLab.GP.StructureIdentification.Classification {
  public class ROCAnalyzer : OperatorBase {
    private ItemList myRocValues;
    private ItemList<DoubleData> myAucValues;


    public override string Description {
      get { return @"Calculate TPR & FPR for various thresholds on dataset"; }
    }

    public ROCAnalyzer()
      : base() {
      AddVariableInfo(new VariableInfo("Values", "Item list holding the estimated and original values for the ROCAnalyzer", typeof(ItemList), VariableKind.In));
      AddVariableInfo(new VariableInfo("ROCValues", "The values of the ROCAnalyzer, namely TPR & FPR", typeof(ItemList), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("AUCValues", "The AUC Values for each ROC", typeof(ItemList<DoubleData>), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      #region initialize HL-variables
      ItemList values = GetVariableValue<ItemList>("Values", scope, true);
      myRocValues = GetVariableValue<ItemList>("ROCValues", scope, false, false);
      if (myRocValues == null) {
        myRocValues = new ItemList();
        IVariableInfo info = GetVariableInfo("ROCValues");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, myRocValues));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), myRocValues));
      } else {
        myRocValues.Clear();
      }

      myAucValues = GetVariableValue<ItemList<DoubleData>>("AUCValues", scope, false, false);
      if (myAucValues == null) {
        myAucValues = new ItemList<DoubleData>();
        IVariableInfo info = GetVariableInfo("AUCValues");
        if (info.Local)
          AddVariable(new HeuristicLab.Core.Variable(info.ActualName, myAucValues));
        else
          scope.AddVariable(new HeuristicLab.Core.Variable(scope.TranslateName(info.FormalName), myAucValues));
      } else {
        myAucValues.Clear();
      }
      #endregion

      //calculate new ROC Values
      double estimated = 0.0;
      double original = 0.0;

      //initialize classes dictionary
      SortedDictionary<double, List<double>> classes = new SortedDictionary<double, List<double>>();
      foreach (ItemList value in values) {
        estimated = ((DoubleData)value[0]).Data;
        original = ((DoubleData)value[1]).Data;
        if (!classes.ContainsKey(original))
          classes[original] = new List<double>();
        classes[original].Add(estimated);
      }
      foreach (double key in classes.Keys)
        classes[key].Sort();

      //calculate ROC Curve
      foreach (double key in classes.Keys) {
        CalculateBestROC(key, classes);
      }

      return null;
    }

    protected void CalculateBestROC(double positiveClassKey, SortedDictionary<double, List<double>> classes) {
      List<KeyValuePair<double, double>> rocCharacteristics;
      List<KeyValuePair<double, double>> bestROC;
      List<KeyValuePair<double, double>> actROC;

      List<double> negatives = new List<double>();
      foreach (double key in classes.Keys) {
        if (key != positiveClassKey)
          negatives.AddRange(classes[key]);
      }
      List<double> actNegatives = negatives.Where<double>(value => value < classes[positiveClassKey].Max<double>()).ToList<double>();
      actNegatives.Add(classes[positiveClassKey].Max<double>());
      actNegatives.Sort();
      actNegatives = actNegatives.Reverse<double>().ToList<double>();

      double bestAUC = double.MinValue;
      double actAUC = 0;
      //first class
      if (classes.Keys.ElementAt<double>(0) == positiveClassKey) {
        rocCharacteristics = null;
        CalculateROCValuesAndAUC(classes[positiveClassKey], actNegatives, negatives.Count, double.MinValue, ref rocCharacteristics, out  actROC, out actAUC);
        myAucValues.Add(new DoubleData(actAUC));
        myRocValues.Add(Convert(actROC));
      }
        //middle classes  
      else if (classes.Keys.ElementAt<double>(classes.Keys.Count - 1) != positiveClassKey) {
        rocCharacteristics = null;
        bestROC = new List<KeyValuePair<double, double>>();
        foreach (double minThreshold in classes[positiveClassKey].Distinct<double>()) {
          CalculateROCValuesAndAUC(classes[positiveClassKey], actNegatives, negatives.Count, minThreshold, ref rocCharacteristics, out  actROC, out actAUC);
          if (actAUC > bestAUC) {
            bestAUC = actAUC;
            bestROC = actROC;
          }
        }
        myAucValues.Add(new DoubleData(bestAUC));
        myRocValues.Add(Convert(bestROC));

      } else { //last class
        actNegatives = negatives.Where<double>(value => value > classes[positiveClassKey].Min<double>()).ToList<double>();
        actNegatives.Add(classes[positiveClassKey].Min<double>());
        actNegatives.Sort();
        CalculateROCValuesAndAUCForLastClass(classes[positiveClassKey], actNegatives, negatives.Count, out bestROC, out bestAUC);
        myAucValues.Add(new DoubleData(bestAUC));
        myRocValues.Add(Convert(bestROC));

      }

    }

    protected void CalculateROCValuesAndAUC(List<double> positives, List<double> negatives, int negativesCount, double minThreshold,
      ref List<KeyValuePair<double, double>> rocCharacteristics, out List<KeyValuePair<double, double>> roc, out double auc) {
      double actTP = -1;
      double actFP = -1;
      double oldTP = -1;
      double oldFP = -1;
      auc = 0;
      roc = new List<KeyValuePair<double, double>>();

      actTP = positives.Count<double>(value => minThreshold <= value && value <= negatives.Max<double>());
      actFP = negatives.Count<double>(value => minThreshold <= value);
      //add point (1,TPR) for AUC 'correct' calculation
      roc.Add(new KeyValuePair<double, double>(1, actTP / positives.Count));
      oldTP = actTP;
      oldFP = negativesCount;
      roc.Add(new KeyValuePair<double, double>(actFP / negativesCount, actTP / positives.Count));

      if (rocCharacteristics == null) {
        rocCharacteristics = new List<KeyValuePair<double, double>>();
        foreach (double maxThreshold in negatives.Distinct<double>()) {
          auc += ((oldTP + actTP) / positives.Count) * ((oldFP - actFP) / negativesCount) / 2;
          oldTP = actTP;
          oldFP = actFP;
          actTP = positives.Count<double>(value => minThreshold <= value && value < maxThreshold);
          actFP = negatives.Count<double>(value => minThreshold <= value && value < maxThreshold);
          rocCharacteristics.Add(new KeyValuePair<double, double>(oldTP - actTP, oldFP - actFP));
          roc.Add(new KeyValuePair<double, double>(actFP / negativesCount, actTP / positives.Count));

          //stop calculation if truePositiveRate == 0 => straight line with y=0 & save runtime
          if ((actTP == 0) || (actFP == 0))
            break;
        }
        auc += ((oldTP + actTP) / positives.Count) * ((oldFP - actFP) / negativesCount) / 2;
      } else { //characteristics of ROCs calculated 
        foreach (KeyValuePair<double, double> rocCharac in rocCharacteristics) {
          auc += ((oldTP + actTP) / positives.Count) * ((oldFP - actFP) / negativesCount) / 2;
          oldTP = actTP;
          oldFP = actFP;
          actTP = oldTP - rocCharac.Key;
          actFP = oldFP - rocCharac.Value;
          roc.Add(new KeyValuePair<double, double>(actFP / negativesCount, actTP / positives.Count));
          if ((actTP == 0) || (actFP == 0))
            break;
        }
        auc += ((oldTP + actTP) / positives.Count) * ((oldFP - actFP) / negativesCount) / 2;
      }
    }

    protected void CalculateROCValuesAndAUCForLastClass(List<double> positives, List<double> negatives, int negativesCount,
      out List<KeyValuePair<double, double>> roc, out double auc) {
      double actTP = -1;
      double actFP = -1;
      double oldTP = -1;
      double oldFP = -1;
      auc = 0;
      roc = new List<KeyValuePair<double, double>>();

      actTP = positives.Count<double>(value => value >= negatives.Min<double>());
      actFP = negatives.Count<double>(value => value >= negatives.Min<double>());
      //add point (1,TPR) for AUC 'correct' calculation
      roc.Add(new KeyValuePair<double, double>(1, actTP / positives.Count));
      oldTP = actTP;
      oldFP = negativesCount;
      roc.Add(new KeyValuePair<double, double>(actFP / negativesCount, actTP / positives.Count));

      foreach (double minThreshold in negatives.Distinct<double>()) {
        auc += ((oldTP + actTP) / positives.Count) * ((oldFP - actFP) / negativesCount) / 2;
        oldTP = actTP;
        oldFP = actFP;
        actTP = positives.Count<double>(value => minThreshold < value);
        actFP = negatives.Count<double>(value => minThreshold < value);
        roc.Add(new KeyValuePair<double, double>(actFP / negativesCount, actTP / positives.Count));

        //stop calculation if truePositiveRate == 0 => straight line with y=0 & save runtime
        if (actTP == 0 || actFP == 0)
          break;
      }
      auc += ((oldTP + actTP) / positives.Count) * ((oldFP - actFP) / negativesCount) / 2;

    }

    private ItemList Convert(List<KeyValuePair<double, double>> data) {
      ItemList list = new ItemList();
      ItemList row;
      foreach (KeyValuePair<double, double> dataPoint in data) {
        row = new ItemList();
        row.Add(new DoubleData(dataPoint.Key));
        row.Add(new DoubleData(dataPoint.Value));
        list.Add(row);
      }
      return list;
    }

  }

}
