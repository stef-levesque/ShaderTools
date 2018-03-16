﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ShaderTools.CodeAnalysis;
using ShaderTools.CodeAnalysis.QuickInfo;

namespace ShaderTools.LanguageServer.Handlers
{
    internal sealed class HoverHandler : IHoverHandler
    {
        private readonly LanguageServerWorkspace _workspace;
        private readonly TextDocumentRegistrationOptions _registrationOptions;

        public HoverHandler(LanguageServerWorkspace workspace, TextDocumentRegistrationOptions registrationOptions)
        {
            _workspace = workspace;
            _registrationOptions = registrationOptions;
        }

        public TextDocumentRegistrationOptions GetRegistrationOptions() => _registrationOptions;

        public async Task<Hover> Handle(TextDocumentPositionParams request, CancellationToken token)
        {
            var (document, position) = _workspace.GetLogicalDocument(request);

            var providerCoordinatorFactory = _workspace.GetGlobalService<IQuickInfoProviderCoordinatorFactory>();
            var providerCoordinator = providerCoordinatorFactory.CreateCoordinator(document);

            var (item, _) = await providerCoordinator.GetItemAsync(document, position, token);

            if (item != null)
            {
                var symbolInfo = new List<MarkupContent>();
                Range symbolRange;

                var markdownText = string.Empty;

                switch (item.Content)
                {
                    case QuickInfoDisplayContent c:
                        markdownText += $"``` {document.Language}\n{c.MainDescription.GetFullText()}\n```\n";

                        if (!c.Documentation.IsEmpty)
                        {
                            markdownText += c.Documentation.GetFullText();
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                symbolRange = Helpers.ToRange(document, item.TextSpan);

                return new Hover
                {
                    Contents = new MarkedStringsOrMarkupContent(new MarkupContent { Kind = MarkupKind.Markdown, Value = markdownText }),
                    Range = symbolRange
                };
            }
            else
            {
                return null;
            }
        }

        public void SetCapability(HoverCapability capability) { }
    }
}
