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

using System.Net.Security;
using System.ServiceModel;

namespace HeuristicLab.Services.OKB {
  /// <summary>
  /// Choose between <see cref="Algorithm"/> or <see cref="Problem"/>.
  /// </summary>
  public enum EntityType {
    /// <summary>
    /// Use <see cref="Algorithm"/> as entity type.
    /// </summary>
    Algorithm,
    /// <summary>
    /// Use <see cref="Problem"/> as entity type.
    /// </summary>
    Problem
  };

  /// <summary>
  /// Download and upload serialized implementations of <see cref="Algorithm"/>s and <see cref="Problem"/>s.
  /// The transfer has to be initiate through either <see cref="IDataService.Request"/> or <see cref="IDataService.Submit"/>
  /// followed by either <see cref="IDataService.GetNextChunk"/> or <see cref="IDataService.SetNextChunk"/> and terminated
  /// with either <see cref="IDataService.TransferDone"/> or <see cref="IDataService.AbortTransfer"/>.
  /// </summary>
  [ServiceContract(SessionMode = SessionMode.Required, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
  public interface IDataService {

    /// <summary>
    /// Request the specified <see cref="Algorithm"/> or <see cref="Problem"/>.
    /// </summary>
    /// <param name="type">The entity type.</param>
    /// <param name="id">The entity id.</param>
    /// <returns>The size of the data blob.</returns>
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    int Request(EntityType type, int id);

    /// <summary>
    /// Prepare submission of the specified <see cref="Algorithm"/> or <see cref="Problem"/>.
    /// </summary>
    /// <param name="type">The entity type.</param>
    /// <param name="id">The entity id.</param>
    [OperationContract(IsInitiating = true, IsTerminating = false)]
    void Submit(EntityType type, int id);

    /// <summary>
    /// Gets the next chunk of bytes.
    /// </summary>
    /// <param name="size">The maximum number of bytes to transfer.</param>
    /// <returns>An array of bytes.</returns>
    [OperationContract(IsInitiating = false, IsTerminating = false)]
    byte[] GetNextChunk(int size);

    /// <summary>
    /// Sets the next chunk of bytes.
    /// </summary>
    /// <param name="data">The data.</param>
    [OperationContract(IsInitiating = false, IsTerminating = false)]
    void SetNextChunk(byte[] data);

    /// <summary>
    /// Commits the transaction in case of an upload and closes
    /// the connection.
    /// </summary>
    [OperationContract(IsInitiating = false, IsTerminating = true)]
    void TransferDone();

    /// <summary>
    /// Aborts the transfer.
    /// </summary>
    [OperationContract(IsInitiating = false, IsTerminating = true)]
    void AbortTransfer();
  }
}
