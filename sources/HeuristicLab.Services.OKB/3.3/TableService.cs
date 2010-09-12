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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using HeuristicLab.Services.OKB.AttributeSelection;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// Implementation of the <see cref="ITableService"/>.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
  public class TableService : ITableService, IDisposable {
    private static List<Type> SupportedTypes = new List<Type>() {
      typeof(int),
      typeof(long),
      typeof(double),
      typeof(string),
      typeof(Guid),
      typeof(DateTime?)
    };

    private DataTable tableTemplate;
    private IEnumerable<PropertyInfo> properties;
    private IEnumerator rowEnumerator;

    #region ITableService Members
    /// <summary>
    /// Updates the data table.
    /// </summary>
    /// <param name="updatedRows">The updated rows.</param>
    /// <param name="tableName">Name of the table.</param>
    public void UpdateDataTable(DataTable updatedRows, string tableName) {
      Type tableType = Assembly.GetAssembly(typeof(Run)).GetType("HeuristicLab.Services.OKB.DataAccess." + tableName, true);
      var properties = from p in tableType.GetProperties()
                       where SupportedTypes.Contains(p.PropertyType)
                       select p;
      using (OKBDataContext okb = new OKBDataContext()) {
        ITable table = okb.GetTable(tableType);
        foreach (DataRow row in updatedRows.Rows) {
          if (row["Id"] == DBNull.Value)
            table.InsertOnSubmit(CreateEntity(tableType, properties, row));
        }
        var updated = updatedRows.Rows.Cast<DataRow>().Where(row => row["Id"] != DBNull.Value).OrderBy(row => row["Id"]);
        var entities = DataSetBuilder.GetEntities(okb, tableType, updated.Select(u => (int)u["Id"]));
        var updateEnum = updated.GetEnumerator();
        var entityEnum = entities.GetEnumerator();
        while (updateEnum.MoveNext() && entityEnum.MoveNext()) {
          bool idChecked = false;
          foreach (var property in properties) {
            Console.WriteLine("{0}.{1} = '{2}' -> '{3}'",
              tableName, property.Name,
              property.GetGetMethod().Invoke(entityEnum.Current, new object[0]),
              updateEnum.Current[property.Name]);
            if (property.Name == "Id") {
              int entityId = (int)property.GetGetMethod().Invoke(entityEnum.Current, new object[0]);
              int updateId = (int)updateEnum.Current["Id"];
              Debug.Assert(entityId == updateId);
              idChecked = true;
            }
            object newValue = updateEnum.Current[property.Name];
            if (newValue != DBNull.Value)
              property.GetSetMethod().Invoke(entityEnum.Current, new object[] { newValue });
          }
          Debug.Assert(idChecked);
        }
        okb.SubmitChanges();
      }
    }

    private object CreateEntity(Type tableType, IEnumerable<PropertyInfo> properties, DataRow row) {
      object instance = Activator.CreateInstance(tableType);
      bool empty = true;
      foreach (var property in properties) {
        if (row[property.Name] != DBNull.Value) {
          property.GetSetMethod().Invoke(instance, new object[] { row[property.Name] });
          Console.WriteLine("{0}.{1} = '{2}'", tableType.Name, property.Name, row[property.Name]);
          empty = false;
        }
      }
      if (empty)
        throw new ArgumentException("cannot create completely empty entity");
      return instance;
    }

    /// <summary>
    /// Deletes the selected table rows using the value of the
    /// "Id" column.
    /// </summary>
    /// <param name="ids">The ids.</param>
    /// <param name="tableName">Name of the table.</param>
    public void DeleteTableRows(int[] ids, string tableName) {
      Type tableType = Assembly.GetAssembly(typeof(Run)).GetType("HeuristicLab.Services.OKB.DataAccess" + tableName, true);
      using (OKBDataContext okb = new OKBDataContext()) {
        ITable table = okb.GetTable(tableType);
        table.DeleteAllOnSubmit(DataSetBuilder.GetEntities(okb, tableType, ids));
        okb.SubmitChanges();
      }
    }

    /// <summary>
    /// Prepares the data table to be downloaded.
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <param name="count">The number of rows.</param>
    /// <returns>
    /// An empyt <see cref="DataType"/> that contains just
    /// the column headers.
    /// </returns>
    public DataTable PrepareDataTable(string tableName, out int count) {
      Type tableType = Assembly.GetAssembly(typeof(Run)).GetType("HeuristicLab.Services.OKB.DataAccess." + tableName, true);
      properties = from p in tableType.GetProperties()
                   where SupportedTypes.Contains(p.PropertyType)
                   select p;
      tableTemplate = new DataTable(tableName);
      foreach (var property in properties) {
        Type type = property.PropertyType;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
          type = type.GetGenericArguments()[0];
        DataColumn column = new DataColumn(property.Name, type);
        if (type.Name == "Id") {
          column.ReadOnly = true;
        }
        tableTemplate.Columns.Add(column);
      }
      using (OKBDataContext okb = new OKBDataContext()) {
        ITable table = okb.GetTable(tableType);
        rowEnumerator = table.GetEnumerator();
        count = DataSetBuilder.CountEntities(new OKBDataContext(), tableType);
        return tableTemplate;
      }
    }

    /// <summary>
    /// Gets the next few rows.
    /// </summary>
    /// <param name="count">The maximum number of rows to return.</param>
    /// <returns>
    /// A partial <see cref="DataTable"/> with the
    /// next few rows.
    /// </returns>
    public DataTable GetNextRows(int count) {
      DataTable dataTable = tableTemplate.Clone();
      int i = 0;
      while (i < count && rowEnumerator.MoveNext()) {
        i++;
        DataRow row = dataTable.NewRow();
        foreach (var property in properties) {
          row[property.Name] = property.GetGetMethod().Invoke(rowEnumerator.Current, new object[0]) ?? DBNull.Value;
        }
        dataTable.Rows.Add(row);
      }
      return dataTable;
    }

    /// <summary>
    /// Finishes fetching rows and closes the connection.
    /// </summary>
    public void FinishFetchingRows() {
      Dispose();
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
      tableTemplate = null;
      properties = null;
    }
    #endregion
  }
}
