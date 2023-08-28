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

using System;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Problems.Dynamic {

  [StorableType("0D21494D-233C-4EF0-8263-06F3AAEEF4C8")]
  public interface IEpochClock : IParameterizedNamedItem {
    long CurrentEpoch { get; }
    long CurrentTime { get; }
    
    void Start(bool throwEvent = true);
    void Pause();
    void Resume();
    void Stop();
    void Reset(bool throwEvent = false);
    
    void Tick();
 
    event EventHandler<EventArgs<long, long>> NewVersion;
  }
}
