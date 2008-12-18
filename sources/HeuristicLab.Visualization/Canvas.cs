using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HeuristicLab.Visualization {
  public class Canvas : ICanvas {
    private WorldShape world;

    public WorldShape WorldShape {
      get { return world; }
      set { world = value; }
    }


    public void Draw(Graphics graphics, Rectangle viewport) {
      Stopwatch sw = new Stopwatch();
      sw.Start();

      GraphicsState gstate = graphics.Save();

      graphics.SetClip(viewport);

      world.Draw(graphics, viewport, world.BoundingBox);

      graphics.Restore(gstate);

      sw.Stop();
      Trace.WriteLine(string.Format("Drawing time: {0:0.0}ms", sw.Elapsed.TotalMilliseconds));
    }
  }
}