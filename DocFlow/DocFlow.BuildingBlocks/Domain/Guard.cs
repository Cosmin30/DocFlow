namespace DocFlow.BuildingBlocks.Domain;

public static class Guard
{
    public static void NotEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
    }

    public static void NotEmpty(DateTime value, string parameterName)
    {
        if (value == default)
            throw new ArgumentException($"{parameterName} cannot be default.", parameterName);
    }

    public static void NotNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
    }

    public static void NotNegative(long value, string parameterName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} cannot be negative.");
    }

    public static void NotNegativeOrZero(int value, string parameterName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be greater than zero.");
    }

    public static void MaxLength(string? value, int maxLength, string parameterName)
    {
        if (value is not null && value.Length > maxLength)
            throw new ArgumentException($"{parameterName} must not exceed {maxLength} characters.", parameterName);
    }

    public static void MinLength(string? value, int minLength, string parameterName)
    {
        if (value is not null && value.Length < minLength)
            throw new ArgumentException($"{parameterName} must be at least {minLength} characters.", parameterName);
    }

    public static void InRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be between {min} and {max}.");
    }

    public static void EnumIsValid<T>(T value, string parameterName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentException($"{parameterName} is not a valid {typeof(T).Name} value.", parameterName);
    }
}
