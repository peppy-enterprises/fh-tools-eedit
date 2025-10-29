// SPDX-License-Identifier: MIT

namespace Fahrenheit.Tools.EEdit;

internal static class Program {

    /// <summary>
    ///     Checks which input files, if any, exist, and opens a <see cref="FileInfo"/> for each.
    /// </summary>
    private static List<FileInfo> _args_validate_input_files(ArgumentResult argr) {
        List<FileInfo> input_files = [];

        foreach (Token token in argr.Tokens) {
            string file_path = token.Value;

            if (!File.Exists(file_path)) {
                argr.AddError($"Input file {file_path} does not exist.");
                continue;
            }

            input_files.Add(new FileInfo(file_path));
        }

        return input_files;
    }

    private static int Main(string[] args) {
        RootCommand cmd_root = new("Perform various operations on FFX/X-2 Excel files.");

        Option<List<FileInfo>> opt_input = new("--input", "-i") {
            Description                    = "Input file(s) to process.",
            Arity                          = ArgumentArity.OneOrMore,
            Recursive                      = true,
            AllowMultipleArgumentsPerToken = true,
            DefaultValueFactory            = _args_validate_input_files
        };

        Option<string> opt_output = new("--output", "-o") {
            Description = "What folder to emit outputs to. The folder must already exist.",
            Arity       = ArgumentArity.ExactlyOne,
            Recursive   = true
        };

        cmd_root.Options.Add(opt_input);
        cmd_root.Options.Add(opt_output);

        Command cmd_decompile = new("decompile", "Decompiles an Excel file.");

        cmd_decompile.SetAction(parse_result => _c_decompile(
            parse_result.GetRequiredValue(opt_input),
            parse_result.GetRequiredValue(opt_output)
            ));

        cmd_root.Subcommands.Add(cmd_decompile);

        ParseResult argparse_result = cmd_root.Parse(args);
        return argparse_result.Invoke();
    }

    private static void _c_decompile(
        List<FileInfo>  input_files,
        string          output_dir)
    {
        Stopwatch perf = Stopwatch.StartNew();

        foreach (FileInfo input_file in input_files) {
            string output_path = Path.Join(output_dir, $"{input_file.Name}.txt");

            using (FileStream input_file_stream  = input_file.OpenRead())
            using (FileStream output_file_stream = new FileStream(output_path, FileMode.Create, FileAccess.Write, FileShare.None)) {
                _decompile(input_file_stream, output_file_stream);
            }

            Console.WriteLine($"{input_file.Name} -> {output_path}");
        }

        Console.WriteLine($"processed {input_files.Count} files in {perf.Elapsed}");
    }

    private static void _decompile(
        FileStream input_file,
        FileStream output_file)
    {
        Span<byte> input_bytes = new byte[input_file.Length];
        input_file.ReadExactly(input_bytes);

        // STUB
    }
}
