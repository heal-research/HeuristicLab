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

using System.Collections.Generic;
using System.Windows.Forms;

namespace HeuristicLab.Clients.Hive.JobManager.ExtensionMethods {
  public static class TreeNodeExtensions {
    public static IEnumerable<TreeNode> DescendantNodes(this TreeNode root) {
      foreach (TreeNode node in root.Nodes) {
        yield return node;
        foreach (TreeNode child in node.DescendantNodes())
          yield return child;
      }
    }
  }
}
