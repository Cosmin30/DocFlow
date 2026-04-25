using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DocFlow.BuildingBlocks.Validation;

public static class DbContextValidationExtensions
{
    public static void ValidateTrackedEntities(this DbContext dbContext)
    {
        var validationErrors = new List<ValidationResult>();

        foreach (var entry in dbContext.ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            var entity = entry.Entity;
            var entityValidationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(entity);

            Validator.TryValidateObject(entity, validationContext, entityValidationResults, validateAllProperties: true);
            ValidateRequiredGuids(entity, entityValidationResults);

            if (entityValidationResults.Count == 0)
            {
                continue;
            }

            var entityName = entity.GetType().Name;
            foreach (var error in entityValidationResults)
            {
                var members = error.MemberNames?.Any() == true
                    ? string.Join(", ", error.MemberNames)
                    : "Entity";

                validationErrors.Add(new ValidationResult($"{entityName}.{members}: {error.ErrorMessage}"));
            }
        }

        if (validationErrors.Count > 0)
        {
            var message = string.Join(Environment.NewLine, validationErrors.Select(x => x.ErrorMessage));
            throw new ValidationException(message);
        }
    }

    private static void ValidateRequiredGuids(object entity, ICollection<ValidationResult> validationResults)
    {
        var properties = entity.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(Guid) && Attribute.IsDefined(p, typeof(RequiredAttribute)));

        foreach (var property in properties)
        {
            var value = (Guid?)property.GetValue(entity);
            if (value is null || value == Guid.Empty)
            {
                validationResults.Add(new ValidationResult(
                    $"{property.Name} must not be empty.",
                    [property.Name]));
            }
        }
    }
}
