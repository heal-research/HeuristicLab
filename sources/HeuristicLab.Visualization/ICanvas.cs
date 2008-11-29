using System.Drawing;

namespace HeuristicLab.Visualization {
  /// <summary>
  /// ICanvas Interface
  /// </summary>
  public interface ICanvas {
    void Draw(Graphics graphics, Rectangle viewport);
    WorldShape WorldShape { get; set; }
  }
}
