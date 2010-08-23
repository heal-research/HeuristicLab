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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.AttributeSelector {

  public static class ExpressionTools {

    /// <summary>
    /// p => f(g(p)) where p : T
    /// </summary>
    public static LambdaExpression CallAfter<T>(Expression f, Expression g) {
      ParameterExpression p = Expression.Parameter(typeof(T), "p");
      return Expression.Lambda(Expression.Invoke(f, Expression.Invoke(g, p)), p);
    }

    public static Selector GetSelector<T, D>(Expression<Func<Run, T>> tableSelector, Expression<Func<T, D>> fieldSelector) {
      Expression member = fieldSelector.Body;
      while (member is UnaryExpression)
        member = ((UnaryExpression)member).Operand;
      if (!(member is MemberExpression))
        throw new ArgumentException("cannot determine field name");
      string fieldName = ((MemberExpression)member).Member.Name;
      return new Selector() {
        Table = tableSelector.Body.Type,
        Field = fieldName,
        Expression = ExpressionTools.CallAfter<Run>(fieldSelector, tableSelector),
      };
    }

    public static readonly List<Type> SupportedTypes = new List<Type>() {
      typeof(int), typeof(double), typeof(DateTime), typeof(Guid) };

    public static BinaryExpression GetLessThanOrEqualExpression<T>(Expression left, Expression right) {
      if (SupportedTypes.Contains(typeof(T)))
        return Expression.LessThanOrEqual(left, right);
      else if (typeof(T) == typeof(string))
        return Expression.LessThanOrEqual(
          GetStringComparisonExpression(left, right), Expression.Constant(0));
      else throw new ArgumentException("Unsupported comparison for types of " + typeof(T));
    }

    public static BinaryExpression GetGreaterOrEqualExpression<T>(Expression left, Expression right) {
      if (SupportedTypes.Contains(typeof(T)))
        return Expression.GreaterThanOrEqual(left, right);
      else if (typeof(T) == typeof(string))
        return Expression.GreaterThanOrEqual(
          GetStringComparisonExpression(left, right), Expression.Constant(0));
      else throw new ArgumentException("Unsupported comparison for types of " + typeof(T));
    }

    public static MethodCallExpression GetStringComparisonExpression(Expression left, Expression right) {
      return Expression.Call(null, String_Compare, left, right);
    }

    public static readonly MethodInfo String_Compare =
      typeof(string).GetMethod("Compare", new[] { typeof(string), typeof(string) });

    public static readonly MethodInfo Enumerable_Contains =
      typeof(Enumerable).GetMethods().Single(mi => mi.Name == "Contains" && mi.GetParameters().Count() == 2);

    public static readonly MethodInfo Enumerable_Single =
      typeof(Enumerable).GetMethods().Single(mi => mi.Name == "Single" && mi.GetParameters().Count() == 2);

    public static readonly MethodInfo Enumerable_Where =
      (from mi in typeof(Enumerable).GetMethods()
       where mi.Name == "Where"
       let p = mi.GetParameters()
       where p.Count() == 2
       let pt = p[1].ParameterType
       where pt.IsGenericType
       where pt.GetGenericArguments().Count() == 2
       select mi).Single();

    public static readonly MethodInfo Enumerable_Cast = typeof(Enumerable).GetMethod("Cast");

    public static readonly MethodInfo Enumerable_OrderBy =
      typeof(Enumerable).GetMethods().Single(mi => mi.Name == "OrderBy" && mi.GetParameters().Count() == 2);

    public static readonly MethodInfo Enumerable_Count =
      typeof(Enumerable).GetMethods().Single(mi => mi.Name == "Count" && mi.GetParameters().Count() == 1);
  }
}
