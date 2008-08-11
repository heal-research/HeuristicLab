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
using System.Data;
using System.Data.SQLite;
using System.Data.Common;
using System.Threading;

namespace HeuristicLab.Grid {
  class Database {
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
              cmd.CommandText = "CREATE TABLE Job (ID integer primary key autoincrement, Guid Guid, Status text, CreationTime DateTime, StartTime DateTime, RawData blob)";
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

    internal void InsertJob(Guid guid, JobState jobState, byte[] rawData) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(DbTransaction t = cnn.BeginTransaction()) {
            using(DbCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Insert into Job (Guid, Status, CreationTime, StartTime, RawData) values (@Guid, @Status, @CreationTime, @StartTime, @RawData)";
              DbParameter guidParam = c.CreateParameter();
              guidParam.ParameterName = "@Guid";
              guidParam.Value = guid;
              c.Parameters.Add(guidParam);
              DbParameter statusParam = c.CreateParameter();
              statusParam.ParameterName = "@Status";
              statusParam.Value = jobState.ToString();
              c.Parameters.Add(statusParam);
              DbParameter creationParam = c.CreateParameter();
              creationParam.ParameterName = "@CreationTime";
              DateTime now = DateTime.Now;
              creationParam.Value = now;
              c.Parameters.Add(creationParam);
              DbParameter startParam = c.CreateParameter();
              startParam.ParameterName = "@StartTime";
              if(jobState == JobState.Busy) startParam.Value = now;
              else startParam.Value = null;
              c.Parameters.Add(startParam);
              DbParameter rawDataParam = c.CreateParameter();
              rawDataParam.ParameterName = "@RawData";
              rawDataParam.Value = rawData;
              c.Parameters.Add(rawDataParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }

    }

    internal List<JobEntry> GetWaitingJobs() {
      rwLock.EnterReadLock();
      List<JobEntry> jobs = new List<JobEntry>();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          SQLiteCommand c = cnn.CreateCommand();
          c.CommandText = "Select Guid, CreationTime, StartTime, Rawdata from Job where Status=@Status";
          DbParameter statusParameter = c.CreateParameter();
          statusParameter.ParameterName = "@Status";
          statusParameter.Value = JobState.Waiting.ToString();
          c.Parameters.Add(statusParameter);
          SQLiteDataReader r = c.ExecuteReader();
          while(r.Read()) {
            JobEntry job = new JobEntry();
            job.Status = JobState.Waiting;
            job.Guid = r.GetGuid(0);
            job.CreationTime = r.GetDateTime(1);
            job.StartTime = r.IsDBNull(2)?null:new Nullable<DateTime>(r.GetDateTime(2));
            job.RawData = (byte[])r.GetValue(3);
            jobs.Add(job);
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return jobs;
    }

    internal void SetJobResult(Guid guid, byte[] result) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Job set Status=@Status, RawData=@RawData where Guid=@Guid";
              DbParameter rawDataParam = c.CreateParameter();
              DbParameter statusParam = c.CreateParameter();
              DbParameter guidParam = c.CreateParameter();
              rawDataParam.ParameterName = "@RawData";
              rawDataParam.Value = result;
              guidParam.ParameterName = "@Guid";
              guidParam.Value = guid;
              statusParam.ParameterName = "@Status";
              statusParam.Value = JobState.Finished;
              c.Parameters.Add(rawDataParam);
              c.Parameters.Add(statusParam);
              c.Parameters.Add(guidParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    internal void UpdateJobState(Guid guid, JobState jobState) {
      rwLock.EnterWriteLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          using(SQLiteTransaction t = cnn.BeginTransaction()) {
            using(SQLiteCommand c = cnn.CreateCommand()) {
              c.Transaction = t;
              c.CommandText = "Update Job set Status=@Status, StartTime=@StartTime where Guid=@Guid";
              DbParameter statusParam = c.CreateParameter();
              DbParameter startTimeParam = c.CreateParameter();
              DbParameter guidParam = c.CreateParameter();
              startTimeParam.ParameterName = "@StartTime";
              if(jobState == JobState.Busy)
                startTimeParam.Value = DateTime.Now;
              else
                startTimeParam.Value = null;
              guidParam.ParameterName = "@Guid";
              guidParam.Value = guid;
              statusParam.ParameterName = "@Status";
              statusParam.Value = jobState;
              c.Parameters.Add(startTimeParam);
              c.Parameters.Add(statusParam);
              c.Parameters.Add(guidParam);
              c.ExecuteNonQuery();
            }
            t.Commit();
          }
        }
      } finally {
        rwLock.ExitWriteLock();
      }
    }


    internal JobEntry GetJob(Guid guid) {
      rwLock.EnterReadLock();
      try {
        using(SQLiteConnection cnn = new SQLiteConnection(connectionString)) {
          cnn.Open();
          SQLiteCommand c = cnn.CreateCommand();
          c.CommandText = "Select Status, CreationTime, StartTime, Rawdata from Job where Guid=@Guid";
          DbParameter guidParameter = c.CreateParameter();
          guidParameter.ParameterName = "@Guid";
          guidParameter.Value = guid;
          c.Parameters.Add(guidParameter);
          SQLiteDataReader r = c.ExecuteReader();
          while(r.Read()) {
            JobEntry job = new JobEntry();
            job.Guid = guid;
            job.Status = (JobState)Enum.Parse(typeof(JobState), r.GetString(0));
            job.CreationTime = r.GetDateTime(1);
            job.StartTime = r.IsDBNull(2)?null:new Nullable<DateTime>(r.GetDateTime(2));
            job.RawData = (byte[])r.GetValue(3);
            return job;
          }
        }
      } finally {
        rwLock.ExitReadLock();
      }
      return null;
    }

    /// <summary>
    /// Does nothing right now (= running jobs that disappear are never restarted).
    /// </summary>
    internal void RestartExpiredActiveJobs() {
    }

    /// <summary>
    /// Does nothing right now (= results are never deleted).
    /// </summary>
    internal void DeleteExpiredResults() {
    }
  }
}