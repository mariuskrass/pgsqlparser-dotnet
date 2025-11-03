using System.Text.Json;
using System.Text.RegularExpressions;
using Google.Protobuf;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PgSqlParser.Tests;

public class ParserTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ParserTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Normalize()
    {
        var items = Utils.ReadLines("normalize_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = Parser.Normalize(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public void NormalizeUtility()
    {
        var items = Utils.ReadLines("normalize_utility_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = items[i + 1];
            var result = Parser.NormalizeUtility(query);
            result.Value.ShouldBe(expected);
        }
    }
    
    [Fact]
    public void Parse()
    {
        var items = Utils.ReadLines("parse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i];
            var expected = ParseResult.Parser.ParseJson(items[i + 1]);
            var result = Parser.Parse(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public void ParseWithOpts()
    {
        var items = Utils.ReadLines("parse_with_opts_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 3)
        {
            var query = items[i];
            var opts = (ParserOptions)int.Parse(items[i + 1]);
            var expected = ParseResult.Parser.ParseJson(items[i + 2]);
            var result = Parser.Parse(query, opts);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public void DeParse()
    {
        var items = Utils.ReadLines("deparse_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 1)
        {
            var query = items[i];
            var parseResult = Parser.Parse(query).Value;
            var result = Parser.Deparse(parseResult);
            var queryWithoutComments = Regex.Replace(result.Value!, @"/\*\s*Comment\s*\d+\s*\*/", "", RegexOptions.None);
            result.Value.ShouldBe(queryWithoutComments);
        }
    }
    
    [Fact]
    public void SplitWithScanner()
    {
        var items = Utils.ReadLines("split_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", Environment.NewLine);
            var expected = JsonSerializer.Deserialize<SplitResult>(items[i + 1]);
            var result = Parser.SplitWithScanner(query);
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(result.Value));
            // result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public void SplitWithParser()
    {
        var items = Utils.ReadLines("split_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", Environment.NewLine);
            var expected = JsonSerializer.Deserialize<SplitResult>(items[i + 1]);
            var result = Parser.SplitWithParser(query);
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(expected));
            _testOutputHelper.WriteLine(JsonSerializer.Serialize(result.Value));
            // result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public void Scan()
    {
        var items = Utils.ReadLines("scan_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", Environment.NewLine);
            var expected = ScanResult.Parser.ParseJson(items[i + 1]);
            var result = Parser.Scan(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
    
    [Fact]
    public void Fingerprint()
    {
        var items = Utils.ReadLines("fingerprint_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 2)
        {
            var query = items[i].Replace("\\n", Environment.NewLine);
            var expected = items[i + 1];
            var result = Parser.Fingerprint(query);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }

    [Fact]
    public void FingerprintWithOpts()
    {
        var items = Utils.ReadLines("fingerprint_with_opts_tests.txt").ToArray();
        for (var i = 0; i < items.Length; i += 3)
        {
            var query = items[i];
            var opts = (ParserOptions)int.Parse(items[i + 1]);
            var expected = items[i + 2];
            var result = Parser.Fingerprint(query, opts);
            result.Value.ShouldBeEquivalentTo(expected);
        }
    }
}