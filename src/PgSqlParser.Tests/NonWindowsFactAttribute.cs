using System.Runtime.InteropServices;
using Xunit;

namespace PgSqlParser.Tests;

public class NonWindowsFactAttribute : FactAttribute
{
    public NonWindowsFactAttribute()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = "Test only runs on non-windows";
        }
    }
}