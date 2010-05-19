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

using HeuristicLab.Core;

namespace HeuristicLab.Problems.ExternalEvaluation {
  public interface IExternalEvaluationDriver : IItem {
    /// <summary>
    /// A flag that describes whether the driver has been initialized or not.
    /// </summary>
    bool IsInitialized { get; }
    /// <summary>
    /// Tells the driver to start.
    /// </summary>
    /// <remarks>
    /// Must be called before calling <seealso cref="Send"/> or <seealso cref="Receive"/>.
    /// The concrete implementation of the driver may require additional information on how to start a connection.
    /// </remarks>
    void Start();
    /// <summary>
    /// Tells the driver to send a particular message.
    /// </summary>
    /// <param name="msg">The message that should be transmitted.</param>
    /// <remarks>
    /// This call is non-blocking, the message need not be received synchronously.
    /// </remarks>
    void Send(MainMessage msg);
    /// <summary>
    /// Tells the driver to receive the next message.
    /// </summary>
    /// <exception cref="ConnectionTerminatedException">Thrown when the other client sent a message saying it will end the connection. In this case <see cref="Stop"/> is called automatically.</exception>
    /// <exception cref="NotSupportedException">Thrown when there is no notion of a "next" message for a given driver.</exception>
    /// <returns>The message that was received or null if no message is present.</returns>
    /// <remarks>
    /// This call is non-blocking and returns null if no message is present.
    /// </remarks>
    MainMessage Receive();
    /// <summary>
    /// Tells the driver to receive the next message of the type given in the parameter.
    /// </summary>
    /// <exception cref="NotSupportedException">Some implementations may not support receiving a specific message.</exception>
    /// <returns>The message that was received.</returns>
    /// <remarks>
    /// This call is non-blocking and returns null if no message is present.
    /// </remarks>
    /// <param name="messageDescriptor">The fullname of the descriptor for the message type to receive.</param>
    MainMessage Receive(MainMessage message);
    /// <summary>
    /// Tells the driver to stop and terminate open connections.
    /// </summary>
    void Stop();
  }
}
