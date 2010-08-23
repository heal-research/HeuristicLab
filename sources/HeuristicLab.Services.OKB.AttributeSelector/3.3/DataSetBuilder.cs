#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using HeuristicLab.Services.OKB.DataAccess;


namespace HeuristicLab.Services.OKB.AttributeSelection {
  public class DataSetBuilder : IEnumerable<DataRow> {

    public static DataSet GetDataSet(OKBDataContext okb, IEnumerable<RunAttributeSelector> selectors) {
      DataSetBuilder builder = new DataSetBuilder(okb, selectors);
      DataTable runs = new DataTable("Runs");
      foreach (DataRow row in builder) {
        runs.Rows.Add(row);
      }
      DataSet dataSet = new DataSet();
      dataSet.Tables.Add(runs);
      return dataSet;
    }

    private IQueryable<Run> query;
    public DataTable Runs { get; private set; }
    private IEnumerable<RunAttributeSelector> selectors;

    public DataSetBuilder(OKBDataContext okb, IEnumerable<RunAttributeSelector> selectors) {
      DataLoadOptions dlo = new DataLoadOptions();
      query = okb.Runs.AsQueryable();
      Runs = new DataTable("Runs");
      this.selectors = selectors;

      foreach (var selector in selectors) {
        query = query.Where(selector.GetWhereExpression());
        selector.ConfigureDataLoadOptions(dlo);
        if (!selector.IsHidden) {
          AddColumn(selector);
        }
      }

      okb.LoadOptions = dlo;
      Console.WriteLine(query);
    }

    private void AddColumn(RunAttributeSelector selector) {
      Type type = selector.DataType;
      if (type == typeof(object))
        type = typeof(string);
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        type = type.GetGenericArguments()[0];
      int i = 0;
      string columnName = selector.FullName;
      while (Runs.Columns.Contains(columnName)) {
        i++;
        columnName = string.Format("{0}({1})", selector.FullName, i);
      }
      Runs.Columns.Add(columnName, type);
    }

    public IEnumerator<DataRow> GetEnumerator() {
      foreach (var run in query) {
        DataRow row = Runs.NewRow();
        int columnNr = 0;
        foreach (var selector in selectors.Where(s => !s.IsHidden)) {
          object value = selector.CompiledSelector(run);
          row[columnNr] = selector.CompiledSelector(run) ?? DBNull.Value;
          columnNr++;
        }
        yield return row;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }


    public int GetCount() {
      return query.Count();
    }

    public static IEnumerable<object> GetEntities(OKBDataContext okb, Type tableType, IEnumerable<int> ids) {
      ITable table = okb.GetTable(tableType);

      // idFilter = element => ids.Contains(element.Id)
      ParameterExpression param = Expression.Parameter(table.ElementType, "element");
      LambdaExpression idFilter = Expression.Lambda(
        Expression.Call(
          null,
          ExpressionTools.Enumerable_Contains.MakeGenericMethod(typeof(int)),
          Expression.Constant(ids),
          Expression.Property(param, "Id")), param);

      // idSelector = element => element.Id
      ParameterExpression param2 = Expression.Parameter(table.ElementType, "element");
      LambdaExpression idSelector = Expression.Lambda(
        Expression.Property(param2, "Id"), param2);

      // selector = () => okb.<tableType>.Where(idFilter).OrderBy(idSelector).Cast<object>()
      LambdaExpression selector =
        Expression.Lambda(
          Expression.Call(
            null,
            ExpressionTools.Enumerable_Cast.MakeGenericMethod(typeof(object)),
            Expression.Call(
              null,
              ExpressionTools.Enumerable_OrderBy.MakeGenericMethod(table.ElementType, typeof(int)),
              Expression.Call(
                null,
                ExpressionTools.Enumerable_Where.MakeGenericMethod(table.ElementType),
                table.Expression,
                idFilter),
             idSelector)));
      return (IEnumerable<object>)selector.Compile().DynamicInvoke();
    }

    public static int CountEntities(OKBDataContext okb, Type tableType) {
      ITable table = okb.GetTable(tableType);
      // okb.<tableType>.Count();
      LambdaExpression counter =
        Expression.Lambda(
          Expression.Call(
            null,
            ExpressionTools.Enumerable_Count.MakeGenericMethod(table.ElementType),
            table.Expression));
      return (int)counter.Compile().DynamicInvoke();
    }
  }
}
