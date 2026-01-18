namespace TomLauncher.Backend;

public class Dependancy(string m, Version v)
{
    public string Mod { get; set; } = m;
    public Version Version { get; set; } = v;
}