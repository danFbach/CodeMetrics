using FullCodeMetrics.Models;

namespace FullCodeMetrics;

public class Printer
{
    public static readonly object _consoleLock = new();

    private static readonly List<ConsoleColor> ColorSet = [ConsoleColor.White, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Red, ConsoleColor.White, ConsoleColor.Cyan];

    private static readonly Dictionary<string, int> headings = new() {
        { " File Type", 12 },
        { " File Count", 12 },
        { " Ttl Lines", 12 },
        { " Code Lines", 12 },
        { " Cmnt Lines", 12 },
        { " Wht Lines", 12 },
        { " Density", 12 }
    };

    public static void P_Are_Eye_En_Tee(List<MetricCollection> items)
    {
        var index = 0;
        foreach (var c in items)
        {
            Console.WriteLine();
            Console.WriteLine($"Metrics for \"{c.BasePath}\"");

            index = 0;
            foreach (var (k, v) in headings)
            {
                WriteText($"{k}".PadRight(v), index,v);
                index++;
            }
            Console.WriteLine();

            foreach (var item in c.Metrics)
            {
                if (item.Value.FileCount == 0)
                    continue;

                index = 0;
                foreach (var (k, v) in headings)
                {
                    var txt = k switch
                    {
                        " File Type" => item.Value.FileExtension,
                        " File Count" => item.Value.FileCount.ToString("N0"),
                        " Ttl Lines" => item.Value.TotalLines.ToString("N0"),
                        " Code Lines" => item.Value.CodeLines.ToString("N0"),
                        " Cmnt Lines" => item.Value.CommentLines.ToString("N0"),
                        " Wht Lines" => item.Value.WhitespaceLines.ToString("N0"),
                        " Density" => (item.Value.Density.ToString("N1") + "%"),
                        _ => string.Empty
                    };
                    WriteText(txt, index, v);
                    index++;
                }
                Console.WriteLine();
            }

            index = 0;
            foreach (var (k, v) in headings)
            {
                var txt = k switch
                {
                    " File Type" => "Total",
                    " File Count" => c.Metrics.Sum(x => x.Value.FileCount).ToString("N0"),
                    " Ttl Lines" => c.Metrics.Sum(x => x.Value.TotalLines).ToString("N0"),
                    " Code Lines" => c.Metrics.Sum(x => x.Value.CodeLines).ToString("N0"),
                    " Cmnt Lines" => c.Metrics.Sum(x => x.Value.CommentLines).ToString("N0"),
                    " Wht Lines" => c.Metrics.Sum(x => x.Value.WhitespaceLines).ToString("N0"),
                    " Density" => (c.Metrics.Sum(x => x.Value.CodeLines) / (decimal)c.Metrics.Sum(x => x.Value.TotalLines) * 100).ToString("N1") + "%",
                    _ => string.Empty
                };
                WriteText(txt, index, v);
                index++;
            }
            Console.WriteLine();
        }

        var totalFiles = items.SelectMany(x => x.Metrics.Select(x => x.Value.FileCount)).Sum();
        var totalLines = items.SelectMany(x => x.Metrics.Select(x => x.Value.TotalLines)).Sum();
        var totalCodeLines = items.SelectMany(x => x.Metrics.Select(x => x.Value.CodeLines)).Sum();
        var totalCommentLines = items.SelectMany(x => x.Metrics.Select(x => x.Value.CommentLines)).Sum();
        var totalWhiteLines = items.SelectMany(x => x.Metrics.Select(x => x.Value.WhitespaceLines)).Sum();
        var averageDensity = (items.SelectMany(x => x.Metrics.Select(x => x.Value.CodeLines)).Sum() / (decimal)items.SelectMany(x => x.Metrics.Select(x => x.Value.TotalLines)).Sum()) * 100;
        Console.WriteLine();

        index = 0;
        foreach (var (k, v) in headings)
        {
            var txt = k switch
            {
                " File Type" => "Total",
                " File Count" => totalFiles.ToString("N0"),
                " Ttl Lines" => totalLines.ToString("N0"),
                " Code Lines" => totalCodeLines.ToString("N0"),
                " Cmnt Lines" => totalCommentLines.ToString("N0"),
                " Wht Lines" => totalWhiteLines.ToString("N0"),
                " Density" => (averageDensity.ToString("N1") + "%"),
                _ => string.Empty
            };
            WriteText(txt, index, v);
            index++;
        }
        Console.WriteLine();

        Console.WriteLine();
        foreach (var ext in items.SelectMany(x => x.Metrics.Select(xx => xx.Value.FileExtension)).Distinct())
        {
            var extItems = items.SelectMany(x => x.Metrics.Where(x => x.Value.FileExtension == ext));
            index = 0;
            foreach (var (k, v) in headings)
            {
                var txt = k switch
                {
                    " File Type" => $"{ext}",
                    " File Count" => extItems.Sum(x => x.Value.FileCount).ToString("N0"),
                    " Ttl Lines" => extItems.Sum(x => x.Value.TotalLines).ToString("N0"),
                    " Code Lines" => extItems.Sum(x => x.Value.CodeLines).ToString("N0"),
                    " Cmnt Lines" => extItems.Sum(x => x.Value.CommentLines).ToString("N0"),
                    " Wht Lines" => extItems.Sum(x => x.Value.WhitespaceLines).ToString("N0"),
                    " Density" => (((extItems.Sum(x => x.Value.CodeLines) / (decimal)extItems.Sum(x => x.Value.TotalLines)) * 100).ToString("N1") + "%"),
                    _ => string.Empty
                };
                WriteText(txt, index, v);
                index++;
            }
            Console.WriteLine();
        }
    }

    private static void WriteText(string text, int index, int pad)
    {
        Console.ForegroundColor = index < ColorSet.Count ? ColorSet[index] : ConsoleColor.White;
        Console.Write(text.PadLeft(pad - 1).PadRight(pad));
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("|");
    }
}
