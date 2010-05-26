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

using HeuristicLab.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using HeuristicLab.Core;
using Google.ProtocolBuffers;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public static class SolutionMessageBuildRegistry {
    private static readonly Dictionary<Type, Action<string, IItem, SolutionMessage.Builder>> dispatcher = new Dictionary<Type, Action<string, IItem, SolutionMessage.Builder>>();

    static SolutionMessageBuildRegistry() {
      dispatcher.Add(typeof(ValueTypeValue<int>), (name, item, builder) => DefaultSolutionMessageBuilder.AddAsIntegerVar(name, (ValueTypeValue<int>)item, builder));
      dispatcher.Add(typeof(ValueTypeArray<int>), (name, item, builder) => DefaultSolutionMessageBuilder.AddAsIntegerArrayVar(name, (ValueTypeArray<int>)item, builder));
      dispatcher.Add(typeof(ValueTypeValue<double>), (name, item, builder) => DefaultSolutionMessageBuilder.AddAsDoubleVar(name, (ValueTypeValue<double>)item, builder));
      dispatcher.Add(typeof(ValueTypeArray<double>), (name, item, builder) => DefaultSolutionMessageBuilder.AddAsDoubleArrayVar(name, (ValueTypeArray<double>)item, builder));
      dispatcher.Add(typeof(ValueTypeValue<bool>), (name, item, builder) => DefaultSolutionMessageBuilder.AddAsBoolVar(name, (ValueTypeValue<bool>)item, builder));
      dispatcher.Add(typeof(ValueTypeArray<bool>), (name, item, builder) => DefaultSolutionMessageBuilder.AddAsBoolArrayVar(name, (ValueTypeArray<bool>)item, builder));
      dispatcher.Add(typeof(IStringConvertibleValue), (name, item, builder) => DefaultSolutionMessageBuilder.AddAsStringVar(name, (IStringConvertibleValue)item, builder));
    }

    internal static void AddToSolutionMessage(this IItem item, string name, SolutionMessage.Builder builder) {
      Type type = item.GetType();
      while (!dispatcher.ContainsKey(type)) {
        if (type.BaseType != null) type = type.BaseType;
        else break;
      }
      if (type.BaseType == null && !dispatcher.ContainsKey(type)) {
        IEnumerable<Type> interfaces = item.GetType().GetInterfaces().Where(x => dispatcher.ContainsKey(x));
        if (interfaces.Count() != 1) throw new ArgumentException("It is unknown how to handle values of type " + item.GetType().FullName);
        else type = interfaces.Single();
      }
      
      dispatcher[type](name, item, builder);
    }

    public static bool IsRegistered(Type type) {
      return dispatcher.ContainsKey(type);
    }

    public static void Register(Type type, Action<string, IItem, SolutionMessage.Builder> registrar) {
      if (dispatcher.ContainsKey(type)) throw new ArgumentException("Type " + type.FullName + " is already registered.", "type");
      dispatcher.Add(type, registrar);
    }

    public static void Deregister(Type type) {
      if (!dispatcher.ContainsKey(type)) throw new ArgumentException("Type " + type.FullName + " is not registered.", "type");
      dispatcher.Remove(type);
    }
  }

  public static class DefaultSolutionMessageBuilder {
    public static void AddAsIntegerVar(string name, ValueTypeValue<int> value, SolutionMessage.Builder builder) {
      builder.AddIntegerVars(SolutionMessage.Types.IntegerVariable.CreateBuilder().SetName(name).SetData(value.Value).Build());
    }
    public static void AddAsDoubleVar(string name, ValueTypeValue<double> value, SolutionMessage.Builder builder) {
      builder.AddDoubleVars(SolutionMessage.Types.DoubleVariable.CreateBuilder().SetName(name).SetData(value.Value).Build());
    }
    public static void AddAsBoolVar(string name, ValueTypeValue<bool> value, SolutionMessage.Builder builder) {
      builder.AddBoolVars(SolutionMessage.Types.BoolVariable.CreateBuilder().SetName(name).SetData(value.Value).Build());
    }
    public static void AddAsStringVar(string name, IStringConvertibleValue value, SolutionMessage.Builder builder) {
      builder.AddStringVars(SolutionMessage.Types.StringVariable.CreateBuilder().SetName(name).SetData(value.GetValue()).Build());
    }
    public static void AddAsIntegerArrayVar(string name, ValueTypeArray<int> valueTypeArray, SolutionMessage.Builder builder) {
      SolutionMessage.Types.IntegerArrayVariable.Builder tmp = SolutionMessage.Types.IntegerArrayVariable.CreateBuilder();
      tmp.SetName(name);
      for (int i = 0; i < valueTypeArray.Length; i++)
        tmp.SetData(i, valueTypeArray[i]);
      builder.AddIntegerArrayVars(tmp.Build());
    }
    public static void AddAsDoubleArrayVar(string name, ValueTypeArray<double> valueTypeArray, SolutionMessage.Builder builder) {
      SolutionMessage.Types.DoubleArrayVariable.Builder tmp = SolutionMessage.Types.DoubleArrayVariable.CreateBuilder();
      tmp.SetName(name);
      for (int i = 0; i < valueTypeArray.Length; i++)
        tmp.SetData(i, valueTypeArray[i]);
      builder.AddDoubleArrayVars(tmp.Build());
    }
    public static void AddAsBoolArrayVar(string name, ValueTypeArray<bool> valueTypeArray, SolutionMessage.Builder builder) {
      SolutionMessage.Types.BoolArrayVariable.Builder tmp = SolutionMessage.Types.BoolArrayVariable.CreateBuilder();
      tmp.SetName(name);
      for (int i = 0; i < valueTypeArray.Length; i++)
        tmp.SetData(i, valueTypeArray[i]);
      builder.AddBoolArrayVars(tmp.Build());
    }
  }
}
