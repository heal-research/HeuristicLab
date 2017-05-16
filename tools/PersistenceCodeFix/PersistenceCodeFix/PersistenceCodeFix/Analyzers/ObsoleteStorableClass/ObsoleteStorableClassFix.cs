using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace PersistenceCodeFix {
  [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObsoleteStorableClassFix)), Shared]
  public sealed class ObsoleteStorableClassFix : CodeFixProvider {
    private const string title = "Change to StorableType attribute";

    public sealed override ImmutableArray<string> FixableDiagnosticIds {
      get { return ImmutableArray.Create(ObsoleteStorableClassAnalyzer.DiagnosticId); }
    }

    public sealed override FixAllProvider GetFixAllProvider() {
      return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context) {
      var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

      var diagnostic = context.Diagnostics.First();
      var diagnosticSpan = diagnostic.Location.SourceSpan;
      var attributeSyntax = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<AttributeSyntax>().First();

      context.RegisterCodeFix(
        CodeAction.Create(
          title: title,
          createChangedDocument: c => ChangeToStorableTypeAttribute(context.Document, attributeSyntax, c),
          equivalenceKey: title),
        diagnostic);
    }

    private static async Task<Document> ChangeToStorableTypeAttribute(Document document, AttributeSyntax attrDecl, CancellationToken cancellationToken) {
      var oldAttrArgs = attrDecl.ArgumentList;

      // create new identifier
      var name = SyntaxFactory.IdentifierName("StorableType")
        .WithLeadingTrivia(attrDecl.Name.GetLeadingTrivia());

      // create guid arg
      var guid = Guid.NewGuid();
      var guidArg = SyntaxFactory.AttributeArgument(
        SyntaxFactory.LiteralExpression(
          SyntaxKind.StringLiteralExpression,
          SyntaxFactory.Literal(guid.ToString())
        ));

      var newAttrArgs = SyntaxFactory.AttributeArgumentList();
      if (oldAttrArgs != null) {
        // preserve old trivia
        newAttrArgs = oldAttrArgs.WithArguments(newAttrArgs.Arguments);

        // add old args
        foreach (var arg in oldAttrArgs.Arguments)
          newAttrArgs = newAttrArgs.AddArguments(arg);

        // add trailing trivia to name only if there is an old argument list
        // otherwise add it after the new argument list ...
        name = name.WithTrailingTrivia(attrDecl.Name.GetTrailingTrivia());
      } else {
        // ... here
        newAttrArgs = newAttrArgs.WithTrailingTrivia(attrDecl.Name.GetTrailingTrivia());
      }
      newAttrArgs = newAttrArgs.AddArguments(guidArg);

      // rename attr, add old trivia and format
      var newAttrDecl = attrDecl
        .WithName(name)
        .WithArgumentList(newAttrArgs)
        .WithAdditionalAnnotations(Formatter.Annotation);

      var root = await document.GetSyntaxRootAsync(cancellationToken) as CompilationUnitSyntax;
      var newRoot = root.ReplaceNode(attrDecl, newAttrDecl);

      // add using of HeuristicLab.Persistence if the document does not already have it 
      var oldUsings = root.Usings;
      if (oldUsings.All(x => x.Name.WithoutTrivia().ToString() != "HeuristicLab.Persistence")) {
        var persistenceUsing = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("HeuristicLab.Persistence"));
        newRoot = newRoot.WithUsings(oldUsings.Add(persistenceUsing));
      }

      return document.WithSyntaxRoot(newRoot);
    }
  }
}