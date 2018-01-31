using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Netron.Diagramming.Core {
  // ----------------------------------------------------------------------
  /// <summary>
  /// Handles the printing of a diagram.  All pages in the diagram are
  /// printed.  A print preview or actual print can be performed.
  /// </summary>
  // ----------------------------------------------------------------------
  public class DiagramPrinter {
    PrintDocument myDocument;
    IDiagramControl myDiagram;
    //PrinterSettings mySettings;
    PageSettings myPageSettings;
    int currentPageIndex = 0;
    int numberOfPages = 1;

    // ------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="pageSettings">PageSettings: The page setup for the
    /// diagram.</param>
    /// <param name="diagram">IDiagramControl: The diagram to print.
    /// </param>
    /// <param name="printPreviewOnly">bool: Specifies if only a print
    /// preview is to be performed.  If true, then a PrintPreviewDialog
    /// is shown.  If false, then a PrintDialog is shown and the diagram
    /// is printed if the user elects to.</param>
    // ------------------------------------------------------------------
    public DiagramPrinter(
        PageSettings pageSettings,
        IDiagramControl diagram,
        bool printPreviewOnly) {
      this.myDiagram = diagram;
      this.numberOfPages = diagram.Controller.Model.Pages.Count;
      this.myPageSettings = pageSettings;
      this.myDocument = new PrintDocument();
      this.myDocument.DefaultPageSettings = this.myPageSettings;
      this.myDocument.PrintPage +=
          new PrintPageEventHandler(PrintPage);

      if (printPreviewOnly) {
        this.PrintPreview();
      } else {
        this.Print();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Print previews the diagram by displaying a PrintPreviewDialog.
    /// The diagram, however, can be printed from the PrintPreviewDialog.
    /// </summary>
    // ------------------------------------------------------------------
    void PrintPreview() {
      PrintPreviewDialog dialog = new PrintPreviewDialog();
      dialog.Document = this.myDocument;
      dialog.ShowDialog();
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Displays a PrintDialog and prints the diagram if the user elects
    /// to continue.
    /// </summary>
    // ------------------------------------------------------------------
    void Print() {
      PrintDialog dialog = new PrintDialog();
      dialog.Document = this.myDocument;

      if (dialog.ShowDialog() == DialogResult.OK) {
        this.myDocument.Print();
      }
    }

    // ------------------------------------------------------------------
    /// <summary>
    /// Handles the actual rendering of the diagram to the print document.
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">PrintPageEventArgs</param>
    // ------------------------------------------------------------------
    void PrintPage(object sender, PrintPageEventArgs e) {
      Graphics g = e.Graphics;
      IPage page =
          this.myDiagram.Controller.Model.Pages[currentPageIndex];
      Bundle bundle = new Bundle(page.Entities);
      Matrix m = new Matrix();
      m.Translate(
          -page.Bounds.Location.X,
          -page.Bounds.Location.Y);
      g.Transform = m;
      bundle.Paint(g);
      if ((currentPageIndex + 1) < this.numberOfPages) {
        currentPageIndex++;
        e.HasMorePages = true;
      } else {
        currentPageIndex = 0;
        e.HasMorePages = false;
      }
    }
  }
}
