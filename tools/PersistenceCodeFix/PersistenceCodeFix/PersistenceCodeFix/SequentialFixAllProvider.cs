using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace PersistenceCodeFix {
  public sealed class SequentialFixAllProvider : FixAllProvider {
    private static readonly SyntaxAnnotation marker = new SyntaxAnnotation(nameof(SequentialFixAllProvider));
    private const string title = "Apply this fix sequentially";

    private SequentialFixAllProvider() { }
    public static readonly SequentialFixAllProvider Instance = new SequentialFixAllProvider();

    public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext) {
      var diagnosticsToFix = new List<Diagnostic>();

      switch (fixAllContext.Scope) {
        case FixAllScope.Document: {
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(fixAllContext.Document);
            diagnosticsToFix.AddRange(diagnostics);
          }
          break;
        case FixAllScope.Project: {
            var diagnostics = await fixAllContext.GetAllDiagnosticsAsync(fixAllContext.Project);
            diagnosticsToFix.AddRange(diagnostics);
          }
          break;
        case FixAllScope.Solution: {
            foreach (var project in fixAllContext.Solution.Projects) {
              var diagnostics = await fixAllContext.GetAllDiagnosticsAsync(project);
              diagnosticsToFix.AddRange(diagnostics);
            }
          }
          break;
      }

      var solution = fixAllContext.Solution;
      var codeFixProvider = (IDocumentCodeFixProvider)fixAllContext.CodeFixProvider;
      return await Task.FromResult(CodeAction.Create(title, ct => GetFixedSolutionAsync(solution, diagnosticsToFix.ToImmutableArray(), codeFixProvider, ct)));
    }

    private static async Task<Solution> GetFixedSolutionAsync(Solution solution,
                                                              ImmutableArray<Diagnostic> diagnosticsToFix,
                                                              IDocumentCodeFixProvider codeFixProvider,
                                                              CancellationToken cancellationToken) {
      var documentsToFix = diagnosticsToFix.GroupBy(x => solution.GetDocument(x.Location.SourceTree));

      var tasks = new List<Task<Document>>();
      foreach (var grouping in documentsToFix)
        tasks.Add(GetFixedDocumentAsync(grouping.Key, grouping.ToImmutableArray(), codeFixProvider, cancellationToken));

      await Task.WhenAll(tasks);

      foreach (var newDocument in tasks.Select(x => x.Result)) {
        var oldDocument = solution.GetDocument(newDocument.Id);
        if (oldDocument == newDocument) continue;
        solution = solution.WithDocumentSyntaxRoot(newDocument.Id, await newDocument.GetSyntaxRootAsync());
        cancellationToken.ThrowIfCancellationRequested();
      }

      return solution;
    }

    private static async Task<Document> GetFixedDocumentAsync(Document document,
                                                              ImmutableArray<Diagnostic> diagnosticsToFix,
                                                              IDocumentCodeFixProvider codeFixProvider,
                                                              CancellationToken cancellationToken) {

      var root = await document.GetSyntaxRootAsync(cancellationToken);

      foreach (var diagnostic in diagnosticsToFix) {
        var node = root.FindNode(diagnostic.Location.SourceSpan);
        document = document.WithSyntaxRoot(root.ReplaceNode(node, node.WithAdditionalAnnotations(marker)));
        root = await document.GetSyntaxRootAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
      }

      var annotatedNodes = root.GetAnnotatedNodes(marker);

      while (annotatedNodes.Any()) {
        var node = annotatedNodes.First();
        document = await codeFixProvider.FixDocumentAsync(document, node, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        root = await document.GetSyntaxRootAsync(cancellationToken);
        annotatedNodes = root.GetAnnotatedNodes(marker);
        node = annotatedNodes.First();
        document = document.WithSyntaxRoot(root.ReplaceNode(node, node.WithoutAnnotations(marker)));
        root = await document.GetSyntaxRootAsync(cancellationToken);
        annotatedNodes = root.GetAnnotatedNodes(marker);
        cancellationToken.ThrowIfCancellationRequested();
      }

      return document;
    }

  }
}
