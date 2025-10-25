using System.Runtime.InteropServices;
using Google.Protobuf;

namespace PgSqlParser;

public record Error(string? Message, string? FuncName, string? FileName, int LineNo, int CursorPos, string? Context);

public readonly struct Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }
    public bool IsSuccess { get; }

    private Result(T value)
    {
        Value = value;
        Error = null;
        IsSuccess = true;
    }

    private Result(Error error)
    {
        Value = default;
        Error = error;
        IsSuccess = false;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}

public record QuerySplitStmt(int Location, int Length, string Text);

public static class Parser
{
    public static Result<string> Normalize(string query)
    {
        var result = LibPgQuery.pg_query_normalize(query);

        try
        {
            return result.error == IntPtr.Zero
                ? Result<string>.Success(Marshal.PtrToStringUTF8(result.normalized_query) ?? string.Empty)
                : Result<string>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_normalize_result(result);
        }
    }
    
    public static Task<Result<string>> NormalizeAsync(string query, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => Normalize(query), cancellationToken);
    }

    public static Result<string> NormalizeUtility(string query)
    {
        var result = LibPgQuery.pg_query_normalize_utility(query);

        try
        {
            return result.error == IntPtr.Zero
                ? Result<string>.Success(Marshal.PtrToStringUTF8(result.normalized_query) ?? string.Empty)
                : Result<string>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_normalize_result(result);
        }
    }

    public static Task<Result<string>> NormalizeUtilityAsync(string query, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => NormalizeUtility(query), cancellationToken);
    }

    public static Result<ScanResult> Scan(string query)
    {
        var result = LibPgQuery.pg_query_scan(query);
        try
        {
            return result.error == IntPtr.Zero
                ? Result<ScanResult>.Success(ScanResult.Parser.ParseFrom(ReadProtobuf(result.pbuf)))
                : Result<ScanResult>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_scan_result(result);
        }
    }
    
    public static Task<Result<ScanResult>> ScanAsync(string query, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => Scan(query), cancellationToken);
    }

    public static Result<string> Parse(string query)
    {
        var result = LibPgQuery.pg_query_parse(query);

        try
        {
            return result.error == IntPtr.Zero
                ? Result<string>.Success(Marshal.PtrToStringUTF8(result.parse_tree) ?? string.Empty)
                : Result<string>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_parse_result(result);
        }
    }

    public static Task<Result<string>> ParseAsync(string query, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => Parse(query), cancellationToken);
    }

    public static Result<string> ParseOpts(string query, LibPgQuery.PgQueryParserOptions parserOptions = LibPgQuery.PgQueryParserOptions.Default)
    {
        var result = LibPgQuery.pg_query_parse_opts(query, (int)parserOptions);

        try
        {
            return result.error == IntPtr.Zero
                ? Result<string>.Success(Marshal.PtrToStringUTF8(result.parse_tree) ?? string.Empty)
                : Result<string>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_parse_result(result);
        }
    }

    public static Task<Result<string>> ParseOptsAsync(string query, LibPgQuery.PgQueryParserOptions parserOptions = LibPgQuery.PgQueryParserOptions.Default, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => ParseOpts(query, parserOptions), cancellationToken);
    }

    public static Result<ParseResult?> ParseProtoBuf(string query)
    {
        var result = LibPgQuery.pg_query_parse_protobuf(query);

        try
        {
            return result.error == IntPtr.Zero 
                ? Result<ParseResult?>.Success(ParseResult.Parser.ParseFrom(ReadProtobuf(result.parse_tree))) 
                : Result<ParseResult?>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_protobuf_parse_result(result);
        }
    }

    public static Task<Result<ParseResult?>> ParseProtoBufAsync(string query, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => ParseProtoBuf(query), cancellationToken);
    }
    
    public static Result<ParseResult?> ParseProtoBufOpts(string query, LibPgQuery.PgQueryParserOptions parserOptions = LibPgQuery.PgQueryParserOptions.Default)
    {
        var result = LibPgQuery.pg_query_parse_protobuf_opts(query, (int)parserOptions);

        try
        {
            return result.error == IntPtr.Zero 
                ? Result<ParseResult?>.Success(ParseResult.Parser.ParseFrom(ReadProtobuf(result.parse_tree))) 
                : Result<ParseResult?>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_protobuf_parse_result(result);
        }
    }
    
    public static Task<Result<ParseResult?>> ParseProtoBufOptsAsync(string query, LibPgQuery.PgQueryParserOptions parserOptions = LibPgQuery.PgQueryParserOptions.Default, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => ParseProtoBufOpts(query, parserOptions), cancellationToken);
    }
    
    public static Result<string> ParsePlpgsql(string query)
    {
        var result = LibPgQuery.pg_query_parse_plpgsql(query);
        
        try
        {
            return result.error == IntPtr.Zero 
                ? Result<string>.Success(Marshal.PtrToStringUTF8(result.plpgsql_funcs) ?? string.Empty) 
                : Result<string>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_plpgsql_parse_result(result);
        }
    }

    public static Task<Result<string>> ParsePlpgsqlAsync(string query, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => ParsePlpgsql(query), cancellationToken);
    }
    
    public static Result<string> Fingerprint(string query)
    {
        var result = LibPgQuery.pg_query_fingerprint(query);

        try
        {
            return result.error == IntPtr.Zero
                ? Result<string>.Success(Marshal.PtrToStringUTF8(result.fingerprint_str) ?? string.Empty)
                : Result<string>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_fingerprint_result(result);
        }
    }
    
    public static Task<Result<string>> FingerprintAsync(string query, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => Fingerprint(query), cancellationToken);
    }

    public static Result<string> FingerprintOpts(string query,
        LibPgQuery.PgQueryParserOptions parserOptions = LibPgQuery.PgQueryParserOptions.Default)
    {
        var result = LibPgQuery.pg_query_fingerprint_opts(query, (int)parserOptions);
        try
        {
            return result.error == IntPtr.Zero
                ? Result<string>.Success(Marshal.PtrToStringUTF8(result.fingerprint_str) ?? string.Empty)
                : Result<string>.Failure(ParseError(result.error));
        }
        finally
        {
            LibPgQuery.pg_query_free_fingerprint_result(result);
        }
    }
    
    public static Task<Result<string>> FingerprintOptsAsync(string query,
        LibPgQuery.PgQueryParserOptions parserOptions = LibPgQuery.PgQueryParserOptions.Default,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(() => FingerprintOpts(query, parserOptions), cancellationToken);
    }

    public static Result<List<QuerySplitStmt>> SplitWithScanner(string query)
    {
        var result = LibPgQuery.pg_query_split_with_scanner(query);

        try
        {
            if (result.error != IntPtr.Zero)
                return Result<List<QuerySplitStmt>>.Failure(ParseError(result.error));
            
            var nStmts = result.n_stmts;
            var stmts = new List<QuerySplitStmt>();
            for (var i = 0; i < nStmts; i++)
            {
                var stmtPtrPtr = Marshal.ReadIntPtr(result.stmts, i * IntPtr.Size);
                var stmt = Marshal.PtrToStructure<LibPgQuery.PgQuerySplitStmt>(stmtPtrPtr);
                var text = query.Substring(stmt.stmt_location, stmt.stmt_len);
                stmts.Add(new QuerySplitStmt(stmt.stmt_location, stmt.stmt_len, text));
            }

            return Result<List<QuerySplitStmt>>.Success(stmts);
        }
        finally
        {
            LibPgQuery.pg_query_free_split_result(result);
        }
    }

    public static Task<Result<List<QuerySplitStmt>>> SplitWithScannerAsync(string query,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(() => SplitWithScanner(query), cancellationToken);
    }
    
    public static Result<List<QuerySplitStmt>> SplitWithParser(string query)
    {
        var result = LibPgQuery.pg_query_split_with_parser(query);

        try
        {
            if (result.error != IntPtr.Zero)
                return Result<List<QuerySplitStmt>>.Failure(ParseError(result.error));
            
            var nStmts = result.n_stmts;
            var stmts = new List<QuerySplitStmt>();
            for (var i = 0; i < nStmts; i++)
            {
                var stmtPtrPtr = Marshal.ReadIntPtr(result.stmts, i * IntPtr.Size);
                var stmt = Marshal.PtrToStructure<LibPgQuery.PgQuerySplitStmt>(stmtPtrPtr);
                var text = query.Substring(stmt.stmt_location, stmt.stmt_len);
                stmts.Add(new QuerySplitStmt(stmt.stmt_location, stmt.stmt_len, text));
            }

            return Result<List<QuerySplitStmt>>.Success(stmts);
        }
        finally
        {   
            LibPgQuery.pg_query_free_split_result(result);
        }
    }

    public static Task<Result<List<QuerySplitStmt>>> SplitWithParserAsync(string query,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(() => SplitWithParser(query), cancellationToken);
    }

    public static Result<string> DeparseProtoBuf(ParseResult parseResult)
    {
        var updatedBytes = parseResult.ToByteArray();
        LibPgQuery.PgQueryProtobuf parseTree;
        parseTree.len = (UIntPtr)updatedBytes.Length;
        parseTree.data = Marshal.AllocHGlobal(updatedBytes.Length);
        Marshal.Copy(updatedBytes, 0, parseTree.data, updatedBytes.Length);

        var deparseResult = LibPgQuery.pg_query_deparse_protobuf(parseTree);

        try
        {
            return deparseResult.error != IntPtr.Zero
                ? Result<string>.Failure(ParseError(deparseResult.error))
                : Result<string>.Success(Marshal.PtrToStringUTF8(deparseResult.query) ?? string.Empty);
        }
        finally
        {
            LibPgQuery.pg_query_free_deparse_result(deparseResult);
        }
    }

    public static Task<Result<string>> DeparseProtoBufAsync(ParseResult parseResult,
        CancellationToken cancellationToken = default)
    {
        return RunAsync(() => DeparseProtoBuf(parseResult), cancellationToken);
    }
    
    private static byte[] ReadProtobuf(LibPgQuery.PgQueryProtobuf pbuf)
    {
        var len = checked((int)pbuf.len);
        var buffer = new byte[len];
        Marshal.Copy(pbuf.data, buffer, 0, len);
        return buffer;
    }

    private static Error ParseError(IntPtr errorPtr)
    {
        var error = Marshal.PtrToStructure<LibPgQuery.PgQueryError>(errorPtr);
        return new Error(
            Marshal.PtrToStringUTF8(error.message), 
            Marshal.PtrToStringUTF8(error.message),
            Marshal.PtrToStringUTF8(error.funcname),
            error.lineno,
            error.cursorpos,
            Marshal.PtrToStringUTF8(error.context));
    }
    
    private static Task<Result<T>> RunAsync<T>(Func<Result<T>> fn, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return fn();
        }, cancellationToken);
    }
}