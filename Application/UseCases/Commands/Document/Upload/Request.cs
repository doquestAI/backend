using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Commands.Document.Upload;

public record Request(IFormFile File) : IRequest<Response>;