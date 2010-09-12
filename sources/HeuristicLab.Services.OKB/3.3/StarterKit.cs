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

using System.Collections.Generic;
using System.Runtime.Serialization;
using HeuristicLab.Services.OKB.DataAccess;

namespace HeuristicLab.Services.OKB {

  /// <summary>
  /// Initial portion of object graph to enable selection of <see cref="AlgorithmClass"/>,
  /// <see cref="Algorithm"/>, <see cref="ProblemClass"/>, <see cref="Problem"/> and
  /// <see cref="Project"/>.
  /// </summary>
  [DataContract]
  public class StarterKit {
    /// <summary>
    /// Gets the list of algorithm classes. Every
    /// <see cref="AlgorithmClass"/> also contains all its
    /// <see cref="Algorithm"/>s.
    /// </summary>
    /// <value>The algorithm classes.</value>
    [DataMember]
    public IEnumerable<AlgorithmClass> AlgorithmClasses { get; set; }

    /// <summary>
    /// Gets the list of problem classes. Every
    /// <see cref="ProblemClass"/> also contains all tis
    /// <see cref="Problem"/>s.
    /// </summary>
    /// <value>The problem classes.</value>
    [DataMember]
    public IEnumerable<ProblemClass> ProblemClasses { get; set; }
  }

}
