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

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Vertebral_3C : IUCIDataDescriptor {
    public string Name { get { return "Vertebral_3C"; } }
    public string Description {
      get {
        return "Data Set Information:" + Environment.NewLine
        + "Biomedical data set built by Dr. Henrique da Mota during a medical residence "
        + "period in the Group of Applied Research in Orthopaedics (GARO) of the Centre "
        + "Médico-Chirurgical de Réadaptation des Massues, Lyon, France. The data have been "
        + "organized in two different but related classification tasks. The first task consists "
        + "in classifying patients as belonging to one out of three categories: Normal (100 patients), "
        + "Disk Hernia (60 patients) or Spondylolisthesis (150 patients)." + Environment.NewLine
        + "Each patient is represented in the data set by six biomechanical attributes derived from "
        + "the shape and orientation of the pelvis and lumbar spine (in this order): pelvic incidence, "
        + "pelvic tilt, lumbar lordosis angle, sacral slope, pelvic radius and grade of spondylolisthesis." + Environment.NewLine
        + "Note: Normal has the value '0', Spondylolisthesis is '1' and Disk Hernia = '2'.";
      }
    }
    public string Donor { get { return "Dr. Henrique da Mota"; } }
    public int Year { get { return 2011; } }
  }
}
