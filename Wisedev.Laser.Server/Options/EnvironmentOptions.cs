namespace Wisedev.Laser.Server.Options;

internal record EnvironmentOptions
{
    public int MajorVersion { get; set; }
    public int MinorVersion { get; set; }
    public int Build { get; set; }

    public string Environment { get; set; }

    public string Version => $"{MajorVersion}.{Build}.{MinorVersion}";
}
