namespace FullCodeMetrics.Models;

public class MetricCollection
{

    public string BasePath { get; set; } = string.Empty;

    public Dictionary<string, MetricModel> Metrics { get; set; } = [];

    public MetricCollection() { }

    public MetricCollection(string basePath, IEnumerable<string> extensions)
    {
        BasePath = basePath;

        Metrics = extensions.ToDictionary(x => x, v => new MetricModel() { FileExtension = v });
    }
    public class MetricModel
    {
        public string FileExtension { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public int FileCount { get; set; }

        public int TotalLines { get; set; }

        public int CodeLines { get; set; }

        public int CommentLines { get; set; }

        public decimal Density => (TotalLines > 0 ? CodeLines / (decimal)TotalLines : 0) * 100;
    }
}


public class FileMetric
{
    public string FileName { get; set; } = string.Empty;

    public int TotalLines { get; set; }

    public int ExecutableLines { get; set; }
}