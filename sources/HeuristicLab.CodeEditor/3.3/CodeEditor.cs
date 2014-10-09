// CSharp Editor Example with Code Completion
// Copyright (c) 2006, Daniel Grunwald
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
// 
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
// 
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
// 
// - Neither the name of the ICSharpCode team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using HeuristicLab.Common.Resources;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using Dom = ICSharpCode.SharpDevelop.Dom;
using NRefactory = ICSharpCode.NRefactory;

namespace HeuristicLab.CodeEditor {

  public partial class CodeEditor : UserControl {

    #region Fields & Properties

    internal Dom.ProjectContentRegistry projectContentRegistry;
    internal Dom.DefaultProjectContent projectContent;
    internal Dom.ParseInformation parseInformation = new Dom.ParseInformation();
    Dom.ICompilationUnit lastCompilationUnit;
    Thread parserThread;

    /// <summary>
    /// Many SharpDevelop.Dom methods take a file name, which is really just a unique identifier
    /// for a file - Dom methods don't try to access code files on disk, so the file does not have
    /// to exist.
    /// SharpDevelop itself uses internal names of the kind "[randomId]/Class1.cs" to support
    /// code-completion in unsaved files.
    /// </summary>
    public const string DummyFileName = "edited.cs";

    private IDocument Doc {
      get {
        return textEditor.Document;
      }
    }

    private string prefix = "";
    private TextMarker prefixMarker =
      new TextMarker(0, 1, TextMarkerType.SolidBlock, Color.LightGray) {
        IsReadOnly = true,
      };
    public string Prefix {
      get {
        return prefix;
      }
      set {
        if (value == null) value = "";
        if (prefix == value) return;
        Doc.MarkerStrategy.RemoveMarker(prefixMarker);
        Doc.Remove(0, prefix.Length);
        prefix = value;
        if (value.Length > 0) {
          Doc.Insert(0, value);
          prefixMarker.Offset = 0;
          prefixMarker.Length = prefix.Length;
          Doc.MarkerStrategy.AddMarker(prefixMarker);
        }
        Doc.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        Doc.CommitUpdate();
      }
    }

    private string suffix = "";
    private TextMarker suffixMarker =
      new TextMarker(0, 1, TextMarkerType.SolidBlock, Color.LightGray) {
        IsReadOnly = true,
      };
    public string Suffix {
      get {
        return suffix;
      }
      set {
        if (value == null) value = "";
        if (suffix == value) return;
        Doc.MarkerStrategy.RemoveMarker(suffixMarker);
        Doc.Remove(Doc.TextLength - suffix.Length, suffix.Length);
        suffix = value;
        if (value.Length > 0) {
          suffixMarker.Offset = Doc.TextLength;
          Doc.Insert(Doc.TextLength, value);
          suffixMarker.Length = suffix.Length;
          Doc.MarkerStrategy.AddMarker(suffixMarker);
        }
        Doc.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        Doc.CommitUpdate();
      }
    }

    public string UserCode {
      get {
        return Doc.GetText(
          prefix.Length,
          Doc.TextLength - suffix.Length - prefix.Length);
      }
      set {
        Doc.Replace(prefix.Length, Doc.TextLength - suffix.Length - prefix.Length, value);
        Doc.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        Doc.CommitUpdate();
      }
    }

    public bool ReadOnly {
      get { return Doc.ReadOnly; }
      set { Doc.ReadOnly = value; }
    }

    #endregion

    public event EventHandler TextEditorValidated;

    protected void OnTextEditorValidated() {
      if (TextEditorValidated != null)
        TextEditorValidated(this, EventArgs.Empty);
    }

    public event EventHandler TextEditorTextChanged;

    protected void OnTextEditorTextChanged() {
      if (TextEditorTextChanged != null)
        TextEditorTextChanged(this, EventArgs.Empty);
    }

    public CodeEditor() {
      InitializeComponent();

      textEditor.ActiveTextAreaControl.TextEditorProperties.SupportReadOnlySegments = true;

      textEditor.SetHighlighting("C#");
      textEditor.ShowEOLMarkers = false;
      textEditor.ShowInvalidLines = false;
      HostCallbackImplementation.Register(this);
      CodeCompletionKeyHandler.Attach(this, textEditor);
      ToolTipProvider.Attach(this, textEditor);

      projectContentRegistry = new Dom.ProjectContentRegistry(); // Default .NET 2.0 registry
      try {
        string persistencePath = Path.Combine(Path.GetTempPath(), "HeuristicLab.CodeEditor");
        if (!Directory.Exists(persistencePath))
          Directory.CreateDirectory(persistencePath);
        FileStream fs = File.Create(Path.Combine(persistencePath, "test.tmp"));
        fs.Close();
        File.Delete(Path.Combine(persistencePath, "test.tmp"));
        // if we made it this far, enable on-disk parsing cache
        projectContentRegistry.ActivatePersistence(persistencePath);
      } catch (NotSupportedException) {
      } catch (IOException) {
      } catch (System.Security.SecurityException) {
      } catch (UnauthorizedAccessException) {
      } catch (ArgumentException) {
      }
      projectContent = new Dom.DefaultProjectContent();
      projectContent.Language = Dom.LanguageProperties.CSharp;
    }

    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);

      if (DesignMode)
        return;

      textEditor.ActiveTextAreaControl.TextArea.KeyEventHandler += new ICSharpCode.TextEditor.KeyEventHandler(TextArea_KeyEventHandler);
      textEditor.ActiveTextAreaControl.TextArea.DoProcessDialogKey += new DialogKeyProcessor(TextArea_DoProcessDialogKey);

      parserThread = new Thread(ParserThread);
      parserThread.IsBackground = true;
      parserThread.Start();

      textEditor.Validated += (s, a) => { OnTextEditorValidated(); };
      textEditor.TextChanged += (s, a) => { OnTextEditorTextChanged(); };
      InitializeImageList();
    }

    private void InitializeImageList() {
      imageList1.Images.Clear();
      imageList1.Images.Add("Icons.16x16.Class.png", VSImageLibrary.Class);
      imageList1.Images.Add("Icons.16x16.Method.png", VSImageLibrary.Method);
      imageList1.Images.Add("Icons.16x16.Property.png", VSImageLibrary.Properties);
      imageList1.Images.Add("Icons.16x16.Field.png", VSImageLibrary.Field);
      imageList1.Images.Add("Icons.16x16.Enum.png", VSImageLibrary.Enum);
      imageList1.Images.Add("Icons.16x16.NameSpace.png", VSImageLibrary.Namespace);
      imageList1.Images.Add("Icons.16x16.Event.png", VSImageLibrary.Event);
    }

    #region keyboard handlers: filter input in read-only areas

    bool TextArea_KeyEventHandler(char ch) {
      int caret = textEditor.ActiveTextAreaControl.Caret.Offset;
      return caret < prefix.Length || caret > Doc.TextLength - suffix.Length;
    }

    bool TextArea_DoProcessDialogKey(Keys keyData) {
      if (keyData == Keys.Return) {
        int caret = textEditor.ActiveTextAreaControl.Caret.Offset;
        if (caret < prefix.Length ||
            caret > Doc.TextLength - suffix.Length) {
          return true;
        }
      }
      return false;
    }

    #endregion

    public void ScrollAfterPrefix() {
      int lineNr = prefix != null ? Doc.OffsetToPosition(prefix.Length).Line : 0;
      textEditor.ActiveTextAreaControl.JumpTo(lineNr + 1);
    }

    private List<TextMarker> errorMarkers = new List<TextMarker>();
    private List<Bookmark> errorBookmarks = new List<Bookmark>();
    public void ShowCompileErrors(CompilerErrorCollection compilerErrors, string filename) {
      Doc.MarkerStrategy.RemoveAll(m => errorMarkers.Contains(m));
      Doc.BookmarkManager.RemoveMarks(m => errorBookmarks.Contains(m));
      errorMarkers.Clear();
      errorBookmarks.Clear();
      errorLabel.Text = "";
      errorLabel.ToolTipText = null;
      if (compilerErrors == null)
        return;
      foreach (CompilerError error in compilerErrors) {
        if (!error.FileName.EndsWith(filename)) {
          errorLabel.Text = "Error in generated code";
          errorLabel.ToolTipText = string.Format("{0}{1}:{2} -> {3}",
            errorLabel.ToolTipText != null ? (errorLabel.ToolTipText + "\n\n") : "",
            error.Line, error.Column,
            error.ErrorText);
          continue;
        }
        var startPosition = Doc.OffsetToPosition(prefix.Length);
        if (error.Line == 1)
          error.Column += startPosition.Column;
        error.Line += startPosition.Line;
        AddErrorMarker(error);
        AddErrorBookmark(error);
      }
      Doc.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
      Doc.CommitUpdate();
    }

    private void AddErrorMarker(CompilerError error) {
      var segment = GetSegmentAtOffset(error.Line, error.Column);
      Color color = error.IsWarning ? Color.Blue : Color.Red;
      var marker = new TextMarker(segment.Offset, segment.Length, TextMarkerType.WaveLine, color) {
        ToolTip = error.ErrorText,
      };
      errorMarkers.Add(marker);
      Doc.MarkerStrategy.AddMarker(marker);
    }

    private void AddErrorBookmark(CompilerError error) {
      var bookmark = new ErrorBookmark(Doc, new TextLocation(error.Column, error.Line - 1));
      errorBookmarks.Add(bookmark);
      Doc.BookmarkManager.AddMark(bookmark);
    }

    private AbstractSegment GetSegmentAtOffset(int lineNr, int columnNr) {
      lineNr = Math.Max(Doc.OffsetToPosition(prefix.Length).Line, lineNr);
      lineNr = Math.Min(Doc.OffsetToPosition(Doc.TextLength - suffix.Length).Line, lineNr);
      var line = Doc.GetLineSegment(lineNr - 1);
      columnNr = Math.Max(0, columnNr);
      columnNr = Math.Min(line.Length, columnNr);
      var word = line.GetWord(columnNr);
      AbstractSegment segment = new AbstractSegment();
      if (word != null) {
        segment.Offset = line.Offset + word.Offset;
        segment.Length = word.Length;
      } else {
        segment.Offset = line.Offset + columnNr - 1;
        segment.Length = 1;
      }
      return segment;
    }

    private HashSet<Assembly> assemblies = new HashSet<Assembly>();
    public IEnumerable<Assembly> ReferencedAssemblies {
      get { return assemblies; }
    }
    public void AddAssembly(Assembly a) {
      ShowMessage("Loading " + a.GetName().Name + "...");
      if (!assemblies.Contains(a)) {
        var reference = projectContentRegistry.GetProjectContentForReference(a.GetName().Name, a.Location);
        projectContent.AddReferencedContent(reference);
        assemblies.Add(a);
      }
      ShowMessage("Ready");
    }
    public void RemoveAssembly(Assembly a) {
      ShowMessage("Unloading " + a.GetName().Name + "...");
      if (assemblies.Contains(a)) {
        var content = projectContentRegistry.GetExistingProjectContent(a.Location);
        if (content != null) {
          projectContent.ReferencedContents.Remove(content);
          projectContentRegistry.UnloadProjectContent(content);
        }
      }
      ShowMessage("Ready");
    }

    private bool runParser = true;
    private void ParserThread() {
      BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Loading mscorlib..."; }));
      projectContent.AddReferencedContent(projectContentRegistry.Mscorlib);
      ParseStep();
      BeginInvoke(new MethodInvoker(delegate { parserThreadLabel.Text = "Ready"; }));
      while (runParser && !IsDisposed) {
        ParseStep();
        Thread.Sleep(2000);
      }
    }

    private void ParseStep() {
      try {
        string code = null;
        Invoke(new MethodInvoker(delegate { code = textEditor.Text; }));
        TextReader textReader = new StringReader(code);
        Dom.ICompilationUnit newCompilationUnit;
        NRefactory.SupportedLanguage supportedLanguage;
        supportedLanguage = NRefactory.SupportedLanguage.CSharp;
        using (NRefactory.IParser p = NRefactory.ParserFactory.CreateParser(supportedLanguage, textReader)) {
          p.ParseMethodBodies = false;
          p.Parse();
          newCompilationUnit = ConvertCompilationUnit(p.CompilationUnit);
        }
        projectContent.UpdateCompilationUnit(lastCompilationUnit, newCompilationUnit, DummyFileName);
        lastCompilationUnit = newCompilationUnit;
        parseInformation.SetCompilationUnit(newCompilationUnit);
      } catch { }
    }

    Dom.ICompilationUnit ConvertCompilationUnit(NRefactory.Ast.CompilationUnit cu) {
      Dom.NRefactoryResolver.NRefactoryASTConvertVisitor converter;
      converter = new Dom.NRefactoryResolver.NRefactoryASTConvertVisitor(projectContent);
      cu.AcceptVisitor(converter, null);
      return converter.Cu;
    }

    private void ShowMessage(string m) {
      if (this.Handle == null)
        return;
      BeginInvoke(new Action(() => parserThreadLabel.Text = m));
    }

    private void toolStripStatusLabel1_Click(object sender, EventArgs e) {
      var proc = new Process();
      proc.StartInfo.FileName = sharpDevelopLabel.Tag.ToString();
      proc.Start();
    }

    private void CodeEditor_Resize(object sender, EventArgs e) {
      var textArea = textEditor.ActiveTextAreaControl.TextArea;
      var vScrollBar = textEditor.ActiveTextAreaControl.VScrollBar;
      var hScrollBar = textEditor.ActiveTextAreaControl.HScrollBar;

      textArea.SuspendLayout();
      textArea.Width = textEditor.Width - vScrollBar.Width;
      textArea.Height = textEditor.Height - hScrollBar.Height;
      textArea.ResumeLayout();

      vScrollBar.SuspendLayout();
      vScrollBar.Location = new Point(textArea.Width, 0);
      vScrollBar.Height = textArea.Height;
      vScrollBar.ResumeLayout();

      hScrollBar.SuspendLayout();
      hScrollBar.Location = new Point(0, textArea.Height);
      hScrollBar.Width = textArea.Width;
      hScrollBar.ResumeLayout();
    }
  }
}
