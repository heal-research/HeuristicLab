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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  internal class ParetoFrontStore {
    internal static IEnumerable<double[]> GetParetoFront(String filename) {
      List<double[]> data = new List<double[]>();
      var assembly = Assembly.GetExecutingAssembly();
      String ressourcename = typeof(ParetoFrontStore).Namespace + ".TestFunctions." + filename + ".pf";

      //check if file is listed
      var names = assembly.GetManifestResourceNames();
      if (!names.Contains(ressourcename)) return null;

      //read from file
      var ressourceReader = new StreamReader(assembly.GetManifestResourceStream(ressourcename));
      String s;
      while ((s = ressourceReader.ReadLine()) != null) {
        var tokens = s.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
        double[] point = new double[tokens.Length];
        for (int i = 0; i < point.Length; i++) {
          point[i] = Double.Parse(tokens[i], CultureInfo.InvariantCulture);
        }
        data.Add(point);
      }
      return data;
    }
  }
}
