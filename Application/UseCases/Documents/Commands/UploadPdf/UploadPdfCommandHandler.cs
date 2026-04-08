using Application.Common;
using MediatR;

namespace Application.UseCases.Documents.Commands.UploadPdf;

internal class UploadPdfCommandHandler(

)
    : IRequestHandler<Request, Result<UploadPdfResponse>>
{
    public Task<Result<UploadPdfResponse>> Handle(Request request,
         CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
