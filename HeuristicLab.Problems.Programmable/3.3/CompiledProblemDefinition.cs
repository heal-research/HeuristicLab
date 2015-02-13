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
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.Programmable {
  public abstract class CompiledProblemDefinition : IProblemDefinition {
    private IEncoding encoding;
    public IEncoding Encoding {
      get { return encoding; }
      protected set {
        if (value == null) throw new ArgumentNullException("The encoding must not be null.");
        encoding = value;
      }
    }

    public dynamic vars { get; set; }
    public abstract void Initialize();

    protected CompiledProblemDefinition() { }
    protected CompiledProblemDefinition(IEncoding encoding)
      : base() {
      Encoding = encoding;
    }
  }
}
