#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using Google.ProtocolBuffers;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.ExternalEvaluation {
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
    IMessage Receive(IBuilder builder, ExtensionRegistry extensions);
    /// <summary>
    /// Tells the channel to close down and terminate open connections.
    /// </summary>
    void Close();
  }
}
