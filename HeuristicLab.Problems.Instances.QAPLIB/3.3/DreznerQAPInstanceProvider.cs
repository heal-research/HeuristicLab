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

namespace HeuristicLab.Problems.Instances.QAPLIB {
  public class DreznerQAPInstanceProvider : QAPLIBInstanceProvider {
    
    public override string Name {
      get { return "Drezner's QAP instances"; }
    }

    public override string Description {
      get { return "QAP instances from Zvi Drezner"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://cbeweb-1.fullerton.edu/isds/zdrezner/programs.htm"); }
    }

    public override string ReferencePublication {
      get {
        return @"Z. Drezner, P.M. Hahn, E.D. Taillard. 2005.
Recent Advances for the Quadratic Assignment Problem with Special Emphasis on Instances that are Difficult for Meta-heuristic Methods.
Annals of Operations Research, 139, pp. 65–94.";
      }
    }

    protected override string FileName { get { return "drezner"; } }
  }
}
