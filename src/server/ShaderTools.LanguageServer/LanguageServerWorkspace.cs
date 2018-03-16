﻿using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ShaderTools.CodeAnalysis;
using ShaderTools.CodeAnalysis.Host.Mef;
using ShaderTools.CodeAnalysis.Text;

namespace ShaderTools.LanguageServer
{
    public sealed class LanguageServerWorkspace : Workspace
    {
        private readonly IMefHostExportProvider _hostServices;
        private readonly string _rootPath;

        public LanguageServerWorkspace(MefHostServices hostServices, string rootPath)
            : base(hostServices)
        {
            _hostServices = hostServices;
            _rootPath = rootPath;
        }

        public Document GetDocument(Uri uri)
        {
            return CurrentDocuments.GetDocumentWithFilePath(Helpers.FromUri(uri));
        }

        public (LogicalDocument logicalDocument, int position) GetLogicalDocument(TextDocumentPositionParams textDocumentPositionParams)
        {
            var document = GetDocument(textDocumentPositionParams.TextDocument.Uri);

            var documentPosition = document.SourceText.Lines.GetPosition(new LinePosition(
                (int) textDocumentPositionParams.Position.Line,
                (int) textDocumentPositionParams.Position.Character));

            var logicalDocument = document.GetLogicalDocument(new TextSpan(documentPosition, 0));

            return (logicalDocument, documentPosition - logicalDocument.SpanInParentRootFile.Start);
        }

        public Document OpenDocument(Uri uri, string text, string languageId)
        {
            var filePath = Helpers.FromUri(uri);

            var documentId = DocumentId.CreateNewId(filePath);
            var sourceText = SourceText.From(text, filePath);

            var document = CreateDocument(documentId, languageId, sourceText, filePath);
            OnDocumentOpened(document);
            return document;
        }

        public Document UpdateDocument(Document document, IEnumerable<TextChange> changes)
        {
            var newText = document.SourceText.WithChanges(changes);
            OnDocumentTextChanged(document.Id, newText);
            return CurrentDocuments.GetDocument(document.Id);
        }

        public void CloseDocument(DocumentId documentId)
        {
            OnDocumentClosed(documentId);
        }

        public T GetGlobalService<T>()
            where T : class
        {
            return _hostServices.GetExports<T>().FirstOrDefault()?.Value;
        }
    }
}
