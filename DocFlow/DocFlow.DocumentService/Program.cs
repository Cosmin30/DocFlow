using DocFlow.BuildingBlocks.Security;
using DocFlow.DocumentService.Application.CQRS.Abstractions;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.RestoreDocumentVersion;
using DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;
using DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocumentVersions;
using DocFlow.DocumentService.Application.CQRS.Documents.Queries.GetDocuments;
using DocFlow.DocumentService.Domain.Entities;
using DocFlow.DocumentService.Infrastructure.Persistence;
using DocFlow.DocumentService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DocumentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDocFlowJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IQueryHandler<GetDocumentsQuery, List<Document>>, GetDocumentsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetDocumentVersionsQuery, List<DocumentVersion>>, GetDocumentVersionsQueryHandler>();
builder.Services.AddScoped<ICommandHandler<CreateDocumentCommand, Document>, CreateDocumentCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateDocumentCommand, Document?>, UpdateDocumentCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RestoreDocumentVersionCommand, bool>, RestoreDocumentVersionCommandHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();
    db.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
