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

using System;
using System.Text;
using System.Text.RegularExpressions;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Auxiliary;

namespace HeuristicLab.DebugEngine {
  public static class Utils {

    public static string Name(IAtomicOperation operation) {
      return string.IsNullOrEmpty(operation.Operator.Name) ? operation.Operator.ItemName : operation.Operator.Name;
    }

    public static string TypeName(object obj) {
      if (obj == null)
        return "null";
      return TypeNameParser.Parse(obj.GetType().FullName).GetTypeNameInCode(true);
    }

    public static string Wrap(string text, int columns) {
      StringBuilder sb = new StringBuilder();
      Regex whitespace = new Regex("\\s");
      int lineLength = 0;
      foreach (var word in whitespace.Split(text)) {
        if (lineLength + word.Length < columns) {
          if (lineLength > 0) {
            sb.Append(' ');
            lineLength++;
          }
          sb.Append(word);
          lineLength += word.Length;
        } else {
          sb.Append(Environment.NewLine).Append(word);
          lineLength = word.Length;
        }
      }
      return sb.ToString();
    }
  }
}
