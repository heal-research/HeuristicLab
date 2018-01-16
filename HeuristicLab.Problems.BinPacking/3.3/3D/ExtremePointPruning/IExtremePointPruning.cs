#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointPruning {
  public interface IExtremePointPruning {

    /// <summary>
    /// Prunes the extreme points of the given bin depending on the given pruning method.
    /// </summary>
    /// <param name="pruningMethod"></param>
    /// <param name="bin"></param>
    /// <param name="positions"></param>
    void PruneExtremePoints(ExtremePointPruningMethod pruningMethod, PackingShape bin, Dictionary<BinPacking3D, List<KeyValuePair<int, PackingPosition>>> positions);

    /// <summary>
    /// Prunes the extreme points of the given bins depending on the given pruning method.
    /// </summary>
    /// <param name="pruningMethod"></param>
    /// <param name="bins"></param>
    void PruneExtremePoints(ExtremePointPruningMethod pruningMethod, IList<BinPacking3D> binPackings);
  }
}
