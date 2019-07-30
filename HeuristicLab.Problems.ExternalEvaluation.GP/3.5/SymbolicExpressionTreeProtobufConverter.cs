#region License Information
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

using System.IO;
using Google.ProtocolBuffers;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.ExternalEvaluation.GP {
  [Item("SymbolicExpressionTreeProtobufConverter", "Converts a symbolic expression tree into a binary representation. The binary format is based on Google.Protobuf and defined in HeuristicLab.Persistence.")]
  [StorableType("95569F3E-0953-472E-BC20-78DBD6E572D6")]
  public class SymbolicExpressionTreeProtobufConverter : SymbolicExpressionTreeConverter {
    [StorableConstructor]
    protected SymbolicExpressionTreeProtobufConverter(StorableConstructorFlag _) : base(_) { }
    protected SymbolicExpressionTreeProtobufConverter(SymbolicExpressionTreeProtobufConverter original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicExpressionTreeProtobufConverter(this, cloner);
    }
    public SymbolicExpressionTreeProtobufConverter() : base() { }

    protected override void ConvertSymbolicExpressionTree(SymbolicExpressionTree tree, string name, SolutionMessage.Builder builder) {
      using (MemoryStream memoryStream = new MemoryStream()) {
        var ser = new ProtoBufSerializer();
        ser.Serialize(tree, memoryStream, disposeStream: false);
        memoryStream.Flush();
        byte[] byteRep = memoryStream.ToArray();
        SolutionMessage.Types.RawVariable.Builder rawVariable = SolutionMessage.Types.RawVariable.CreateBuilder();
        rawVariable.SetName(name).SetData(ByteString.CopyFrom(byteRep));
        builder.AddRawVars(rawVariable.Build());
      }
    }
  }
}
