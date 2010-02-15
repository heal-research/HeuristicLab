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

using System;
using System.Globalization;
using System.IO;

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// Parses a *.tsp file in the TSPLIB format and extracts its information about a TSP.
  /// </summary>
  public class TSPParser {
    private const int EOF = 0;
    private const int NAME = 1;
    private const int TYPE = 2;
    private const int DIM = 3;
    private const int WEIGHTTYPE = 4;
    private const int NODETYPE = 5;
    private const int NODESECTION = 6;

    private StreamReader source;

    private string myName;
    /// <summary>
    /// Gets the name of the parsed TSP.
    /// </summary>
    public string Name {
      get { return myName; }
    }
    private double[,] myVertices;
    /// <summary>
    /// Gets the vertices of the parsed TSP.
    /// </summary>
    public double[,] Vertices {
      get { return myVertices; }
    }
    private int myWeightType;
    /// <summary>
    /// Gets the weight type of the parsed TSP.
    /// </summary>
    public int WeightType {
      get { return myWeightType; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TSPParser"/> with the given <paramref name="path"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the input file name is not in TSP format (*.tsp)
    /// </exception>
    /// <param name="path">The path where the TSP is stored.</param>
    public TSPParser(String path) {
      if (!path.EndsWith(".tsp"))
        throw new ArgumentException("Input file name has to be in TSP format (*.tsp)");

      source = new StreamReader(path);
      myName = path;
      myVertices = null;
      myWeightType = -1;
    }

    /// <summary>
    /// Reads the TSP file and parses the elements.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown when file contains unknown (edge) types.</exception>
    public void Parse() {
      int section = -1;
      string str = null;
      bool typeIsChecked = false;
      bool weightTypeIsChecked = false;

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
            case DIM:
              InitVerticesArray(str);
              break;
            case WEIGHTTYPE:
              ReadWeightType(str);
              weightTypeIsChecked = true;
              break;
            case NODETYPE:
              CheckNodeType(str);
              break;
            case NODESECTION:
              ReadVertices();
              break;
          }
        }
      } while (!((section == EOF) || (str == null)));

      if (!(typeIsChecked && weightTypeIsChecked))
        throw new InvalidDataException("File contains unknown (edge) types");
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
      if (token.Equals("dimension", StringComparison.OrdinalIgnoreCase))
        return DIM;
      if (token.Equals("edge_weight_type", StringComparison.OrdinalIgnoreCase))
        return WEIGHTTYPE;
      if (token.Equals("node_coord_type", StringComparison.OrdinalIgnoreCase))
        return NODETYPE;
      if (token.Equals("node_coord_section", StringComparison.OrdinalIgnoreCase))
        return NODESECTION;

      return -1;
    }

    private void ReadName(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      myName = tokens[tokens.Length - 1].Trim();
    }

    private void CheckType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);

      string type = tokens[tokens.Length - 1].Trim();
      if (!type.Equals("tsp", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException("Input data format is not \"TSP\"");
    }

    private void InitVerticesArray(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string dimension = tokens[tokens.Length - 1].Trim();

      int dim = Int32.Parse(dimension);
      myVertices = new double[dim, 2];
    }

    private void ReadWeightType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string type = tokens[tokens.Length - 1].Trim();

      if (type.Equals("euc_2d", StringComparison.OrdinalIgnoreCase))
        myWeightType = 0;
      else if (type.Equals("geo", StringComparison.OrdinalIgnoreCase))
        myWeightType = 1;
      else
        throw new InvalidDataException("Unsupported type of edge weights");
    }

    private void CheckNodeType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string type = tokens[tokens.Length - 1].Trim();

      if (!type.Equals("twod_coords", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException("Unsupported node type");
    }

    private void ReadVertices() {
      if (myVertices == null)
        throw new InvalidDataException("Dimension not found");

      for (int i = 0; i < (myVertices.Length / 2); i++) {
        string str = source.ReadLine();
        string[] tokens = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length != 3)
          throw new InvalidDataException("Invalid node format");

        CultureInfo culture = new CultureInfo("en-US");
        myVertices[i, 0] = Double.Parse(tokens[1], culture.NumberFormat);
        myVertices[i, 1] = Double.Parse(tokens[2], culture.NumberFormat);
      }
    }
  }
}
