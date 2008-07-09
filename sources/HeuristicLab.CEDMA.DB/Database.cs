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
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
  public class Database : IDatabase {
    private string connectionString;
    private object bigLock = new object();
    public Database(string connectionString) {
      this.connectionString = connectionString;
    }

    #region create empty database
    public void CreateNew() {
      lock(bigLock) {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Project (ID integer primary key autoincrement, Name text, Description text, CreationTime DateTime)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Agent (ID integer primary key autoincrement, ProjectId integer, Name text, Status text default 'Unknown', RawData Blob)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Run (ID integer primary key autoincrement, AgentId integer, CreationTime DateTime, StartTime DateTime, FinishedTime DateTime, Status text default 'Unknown', RawData Blob)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            using(DbCommand cmd = cnn.CreateCommand()) {
              cmd.CommandText = "CREATE TABLE Result (ID integer primary key autoincrement, RunId integer, ResultId integer, CreationTime DateTime, RawData Blob)";
              cmd.Transaction = t;
              cmd.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      }
    }
    #endregion

    #region insert agent/run/result/sub-result
    public long InsertAgent(string name, byte[] rawData) {
      lock(bigLock) {
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Agent (Name, RawData) values (@Name, @RawData); select last_insert_rowid()";
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
      }
    }

    public long InsertRun(long agentId, byte[] rawData) {
      lock(bigLock) {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Run (AgentId, CreationTime, RawData) values (@AgentId, @CreationTime, @RawData); select last_insert_rowid()";
              DbParameter agentIdParam = c.CreateParameter();
              agentIdParam.ParameterName = "@AgentId";
              agentIdParam.Value = agentId;
              c.Parameters.Add(agentIdParam);
              DbParameter creationParam = c.CreateParameter();
              creationParam.ParameterName = "@CreationTime";
              DateTime now = DateTime.Now;
              creationParam.Value = now;
              c.Parameters.Add(creationParam);
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
      }
    }

    public long InsertResult(long runId, byte[] rawData) {
      lock(bigLock) {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Result (RunId, CreationTime, RawData) values (@RunId, @CreationTime, @RawData); select last_insert_rowid()";
              DbParameter runIdParam = c.CreateParameter();
              runIdParam.ParameterName = "@RunId";
              runIdParam.Value = runId;
              c.Parameters.Add(runIdParam);
              DbParameter creationParam = c.CreateParameter();
              creationParam.ParameterName = "@CreationTime";
              DateTime now = DateTime.Now;
              creationParam.Value = now;
              c.Parameters.Add(creationParam);
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
      }
    }

    public long InsertSubResult(long resultId, byte[] rawData) {
      lock(bigLock) {

        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          long id;
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Result (ResultId, CreationTime, RawData) values (@ResultId, @CreationTime, @RawData); select last_insert_rowid()";
              DbParameter resultIdParam = c.CreateParameter();
              resultIdParam.ParameterName = "@ResultId";
              resultIdParam.Value = resultId;
              c.Parameters.Add(resultIdParam);
              DbParameter creationParam = c.CreateParameter();
              creationParam.ParameterName = "@CreationTime";
              DateTime now = DateTime.Now;
              creationParam.Value = now;
              c.Parameters.Add(creationParam);
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
      }
    }
    #endregion

    #region update agent/run
    public void UpdateAgent(long id, string name) {
      lock(bigLock) {
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
      }
    }

    public void UpdateAgent(long id, ProcessStatus status) {
      lock(bigLock) {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Agent set Status=@Status where id=@Id";
              DbParameter statusParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              statusParam.ParameterName = "@Status";
              statusParam.Value = status;
              idParam.ParameterName = "@Id";
              idParam.Value = id;
              c.Parameters.Add(statusParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      }
    }

    public void UpdateAgent(long id, byte[] rawData) {
      lock(bigLock) {
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
      }
    }

    public void UpdateRunStart(long runId, DateTime startTime) {
      lock(bigLock) {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Run set StartTime=@StartTime where id=@Id";
              DbParameter startTimeParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              startTimeParam.ParameterName = "@StartTime";
              startTimeParam.Value = startTime;
              idParam.ParameterName = "@Id";
              idParam.Value = runId;
              c.Parameters.Add(startTimeParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      }
    }

    public void UpdateRunFinished(long runId, DateTime finishedTime) {
      lock(bigLock) {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Run set FinishedTime=@FinishedTime where id=@Id";
              DbParameter finishedTimeParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              finishedTimeParam.ParameterName = "@FinishedTime";
              finishedTimeParam.Value = finishedTime;
              idParam.ParameterName = "@Id";
              idParam.Value = runId;
              c.Parameters.Add(finishedTimeParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      }
    }

    public void UpdateRunStatus(long runId, ProcessStatus status) {
      lock(bigLock) {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Run set Status=@Status where id=@Id";
              DbParameter statusParam = c.CreateParameter();
              DbParameter idParam = c.CreateParameter();
              statusParam.ParameterName = "@Status";
              statusParam.Value = status;
              idParam.ParameterName = "@Id";
              idParam.Value = runId;
              c.Parameters.Add(statusParam);
              c.Parameters.Add(idParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      }
    }



    #endregion


    #region get agent/run/result/sub-result

    public ICollection<AgentEntry> GetAgents(ProcessStatus status) {
      lock(bigLock) {
        List<AgentEntry> agents = new List<AgentEntry>();
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          SQLiteCommand c = cnn.CreateCommand();
          c.CommandText = "Select id, name, status, rawdata from Agent where Status=@Status";
          DbParameter statusParameter = c.CreateParameter();
          statusParameter.ParameterName = "@Status";
          statusParameter.Value = status;
          c.Parameters.Add(statusParameter);

          SQLiteDataReader r = c.ExecuteReader();
          while(r.Read()) {
            AgentEntry agent = new AgentEntry();
            agent.Id = r.GetInt32(0);
            agent.Name = r.GetString(1);
            agent.Status = (ProcessStatus)Enum.Parse(typeof(ProcessStatus), r.GetString(2));
            agent.RawData = (byte[])r.GetValue(3);
            agents.Add(agent);
          }
        }
        return agents;
      }
    }

    public ICollection<AgentEntry> GetAgents() {
      lock(bigLock) {
        List<AgentEntry> agents = new List<AgentEntry>();
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select id, name, status, rawdata from Agent";
            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                AgentEntry agent = new AgentEntry();
                agent.Id = r.GetInt32(0);
                agent.Name = r.GetString(1);
                agent.Status = (ProcessStatus)Enum.Parse(typeof(ProcessStatus), r.GetString(2));
                agent.RawData = (byte[])r.GetValue(3);
                agents.Add(agent);
              }
            }
          }
        }
        return agents;
      }
    }

    //(ID integer primary key autoincrement, AgentId integer, CreationTime DateTime, StartTime DateTime, FinishedTime DateTime, Status text default 'Unknown', RawData Blob)";

    public ICollection<RunEntry> GetRuns() {
      lock(bigLock) {
        List<RunEntry> runs = new List<RunEntry>();
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select Id, AgentId, CreationTime, StartTime, FinishedTime, Status, Rawdata from Run";
            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                RunEntry run = new RunEntry();
                run.Id = r.GetInt32(0);
                run.AgentId = r.GetInt32(1);
                run.CreationTime = r.GetDateTime(2);
                run.StartTime = r.GetDateTime(3);
                run.FinishedTime = r.GetDateTime(4);
                run.Status = (ProcessStatus)Enum.Parse(typeof(ProcessStatus), r.GetString(5));
                run.RawData = (byte[])r.GetValue(6);
                runs.Add(run);
              }
            }
          }
        }
        return runs;
      }
    }

    public ICollection<RunEntry> GetRuns(ProcessStatus status) {
      lock(bigLock) {
        List<RunEntry> runs = new List<RunEntry>();
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select Id, AgentId, CreationTime, StartTime, FinishedTime, Status, Rawdata from Run where Status=@Status";
            DbParameter statusParameter = c.CreateParameter();
            statusParameter.ParameterName = "@Status";
            statusParameter.Value = status;
            c.Parameters.Add(statusParameter);

            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                RunEntry run = new RunEntry();
                run.Id = r.GetInt32(0);
                run.AgentId = r.GetInt32(1);
                run.CreationTime = r.GetDateTime(2);
                run.StartTime = r.IsDBNull(3)? null : new Nullable<DateTime>(r.GetDateTime(3));
                run.FinishedTime = r.IsDBNull(4) ? null:new Nullable<DateTime>(r.GetDateTime(4));
                run.Status = (ProcessStatus)Enum.Parse(typeof(ProcessStatus), r.GetString(5));
                run.RawData = (byte[])r.GetValue(6);
                runs.Add(run);
              }
            }
          }
        }
        return runs;
      }
    }

    // (ID integer primary key autoincrement, RunId integer, ResultId integer, CreationTime DateTime, RawData Blob)";
    public ICollection<ResultEntry> GetResults(long runId) {
      lock(bigLock) {
        List<ResultEntry> results = new List<ResultEntry>();
        using(DbConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbCommand c = cnn.CreateCommand()) {
            c.CommandText = "Select Id, RunId, CreationTime, Rawdata from Result";
            using(DbDataReader r = c.ExecuteReader()) {
              while(r.Read()) {
                ResultEntry result = new ResultEntry();
                result.Id = r.GetInt32(0);
                result.RunId = r.GetInt32(1);
                result.CreationTime = r.GetDateTime(2);
                result.RawData = (byte[])r.GetValue(3);
                results.Add(result);
              }
            }
          }
        }
        return results;
      }
    }

    public ICollection<ResultEntry> GetSubResults(long resultId) {
      throw new NotImplementedException();
    }
    #endregion
  }
}