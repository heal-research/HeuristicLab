#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Core {

  /// <summary>
  /// Base class for serialization generators. Provides a common entry point
  /// <c>Format</c> and dispatches to different abstract methods for
  /// every token type.
  /// </summary>
  /// <typeparam name="T">The type of the serialization format.</typeparam>
  public abstract class GeneratorBase<T> {

    /// <summary>
    /// Processes a serialization token and formats the specified token.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <returns>An object suitable for serialziation</returns>
    public T Format(ISerializationToken token) {
      Type type = token.GetType();
      if (type == typeof(BeginToken))
        return Format((BeginToken)token);
      if (type == typeof(EndToken))
        return Format((EndToken)token);
      if (type == typeof(PrimitiveToken))
        return Format((PrimitiveToken)token);
      if (type == typeof(ReferenceToken))
        return Format((ReferenceToken)token);
      if (type == typeof(NullReferenceToken))
        return Format((NullReferenceToken)token);
      if (type == typeof(MetaInfoBeginToken))
        return Format((MetaInfoBeginToken)token);
      if (type == typeof(MetaInfoEndToken))
        return Format((MetaInfoEndToken)token);
      if (type == typeof(TypeToken))
        return Format((TypeToken)token);
      throw new ApplicationException("Invalid token of type " + type.FullName);
    }

    /// <summary>
    /// Formats the specified begin token.
    /// </summary>
    /// <param name="beginToken">The begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(BeginToken beginToken);

    /// <summary>
    /// Formats the specified end token.
    /// </summary>
    /// <param name="endToken">The end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(EndToken endToken);

    /// <summary>
    /// Formats the specified primitive token.
    /// </summary>
    /// <param name="primitiveToken">The primitive token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(PrimitiveToken primitiveToken);

    /// <summary>
    /// Formats the specified reference token.
    /// </summary>
    /// <param name="referenceToken">The reference token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(ReferenceToken referenceToken);

    /// <summary>
    /// Formats the specified null reference token.
    /// </summary>
    /// <param name="nullReferenceToken">The null reference token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(NullReferenceToken nullReferenceToken);

    /// <summary>
    /// Formats the specified meta info begin token.
    /// </summary>
    /// <param name="metaInfoBeginToken">The meta info begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(MetaInfoBeginToken metaInfoBeginToken);

    /// <summary>
    /// Formats the specified meta info end token.
    /// </summary>
    /// <param name="metaInfoEndToken">The meta info end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(MetaInfoEndToken metaInfoEndToken);

    /// <summary>
    /// Formats the specified type token.
    /// </summary>
    /// <param name="typeToken">The type token.</param>
    /// <returns>The token in serialized form.</returns>
    protected abstract T Format(TypeToken typeToken);

  }
}