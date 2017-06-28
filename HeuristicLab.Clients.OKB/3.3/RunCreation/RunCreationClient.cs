#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.IO;
using System.Linq;
using HeuristicLab.Clients.Common;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Clients.OKB.RunCreation {
  [Item("RunCreationClient", "OKB run creation client.")]
  public sealed class RunCreationClient : IContent {
    private static RunCreationClient instance;
    public static RunCreationClient Instance {
      get {
        if (instance == null) instance = new RunCreationClient();
        return instance;
      }
    }

    #region Properties
    private List<Algorithm> algorithms;
    public IEnumerable<Algorithm> Algorithms {
      get { return algorithms; }
    }
    private List<Problem> problems;
    public IEnumerable<Problem> Problems {
      get { return problems; }
    }
    #endregion

    private RunCreationClient() {
      algorithms = new List<Algorithm>();
      problems = new List<Problem>();
    }

    #region Refresh
    public void Refresh() {
      OnRefreshing();
      algorithms = new List<Algorithm>();
      problems = new List<Problem>();
      try {
        algorithms.AddRange(CallRunCreationService<List<Algorithm>>(s => s.GetAlgorithms("HeuristicLab 3.3")));
        problems.AddRange(CallRunCreationService<List<Problem>>(s => s.GetProblems("HeuristicLab 3.3")));
      } finally {
        OnRefreshed();
      }
    }
    public void RefreshAsync(Action<Exception> exceptionCallback) {
      var call = new Func<Exception>(delegate() {
        try {
          Refresh();
        } catch (Exception ex) {
          return ex;
        }
        return null;
      });
      call.BeginInvoke(delegate(IAsyncResult result) {
        Exception ex = call.EndInvoke(result);
        if (ex != null) exceptionCallback(ex);
      }, null);
    }
    #endregion

    #region Algorithm Methods
    public static byte[] GetAlgorithmData(long algorithmId) {
      return CallRunCreationService<byte[]>(s => s.GetAlgorithmData(algorithmId));
    }
    #endregion

    #region Problem Methods
    public static byte[] GetProblemData(long problemId) {
      return CallRunCreationService<byte[]>(s => s.GetProblemData(problemId));
    }
    #endregion

    #region Run Methods
    public void AddRun(Run run) {
      CallRunCreationService(s => s.AddRun(run));
    }
    #endregion

    #region Characteristic Methods
    public static IEnumerable<Value> GetCharacteristicValues(long problemId) {
      return CallRunCreationService(s => s.GetCharacteristicValues(problemId));
    }

    public static void SetCharacteristicValue(long problemId, Value v) {
      CallRunCreationService(s => s.SetCharacteristicValue(problemId, v));
    }

    public static void SetCharacteristicValues(long problemId, IEnumerable<Value> values) {
      CallRunCreationService(s => s.SetCharacteristicValues(problemId, values.ToList()));
    }
    #endregion

    #region OKB-Item Conversion
    public IItem ConvertToItem(Value value) {
      if (value is BinaryValue) {
        IItem item = null;
        var binaryValue = (BinaryValue)value;
        if (binaryValue.Value != null) {
          using (var stream = new MemoryStream(binaryValue.Value)) {
            try {
              item = XmlParser.Deserialize<IItem>(stream);
            } catch (Exception) { }
            stream.Close();
          }
        }
        return item ?? new Data.StringValue(value.DataType.Name);
      } else if (value is BoolValue) {
        return new Data.BoolValue(((BoolValue)value).Value);
      } else if (value is FloatValue) {
        return new Data.DoubleValue(((FloatValue)value).Value);
      } else if (value is PercentValue) {
        return new Data.PercentValue(((PercentValue)value).Value);
      } else if (value is DoubleValue) {
        return new Data.DoubleValue(((DoubleValue)value).Value);
      } else if (value is IntValue) {
        return new Data.IntValue((int)((IntValue)value).Value);
      } else if (value is LongValue) {
        return new Data.IntValue((int)((LongValue)value).Value);
      } else if (value is StringValue) {
        return new Data.StringValue(((StringValue)value).Value);
      } else if (value is TimeSpanValue) {
        return new Data.TimeSpanValue(TimeSpan.FromSeconds((long)((TimeSpanValue)value).Value));
      }
      return null;
    }

    public Value ConvertToValue(IItem item, string name) {
      Value result = null;
      if (item is ValueTypeValue<bool>) {
        var boolValue = (ValueTypeValue<bool>)item;
        result = new BoolValue() { Value = boolValue.Value };
      } else if (item is ValueTypeValue<int>) {
        var intValue = (ValueTypeValue<int>)item;
        result = new IntValue() { Value = intValue.Value };
      } else if (item is ValueTypeValue<long>) {
        var longValue = (ValueTypeValue<long>)item;
        result = new LongValue() { Value = longValue.Value };
      } else if (item is ValueTypeValue<float>) {
        var floatValue = (ValueTypeValue<float>)item;
        result = new FloatValue() { Value = floatValue.Value };
      } else if (item is ValueTypeValue<double>) {
        var doubleValue = (ValueTypeValue<double>)item;
        if (item is Data.PercentValue) result = new PercentValue() { Value = doubleValue.Value };
        else result = new DoubleValue() { Value = doubleValue.Value };
      } else if (item is ValueTypeValue<TimeSpan>) {
        var timeSpanValue = (ValueTypeValue<TimeSpan>)item;
        result = new TimeSpanValue() { Value = (long)timeSpanValue.Value.TotalSeconds };
      } else if (item is Data.StringValue) {
        var stringValue = (Data.StringValue)item;
        result = new StringValue() { Value = stringValue.Value };
      }
      if (result == null) {
        var binaryValue = new BinaryValue {
          DataType = new DataType() {
            Name = item.GetType().Name,
            TypeName = item.GetType().AssemblyQualifiedName
          }
        };
        using (var memStream = new MemoryStream()) {
          XmlGenerator.Serialize(item, memStream);
          binaryValue.Value = memStream.ToArray();
        }
        result = binaryValue;
      }
      result.Name = name;
      return result;
    }
    #endregion

    #region Events
    public event EventHandler Refreshing;
    private void OnRefreshing() {
      EventHandler handler = Refreshing;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Refreshed;
    private void OnRefreshed() {
      EventHandler handler = Refreshed;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion

    #region Helpers
    private static void CallRunCreationService(Action<IRunCreationService> call) {
      RunCreationServiceClient client = ClientFactory.CreateClient<RunCreationServiceClient, IRunCreationService>();
      try {
        call(client);
      } finally {
        try {
          client.Close();
        } catch (Exception) {
          client.Abort();
        }
      }
    }
    private static T CallRunCreationService<T>(Func<IRunCreationService, T> call) {
      RunCreationServiceClient client = ClientFactory.CreateClient<RunCreationServiceClient, IRunCreationService>();
      try {
        return call(client);
      } finally {
        try {
          client.Close();
        } catch (Exception) {
          client.Abort();
        }
      }
    }
    #endregion
  }
}
