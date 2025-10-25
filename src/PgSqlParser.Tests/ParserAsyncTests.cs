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
        var items = ReadLines("normalize_tests.txt").ToArray();
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
        var items = ReadLines("normalize_utility_tests.txt").ToArray();
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
        var items = ReadLines("parse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = await Parser.ParseAsync(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public async Task ParseOpts()
    {
        var items = ReadLines("parse_with_opts_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 3)
        {
            var query = items[i];
            var opts = (LibPgQuery.PgQueryParserOptions)int.Parse(items[i + 1]);
            var expected = items[i + 2];
            var result = await Parser.ParseOptsAsync(query, opts);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public async Task DeParseOpts()
    {
        var items = ReadLines("deparse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 1)
        {
            var query = items[i];
            var parseJsonStr = Parser.Parse(query).Value;
            var result = await Parser.DeparseProtoBufAsync(ParseResult.Parser.ParseJson(parseJsonStr));
            var queryWithoutComments = Regex.Replace(result.Value!, @"/\*\s*Comment\s*\d+\s*\*/", "", RegexOptions.None);
            result.Value.ShouldBe(queryWithoutComments);
        }
    }
}