using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.Mef;
using OmniSharp.Models;
using OmniSharp.Options;
using OmniSharp.Roslyn.CSharp.Workers.Formatting;

namespace OmniSharp.Roslyn.CSharp.Services.Formatting
{
    [OmniSharpHandler(OmnisharpEndpoints.FormatRange, LanguageNames.CSharp)]
    public class FormatRangeService : RequestHandler<FormatRangeRequest, FormatRangeResponse>
    {
        private readonly OmniSharpWorkspace _workspace;

        [ImportingConstructor]
        public FormatRangeService(OmniSharpWorkspace workspace)
        {
            _workspace = workspace;
        }

        public async Task<FormatRangeResponse> Handle(FormatRangeRequest request)
        {
            var document = _workspace.GetDocument(request.FileName);
            if (document == null)
            {
                return null;
            }

            var text = await document.GetTextAsync();
            var start = text.Lines.GetPosition(new LinePosition(request.Line, request.Column));
            var end = text.Lines.GetPosition(new LinePosition(request.EndLine, request.EndColumn));
            var changes = await FormattingWorker.GetFormattingChangesForRange(_workspace, document, start, end);

            return new FormatRangeResponse()
            {
                Changes = changes
            };
        }
    }
}
