#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.IO;
using Google.ProtocolBuffers;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;
using System;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [Item("SymbolicExpressionTreeBinaryConverter", "Converts a symbolic expression tree into a binary representation by prefix iteration over all nodes in the tree. The binary format is defined in HeuristicLab.Persistence.")]
  [StorableType("E3C9DE32-6EF2-4BA5-AFDF-23AE7D198AC6")]
  [Obsolete("Use the SymbolicExpressionTreeProtobufConverter instead; The SymbolicExpressionTreeBinaryConverter uses the old serialization format and will be removed in the next major release of HeuristicLab.")]
  public class SymbolicExpressionTreeBinaryConverter : SymbolicExpressionTreeConverter {
    [StorableConstructor]
    protected SymbolicExpressionTreeBinaryConverter(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeBinaryConverter(SymbolicExpressionTreeBinaryConverter original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeBinaryConverter(this, cloner);
    }
    public SymbolicExpressionTreeBinaryConverter() : base() { }

    protected override void ConvertSymbolicExpressionTree(SymbolicExpressionTree tree, string name, SolutionMessage.Builder builder) {
      using (MemoryStream memoryStream = new MemoryStream()) {
        Persistence.Default.Xml.XmlGenerator.Serialize(tree, memoryStream);
        byte[] byteRep = memoryStream.ToArray();
        SolutionMessage.Types.RawVariable.Builder rawVariable = SolutionMessage.Types.RawVariable.CreateBuilder();
        rawVariable.SetName(name).SetData(ByteString.CopyFrom(byteRep));
        builder.AddRawVars(rawVariable.Build());
      }
    }
  }
}
