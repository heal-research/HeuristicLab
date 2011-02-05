#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// Parses a *.tsp file in TSPLIB format and extracts its information about a TSP.
  /// </summary>
  public class TSPLIBParser {
    #region Inner Enum TSPLIBEdgeWeightType
    public enum TSPLIBEdgeWeightType {
      UNDEFINED,
      EUC_2D,
      GEO
    }
    #endregion

    private const int EOF = 0;
    private const int NAME = 1;
    private const int TYPE = 2;
    private const int COMMENT = 3;
    private const int DIM = 4;
    private const int WEIGHTTYPE = 5;
    private const int NODETYPE = 6;
    private const int NODESECTION = 7;

    private StreamReader source;

    private string name;
    /// <summary>
    /// Gets the name of the parsed TSP.
    /// </summary>
    public string Name {
      get { return name; }
    }
    private string comment;
    /// <summary>
    /// Gets the comment of the parsed TSP.
    /// </summary>
    public string Comment {
      get { return comment; }
    }
    private double[,] vertices;
    /// <summary>
    /// Gets the vertices of the parsed TSP.
    /// </summary>
    public double[,] Vertices {
      get { return vertices; }
    }
    private TSPLIBEdgeWeightType weightType;
    /// <summary>
    /// Gets the weight type of the parsed TSP.
    /// </summary>
    public TSPLIBEdgeWeightType WeightType {
      get { return weightType; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TSPLIBParser"/> with the given <paramref name="path"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if the input file is not a TSPLIB TSP file (*.tsp)
    /// </exception>
    /// <param name="path">The path where the TSP is stored.</param>
    public TSPLIBParser(String path) {
      if (!path.EndsWith(".tsp"))
        throw new ArgumentException("Input file has to be a TSPLIB TSP file (*.tsp).");

      source = new StreamReader(path);
      name = path;
      comment = string.Empty;
      vertices = null;
      weightType = TSPLIBEdgeWeightType.UNDEFINED;
    }

    /// <summary>
    /// Reads the TSPLIB TSP file and parses the elements.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown if the file has an invalid format or contains invalid data.</exception>
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
            case COMMENT:
              ReadComment(str);
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
        throw new InvalidDataException("Input file does not contain type or edge weight type information.");
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
      name = tokens[tokens.Length - 1].Trim();
    }

    private void CheckType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);

      string type = tokens[tokens.Length - 1].Trim();
      if (!type.Equals("tsp", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException("Input file type is not \"TSP\"");
    }

    private void ReadComment(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      comment = tokens[tokens.Length - 1].Trim();
    }

    private void InitVerticesArray(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string dimension = tokens[tokens.Length - 1].Trim();

      int dim = Int32.Parse(dimension);
      vertices = new double[dim, 2];
    }

    private void ReadWeightType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string type = tokens[tokens.Length - 1].Trim();

      if (type.Equals("euc_2d", StringComparison.OrdinalIgnoreCase))
        weightType = TSPLIBEdgeWeightType.EUC_2D;
      else if (type.Equals("geo", StringComparison.OrdinalIgnoreCase))
        weightType = TSPLIBEdgeWeightType.GEO;
      else
        throw new InvalidDataException("Input file contains an unsupported edge weight type (only \"EUC_2D\" and \"GEO\" are supported).");
    }

    private void CheckNodeType(string str) {
      string[] tokens = str.Split(new string[] { ":" }, StringSplitOptions.None);
      string type = tokens[tokens.Length - 1].Trim();

      if (!type.Equals("twod_coords", StringComparison.OrdinalIgnoreCase))
        throw new InvalidDataException("Input file contains an unsupported node coordinates type (only \"TWOD_COORDS\" is supported).");
    }

    private void ReadVertices() {
      if (vertices == null)
        throw new InvalidDataException("Input file does not contain dimension information.");

      for (int i = 0; i < (vertices.Length / 2); i++) {
        string str = source.ReadLine();
        string[] tokens = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length != 3)
          throw new InvalidDataException("Input file contains invalid node coordinates.");

        CultureInfo culture = new CultureInfo("en-US");
        vertices[i, 0] = double.Parse(tokens[1], culture.NumberFormat);
        vertices[i, 1] = double.Parse(tokens[2], culture.NumberFormat);
      }
    }
  }
}
