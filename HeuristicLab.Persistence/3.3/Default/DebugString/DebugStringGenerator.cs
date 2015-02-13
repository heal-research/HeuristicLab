#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Text;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Core.Tokens;
using HeuristicLab.Persistence.Interfaces;

namespace HeuristicLab.Persistence.Default.DebugString {

  /// <summary>
  /// Generate a string that recursively describes an object graph.
  /// </summary>
  public class DebugStringGenerator : GeneratorBase<string> {

    private bool isSepReq;
    private readonly bool showRefs;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugStringGenerator"/> class.
    /// </summary>
    public DebugStringGenerator() : this(true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugStringGenerator"/> class.
    /// </summary>
    /// <param name="showRefs">if set to <c>true</c> show references.</param>
    public DebugStringGenerator(bool showRefs) {
      isSepReq = false;
      this.showRefs = showRefs;
    }

    /// <summary>
    /// Formats the specified begin token.
    /// </summary>
    /// <param name="beginToken">The begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(BeginToken beginToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(beginToken.Name)) {
        sb.Append(beginToken.Name);
        if (beginToken.Id != null && showRefs) {
          sb.Append('[');
          sb.Append(beginToken.Id);
          sb.Append(']');
        }
      }
      sb.Append("(");
      isSepReq = false;
      return sb.ToString();
    }

    /// <summary>
    /// Formats the specified end token.
    /// </summary>
    /// <param name="endToken">The end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(EndToken endToken) {
      isSepReq = true;
      return ")";
    }

    /// <summary>
    /// Formats the specified primitive token.
    /// </summary>
    /// <param name="primitiveToken">The primitive token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(PrimitiveToken primitiveToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(primitiveToken.Name)) {
        sb.Append(primitiveToken.Name);
        if (primitiveToken.Id != null && showRefs) {
          sb.Append('[');
          sb.Append(primitiveToken.Id);
          sb.Append(']');
        }
        sb.Append('=');
      }
      sb.Append(((DebugString)primitiveToken.SerialData).Data);
      isSepReq = true;
      return sb.ToString();
    }

    /// <summary>
    /// Formats the specified reference token.
    /// </summary>
    /// <param name="referenceToken">The reference token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(ReferenceToken referenceToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(referenceToken.Name)) {
        sb.Append(referenceToken.Name);
        sb.Append('=');
      }
      sb.Append('{');
      sb.Append(referenceToken.Id);
      sb.Append('}');
      isSepReq = true;
      return sb.ToString();
    }

    /// <summary>
    /// Formats the specified null reference token.
    /// </summary>
    /// <param name="nullReferenceToken">The null reference token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(NullReferenceToken nullReferenceToken) {
      StringBuilder sb = new StringBuilder();
      if (isSepReq)
        sb.Append(", ");
      if (!string.IsNullOrEmpty(nullReferenceToken.Name)) {
        sb.Append(nullReferenceToken.Name);
        sb.Append('=');
      }
      sb.Append("<null>");
      isSepReq = true;
      return sb.ToString();
    }

    /// <summary>
    /// Formats the specified meta info begin token.
    /// </summary>
    /// <param name="metaInfoBeginToken">The meta info begin token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(MetaInfoBeginToken metaInfoBeginToken) {
      return "[";
    }

    /// <summary>
    /// Formats the specified meta info end token.
    /// </summary>
    /// <param name="metaInfoEndToken">The meta info end token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(MetaInfoEndToken metaInfoEndToken) {
      return "]";
    }

    /// <summary>
    /// Formats the specified type token.
    /// </summary>
    /// <param name="typeToken">The type token.</param>
    /// <returns>The token in serialized form.</returns>
    protected override string Format(TypeToken typeToken) {
      return string.Empty;
    }

    /// <summary>
    /// Serializes the specified object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <returns>A string representation of the complete object graph</returns>
    public static string Serialize(object o) {
      return Serialize(o, ConfigurationService.Instance.GetDefaultConfig(new DebugStringFormat()));
    }

    /// <summary>
    /// Serializes the specified object.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="configuration">The persistence configuration.</param>
    /// <returns>A string representation of the complete object graph.</returns>
    public static string Serialize(object o, Configuration configuration) {
      Serializer s = new Serializer(o, configuration);
      DebugStringGenerator generator = new DebugStringGenerator();
      StringBuilder sb = new StringBuilder();
      foreach (ISerializationToken token in s) {
        sb.Append(generator.Format(token));
      }
      return sb.ToString();
    }
  }
}
