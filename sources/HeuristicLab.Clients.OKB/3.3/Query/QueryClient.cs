#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.Xml;

namespace HeuristicLab.Clients.OKB.Query {
  [Item("QueryClient", "OKB query client.")]
  public sealed class QueryClient : IContent {
    private static QueryClient instance;
    public static QueryClient Instance {
      get {
        if (instance == null) instance = new QueryClient();
        return instance;
      }
    }

    #region Properties
    private List<Filter> filters;
    public IEnumerable<Filter> Filters {
      get { return filters; }
    }
    private List<ValueName> valueNames;
    public IEnumerable<ValueName> ValueNames {
      get { return valueNames; }
    }
    #endregion

    private QueryClient() {
      filters = new List<Filter>();
      valueNames = new List<ValueName>();
    }

    #region Refresh
    public void Refresh() {
      OnRefreshing();
      filters = new List<Filter>();
      try {
        filters.AddRange(CallQueryService<List<Filter>>(s => s.GetFilters()));
        valueNames.AddRange(CallQueryService<List<ValueName>>(s => s.GetValueNames()));
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

    #region Query Methods
    public long GetNumberOfRuns(Filter filter) {
      return CallQueryService<long>(x => x.GetNumberOfRuns(filter));
    }
    public IEnumerable<long> GetRunIds(Filter filter) {
      return CallQueryService<IEnumerable<long>>(x => x.GetRunIds(filter));
    }
    public IEnumerable<Run> GetRuns(IEnumerable<long> ids, bool includeBinaryValues) {
      return CallQueryService<IEnumerable<Run>>(s => s.GetRuns(ids.ToList(), includeBinaryValues));
    }
    public IEnumerable<Run> GetRunsWithValues(IEnumerable<long> ids, bool includeBinaryValues, IEnumerable<ValueName> vn) {
      return CallQueryService<IEnumerable<Run>>(s => s.GetRunsWithValues(ids.ToList(), includeBinaryValues, vn.ToList()));
    }
    #endregion

    #region OKB-Item Conversion
    public Optimization.IRun ConvertToOptimizationRun(Run run) {
      Optimization.Run optRun = new Optimization.Run();
      foreach (Value value in run.ParameterValues)
        optRun.Parameters.Add(value.Name, ConvertToItem(value));
      foreach (Value value in run.ResultValues)
        optRun.Results.Add(value.Name, ConvertToItem(value));
      return optRun;
    }

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
    private T CallQueryService<T>(Func<IQueryService, T> call) {
      QueryServiceClient client = ClientFactory.CreateClient<QueryServiceClient, IQueryService>();
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
