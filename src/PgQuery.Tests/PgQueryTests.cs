using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace PgQuery.Tests;

public class PgParserTests
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
    public void NormalizeTests()
    {
        var items = ReadLines("normalize_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = Parser.Normalize(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public void NormalizeUtilityTests()
    {
        var items = ReadLines("normalize_utility_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = Parser.NormalizeUtility(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public void ParseTests()
    {
        var items = ReadLines("parse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = Parser.Parse(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public void ParseOptsTests()
    {
        var items = ReadLines("parse_with_opts_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 3)
        {
            var query = items[i];
            var opts = (LibPgQuery.PgQueryParserOptions)int.Parse(items[i + 1]);
            var expected = items[i + 2];
            var result = Parser.ParseOpts(query, opts);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public void DeParseOptsTests()
    {
        var items = ReadLines("deparse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 1)
        {
            var query = items[i];
            var parseJsonStr = Parser.Parse(query).Value;
            var result = Parser.DeparseProtoBuf(ParseResult.Parser.ParseJson(parseJsonStr));
            var queryWithoutComments = Regex.Replace(result.Value!, @"/\*\s*Comment\s*\d+\s*\*/", "", RegexOptions.None);
            result.Value.ShouldBe(queryWithoutComments);
        }
    }
}