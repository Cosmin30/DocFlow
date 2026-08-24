using DocFlow.DocumentService.Application.CQRS.Documents.Commands.CreateDocument;
using FluentValidation;

namespace DocFlow.DocumentService.Application.Validators;

public sealed class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("TenantId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Request.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(2).WithMessage("Title must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Request.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MinimumLength(2).WithMessage("Category must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

        RuleFor(x => x.Request.Department)
            .NotEmpty().WithMessage("Department is required.")
            .MinimumLength(2).WithMessage("Department must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters.");

        RuleFor(x => x.Request.TagsCsv)
            .MaximumLength(500).WithMessage("TagsCsv must not exceed 500 characters.");

        RuleFor(x => x.Request.FileName)
            .NotEmpty().WithMessage("FileName is required.")
            .MaximumLength(255).WithMessage("FileName must not exceed 255 characters.");

        RuleFor(x => x.Request.StoragePath)
            .NotEmpty().WithMessage("StoragePath is required.")
            .MaximumLength(1000).WithMessage("StoragePath must not exceed 1000 characters.");

        RuleFor(x => x.Request.SizeBytes)
            .GreaterThan(0).WithMessage("SizeBytes must be greater than 0.");

        RuleFor(x => x.Request.ExpiresAtUtc)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration must be in the future.")
            .When(x => x.Request.ExpiresAtUtc.HasValue);
    }
}
