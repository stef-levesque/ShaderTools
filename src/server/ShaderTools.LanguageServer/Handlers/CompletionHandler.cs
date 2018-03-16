using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using ShaderTools.CodeAnalysis.Completion;
using CompletionList = OmniSharp.Extensions.LanguageServer.Protocol.Models.CompletionList;
using CompletionItem = OmniSharp.Extensions.LanguageServer.Protocol.Models.CompletionItem;
using ShaderTools.CodeAnalysis.Shared.Extensions;
using ShaderTools.CodeAnalysis;

namespace ShaderTools.LanguageServer.Handlers
{
    internal sealed class CompletionHandler : ICompletionHandler
    {
        private readonly LanguageServerWorkspace _workspace;
        private readonly TextDocumentRegistrationOptions _registrationOptions;

        public CompletionHandler(LanguageServerWorkspace workspace, TextDocumentRegistrationOptions registrationOptions)
        {
            _workspace = workspace;
            _registrationOptions = registrationOptions;
        }

        public CompletionRegistrationOptions GetRegistrationOptions()
        {
            return new CompletionRegistrationOptions
            {
                DocumentSelector = _registrationOptions.DocumentSelector,
                TriggerCharacters = new Container<string>(".", ":"),
                ResolveProvider = false
            };
        }

        public async Task<CompletionList> Handle(TextDocumentPositionParams request, CancellationToken token)
        {
            var (document, position) = _workspace.GetLogicalDocument(request);

            var completionService = document.GetLanguageService<CompletionService>();

            var completionList = await completionService.GetCompletionsAsync(document, position);
            if (completionList == null)
            {
                return new CompletionList();
            }

            var completionItems = completionList.Items
                .Select(x => ConvertCompletionItem(document, completionList.Rules, x))
                .ToArray();

            return completionItems;
        }

        public void SetCapability(CompletionCapability capability) { }

        private static CompletionItem ConvertCompletionItem(LogicalDocument document, CompletionRules completionRules, CodeAnalysis.Completion.CompletionItem item)
        {
            var documentation = CommonCompletionItem.HasDescription(item)
                ? CommonCompletionItem.GetDescription(item).Text
                : string.Empty;

            return new CompletionItem
            {
                Label = item.DisplayText,
                SortText = item.SortText,
                FilterText = item.FilterText,
                Kind = CompletionItemKind.Class,
                TextEdit = new TextEdit
                {
                    NewText = item.DisplayText,
                    Range = Helpers.ToRange(document, item.Span)
                },
                Documentation = documentation,
                CommitCharacters = completionRules.DefaultCommitCharacters.Select(x => x.ToString()).ToArray()
            };
        }
    }
}
