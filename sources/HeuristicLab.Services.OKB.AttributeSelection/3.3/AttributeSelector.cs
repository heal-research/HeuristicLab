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
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.AttributeSelection {

  public class AttributeSelector : IAttributeSelector {

    public string TableName { get; private set; }
    public string FieldName { get; private set; }
    public bool IsHidden { get; set; }
    public object MinValue { get; set; }
    public object MaxValue { get; set; }
    public ICollection<object> AllowedValues { get; set; }

    public Type TableType { get; private set; }
    public bool IsDynamic { get; private set; }
    public Type DataType { get; private set; }
    public int ParentId { get; private set; }

    protected static readonly List<string> staticTables = new List<string>() {
      typeof(AlgorithmClass).Name,
      typeof(Algorithm).Name,
      typeof(ProblemClass).Name,
      typeof(Problem).Name,
      typeof(Experiment).Name,
      typeof(Platform).Name,
      typeof(SolutionRepresentation).Name,
      typeof(Project).Name,
      typeof(Run).Name,
      typeof(Client).Name,
      typeof(User).Name,
    };

    protected static readonly List<string> dynamicTables = new List<string>() {
      typeof(ProblemCharacteristic).Name,
      typeof(Parameter).Name,
      typeof(Result).Name,
    };

    protected AttributeSelector(Type tableType, string fieldName, bool isDynamic, Type dataType, int parentId) {
      TableName = tableType.Name;
      FieldName = fieldName;
      TableType = tableType;
      IsDynamic = isDynamic;
      DataType = dataType;
      ParentId = parentId;
    }

    public AttributeSelector(OKBDataContext okb, string tableName, string fieldName) {
      TableName = tableName;
      FieldName = fieldName;
      TableType = GetTableType(tableName);
      ITable Table = GetTable(okb, tableName);
      if (staticTables.Contains(tableName)) {
        IsDynamic = false;
        try {
          DataType = TableType.GetProperty(fieldName).PropertyType;
        }
        catch (Exception x) {
          throw new ArgumentException(String.Format(
            "Invalid field name {0} in table {1}", fieldName, tableName),
            x);
        }
      } else if (dynamicTables.Contains(tableName)) {
        IsDynamic = true;
        try {
          IDynamicParent parent =
            Table
            .Cast<IDynamicParent>()
            .Single(t => t.Name == fieldName);
          ParentId = parent.Id;
          DataType = parent.DataType.Type;
        }
        catch (Exception x) {
          throw new ArgumentException(String.Format(
            "Invalid field name {0} in table {1}", fieldName, tableName),
            x);
        }
      } else {
        throw new ArgumentException(String.Format("Invalid table name {0}", tableName));
      }
    }

    protected static Type GetTableType(string name) {
      try {
        return typeof(Run).Assembly.GetType(typeof(Run).Namespace + "." + name, true);
      }
      catch {
        return null;
      }
    }

    protected static ITable GetTable(OKBDataContext okb, string name) {
      return okb.GetTable(GetTableType(name));
    }

    public string FullName {
      get {
        return new StringBuilder().Append(TableName).Append('_').Append(FieldName).ToString();
      }
    }

    protected static void LoadWith(DataLoadOptions dlo, Type parentType, Type type) {
      ParameterExpression parent = Expression.Parameter(parentType, parentType.Name.ToLower());
      LambdaExpression expr = Expression.Lambda(Expression.PropertyOrField(parent, type.Name), parent);
      try {
        dlo.LoadWith(expr);
      }
      catch (ArgumentException) {
        Console.WriteLine(String.Format("Info: dlo.LoadWith<{0}>({1}) has already been added", parent.Type.Name, expr));
      }
    }

    protected LambdaExpression GetDynamicSelector<T>(
      Type masterType,
      Expression<Func<Run, T>> parentSelector,
      Expression<Func<T, int>> intSelector,
      Expression<Func<T, double>> doubleSelector,
      Expression<Func<T, string>> stringSelector,
      Expression<Func<T, object>> operatorNameSelector) {
      if (TableType != masterType)
        return null;
      ParameterExpression run = Expression.Parameter(typeof(Run), "run");
      switch (Type.GetTypeCode(DataType)) {
        case TypeCode.Int32:
          return ExpressionTools.CallAfter<Run>(intSelector, parentSelector);
        case TypeCode.Double:
          return ExpressionTools.CallAfter<Run>(doubleSelector, parentSelector);
        case TypeCode.String:
          return ExpressionTools.CallAfter<Run>(stringSelector, parentSelector);
        case TypeCode.Object:
          return ExpressionTools.CallAfter<Run>(operatorNameSelector, parentSelector);
      };
      return null;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder()
        .Append(TableName)
        .Append('.')
        .Append(FieldName);
      if (MinValue != null)
        sb.Append(" >").Append(MinValue);
      if (MaxValue != null)
        sb.Append(" <").Append(MaxValue);
      if (AllowedValues != null)
        sb.Append(" in [")
          .Append(string.Join(", ",
                    AllowedValues
                    .Select(v => v.ToString())
                    .ToArray()))
          .Append(']');
      if (IsHidden)
        return String.Format("({0})", sb.ToString());
      else
        return sb.ToString();
    }
  }
}
