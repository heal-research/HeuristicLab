﻿#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using Google.Protobuf;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation {
  [StorableType("8e62ba0d-03f5-4243-8411-3bd5f14c42be")]
  public interface IEvaluationChannel : INamedItem {
    /// <summary>
    /// A flag that describes whether the channel has been initialized or not.
    /// </summary>
    bool IsInitialized { get; }
    /// <summary>
    /// Opens the channel for communication.
    /// </summary>
    /// <remarks>
    /// Must be called before calling <seealso cref="Send"/> or <seealso cref="Receive"/>.
    /// The concrete implementation of the channel may require additional information on how to start a connection,
    /// which should be passed into the concrete channel's constructor.
    /// </remarks>
    void Open();
    /// <summary>
    /// Sends a message over the channel.
    /// </summary>
    /// <param name="solution">The message to send.</param>
    void Send(IMessage solution);
    /// <summary>
    /// Receives a message from the channel and merges the message with the given builder.
    /// </summary>
    /// <param name="builder">The builder that must match the message type that is to be received.</param>
    /// <returns>The received message.</returns>
    IMessage Receive(IMessage builder, ExtensionRegistry extensions);
    /// <summary>
    /// Tells the channel to close down and terminate open connections.
    /// </summary>
    void Close();
  }
}
