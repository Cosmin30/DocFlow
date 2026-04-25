namespace DocFlow.BuildingBlocks.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "DocFlow";
    public string Audience { get; set; } = "DocFlow.Client";
    public string Key { get; set; } = "DocFlow-Change-This-Key-Before-Production-32-Bytes-Minimum";
}
