using FullCodeMetrics;
using FullCodeMetrics.Models;

using System.Diagnostics;
var sw = Stopwatch.StartNew();


var dirs = args.ToList();

var cp = new CodeParser();

var collection = new List<MetricCollection>();

await Parallel.ForEachAsync(dirs, async (d, cancellationToken) =>
{
    lock (Printer._consoleLock)
    {
        Console.SetCursorPosition(0, dirs.IndexOf(d));
        Console.Write(d);
    }

    var result = await cp.ParseAsync(d);

    lock (Printer._consoleLock)
    {
        Console.SetCursorPosition(d.Length + 1, dirs.IndexOf(d));
        Console.Write("...done");
    }

    lock (collection)
    {
        collection.Add(result);
    }
});

Console.SetCursorPosition(0, dirs.Count + 1);

Printer.P_Are_Eye_En_Tee(collection);

sw.Stop();

Console.WriteLine($"Completed in {sw.Elapsed.TotalSeconds}s");
