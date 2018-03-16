﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ShaderTools.CodeAnalysis;
using ShaderTools.CodeAnalysis.NavigateTo;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.LanguageServer
{
    internal static class Helpers
    {
        public static string FromUri(Uri uri)
        {
            if (uri.Segments.Length > 1)
            {
                if (uri.Segments[1].IndexOf("%3a", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    return FromUri(new Uri(uri.AbsoluteUri.Replace("%3a", ":").Replace("%3A", ":")));
                }
            }
            return uri.LocalPath;
        }

        public static Uri ToUri(string filePath)
        {
            return (!filePath.StartsWith("/"))
                ? new Uri($"file:///{filePath}")
                : new Uri($"file://{filePath}");
        }

        public static Range ToRange(SourceText sourceText, TextSpan textSpan)
        {
            var linePositionSpan = sourceText.Lines.GetLinePositionSpan(textSpan);

            return new Range
            {
                Start = new Position
                {
                    Line = linePositionSpan.Start.Line,
                    Character = linePositionSpan.Start.Character
                },
                End = new Position
                {
                    Line = linePositionSpan.End.Line,
                    Character = linePositionSpan.End.Character
                }
            };
        }

        public static Range ToRange(LogicalDocument document, TextSpan textSpan)
        {
            // Map textSpan to parent document.
            textSpan = new TextSpan(textSpan.Start + document.SpanInParentRootFile.Start, textSpan.Length);

            return ToRange(document.Parent.SourceText, textSpan);
        }

        private const string DiagnosticSourceName = "Shader Tools";

        public static Diagnostic ToDiagnostic(this CodeAnalysis.Diagnostics.MappedDiagnostic diagnostic)
        {
            var sourceFileSpan = diagnostic.FileSpan;

            var linePositionSpan = sourceFileSpan.File.IsRootFile
                ? ToRange(diagnostic.LogicalDocument, sourceFileSpan.Span)
                : ToRange(sourceFileSpan.File.Text, sourceFileSpan.Span);

            return new Diagnostic
            {
                Severity = ToDiagnosticSeverity(diagnostic.Diagnostic.Severity),
                Message = diagnostic.Diagnostic.Message,
                Code = diagnostic.Diagnostic.Descriptor.Id,
                Source = DiagnosticSourceName,
                Range = linePositionSpan
            };
        }

        private static DiagnosticSeverity ToDiagnosticSeverity(CodeAnalysis.Diagnostics.DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case CodeAnalysis.Diagnostics.DiagnosticSeverity.Error:
                    return DiagnosticSeverity.Error;

                case CodeAnalysis.Diagnostics.DiagnosticSeverity.Warning:
                    return DiagnosticSeverity.Warning;

                default:
                    return DiagnosticSeverity.Error;
            }
        }

        public static TextChange ToTextChange(Document document, Range changeRange, string insertString)
        {
            var startPosition = document.SourceText.Lines.GetPosition(ToLinePosition(changeRange.Start));
            var endPosition = document.SourceText.Lines.GetPosition(ToLinePosition(changeRange.End));

            return new TextChange(
                TextSpan.FromBounds(startPosition, endPosition), 
                insertString);
        }

        private static LinePosition ToLinePosition(Position position)
        {
            return new LinePosition((int)position.Line, (int) position.Character);
        }

        public static async Task FindSymbolsInDocument<TSymbolInformation>(
            INavigateToSearchService searchService,
            LogicalDocument document,
            string searchPattern,
            CancellationToken cancellationToken,
            ImmutableArray<TSymbolInformation>.Builder resultsBuilder)
            where TSymbolInformation : ISymbolInformation, new()
        {
            var foundSymbols = await searchService.SearchDocumentAsync(document, searchPattern, cancellationToken);

            resultsBuilder.AddRange(foundSymbols
               .Select(r => new TSymbolInformation
               {
                   ContainerName = r.AdditionalInformation,
                   Kind = GetSymbolKind(r.Kind),
                   Location = new Location
                   {
                       Uri = Helpers.ToUri(r.NavigableItem.SourceSpan.File.FilePath),
                       Range = Helpers.ToRange(r.NavigableItem.Document, r.NavigableItem.SourceSpan.Span)
                   },
                   Name = r.Name
               }));
        }

        private static SymbolKind GetSymbolKind(string symbolType)
        {
            switch (symbolType)
            {
                case NavigateToItemKind.Class:
                    return SymbolKind.Class;

                case NavigateToItemKind.Structure:
                    return SymbolKind.Struct;

                case NavigateToItemKind.Module:
                    return SymbolKind.Namespace;

                case NavigateToItemKind.Interface:
                    return SymbolKind.Interface;

                case NavigateToItemKind.Field:
                    return SymbolKind.Field;

                case NavigateToItemKind.Method:
                    return SymbolKind.Method;

                default:
                    return SymbolKind.Variable;
            }
        }
    }
}
