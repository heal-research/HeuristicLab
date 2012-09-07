#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public abstract class DataAnalysisInstanceProvider<TData> : ProblemInstanceProvider<TData>
    where TData : class, IDataAnalysisProblemData {

    // has to be implemented, if CanImportData is true
    public virtual TData ImportData(string path, DataAnalysisImportType type) {
      throw new NotSupportedException();
    }

    protected List<IList> Shuffle(List<IList> values) {
      int count = values.First().Count;
      int[] indices = GetRandomIndices(count);
      List<IList> shuffeledValues = new List<IList>(values.Count);
      for (int col = 0; col < values.Count; col++) {

        if (values[col] is List<double>)
          shuffeledValues.Add(new List<double>());
        else if (values[col] is List<DateTime>)
          shuffeledValues.Add(new List<DateTime>());
        else if (values[col] is List<string>)
          shuffeledValues.Add(new List<string>());
        else
          throw new InvalidOperationException();

        for (int i = 0; i < count; i++) {
          shuffeledValues[col].Add(values[col][indices[i]]);
        }
      }
      return shuffeledValues;
    }

    //Fisher–Yates shuffle
    private int[] GetRandomIndices(int amount) {
      int[] randomIndices = Enumerable.Range(0, amount).ToArray();
      System.Random rand = new System.Random();
      int n = amount;
      while (n > 1) {
        n--;
        int k = rand.Next(n + 1);
        int value = randomIndices[k];
        randomIndices[k] = randomIndices[n];
        randomIndices[n] = value;
      }
      return randomIndices;
    }
  }
}
