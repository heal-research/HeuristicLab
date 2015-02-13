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

namespace HeuristicLab.Problems.Instances.VehicleRouting {
  public class TaillardInstanceProvider : TaillardFormatInstanceProvider {
    public override string Name {
      get { return "Taillard (CVRP)"; }
    }

    public override string Description {
      get { return "Taillard test set"; }
    }

    public override Uri WebLink {
      get { return new Uri(@"http://mistic.heig-vd.ch/taillard/problemes.dir/vrp.dir/vrp.html"); }
    }

    public override string ReferencePublication {
      get { return null; }
    }

    protected override string FileName {
      get { return "Taillard"; }
    }
  }
}
