using System.Text.Json;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace PgSqlParser.Tests;

public class ParserAsyncTests
{
    private IEnumerable<string> ReadLines(string path)
    {
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is { } line)
        {
            yield return line;
        }
    }
    
    [Fact]
    public async Task Normalize()
    {
        var items = Utils.ReadLines("normalize_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = await Parser.NormalizeAsync(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public async Task NormalizeUtility()
    {
        var items = Utils.ReadLines("normalize_utility_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = await Parser.NormalizeUtilityAsync(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public async Task Parse()
    {
        var items = Utils.ReadLines("parse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = ParseResult.Parser.ParseJson(items[i + 1]);
            var result = await Parser.ParseAsync(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }

    [Fact]
    public async Task ParseWithOpts()
    {
        var items = Utils.ReadLines("parse_with_opts_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 3)
        {
            var query = items[i];
            var opts = (ParserOptions)int.Parse(items[i + 1]);
            var expected = ParseResult.Parser.ParseJson(items[i + 2]);
            var result = await Parser.ParseAsync(query, opts);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public async Task DeParse()
    {
        var items = Utils.ReadLines("deparse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 1)
        {
            var query = items[i];
            var parseResult = Parser.Parse(query).Value;
            var result = await Parser.DeparseAsync(parseResult);
            var queryWithoutComments = Regex.Replace(result.Value!, @"/\*\s*Comment\s*\d+\s*\*/", "", RegexOptions.None);
            result.Value.ShouldBe(queryWithoutComments);
        }
    }
    
    [Fact]
    public async Task SplitWithScanner()
    {
        var items = Utils.ReadLines("split_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", "\n");
            var expected = JsonSerializer.Deserialize<SplitResult>(items[i + 1]);
            var result = await Parser.SplitWithScannerAsync(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public async Task SplitWithParser()
    {
        var items = Utils.ReadLines("split_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", "\n");
            var expected = JsonSerializer.Deserialize<SplitResult>(items[i + 1]);
            var result = await Parser.SplitWithParserAsync(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public async Task Scan()
    {
        var items = Utils.ReadLines("scan_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", "\n");
            var expected = ScanResult.Parser.ParseJson(items[i + 1]);
            var result = await Parser.ScanAsync(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public async Task Fingerprint()
    {
        var items = Utils.ReadLines("fingerprint_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", "\n");
            var expected = items[i + 1];
            var result = await Parser.FingerprintAsync(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }

    [Fact]
    public async Task FingerprintWithOpts()
    {
        var items = Utils.ReadLines("fingerprint_with_opts_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 3)
        {
            var query = items[i];
            var opts = (ParserOptions)int.Parse(items[i + 1]);
            var expected = items[i + 2];
            var result = await Parser.FingerprintAsync(query, opts);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [NonWindowsFact]
    public async Task ParsePlpgsql()
    {
        var sql = Utils.ReadFile("plpgsql_samples.sql");
        sql = sql.Replace("\r\n", "\n");
        var result = await Parser.ParsePlpgsqlAsync(sql);
        var resultVal = result.Value.Replace("\r\n", "\n");
        var expected = Utils.ReadFile("plpgsql_samples.expected.json");
        expected = expected.Replace("\r\n", "\n");
        resultVal.ShouldBe(expected);
    }
}