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
using System.Threading;

namespace HeuristicLab.CEDMA.DB {
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public class Database : IDatabase {
    private string connectionString;
    private ReaderWriterLockSlim rwLock;
    public Database(string connectionString) {
      this.connectionString = connectionString;
      rwLock = new ReaderWriterLockSlim();
    }

    #region create empty database
    public void CreateNew() {
      rwLock.EnterWriteLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Project (ID integer primary key autoincrement, Name text, Description text, CreationTime DateTime)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Agent (ID integer primary key autoincrement, ProjectId integer, ParentAgentId integer, Name text, Status text default " + ProcessStatus.Unknown + ", CreationTime DateTime, RawData Blob)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Result (ID integer primary key autoincrement, AgentId integer, ParentResultId integer, Summary text, Description text, CreationTime DateTime, RawData Blob)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Operator (ID integer primary key autoincrement, Name text, RawData Blob)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }
    #endregion

    #region insert agent/result/sub-result
    public long InsertAgent(long? parentAgentId, string name, byte[] rawData) {
      rwLock.EnterWriteLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Agent (Name, ParentAgentId, CreationTime, RawData) values (@Name, @ParentAgentId, @CreationTime, @RawData); select last_insert_rowid()";
              DbParameter nameParam = c.CreateParameter();
              nameParam.ParameterName = "@Name";
              nameParam.Value = name;
              c.Parameters.Add(nameParam);
              DbParameter parentParam = c.CreateParameter();
              parentParam.ParameterName = "@ParentAgentId";
              parentParam.Value = parentAgentId;
              c.Parameters.Add(parentParam);
              DbParameter creationTimeParam = c.CreateParameter();
              creationTimeParam.ParameterName = "@CreationTime";
              creationTimeParam.Value = DateTime.Now;
              c.Parameters.Add(creationTimeParam);
              DbParameter dataParam = c.CreateParameter();
              dataParam.ParameterName = "@RawData";
              dataParam.Value = rawData;
              c.Parameters.Add(dataParam);
              id = (long)c.ExecuteScalar();
            }
            t.Commit();
            return id;
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    public long InsertResult(long agentId, string summary, string description, byte[] rawData) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Result (AgentId, CreationTime, Summary, Description, RawData) values (@AgentId, @CreationTime, @Summary, @Description, @RawData); select last_insert_rowid()";
              DbParameter agentIdParam = c.CreateParameter();
              agentIdParam.ParameterName = "@AgentId";
              agentIdParam.Value = agentId;
              c.Parameters.Add(agentIdParam);
              DbParameter creationParam = c.CreateParameter();
              creationParam.ParameterName = "@CreationTime";
              DateTime now = DateTime.Now;
              creationParam.Value = now;
              c.Parameters.Add(creationParam);
              DbParameter summaryParam = c.CreateParameter();
              summaryParam.ParameterName = "@Summary";
              summaryParam.Value = summary;
              c.Parameters.Add(summaryParam);
              DbParameter descParam = c.CreateParameter();
              descParam.ParameterName = "@Description";
              descParam.Value = description;
              c.Parameters.Add(descParam);
              DbParameter rawDataParam = c.CreateParameter();
              rawDataParam.ParameterName = "@RawData";
              rawDataParam.Value = rawData;
              c.Parameters.Add(rawDataParam);
              id = (long)c.ExecuteScalar();
            }
            t.Commit();
            return id;
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    public long InsertSubResult(long resultId, string summary, string description, byte[] rawData) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Result (ParentResultId, CreationTime, Summary, Description, RawData) values (@ParentResultId, @CreationTime, @Summary, @Description, @RawData); select last_insert_rowid()";
              DbParameter resultIdParam = c.CreateParameter();
              resultIdParam.ParameterName = "@ParentResultId";
              resultIdParam.Value = resultId;
              c.Parameters.Add(resultIdParam);
              DbParameter creationParam = c.CreateParameter();
              creationParam.ParameterName = "@CreationTime";
              DateTime now = DateTime.Now;
              creationParam.Value = now;
              c.Parameters.Add(creationParam);
              DbParameter summaryParam = c.CreateParameter();
              summaryParam.ParameterName = "@Summary";
              summaryParam.Value = summary;
              c.Parameters.Add(summaryParam);
              DbParameter descParam = c.CreateParameter();
              descParam.ParameterName = "@Description";
              descParam.Value = description;
              c.Parameters.Add(descParam);
              DbParameter rawDataParam = c.CreateParameter();
              rawDataParam.ParameterName = "@RawData";
              rawDataParam.Value = rawData;
              c.Parameters.Add(rawDataParam);
              id = (long)c.ExecuteScalar();
            }
            t.Commit();
            return id;
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    public long InsertOperator(string name, byte[] rawData) {
      rwLock.EnterWriteLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Operator (Name, RawData) values (@Name, @RawData); select last_insert_rowid()";
              DbParameter nameParam = c.CreateParameter();
              nameParam.ParameterName = "@Name";
              nameParam.Value = name;
              c.Parameters.Add(nameParam);
              DbParameter dataParam = c.CreateParameter();
              dataParam.ParameterName = "@RawData";
              dataParam.Value = rawData;
              c.Parameters.Add(dataParam);
              id = (long)c.ExecuteScalar();
            }
            t.Commit();
            return id;
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }
    #endregion

    #region update agent, run, operator
    public void UpdateAgent(long id, string name) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Agent set Name=@Name where id=@Id";
              DbParameter nameParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              nameParam.ParameterName = "@Name";
              nameParam.Value = name;
              idParam.ParameterName = "@Id";
              idParam.Value = id;
              c.Parameters.Add(nameParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    public void UpdateAgent(long id, ProcessStatus status) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Agent set Status=@Status where id=@Id";
              DbParameter statusParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              statusParam.ParameterName = "@Status";
              statusParam.Value = status.ToString();
              idParam.ParameterName = "@Id";
              idParam.Value = id;
              c.Parameters.Add(statusParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    public void UpdateAgent(long id, byte[] rawData) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Agent set RawData=@RawData where id=@Id";
              DbParameter rawDataParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              rawDataParam.ParameterName = "@RawData";
              rawDataParam.Value = rawData;
              idParam.ParameterName = "@Id";
              idParam.Value = id;
              c.Parameters.Add(rawDataParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    public void UpdateOperator(long id, string name, byte[] rawData) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Operator set Name=@Name, RawData=@RawData where id=@Id";
              DbParameter rawDataParam = c.CreateParameter();
              DbParameter nameParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              rawDataParam.ParameterName = "@RawData";
              rawDataParam.Value = rawData;
              idParam.ParameterName = "@Id";
              idParam.Value = id;
              nameParam.ParameterName = "@Name";
              nameParam.Value = name;
              c.Parameters.Add(rawDataParam);
              c.Parameters.Add(nameParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    #endregion

    #region get agent/result/sub-result/operator

    public ICollection<AgentEntry> GetAgents(ProcessStatus status) {
      rwLock.EnterReadLock();
      List<AgentEntry> agents = new List<AgentEntry>();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          SQLiteCommand c = cnn.CreateCommand();
          c.CommandText = "Select id, name from Agent where Status=@Status";
          DbParameter statusParameter = c.CreateParameter();
          statusParameter.ParameterName = "@Status";
          statusParameter.Value = status.ToString();
          c.Parameters.Add(statusParameter);
          SQLiteDataReader r = c.ExecuteReader();
          while(r.Read()) {
            AgentEntry agent = new AgentEntry();
            agent.ParentAgentId = null;
            agent.Status = status;
            agent.Id = r.GetInt32(0);
            agent.Name = r.IsDBNull(1) ? "" : r.GetString(1);
            agents.Add(agent);
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return agents;
    }

    public ICollection<AgentEntry> GetAgents() {
      rwLock.EnterReadLock();
      List<AgentEntry> agents = new List<AgentEntry>();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select id, name, status from Agent where ParentAgentId isnull";
            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                AgentEntry agent = new AgentEntry();
                agent.ParentAgentId = null;
                agent.Id = r.GetInt32(0);
                agent.Name = r.IsDBNull(1) ? "-" : r.GetString(1);
                agent.Status = (ProcessStatus)Enum.Parse(typeof(ProcessStatus), r.GetString(2));
                agents.Add(agent);
              }
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return agents;
    }

    public ICollection<AgentEntry> GetSubAgents(long parentAgentId) {
      rwLock.EnterReadLock();
      List<AgentEntry> agents = new List<AgentEntry>();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select id, name, status from Agent where ParentAgentId=@ParentAgentId";
            DbParameter parentParameter = c.CreateParameter();
            parentParameter.ParameterName = "@ParentAgentId";
            parentParameter.Value = parentAgentId;
            c.Parameters.Add(parentParameter);

            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                AgentEntry agent = new AgentEntry();
                agent.ParentAgentId = parentAgentId;
                agent.Id = r.GetInt32(0);
                agent.Name = r.IsDBNull(1) ? "-" : r.GetString(1);
                agent.Status = (ProcessStatus)Enum.Parse(typeof(ProcessStatus), r.GetString(2));
                agents.Add(agent);
              }
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return agents;
    }

    public byte[] GetAgentRawData(long id) {
      rwLock.EnterReadLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select RawData from Agent where Id=@Id";
            DbParameter idParameter = c.CreateParameter();
            idParameter.ParameterName = "@Id";
            idParameter.Value = id;
            c.Parameters.Add(idParameter);
            using(DbDataReader r = c.ExecuteReader()) {
              if(r.HasRows) {
                r.Read();
                return (byte[])r.GetValue(0);
              }
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return null; // agent with the given id not found
    }

    public ICollection<ResultEntry> GetResults(long agentId) {
      List<ResultEntry> results = new List<ResultEntry>();
      rwLock.EnterReadLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select Id, CreationTime, Summary, Description from Result where AgentId=@AgentId";
            DbParameter agentParam = c.CreateParameter();
            agentParam.ParameterName = "@AgentId";
            agentParam.Value = agentId;
            c.Parameters.Add(agentParam);
            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                ResultEntry result = new ResultEntry();
                result.AgentId = agentId;
                result.Id = r.GetInt32(0);
                result.CreationTime = r.GetDateTime(1);
                result.Summary = r.GetString(2);
                result.Description = r.GetString(3);
                results.Add(result);
              }
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return results;
    }

    public ICollection<ResultEntry> GetSubResults(long resultId) {
      List<ResultEntry> results = new List<ResultEntry>();
      rwLock.EnterReadLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select Id, CreationTime, Summary, Description from Result where ParentResultId=@ParentResultId";
            DbParameter parentParam = c.CreateParameter();
            parentParam.ParameterName = "@ParentResultId";
            parentParam.Value = resultId;
            c.Parameters.Add(parentParam);
            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                ResultEntry result = new ResultEntry();
                result.ParentResultId = resultId;
                result.Id = r.GetInt32(0);
                result.CreationTime = r.GetDateTime(1);
                result.Summary = r.GetString(2);
                result.Description = r.GetString(3);
                results.Add(result);
              }
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return results;
    }

    public byte[] GetResultRawData(long id) {
      rwLock.EnterReadLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select RawData from Result where Id=@Id";
            DbParameter idParameter = c.CreateParameter();
            idParameter.ParameterName = "@Id";
            idParameter.Value = id;
            c.Parameters.Add(idParameter);
            using(DbDataReader r = c.ExecuteReader()) {
              if(r.HasRows) {
                r.Read();
                return (byte[])r.GetValue(0);
              }
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return null; // result with the given id not found
    }

    public ICollection<OperatorEntry> GetOperators() {
      rwLock.EnterReadLock();
      List<OperatorEntry> operators = new List<OperatorEntry>();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select id, name, rawdata from Operator";
            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                OperatorEntry op = new OperatorEntry();
                op.Id = r.GetInt32(0);
                op.Name = r.IsDBNull(1) ? "-" : r.GetString(1);
                op.RawData = (byte[])r.GetValue(2);
                operators.Add(op);
              }
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return operators;
    }
    public OperatorEntry GetOperator(long id) {
      rwLock.EnterReadLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select id, name, rawdata from Operator where id=@Id";
            DbParameter idParam = c.CreateParameter();
            idParam.ParameterName = "@Id";
            idParam.Value = id;
            c.Parameters.Add(idParam);
            using(DbDataReader r = c.ExecuteReader()) {
              r.Read();
              OperatorEntry op = new OperatorEntry();
              op.Id = r.GetInt32(0);
              op.Name = r.IsDBNull(1) ? "-" : r.GetString(1);
              op.RawData = (byte[])r.GetValue(2);
              return op;
            }
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
    }
    #endregion

    #region delete operator
    public void DeleteOperator(long id) {
      rwLock.EnterWriteLock();
      try {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Delete from Operator where id=@Id";
              DbParameter idParam = c.CreateParameter();
              idParam.ParameterName = "@Id";
              idParam.Value = id;
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }
    #endregion
  }
}