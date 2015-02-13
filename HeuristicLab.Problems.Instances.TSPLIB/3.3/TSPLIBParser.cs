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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace HeuristicLab.Problems.Instances.TSPLIB {

  #region Public Enums
  public enum TSPLIBTypes {
    UNKNOWN = 0,
    TSP = 1,
    ATSP = 2,
    SOP = 3,
    HCP = 4,
    CVRP = 5,
    TOUR = 6
  }

  public enum TSPLIBEdgeWeightTypes {
    UNKNOWN = 0,
    EXPLICIT = 1,
    EUC_2D = 2,
    EUC_3D = 3,
    MAX_2D = 4,
    MAX_3D = 5,
    MAN_2D = 6,
    MAN_3D = 7,
    CEIL_2D = 8,
    GEO = 9,
    ATT = 10,
    XRAY1 = 11,
    XRAY2 = 12,
    SPECIAL = 13
  }

  public enum TSPLIBDisplayDataTypes {
    UNKNOWN = 0,
    COORD_DISPLAY = 1,
    TWOD_DISPLAY = 2,
    NO_DISPLAY = 3
  }
  #endregion

  public class TSPLIBParser {

    public string Name { get; private set; }
    public TSPLIBTypes Type { get; private set; }
    public string Comment { get; private set; }
    public int Dimension { get; private set; }
    public double? Capacity { get; private set; }
    public TSPLIBEdgeWeightTypes EdgeWeightType { get; private set; }
    public TSPLIBDisplayDataTypes DisplayDataType { get; private set; }
    public double[,] Vertices { get; private set; }
    public double[,] DisplayVertices { get; private set; }
    public double[,] Distances { get; private set; }
    public int[,] FixedEdges { get; private set; }
    public int[] Depots { get; private set; }
    public double[] Demands { get; private set; }
    public int[][] Tour { get; private set; }

    private static readonly char sectionSeparator = ':';
    private static readonly char[] itemSeparator = new char[] { ' ', '\t' };

    #region Private Enums
    private enum TSPLIBSections {
      UNKNOWN = 0,
      EOF = 1,
      NAME = 2,
      TYPE = 3,
      COMMENT = 4,
      DIMENSION = 5,
      CAPACITY = 6,
      EDGE_WEIGHT_TYPE = 7,
      EDGE_WEIGHT_FORMAT = 8,
      EDGE_DATA_FORMAT = 9,
      NODE_COORD_TYPE = 10,
      DISPLAY_DATA_TYPE = 11,
      NODE_COORD_SECTION = 12,
      DEPOT_SECTION = 13,
      DEMAND_SECTION = 14,
      EDGE_DATA_SECTION = 15,
      FIXED_EDGES_SECTION = 16,
      DISPLAY_DATA_SECTION = 17,
      TOUR_SECTION = 18,
      EDGE_WEIGHT_SECTION = 19
    }
    private enum TSPLIBEdgeWeightFormats {
      UNKNWON = 0,
      FUNCTION = 1,
      FULL_MATRIX = 2,
      UPPER_ROW = 3,
      LOWER_ROW = 4,
      UPPER_DIAG_ROW = 5,
      LOWER_DIAG_ROW = 6,
      UPPER_COL = 7,
      LOWER_COL = 8,
      UPPER_DIAG_COL = 9,
      LOWER_DIAG_COL = 10
    }
    private enum TSPLIBEdgeWeightDataFormats {
      UNKNOWN = 0,
      EDGE_LIST = 1,
      ADJ_LIST = 2
    }
    private enum TSLPLIBNodeCoordTypes {
      UNKNOWN = 0,
      TWOD_COORDS = 1,
      THREED_COORDS = 2,
      NO_COORDS = 3
    }
    #endregion

    private StreamReader source;
    private int currentLineNumber;

    private TSPLIBEdgeWeightFormats edgeWeightFormat;
    private TSPLIBEdgeWeightDataFormats edgeWeightDataFormat;
    private TSLPLIBNodeCoordTypes nodeCoordType;
    private TSPLIBDisplayDataTypes displayDataType;

    private TSPLIBParser() {
      Name = String.Empty;
      Comment = String.Empty;
      Type = TSPLIBTypes.UNKNOWN;
      EdgeWeightType = TSPLIBEdgeWeightTypes.UNKNOWN;

      edgeWeightFormat = TSPLIBEdgeWeightFormats.UNKNWON;
      edgeWeightDataFormat = TSPLIBEdgeWeightDataFormats.UNKNOWN;
      nodeCoordType = TSLPLIBNodeCoordTypes.UNKNOWN;
      displayDataType = TSPLIBDisplayDataTypes.UNKNOWN;
    }

    public TSPLIBParser(String path)
      : this() {
      source = new StreamReader(path);
    }

    public TSPLIBParser(Stream stream)
      : this() {
      source = new StreamReader(stream);
    }

    /// <summary>
    /// Reads the TSPLIB file and parses the elements.
    /// </summary>
    /// <exception cref="InvalidDataException">Thrown if the file has an invalid format or contains invalid data.</exception>
    public void Parse() {
      TSPLIBSections section;
      string line;
      currentLineNumber = 0;

      try {
        do {
          line = NextLine();
          string value;
          section = GetSection(line, out value);

          switch (section) {
            case TSPLIBSections.UNKNOWN:
              break;
            case TSPLIBSections.NAME:
              ReadName(value);
              break;
            case TSPLIBSections.TYPE:
              ReadType(value);
              break;
            case TSPLIBSections.COMMENT:
              ReadComment(value);
              break;
            case TSPLIBSections.DIMENSION:
              ReadDimension(value);
              break;
            case TSPLIBSections.CAPACITY:
              ReadCapacity(value);
              break;
            case TSPLIBSections.EDGE_WEIGHT_TYPE:
              ReadEdgeWeightType(value);
              break;
            case TSPLIBSections.EDGE_WEIGHT_FORMAT:
              ReadEdgeWeightFormat(value);
              break;
            case TSPLIBSections.EDGE_DATA_FORMAT:
              ReadEdgeWeightDataFormat(value);
              break;
            case TSPLIBSections.NODE_COORD_TYPE:
              ReadNodeCoordType(value);
              break;
            case TSPLIBSections.DISPLAY_DATA_TYPE:
              ReadDisplayDataType(value);
              break;
            case TSPLIBSections.NODE_COORD_SECTION:
              ReadNodeCoordsSection();
              break;
            case TSPLIBSections.DEPOT_SECTION:
              ReadDepotSection();
              break;
            case TSPLIBSections.DEMAND_SECTION:
              ReadDemandSection();
              break;
            case TSPLIBSections.EDGE_DATA_SECTION:
              ReadEdgeDataSection();
              break;
            case TSPLIBSections.FIXED_EDGES_SECTION:
              ReadFixedEdgesSection();
              break;
            case TSPLIBSections.DISPLAY_DATA_SECTION:
              ReadDisplayDataSection();
              break;
            case TSPLIBSections.TOUR_SECTION:
              ReadTourSection();
              break;
            case TSPLIBSections.EDGE_WEIGHT_SECTION:
              ReadEdgeWeightSection();
              break;
            case TSPLIBSections.EOF:
              break;
            default:
              throw new InvalidDataException("Input file contains unknown or unsupported section (" + line + ") in line " + currentLineNumber.ToString());
          }
        } while (!(section == TSPLIBSections.EOF || source.EndOfStream));
      } finally {
        source.Close();
      }
    }

    private string NextLine() {
      currentLineNumber++;
      return source.ReadLine();
    }

    private TSPLIBSections GetSection(string line, out string value) {
      value = String.Empty;

      if (line == null)
        return TSPLIBSections.UNKNOWN;

      int sectionNameEnd = line.IndexOf(sectionSeparator);
      if (sectionNameEnd == -1) sectionNameEnd = line.Length;

      string sectionName = line.Substring(0, sectionNameEnd).Trim();
      if (sectionNameEnd + 1 < line.Length)
        value = line.Substring(sectionNameEnd + 1);

      TSPLIBSections section;
      if (Enum.TryParse(sectionName, out section)) {
        return section;
      } else return TSPLIBSections.UNKNOWN;
    }

    private void ReadName(string value) {
      Name += value.Trim();
    }

    private void ReadType(string value) {
      TSPLIBTypes t;
      if (Enum.TryParse(value.Trim().ToUpper(), out t))
        Type = t;
      else Type = TSPLIBTypes.UNKNOWN;
    }

    private void ReadComment(string value) {
      Comment += value.Trim() + Environment.NewLine;
    }

    private void ReadCapacity(string value) {
      double c;
      if (double.TryParse(value.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out c))
        Capacity = c;
      else throw new InvalidDataException("Parsing the capacity in line " + currentLineNumber + " failed. It is not recognized as double value.");
    }

    private void ReadDimension(string value) {
      int dimension;
      if (int.TryParse(value.Trim(), out dimension))
        Dimension = dimension;
      else throw new InvalidDataException("Parsing the dimension in line " + currentLineNumber + " failed. It is not recognized as an integer number.");
    }

    private void ReadEdgeWeightType(string value) {
      TSPLIBEdgeWeightTypes e;
      if (Enum.TryParse(value.Trim().ToUpper(), out e))
        EdgeWeightType = e;
      else throw new InvalidDataException("Input file contains an unsupported edge weight type (" + value + ") in line " + currentLineNumber + ".");
    }

    private void ReadEdgeWeightFormat(string value) {
      TSPLIBEdgeWeightFormats e;
      if (Enum.TryParse(value.Trim().ToUpper(), out e))
        edgeWeightFormat = e;
      else throw new InvalidDataException("Input file contains an unsupported edge weight format (" + value + ") in line " + currentLineNumber + ".");
    }

    private void ReadEdgeWeightDataFormat(string value) {
      TSPLIBEdgeWeightDataFormats e;
      if (Enum.TryParse(value.Trim().ToUpper(), out e))
        edgeWeightDataFormat = e;
      else throw new InvalidDataException("Input file contains an unsupported edge weight data format (" + value + ") in line " + currentLineNumber + ".");
    }

    private void ReadNodeCoordType(string value) {
      TSLPLIBNodeCoordTypes n;
      if (Enum.TryParse(value.Trim().ToUpper(), out n))
        nodeCoordType = n;
      else throw new InvalidDataException("Input file contains an unsupported node coordinates type (" + value + ") in line " + currentLineNumber + ".");
    }

    private void ReadDisplayDataType(string value) {
      TSPLIBDisplayDataTypes d;
      if (Enum.TryParse(value.Trim().ToUpper(), out d))
        displayDataType = d;
      else throw new InvalidDataException("Input file contains an unsupported display data type (" + value + ") in line " + currentLineNumber + ".");
    }

    private void ReadNodeCoordsSection() {
      if (Dimension == 0)
        throw new InvalidDataException("Input file does not contain dimension information.");
      switch (nodeCoordType) {
        case TSLPLIBNodeCoordTypes.NO_COORDS:
          return;
        case TSLPLIBNodeCoordTypes.UNKNOWN: // It's a pity that there is a documented standard which is ignored in most files.
        case TSLPLIBNodeCoordTypes.TWOD_COORDS:
          Vertices = new double[Dimension, 2];
          break;
        case TSLPLIBNodeCoordTypes.THREED_COORDS:
          Vertices = new double[Dimension, 3];
          break;
        default:
          throw new InvalidDataException("Input files does not specify a valid node coord type.");
      }

      for (int i = 0; i < Dimension; i++) {
        string line = NextLine();
        string[] tokens = line.Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length != Vertices.GetLength(1) + 1)
          throw new InvalidDataException("Input file contains invalid number of node coordinates in line " + currentLineNumber + ".");

        int node;
        if (int.TryParse(tokens[0], out node)) {
          for (int j = 0; j < Vertices.GetLength(1); j++)
            Vertices[node - 1, j] = double.Parse(tokens[j + 1], CultureInfo.InvariantCulture.NumberFormat);
        } else throw new InvalidDataException("Input file does not specify a valid node in line " + currentLineNumber + ".");
      }
    }

    private void ReadDepotSection() {
      var depots = new List<int>();
      do {
        string line = NextLine();
        int node;
        if (!int.TryParse(line, out node))
          throw new InvalidDataException("Input file contains an unknown depot entry at line " + currentLineNumber + ".");
        if (node == -1) break;
        depots.Add(node);
      } while (true);
      Depots = depots.ToArray();
    }

    private void ReadDemandSection() {
      if (Dimension == 0)
        throw new InvalidDataException("Input file does not contain dimension information.");
      Demands = new double[Dimension];
      for (int i = 0; i < Dimension; i++) {
        string[] tokens = NextLine().Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);
        int node;
        if (int.TryParse(tokens[0], out node)) {
          double demand;
          if (double.TryParse(tokens[1], NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out demand))
            Demands[node - 1] = demand;
          else throw new InvalidDataException("Input file contains invalid demand information in line " + currentLineNumber + ".");
        } else throw new InvalidDataException("Input file contains invalid node information in line " + currentLineNumber + ".");
      }
    }

    private void ReadEdgeDataSection() {
      throw new NotSupportedException("Files with an edge data section are not supported.");
    }

    private void ReadFixedEdgesSection() {
      var edges = new List<Tuple<int, int>>();
      bool finished = false;
      while (!finished) {
        string line = NextLine();
        string[] tokens = line.Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 1) {
          int number;
          if (!int.TryParse(tokens[0], NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out number))
            throw new InvalidDataException("Input file does not end the fixed edges section with \"-1\" in line " + currentLineNumber + ".");
          if (number != -1)
            throw new InvalidDataException("Input file must end the fixed edges section with a -1 in line " + currentLineNumber + ".");
          finished = true;
        } else {
          if (tokens.Length != 2)
            throw new InvalidDataException("Input file contains an error in line " + currentLineNumber + ", exactly two nodes need to be given in each line.");
          int node1 = int.Parse(tokens[0], CultureInfo.InvariantCulture.NumberFormat) - 1;
          int node2 = int.Parse(tokens[1], CultureInfo.InvariantCulture.NumberFormat) - 1;
          edges.Add(Tuple.Create(node1, node2));
        }
      }
      FixedEdges = new int[edges.Count, 2];
      for (int i = 0; i < edges.Count; i++) {
        FixedEdges[i, 0] = edges[i].Item1;
        FixedEdges[i, 1] = edges[i].Item2;
      }
    }

    private void ReadDisplayDataSection() {
      if (Dimension == 0)
        throw new InvalidDataException("Input file does not contain dimension information.");

      switch (displayDataType) {
        case TSPLIBDisplayDataTypes.NO_DISPLAY:
        case TSPLIBDisplayDataTypes.COORD_DISPLAY:
          return;
        case TSPLIBDisplayDataTypes.TWOD_DISPLAY:
          DisplayVertices = new double[Dimension, 2];
          break;
        default:
          throw new InvalidDataException("Input files does not specify a valid display data type.");
      }

      for (int i = 0; i < Dimension; i++) {
        string line = NextLine();
        string[] tokens = line.Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);

        if (tokens.Length != DisplayVertices.GetLength(1) + 1)
          throw new InvalidDataException("Input file contains invalid number of display data coordinates in line " + currentLineNumber + ".");

        int node;
        if (int.TryParse(tokens[0], out node)) {
          for (int j = 0; j < DisplayVertices.GetLength(1); j++)
            DisplayVertices[node - 1, j] = double.Parse(tokens[j + 1], CultureInfo.InvariantCulture.NumberFormat);
        } else throw new InvalidDataException("Input file does not specify a valid node in line " + currentLineNumber + ".");
      }
    }

    /// <summary>
    /// Parses the tour section.
    /// </summary>
    /// <remarks>
    /// Unfortunately, none of the given files for the TSP follow the description
    /// in the TSPLIB documentation.
    /// The faulty files use only one -1 to terminate the section as well as the tour
    /// whereas the documentation says that one -1 terminates the tour and another -1
    /// terminates the section.
    /// So the parser peeks at the next character after a -1. If the next character
    /// is an E as in EOF or the stream ends, the parser ends. Otherwise it will
    /// continue to read tours until a -1 is followed by a -1.
    /// </remarks>
    private void ReadTourSection() {
      if (Dimension == 0)
        throw new InvalidDataException("Input file does not contain dimension information.");

      var tours = new List<List<int>>();
      do {
        var line = NextLine();
        if (String.IsNullOrEmpty(line)) break;
        string[] nodes = line.Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (!nodes.Any()) break;

        bool finished = false;
        foreach (var nodeString in nodes) {
          int node = int.Parse(nodeString, CultureInfo.InvariantCulture.NumberFormat);
          if (node == -1) {
            finished = (tours.Any() && tours.Last().Count == 0) // -1 followed by -1
              || (source.BaseStream.CanSeek && source.Peek() == -1)
              || source.Peek() == (int)'E';
            if (finished) break;
            tours.Add(new List<int>());
          } else {
            if (!tours.Any()) tours.Add(new List<int>());
            tours.Last().Add(node - 1);
          }
        }
        if (finished) break;
      } while (true);
      if (tours.Last().Count == 0) tours.RemoveAt(tours.Count - 1);
      Tour = tours.Select(x => x.ToArray()).ToArray();
    }

    private void ReadEdgeWeightSection() {
      if (Dimension == 0)
        throw new InvalidDataException("Input file does not contain dimension information.");

      if (edgeWeightFormat == TSPLIBEdgeWeightFormats.UNKNWON)
        throw new InvalidDataException("Input file does not specify an edge weight format.");

      if (edgeWeightFormat == TSPLIBEdgeWeightFormats.FUNCTION)
        return;

      Distances = new double[Dimension, Dimension];

      bool triangular = edgeWeightFormat != TSPLIBEdgeWeightFormats.FULL_MATRIX;
      bool upperTriangular = edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_COL
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_DIAG_COL
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_DIAG_ROW
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_ROW;
      bool diagonal = edgeWeightFormat == TSPLIBEdgeWeightFormats.LOWER_DIAG_COL
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.LOWER_DIAG_ROW
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_DIAG_COL
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_DIAG_ROW;
      bool rowWise = edgeWeightFormat == TSPLIBEdgeWeightFormats.LOWER_DIAG_ROW
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.LOWER_ROW
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_DIAG_ROW
        || edgeWeightFormat == TSPLIBEdgeWeightFormats.UPPER_ROW;

      int dim1 = triangular && !upperTriangular ? (diagonal ? 0 : 1) : 0;
      int dim2 = triangular && upperTriangular ? (diagonal ? 0 : 1) : 0;
      bool finished = false;
      while (!finished) {
        string line = NextLine();
        string[] weights = line.Split(itemSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var weightString in weights) {
          double weight;
          if (!double.TryParse(weightString, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out weight))
            throw new InvalidDataException("Input file contains unreadable weight information (" + weightString + ") in line " + currentLineNumber + ".");

          if (triangular) {
            Distances[dim1, dim2] = weight;
            Distances[dim2, dim1] = weight;
            if (upperTriangular) {
              if (rowWise) {
                dim2++;
                if (dim2 == Dimension) {
                  dim1++;
                  if (diagonal) dim2 = dim1;
                  else dim2 = dim1 + 1;
                }
              } else { // column-wise
                dim1++;
                if (diagonal && dim1 == dim2 + 1
                  || !diagonal && dim1 == dim2) {
                  dim2++;
                  dim1 = 0;
                }
              }
            } else { // lower-triangular
              if (rowWise) {
                dim2++;
                if (diagonal && dim2 == dim1 + 1
                  || !diagonal && dim2 == dim1) {
                  dim1++;
                  dim2 = 0;
                }
              } else { // column-wise
                dim1++;
                if (dim1 == Dimension) {
                  dim2++;
                  if (diagonal) dim1 = dim2;
                  else dim1 = dim2 + 1;
                }
              }
            }
          } else { // full-matrix
            Distances[dim1, dim2] = weight;
            dim2++;
            if (dim2 == Dimension) {
              dim1++;
              dim2 = 0;
            }
          }
        }
        finished = dim1 == Dimension || dim2 == Dimension;
      }
    }

  }
}
