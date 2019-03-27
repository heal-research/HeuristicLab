#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq.Expressions;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.DataAnalysis {
  using ValuesType = Dictionary<string, IList>;

  public static class DatasetUtil {
    /// <summary>
    /// Shuffle all the lists with the same shuffling.
    /// </summary>
    /// <param name="values">The value lists to be shuffled.</param>
    /// <param name="random">The random number generator</param>
    /// <returns>A new list containing shuffled copies of the original value lists.</returns>
    public static List<IList> ShuffleLists(this List<IList> values, IRandom random) {
      int count = values.First().Count;
      int[] indices = Enumerable.Range(0, count).Shuffle(random).ToArray();
      List<IList> shuffled = new List<IList>(values.Count);
      for (int col = 0; col < values.Count; col++) {

        if (values[col] is IList<double>)
          shuffled.Add(new List<double>());
        else if (values[col] is IList<DateTime>)
          shuffled.Add(new List<DateTime>());
        else if (values[col] is IList<string>)
          shuffled.Add(new List<string>());
        else
          throw new InvalidOperationException();

        for (int i = 0; i < count; i++) {
          shuffled[col].Add(values[col][indices[i]]);
        }
      }
      return shuffled;
    }

    private static readonly Action<Dataset, ValuesType> setValues;
    private static readonly Func<Dataset, ValuesType> getValues;
    static DatasetUtil() {
      var dataset = Expression.Parameter(typeof(Dataset));
      var variableValues = Expression.Parameter(typeof(ValuesType));
      var valuesExpression = Expression.Field(dataset, "variableValues");
      var assignExpression = Expression.Assign(valuesExpression, variableValues);

      var variableValuesSetExpression = Expression.Lambda<Action<Dataset, ValuesType>>(assignExpression, dataset, variableValues);
      setValues = variableValuesSetExpression.Compile();

      var variableValuesGetExpression = Expression.Lambda<Func<Dataset, ValuesType>>(valuesExpression, dataset);
      getValues = variableValuesGetExpression.Compile();
    }

    public static void RemoveDuplicateDatasets(IContent content) {
      var variableValuesMapping = new Dictionary<ValuesType, ValuesType>();

      foreach (var problemData in content.GetObjectGraphObjects(excludeStaticMembers: true).OfType<IDataAnalysisProblemData>()) {
        var dataset = problemData.Dataset as Dataset;
        if (dataset == null) continue;

        var originalValues = getValues(dataset);

        ValuesType matchingValues;

        variableValuesMapping.GetEqualValues(originalValues, out matchingValues);

        setValues(dataset, matchingValues);
      }
    }

    public static Dictionary<string, Interval> GetVariableRanges(IDataset dataset, IEnumerable<int> rows = null) {
      Dictionary<string, Interval> variableRanges = new Dictionary<string, Interval>();

      foreach (var variable in dataset.VariableNames) {
        IEnumerable<double> values = null;

        if (rows == null) values = dataset.GetDoubleValues(variable);
        else values = dataset.GetDoubleValues(variable, rows);

        var range = Interval.GetInterval(values);
        variableRanges.Add(variable, range);
      }

      return variableRanges;
    }

    private static bool GetEqualValues(this Dictionary<ValuesType, ValuesType> variableValuesMapping, ValuesType originalValues, out ValuesType matchingValues) {
      if (variableValuesMapping.ContainsKey(originalValues)) {
        matchingValues = variableValuesMapping[originalValues];
        return true;
      }
      matchingValues = variableValuesMapping.FirstOrDefault(kv => kv.Key == kv.Value && EqualVariableValues(originalValues, kv.Key)).Key;
      bool result = true;
      if (matchingValues == null) {
        matchingValues = originalValues;
        result = false;
      }
      variableValuesMapping[originalValues] = matchingValues;
      return result;
    }

    private static bool EqualVariableValues(ValuesType values1, ValuesType values2) {
      //compare variable names for equality
      if (!values1.Keys.SequenceEqual(values2.Keys)) return false;
      foreach (var key in values1.Keys) {
        var v1 = values1[key];
        var v2 = values2[key];
        if (v1.Count != v2.Count) return false;
        for (int i = 0; i < v1.Count; i++) {
          if (!v1[i].Equals(v2[i])) return false;
        }
      }
      return true;
    }
  }
}
