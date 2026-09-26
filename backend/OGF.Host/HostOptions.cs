namespace OGF.Host;

public sealed record HostOptions
{
    public required string[] Modules { get; init; }
}