namespace HeuristicLab.Visualization {
  public struct RectangleD {
    public static readonly RectangleD Empty = new RectangleD();

    private PointD bottomLeft;
    private PointD topRight;

    private double width;
    private double height;

    public RectangleD(double x1, double y1, double x2, double y2) {
      bottomLeft = PointD.Empty;
      topRight = PointD.Empty;

      bottomLeft.X = x1;
      bottomLeft.Y = y1;

      topRight.X = x2;
      topRight.Y = y2;

      width = x2 - x1;
      height = y2 - y1;
    }

    public PointD BottomLeft {
      get { return bottomLeft; }
      set {
        bottomLeft = value;
        width = topRight.X - bottomLeft.X;
        height = topRight.Y - bottomLeft.Y;
      }
    }

    public PointD TopRight {
      get { return topRight; }
      set {
        topRight = value;
        width = topRight.X - bottomLeft.X;
        height = topRight.Y - bottomLeft.Y;
      }
    }

    public double X1 {
      get { return bottomLeft.X; }
      set {
        bottomLeft.X = value;
        width = topRight.X - bottomLeft.X;
      }
    }

    public double X2 {
      get { return topRight.X; }
      set {
        topRight.X = value;
        width = topRight.X - bottomLeft.X;
      }
    }

    public double Y1 {
      get { return bottomLeft.Y; }
      set {
        bottomLeft.Y = value;
        height = topRight.Y - bottomLeft.Y;
      }
    }

    public double Y2 {
      get { return topRight.Y; }
      set {
        topRight.Y = value;
        height = topRight.Y - bottomLeft.Y;
      }
    }

    public double Width {
      get { return width; }
      set {
        width = value;
        topRight.X = bottomLeft.X + width;
      }
    }

    public double Height {
      get { return height; }
      set {
        height = value;
        topRight.Y = bottomLeft.Y + height;
      }
    }

    public override string ToString() {
      return (string.Format("{{X1={0},Y1={1},Width={2},Height={3}}}", X1, Y1, Width, Height));
    }
  }
}