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
using System.Globalization;
using System.IO;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// Parses a *.opt.tour file in TSPLIB format and extracts its information about an optimal tour of a TSP.
  /// </summary>
  public class TSPLIBTourParser {
    private const int EOF = 0;
    private const int NAME = 1;
    private const int TYPE = 2;
    private const int COMMENT = 3;
    private const int DIM = 4;
    private const int TOURSECTION = 5;

    private StreamReader source;

    private string name;
    /// <summary>
    /// Gets the name of the parsed tour.
    /// </summary>
    public string Name {
      get { return name; }
    }
    private string comment;
    /// <summary>
    /// Gets the comment of the parsed tour.
    /// </summary>
    public string Comment {
      get { return comment; }
    }
    private int[] tour;
    /// <summary>
    /// Gets the parsed tour.
    /// </summary>
    public int[] Tour {
      get { return tour; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TSPLIBTourParser"/> with the given <paramref name="path"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the input file is not a TSPLIB optimal tour file (*.opt.tour)
    /// </exception>
    /// <param name="path">The path where the optimal tour is stored.</param>
    public TSPLIBTourParser(String path) {
      if (!path.EndsWith(".opt.tour"))
        throw new ArgumentException("Input file has to be a TSPLIB optimal tour file (*.opt.tour).");

      source = new StreamReader(path);
      name = path;
      comment = string.Empty;
      tour = null;
    }

    /// <summary>
    /// Reads the TSPLIB optimal tour file and parses its elements.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown if the file has an invalid format or contains invalid data.</exception>
    public void Parse() {
      int section = -1;
      string str = null;
      bool typeIsChecked = false;

      do {
        str = source.ReadLine();
        section = GetSection(str);

        if (section != -1) {
          switch (section) {
            case NAME:
              ReadName(str);
              break;
            case TYPE:
              CheckType(str);
              typeIsChecked = true;
              break;
            case COMMENT:
              ReadComment(str);
              break;
            case DIM:
              InitTour(str);
              break;
            case TOURSECTION:
              ReadTour();
              break;
          }
        }
      } while (!((section == EOF) || (str == null)));

      if (!typeIsChecked)
        throw new InvalidDataException("Input file does not contain type information.");
    }

    private int GetSection(string str) {
      if (str == null)
        return EOF;

      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      if (tokens.Length == 0)
        return -1;

      string token = tokens[0].Trim();
      if (token.Equals("eof", StringComparison.OrdinalIgnoreCase))
        return EOF;
      if (token.Equals("name", StringComparison.OrdinalIgnoreCase))
        return NAME;
      if (token.Equals("type", StringComparison.OrdinalIgnoreCase))
        return TYPE;
      if (token.Equals("comment", StringComparison.OrdinalIgnoreCase))
        return COMMENT;
      if (token.Equals("dimension", StringComparison.OrdinalIgnoreCase))
        return DIM;
      if (token.Equals("tour_section", StringComparison.OrdinalIgnoreCase))
        return TOURSECTION;

      return -1;
    }

    private void ReadName(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      name = tokens[tokens.Length - 1].Trim();
    }

    private void CheckType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);

      string type = tokens[tokens.Length - 1].Trim();
      if (!type.Equals("tour", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException("Input data format is not \"TOUR\"");
    }

    private void ReadComment(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      comment = tokens[tokens.Length - 1].Trim();
    }

    private void InitTour(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string dimension = tokens[tokens.Length - 1].Trim();

      int dim = Int32.Parse(dimension);
      tour = new int[dim];
    }

    private void ReadTour() {
      if (tour == null)
        throw new InvalidDataException("Input file does not contain dimension information.");

      for (int i = 0; i < (tour.Length); i++) {
        string str = source.ReadLine();

        CultureInfo culture = new CultureInfo("en-US");
        tour[i] = int.Parse(str, culture.NumberFormat) - 1;
      }
    }
  }
}
