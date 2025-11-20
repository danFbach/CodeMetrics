using FullCodeMetrics.Models;

using System.Text.RegularExpressions;

namespace FullCodeMetrics;

public partial class CodeParser
{
    private readonly string[] _includeFileExtensions = [".cs", ".cshtml", ".js", ".css"];

    private readonly HashSet<string> _excludeFileNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "jquery",
        "\\modernizr\\",
        "\\node_modules\\",
        "\\obj\\",
        "\\bin\\",
        "\\Migrations\\"
    };

    public CodeParser() { }

    public Task<MetricCollection> ParseAsync(string baseDir)
    {
        return NextDirAsync(baseDir);
    }

    private async Task<MetricCollection> NextDirAsync(string path)
    {
        var cwd = new DirectoryInfo(path);
        var model = new MetricCollection(cwd.FullName, _includeFileExtensions);
        foreach (var metric in model.Metrics)
        {
            var dirFiles = cwd.EnumerateFiles($"*{metric.Value.FileExtension}", SearchOption.AllDirectories)
                .Where(x => !_excludeFileNames.Any(x.FullName.Contains));

            await Parallel.ForEachAsync(dirFiles, async (item, cancellationToken) =>
            {
                await ParseFileAsync(metric.Value, item);
            });
        }

        return model;
    }

    private static async Task ParseFileAsync(MetricCollection.MetricModel metric, FileInfo fi)
    {
        var totalLines = 0;
        var codeLines = 0;
        var commentLines = 0;
        var whitespaceLines = 0;

        await foreach (var line in File.ReadLinesAsync(fi.FullName))
        {
            totalLines++;

            if (CommentLineRegex.IsMatch(line))
                commentLines++;
            else if (string.IsNullOrWhiteSpace(line))
                whitespaceLines++;
            else
                codeLines++;
        }

        lock (metric)
        {
            metric.FileCount++;
            metric.TotalLines += totalLines;
            metric.CodeLines += codeLines;
            metric.CommentLines += commentLines;
            metric.WhitespaceLines += whitespaceLines;
        }
    }

    private static readonly Regex CommentLineRegex = MyRegex();

    [GeneratedRegex(@"^\s*(//|///|/\*|@\*|#)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
