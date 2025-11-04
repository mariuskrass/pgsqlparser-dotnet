namespace PgSqlParser.Tests;

static class Utils
{
    public static IEnumerable<string> ReadLines(string path)
    {
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }
    
    public static string ReadFile(string path)
    {
        return File.ReadAllText(path);
    }
}