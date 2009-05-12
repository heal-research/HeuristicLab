#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2009 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.StatisticalAnalysis {
  public class SimpleStatisticsCalculator : OperatorBase {

    public override string Description {
      get { return @"Takes a DoubleArrayData, IntArrayData, ItemList (containing IntData or DoubleData), ItemList<IntData>, or ItemList<DoubleData> and calculates mean, median, standard deviation, sum, minimum, and maximum."; }
    }

    public SimpleStatisticsCalculator()
      : base() {
      AddVariableInfo(new VariableInfo("Samples", "The array or ItemList containing the samples", typeof(IItem), VariableKind.In));
      AddVariableInfo(new VariableInfo("Mean", "The mean of the samples", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Median", "The median of the samples", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("StdDev", "The standard deviation of the samples", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Sum", "The sum of the samples", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Minimum", "The smallest of the samples", typeof(DoubleData), VariableKind.New | VariableKind.Out));
      AddVariableInfo(new VariableInfo("Maximum", "The largest of the samples", typeof(DoubleData), VariableKind.New | VariableKind.Out));
    }

    public override IOperation Apply(IScope scope) {
      IItem data = GetVariableValue<IItem>("Samples", scope, false);
      double[] samples; // put the samples into a double array
      #region fill samples with data
      // find out which type it actually is
      Type t = data.GetType();
      if (t.Equals(typeof(DoubleArrayData))) {
        DoubleArrayData dAD = (DoubleArrayData)data;
        samples = new double[dAD.Data.Length];
        for (int i = 0; i < samples.Length; i++)
          samples[i] = dAD.Data[i];
      } else if (t.Equals(typeof(IntArrayData))) {
        IntArrayData iAD = (IntArrayData)data;
        samples = new double[iAD.Data.Length];
        for (int i = 0; i < samples.Length; i++)
          samples[i] = (double)iAD.Data[i];
      } else if (t.Equals(typeof(ItemList<DoubleData>))) {
        ItemList<DoubleData> iLDD = (ItemList<DoubleData>)data;
        samples = new double[iLDD.Count];
        for (int i = 0; i < samples.Length; i++)
          samples[i] = iLDD[i].Data;
      } else if (t.Equals(typeof(ItemList<IntData>))) {
        ItemList<IntData> iLID = (ItemList<IntData>)data;
        samples = new double[iLID.Count];
        for (int i = 0; i < samples.Length; i++)
          samples[i] = iLID[i].Data;
      } else if (t.Equals(typeof(ItemList))) {
        ItemList iL = (ItemList)data;
        samples = new double[iL.Count];
        for (int i = 0; i < samples.Length; i++) {
          if (iL[i] is DoubleData) samples[i] = ((DoubleData)(iL[i])).Data;
          else if (iL[i] is IntData) samples[i] = (double)((IntData)(iL[i])).Data;
          else throw new ArgumentException("ERROR in SimpleStatisticsCalculator: The ItemList does not contain DoubleData or IntData");
        }
      } else throw new ArgumentException("ERROR in SimpleStatisticsCalculator: Samples are not in a recognized data format");
      #endregion

      int len = samples.Length;
      if (len < 1) throw new ArgumentException("ERROR in SimpleStatisticsCalculator: Sample size is less than 1");

      Array.Sort<double>(samples);
      double mean = 0.0;
      double median = ((len % 2 == 0) ? ((samples[len / 2 - 1] + samples[len / 2]) / 2.0) : (samples[len / 2]));
      double stdDev = 0.0;
      double sum = 0.0;
      double min = samples[0];
      double max = samples[samples.Length - 1];
      for (int i = 0; i < samples.Length; i++) {
        sum += samples[i];
      }
      mean = sum / (double)len;
      if (len > 1) {
        for (int i = 0; i < samples.Length; i++) {
          stdDev = Math.Pow(mean - samples[i], 2);
        }
        stdDev = Math.Sqrt(stdDev / (double)(len - 1));
      }

      #region output variables
      WriteVariable(GetVariableInfo("Mean"), mean, scope);
      WriteVariable(GetVariableInfo("Median"), median, scope);
      WriteVariable(GetVariableInfo("StdDev"), stdDev, scope);
      WriteVariable(GetVariableInfo("Sum"), sum, scope);
      WriteVariable(GetVariableInfo("Minimum"), min, scope);
      WriteVariable(GetVariableInfo("Maximum"), max, scope);
      #endregion
      return null;
    }

    private void WriteVariable(IVariableInfo info, double value, IScope scope) {
      if (info.Local) {
        IVariable var = GetVariable(info.ActualName);
        if (var == null) AddVariable(new Variable(info.ActualName, new DoubleData(value)));
        else (var.Value as DoubleData).Data = value;
      } else {
        string name = scope.TranslateName(info.FormalName);
        IVariable var = scope.GetVariable(name);
        if (var == null) scope.AddVariable(new Variable(name, new DoubleData(value)));
        else (var.Value as DoubleData).Data = value;
      }
    }
  }
}
