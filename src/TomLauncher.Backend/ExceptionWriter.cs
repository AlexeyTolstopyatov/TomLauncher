namespace TomLauncher.Backend;

public static class ExceptionWriter
{
    public static void Write(Exception e)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory + "Crash.log";
        using var fs = new FileStream(path, FileMode.Create);
        using var writer = new StreamWriter(fs);

        writer.WriteAsync(e.ToString());
    }
}