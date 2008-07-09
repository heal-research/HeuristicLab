#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Data.Linq;
using HeuristicLab.CEDMA.DB.Interfaces;
using System.ServiceModel;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;

namespace HeuristicLab.CEDMA.DB {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = true)]
  public class Database : IDatabase {
    private string connectionString;
    public Database(string connectionString) {
      this.connectionString = connectionString;
    }

    public void CreateNew() {
      using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
        cnn.ConnectionString = connectionString;
        cnn.Open();

        SQLiteCommand cAgent = cnn.CreateCommand();
        cAgent.CommandText = "CREATE TABLE Agent (ID integer primary key autoincrement, ProjectId integer, Name text, Status text default 'Unknown', RawData Blob)";
        SQLiteCommand cProject = cnn.CreateCommand();
        cProject.CommandText = "CREATE TABLE Project (ID integer primary key autoincrement, Name text, Description text, CreationDate DateTime)";
        SQLiteCommand cResult = cnn.CreateCommand();
        cResult.CommandText = "CREATE TABLE Result (ID integer primary key autoincrement, AgentId integer, ParentResultId integer, CreationDate DateTime, RawData Blob)";
        cAgent.ExecuteNonQuery();
        cProject.ExecuteNonQuery();
        cResult.ExecuteNonQuery();
      }
    }

    public long CreateAgent() {
      using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
        cnn.Open();
        SQLiteCommand c = cnn.CreateCommand();
        c.CommandText = "Insert into Agent (Name) values (@Name); select last_insert_rowid()";
        DbParameter nameParam = c.CreateParameter();
        nameParam.ParameterName = "@Name";
        nameParam.Value = DateTime.Now.ToString();
        c.Parameters.Add(nameParam);
        long id = (long)c.ExecuteScalar();
        return id;
      }
    }

    public ICollection<AgentEntry> GetAgentEntries() {
      List<AgentEntry> agents = new List<AgentEntry>();
      using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
        cnn.Open();
        SQLiteCommand c = cnn.CreateCommand();
        c.CommandText = "Select id, name, status, rawdata from Agent";
        SQLiteDataReader r = c.ExecuteReader();
        while(r.Read()) {
          AgentEntry agent = new AgentEntry(r.GetInt32(0), r.GetString(1), (AgentStatus)Enum.Parse(typeof(AgentStatus), r.GetString(2)), (byte[])r.GetValue(3));
          agents.Add(agent);
        }
      }
      return agents;
    }

    public ICollection<AgentEntry> GetAgentEntries(AgentStatus status) {
      List<AgentEntry> agents = new List<AgentEntry>();
      using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
        cnn.Open();
        SQLiteCommand c = cnn.CreateCommand();
        c.CommandText = "Select id, name, status, rawdata from Agent where Status=@Status";
        DbParameter statusParameter = c.CreateParameter();
        statusParameter.ParameterName = "@Status";
        statusParameter.Value = (int)status;
        c.Parameters.Add(statusParameter);

        SQLiteDataReader r = c.ExecuteReader();
        while(r.Read()) {
          AgentEntry agent = new AgentEntry(r.GetInt32(0), r.GetString(1), (AgentStatus)Enum.Parse(typeof(AgentStatus), r.GetString(2)), (byte[])r.GetValue(3));
          agents.Add(agent);
        }
      }
      return agents;
    }

    public void Update(AgentEntry entry) {
      using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
        cnn.Open();
        SQLiteCommand c = cnn.CreateCommand();
        c.CommandText = "Update Agent set Name=@Name, Status=@Status, RawData=@RawData where id=@Id";
        DbParameter nameParam = c.CreateParameter();
        DbParameter statusParam = c.CreateParameter();
        DbParameter rawDataParam = c.CreateParameter();
        DbParameter idParam = c.CreateParameter();
        nameParam.ParameterName = "@Name";
        nameParam.Value = entry.Name;
        statusParam.ParameterName = "@Status";
        statusParam.Value = entry.Status;
        rawDataParam.ParameterName = "@RawData";
        rawDataParam.Value = entry.RawData;
        idParam.ParameterName = "@Id";
        idParam.Value = entry.Id;
        c.Parameters.Add(nameParam);
        c.Parameters.Add(statusParam);
        c.Parameters.Add(rawDataParam);
        c.Parameters.Add(idParam);
        c.ExecuteNonQuery();
      }
    }

    public void Update(ResultEntry result) {
      throw new NotImplementedException();
    }
  }
}