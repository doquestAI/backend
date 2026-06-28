using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Commands.Document.Upload;

internal record Request(IFormFile File) : IRequest<Response>;