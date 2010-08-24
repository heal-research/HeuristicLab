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
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB.AttributeSelection {

  public class AttributeSpecifier {
    public string TableName { get; set; }
    public string FieldName { get; set; }
    public Type DataType { get; set; }
  }

  public class RunAttributeSelector : AttributeSelector {

    public RunAttributeSelector(OKBDataContext okb, string table, string field)
      : base(okb, table, field) { }

    public RunAttributeSelector(OKBDataContext okb, IAttributeSelector selector)
      : base(okb, selector.TableName, selector.FieldName) {
      IsHidden = selector.IsHidden;
      MinValue = selector.MinValue;
      MaxValue = selector.MaxValue;
      AllowedValues = selector.AllowedValues;
    }

    protected RunAttributeSelector(Type tableType, string fieldName, bool isDynamic, Type dataType, int parentId)
      : base(tableType, fieldName, isDynamic, dataType, parentId) { }

    public static IEnumerable<RunAttributeSelector> GetAllAttributeSelectors(OKBDataContext okb) {
      OKBDataContext o = new OKBDataContext(okb.Connection.ConnectionString);
      DataLoadOptions dlo = new DataLoadOptions();
      dlo.LoadWith<Result>(r => r.DataType);
      dlo.LoadWith<Parameter>(p => p.DataType);
      dlo.LoadWith<ProblemCharacteristic>(pc => pc.DataType);
      o.LoadOptions = dlo;
      foreach (Selector selector in Selectors) {
        yield return new RunAttributeSelector(o, selector.Table.Name, selector.Field);
      }
      foreach (Result result in okb.Results) {
        yield return new RunAttributeSelector(typeof(Result), result.Name, true, result.DataType.Type, result.Id);
      }
      foreach (Parameter p in okb.Parameters) {
        yield return new RunAttributeSelector(typeof(Parameter), p.Name, true, p.DataType.Type, p.Id);
      }
      foreach (ProblemCharacteristic pc in okb.ProblemCharacteristics) {
        yield return new RunAttributeSelector(typeof(ProblemCharacteristic), pc.Name, true, pc.DataType.Type, pc.Id);
      }
    }

    public void ConfigureDataLoadOptions(DataLoadOptions dlo) {
      if (IsHidden)
        return;
      Include(TableType, dlo);
    }

    private static readonly Dictionary<Type, Type> parents = new Dictionary<Type, Type>() {
      { typeof(Run), null },
      { typeof(Client), typeof(Run) },
      { typeof(User), typeof(Run) },
      { typeof(Experiment), typeof(Run) },
      { typeof(Project), typeof(Experiment) },
      { typeof(Algorithm), typeof(Experiment) },
      { typeof(AlgorithmClass), typeof(Algorithm) },
      { typeof(Platform), typeof(Algorithm) },
      { typeof(Problem), typeof(Experiment) },
      { typeof(ProblemClass), typeof(Problem) },
      { typeof(SolutionRepresentation), typeof(Problem) },
    };

    public static MethodInfo Enumerable_Where =
      typeof(Enumerable).GetMethods().Single(mi =>
        mi.Name == "Where" &&
        mi.GetParameters().Count() == 2 &&
        mi.GetParameters()[1].ParameterType.GetGenericArguments().Count() == 2);

    public static MethodInfo Enumerable_Cast = typeof(Enumerable).GetMethod("Cast");

    private void AssociateWith<T, V>(DataLoadOptions dlo,
      Expression<Func<T, IEnumerable>> intSelector,
      Expression<Func<T, IEnumerable>> doubleSelector,
      Expression<Func<T, IEnumerable>> charSelector,
      Expression<Func<T, IEnumerable>> blobSelector,
      Expression<Func<V, bool>> filterPredicate) {
      Expression<Func<T, IEnumerable>> selector;
      switch (Type.GetTypeCode(DataType)) {
        case TypeCode.Int32: selector = intSelector; break;
        case TypeCode.Double: selector = doubleSelector; break;
        case TypeCode.String: selector = charSelector; break;
        default:
          if (blobSelector != null)
            selector = blobSelector;
          else
            throw new ArgumentException("DataType");
          break;
      }
      ParameterExpression param = Expression.Parameter(typeof(T), "param");
      dlo.AssociateWith<T>((Expression<Func<T, object>>)
        Expression.Lambda(
          Expression.Convert(
            Expression.Call(
              null,
              Enumerable_Where.MakeGenericMethod(typeof(V)),
              Expression.Convert(Expression.Invoke(selector, param), typeof(IEnumerable<V>)),
              filterPredicate),
          typeof(object)), param));
      try {
        dlo.LoadWith(selector);
      }
      catch (ArgumentException) {
        Console.WriteLine("INFO: not adding (duplicate) load option: dlo.LoadWith<{0}>({1})", typeof(T).Name, selector.ToString());
        Console.WriteLine("INFO: clearing association filters to allow multiple associations");
        dlo.AssociateWith<T>((Expression<Func<T, object>>)
          Expression.Lambda(
            Expression.Convert(
            Expression.Invoke(selector, param),
            typeof(object)),
          param));
      }
    }

    private void Include(Type type, DataLoadOptions dlo) {
      if (parents.ContainsKey(type)) {
        Type parentType = parents[type];
        if (parentType == null)
          return;
        LoadWith(dlo, parentType, type);
        Include(parentType, dlo);
      } else if (type == typeof(ProblemCharacteristic)) {
        AssociateWith<Problem, IProblemCharacteristicValue>(dlo,
          p => p.IntProblemCharacteristicValues,
          p => p.FloatProblemCharacteristicValues,
          p => p.CharProblemCharacteristicValues,
          null,
          pc => pc.ProblemId == ParentId);
        Include(typeof(Problem), dlo);
      } else if (type == typeof(Parameter)) {
        AssociateWith<Experiment, IParameterValue>(dlo,
          x => x.IntParameterValues,
          x => x.FloatParameterValues,
          x => x.CharParameterValues,
          x => x.OperatorParameterValues,
          ipv => ipv.ParameterId == ParentId);
        try {
          dlo.LoadWith<OperatorParameterValue>(opv => opv.DataType);
        }
        catch (ArgumentException) {
          Console.WriteLine("INFO: not adding (duplicate) load option for OperatorParameterValue.DataType");
        }
        Include(typeof(Experiment), dlo);
      } else if (type == typeof(Result)) {
        AssociateWith<Run, IResultValue>(dlo,
          r => r.IntResultValues,
          r => r.FloatResultValues,
          r => r.CharResultValues,
          r => r.BlobResultValues,
          irv => irv.ResultId == ParentId);
      } else
        throw new ArgumentException("type");
    }

    protected static List<Selector> Selectors = new List<Selector>() {
        ExpressionTools.GetSelector(run => run, r => r.Id),
        ExpressionTools.GetSelector(run => run, r => r.FinishedDate),

        ExpressionTools.GetSelector(run => run.Client, c => c.Id),
        ExpressionTools.GetSelector(run => run.Client, c => c.Name),

        ExpressionTools.GetSelector(run => run.User, u => u.Id),
        ExpressionTools.GetSelector(run => run.User, u => u.Name),

        ExpressionTools.GetSelector(run => run.Experiment, x => x.Id),

        ExpressionTools.GetSelector(run => run.Experiment.Project, p => p.Id),
        ExpressionTools.GetSelector(run => run.Experiment.Project, p => p.Name),
        ExpressionTools.GetSelector(run => run.Experiment.Project, p => p.Description),

        ExpressionTools.GetSelector(run => run.Experiment.Algorithm, a => a.Id),
        ExpressionTools.GetSelector(run => run.Experiment.Algorithm, a => a.Name),
        ExpressionTools.GetSelector(run => run.Experiment.Algorithm, a => a.Description),

        ExpressionTools.GetSelector(run => run.Experiment.Algorithm.AlgorithmClass, ac => ac.Id),
        ExpressionTools.GetSelector(run => run.Experiment.Algorithm.AlgorithmClass, ac => ac.Name),
        ExpressionTools.GetSelector(run => run.Experiment.Algorithm.AlgorithmClass, ac => ac.Description),

        ExpressionTools.GetSelector(run => run.Experiment.Algorithm.Platform, p => p.Id),
        ExpressionTools.GetSelector(run => run.Experiment.Algorithm.Platform, p => p.Name),
        ExpressionTools.GetSelector(run => run.Experiment.Algorithm.Platform, p => p.Description),

        ExpressionTools.GetSelector(run => run.Experiment.Problem, p => p.Id),
        ExpressionTools.GetSelector(run => run.Experiment.Problem, p => p.Name),
        ExpressionTools.GetSelector(run => run.Experiment.Problem, p => p.Description),

        ExpressionTools.GetSelector(run => run.Experiment.Problem.ProblemClass, pc => pc.Id),
        ExpressionTools.GetSelector(run => run.Experiment.Problem.ProblemClass, pc => pc.Name),
        ExpressionTools.GetSelector(run => run.Experiment.Problem.ProblemClass, pc => pc.Description),

        ExpressionTools.GetSelector(run => run.Experiment.Problem.SolutionRepresentation, sr => sr.Id),
        ExpressionTools.GetSelector(run => run.Experiment.Problem.SolutionRepresentation, sr => sr.Name),
        ExpressionTools.GetSelector(run => run.Experiment.Problem.SolutionRepresentation, sr => sr.Description)
    };

    private Func<Run, object> compiledSelector;

    protected void CreateCompiledSelector<T>() {
      var genericCompiledSelector = ((Expression<Func<Run, T>>)GetSelectorExpression()).Compile();
      compiledSelector = run => {
        try {
          return (object)genericCompiledSelector(run);
        }
        catch (InvalidOperationException x) {
          if (x.TargetSite == ExpressionTools.Enumerable_Single)
            return null;
          else
            throw;
        }
      };
    }

    public Func<Run, object> CompiledSelector {
      get {
        if (compiledSelector == null) {
          typeof(RunAttributeSelector)
            .GetMethod("CreateCompiledSelector", BindingFlags.Instance | BindingFlags.NonPublic)
            .MakeGenericMethod(DataType).Invoke(this, new object[0]);
        }
        return compiledSelector;
      }
    }

    public LambdaExpression GetSelectorExpression() {
      if (TableType == typeof(Parameter))
        return
          GetDynamicSelector(typeof(Parameter), run => run.Experiment,
            x => x.IntParameterValues.Single(ipv => ipv.ParameterId == ParentId).Value,
            x => x.FloatParameterValues.Single(fpv => fpv.ParameterId == ParentId).Value,
            x => x.CharParameterValues.Single(cpv => cpv.ParameterId == ParentId).Value,
            x => x.OperatorParameterValues.Single(opv => opv.ParameterId == ParentId).DataType.ClrName);
      else if (TableType == typeof(Result))
        return
          GetDynamicSelector(typeof(Result), run => run,
            r => r.IntResultValues.Single(irv => irv.ResultId == ParentId).Value,
            r => r.FloatResultValues.Single(frv => frv.ResultId == ParentId).Value,
            r => r.CharResultValues.Single(crv => crv.ResultId == ParentId).Value,
            null);
      else if (TableType == typeof(ProblemCharacteristic))
        return
          GetDynamicSelector(typeof(ProblemCharacteristic), run => run.Experiment.Problem,
            p => p.IntProblemCharacteristicValues.Single(ipcv => ipcv.ProblemCharacteristicId == ParentId).Value,
            p => p.FloatProblemCharacteristicValues.Single(fpcv => fpcv.ProblemCharacteristicId == ParentId).Value,
            p => p.CharProblemCharacteristicValues.Single(cpcv => cpcv.ProblemCharacteristicId == ParentId).Value,
            null);
      else
        return Selectors.Single(s => s.Table == TableType && s.Field == FieldName).Expression;
    }

    public Expression<Func<Run, bool>> GetWhereExpression() {
      return GetWhereExpression(GetSelectorExpression());
    }

    protected Expression<Func<Run, bool>> GetWhereExpression(Expression selector) {
      try {
        return (Expression<Func<Run, bool>>)typeof(RunAttributeSelector)
          .GetMethod("GenericGetWhereExpression", BindingFlags.Instance | BindingFlags.NonPublic)
          .MakeGenericMethod(selector.Type.GetGenericArguments()[1])
          .Invoke(this, new[] { selector });
      }
      catch (TargetInvocationException x) {
        throw x.InnerException;
      }
    }

    protected Expression<Func<Run, bool>> GenericGetWhereExpression<T>(Expression<Func<Run, T>> selector) {
      ParameterExpression parameter = Expression.Parameter(typeof(Run), "run");
      Expression testExpression = Expression.Constant(true);
      if (AllowedValues != null) {
        testExpression = Expression.AndAlso(testExpression,
          Expression.Call(
            null,
            ExpressionTools.Enumerable_Contains.MakeGenericMethod(typeof(T)),
            Expression.Constant(AllowedValues.Cast<T>().ToArray()),
            Expression.Invoke(selector, parameter)));
      }
      if (MinValue != null) {
        testExpression = Expression.AndAlso(testExpression,
          ExpressionTools.GetLessThanOrEqualExpression<T>(Expression.Constant(MinValue),
          Expression.Invoke(selector, parameter)));
      }
      if (MaxValue != null) {
        testExpression = Expression.AndAlso(testExpression,
          ExpressionTools.GetGreaterOrEqualExpression<T>(Expression.Constant(MaxValue),
          Expression.Invoke(selector, parameter)));
      }
      return (Expression<Func<Run, bool>>)
        Expression.Lambda(testExpression, parameter);
    }
  }
}
