#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  public class MicroarrayQAPInstanceProvider : QAPLIBInstanceProvider {
    
    public override string Name {
      get { return "Microarray QAP"; }
    }

    public override string Description {
      get { return "Microarray placement problems (QAPLIB format)"; }
    }

    public override Uri WebLink {
      get { return new Uri("http://www.rahmannlab.de/research/microarray-design"); }
    }

    public override string ReferencePublication {
      get {
        return @"S.A. de Carvalho Jr, S. Rahmann. 2006.
Microarray layout as a quadratic assignment problem.
D. Huson, O. Kohlbacher, A. Lupas, K. Nieselt, A. Zell (Eds.),
Proceedings of the German Conference on Bioinformatics, vol. 83, pp. 11-20.
Gesellschaft für Informatik Bonn, Germany.";
      }
    }

    protected override string FileName { get { return "microarray"; } }
  }
}
