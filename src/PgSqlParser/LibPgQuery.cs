namespace PgSqlParser;

using System;
using System.Runtime.InteropServices;

public static class LibPgQuery
{
    private const string DllName = "libpg_query";

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryError
    {
        public IntPtr message;
        public IntPtr funcname;
        public IntPtr filename;
        public int lineno;
        public int cursorpos;
        public IntPtr context;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryProtobuf
    {
        public UIntPtr len;
        public IntPtr data;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryScanResult
    {
        public PgQueryProtobuf pbuf;
        public IntPtr stderr_buffer;
        public IntPtr error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryParseResult
    {
        public IntPtr parse_tree;
        public IntPtr stderr_buffer;
        public IntPtr error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryProtobufParseResult
    {
        public PgQueryProtobuf parse_tree;
        public IntPtr stderr_buffer;
        public IntPtr error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQuerySplitStmt
    {
        public int stmt_location;
        public int stmt_len;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQuerySplitResult
    {
        public IntPtr stmts; // PgQuerySplitStmt**
        public int n_stmts;
        public IntPtr stderr_buffer;
        public IntPtr error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryDeparseResult
    {
        public IntPtr query;
        public IntPtr error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryPlpgsqlParseResult
    {
        public IntPtr plpgsql_funcs;
        public IntPtr error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryFingerprintResult
    {
        public ulong fingerprint;
        public IntPtr fingerprint_str;
        public IntPtr stderr_buffer;
        public IntPtr error;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PgQueryNormalizeResult
    {
        public IntPtr normalized_query;
        public IntPtr error;
    }

    [Flags]
    public enum PgQueryParserOptions
    {
        Default = 0,
        TypeName = 1,
        PlpgsqlExpr = 2,
        PlpgsqlAssign1 = 3,
        PlpgsqlAssign2 = 4,
        PlpgsqlAssign3 = 5,

        // Flags
        DisableBackslashQuote = 1 << 4, // 16
        DisableStandardConformingStrings = 1 << 5, // 32
        DisableEscapeStringWarning = 1 << 6 // 64
    }
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryNormalizeResult pg_query_normalize(string input);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryNormalizeResult pg_query_normalize_utility(string input);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryScanResult pg_query_scan(string input);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryParseResult pg_query_parse(string input);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryParseResult pg_query_parse_opts(string input, int parser_options);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryProtobufParseResult pg_query_parse_protobuf(string input);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryProtobufParseResult pg_query_parse_protobuf_opts(string input, int parser_options);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryPlpgsqlParseResult pg_query_parse_plpgsql(string input);
  
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryFingerprintResult pg_query_fingerprint(string input);
 
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryFingerprintResult pg_query_fingerprint_opts(string input, int parser_options);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQuerySplitResult pg_query_split_with_scanner(string input);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQuerySplitResult pg_query_split_with_parser(string input);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern PgQueryDeparseResult pg_query_deparse_protobuf(PgQueryProtobuf parse_tree);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_normalize_result(PgQueryNormalizeResult result);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_scan_result(PgQueryScanResult result);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_parse_result(PgQueryParseResult result);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_split_result(PgQuerySplitResult result);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_deparse_result(PgQueryDeparseResult result);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_protobuf_parse_result(PgQueryProtobufParseResult result);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_plpgsql_parse_result(PgQueryPlpgsqlParseResult result);
   
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void pg_query_free_fingerprint_result(PgQueryFingerprintResult result);

    // [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    // public static extern void pg_query_exit();

    public const string PgMajorVersion = "17";
    public const string PgVersion = "17.5";
    public const int PgVersionNum = 170005;
}

