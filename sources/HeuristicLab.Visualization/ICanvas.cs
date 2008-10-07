using System.Drawing;

namespace HeuristicLab.Visualization {
  public interface ICanvas {
    void Draw(Graphics graphics, Rectangle viewport);
    WorldShape WorldShape { get; set; }
  }
}
