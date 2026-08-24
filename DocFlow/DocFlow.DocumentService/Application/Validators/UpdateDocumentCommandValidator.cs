using DocFlow.DocumentService.Application.CQRS.Documents.Commands.UpdateDocument;
using FluentValidation;

namespace DocFlow.DocumentService.Application.Validators;

public sealed class UpdateDocumentCommandValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Document Id is required.");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Request.Title)
            .MinimumLength(2).WithMessage("Title must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .When(x => x.Request.Title is not null);

        RuleFor(x => x.Request.Category)
            .MinimumLength(2).WithMessage("Category must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.")
            .When(x => x.Request.Category is not null);

        RuleFor(x => x.Request.Department)
            .MinimumLength(2).WithMessage("Department must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.")
            .When(x => x.Request.Department is not null);

        RuleFor(x => x.Request.TagsCsv)
            .MaximumLength(500).WithMessage("TagsCsv must not exceed 500 characters.")
            .When(x => x.Request.TagsCsv is not null);

        RuleFor(x => x.Request.NewFileName)
            .NotEmpty().WithMessage("NewFileName cannot be empty when provided.")
            .MaximumLength(255).WithMessage("NewFileName must not exceed 255 characters.")
            .When(x => x.Request.NewFileName is not null);

        RuleFor(x => x.Request.NewStoragePath)
            .NotEmpty().WithMessage("NewStoragePath cannot be empty when provided.")
            .MaximumLength(1000).WithMessage("NewStoragePath must not exceed 1000 characters.")
            .When(x => x.Request.NewStoragePath is not null);

        RuleFor(x => x.Request.NewSizeBytes)
            .GreaterThan(0).WithMessage("NewSizeBytes must be greater than 0.")
            .When(x => x.Request.NewSizeBytes.HasValue);

        RuleFor(x => x.Request)
            .Must(x => (x.NewFileName is null) == (x.NewStoragePath is null))
            .WithMessage("NewFileName and NewStoragePath must both be provided or both be null.");
    }
}
