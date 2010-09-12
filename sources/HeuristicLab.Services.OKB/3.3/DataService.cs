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
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Implementation of the <see cref="IDataService"/>.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
  class DataService : IDisposable, IDataService {
    private enum Mode { Request, Submit, None };

    private Mode mode = Mode.None;
    private EntityType type;
    private int id = -1;
    private MemoryStream dataStream;

    private void EnsureInit() {
      if (mode != Mode.None)
        throw new FaultException(String.Format("Cannot service new request while processing another {0}-Operation.", mode));
    }

    private byte[] GetData(EntityType type, int id) {
      using (OKBDataContext okb = new OKBDataContext()) {
        switch (type) {
          case EntityType.Algorithm:
            Algorithm algorithm = okb.Algorithms.Single(a => a.Id == id);
            if (algorithm.AlgorithmData == null) {
              algorithm.AlgorithmData = new AlgorithmData() {
                AlgorithmId = algorithm.Id,
                Data = new Binary(new byte[0])
              };
              okb.SubmitChanges();
            }
            return algorithm.AlgorithmData.Data.ToArray();
          case EntityType.Problem:
            Problem problem = okb.Problems.Single(p => p.Id == id);
            if (problem.ProblemData == null) {
              problem.ProblemData = new ProblemData() {
                ProblemId = problem.Id,
                Data = new Binary(new byte[0])
              };
              okb.SubmitChanges();
            }
            return problem.ProblemData.Data.ToArray();
          default:
            throw new FaultException("Unsupported EntityType.");
        }
      }
    }

    private void SetData(EntityType type, int id, byte[] data) {
      using (OKBDataContext okb = new OKBDataContext()) {
        switch (type) {
          case EntityType.Algorithm:
            Algorithm algorithm = okb.Algorithms.Single(a => a.Id == id);
            if (algorithm.AlgorithmData == null)
              algorithm.AlgorithmData = new AlgorithmData() {
                AlgorithmId = algorithm.Id,
                Data = new Binary(new byte[0])
              };
            algorithm.AlgorithmData.Data = new Binary(data);
            okb.SubmitChanges();
            break;
          case EntityType.Problem:
            Problem problem = okb.Problems.Single(p => p.Id == id);
            if (problem.ProblemData == null)
              problem.ProblemData = new ProblemData() {
                ProblemId = problem.Id,
                Data = new Binary(new byte[0])
              };
            problem.ProblemData.Data = new Binary(data);
            okb.SubmitChanges();
            break;
          default:
            throw new FaultException("Unsupported EntityType.");
        }
      }
    }

    #region IDataService Members
    /// <summary>
    /// Request the specified <see cref="Algorithm"/> or <see cref="Problem"/>.
    /// </summary>
    /// <param name="type">The entity type.</param>
    /// <param name="id">The entity id.</param>
    /// <returns>The size of the data blob.</returns>
    public int Request(EntityType type, int id) {
      EnsureInit();
      dataStream = new MemoryStream(GetData(type, id));
      mode = Mode.Request;
      return (int)dataStream.Length;
    }

    /// <summary>
    /// Gets the next chunk of bytes.
    /// </summary>
    /// <param name="size">The maximum number of bytes to transfer.</param>
    /// <returns>An array of bytes.</returns>
    public byte[] GetNextChunk(int size) {
      if (dataStream == null || mode != Mode.Request)
        throw new FaultException("No data has been prepared, call Request first.");
      byte[] chunk = new byte[Math.Min(size, dataStream.Length - dataStream.Position)];
      dataStream.Read(chunk, 0, chunk.Length);
      return chunk;
    }

    /// <summary>
    /// Prepare submission of the specified <see cref="Algorithm"/> or <see cref="Problem"/>.
    /// </summary>
    /// <param name="type">The entity type.</param>
    /// <param name="id">The entity id.</param>
    public void Submit(EntityType type, int id) {
      EnsureInit();
      GetData(type, id);
      this.type = type;
      this.id = id;
      mode = Mode.Submit;
      dataStream = new MemoryStream();
    }

    /// <summary>
    /// Sets the next chunk of bytes.
    /// </summary>
    /// <param name="data">The data.</param>
    public void SetNextChunk(byte[] data) {
      dataStream.Write(data, 0, data.Length);
    }

    /// <summary>
    /// Commits the transaction in case of an upload and closes
    /// the connection.
    /// </summary>
    public void TransferDone() {
      if (mode == Mode.Submit)
        SetData(type, id, dataStream.ToArray());
      Dispose();
    }

    /// <summary>
    /// Aborts the transfer.
    /// </summary>
    public void AbortTransfer() {
      Dispose();
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
      mode = Mode.None;
      if (dataStream != null)
        dataStream.Dispose();
    }
    #endregion
  }
}
