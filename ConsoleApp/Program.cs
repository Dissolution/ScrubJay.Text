using System.Diagnostics;
using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using ScrubJay.Text.Benchmarks;

#region Console Setup
Console.OutputEncoding = System.Text.Encoding.UTF8;
#endregion


// Benchmarks

var benchmarkConfig = ManualConfig.CreateEmpty()
    .AddColumnProvider(DefaultColumnProviders.Instance)
    .AddExporter(HtmlExporter.Default, MarkdownExporter.Default)
    .AddLogger(ConsoleLogger.Unicode)
    .AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray())
    .AddValidator(DefaultConfig.Instance.GetValidators().ToArray())
    .AddJob(Job.ShortRun);

var results = BenchmarkRunner.Run<TextToStringBenchmarks>(benchmarkConfig);

Process.Start("explorer.exe", results.ResultsDirectoryPath);
    
    
#region Console Teardown
#if DEBUG
Console.WriteLine("Press Enter to close this window.");
Console.ReadLine();
return 0;
#endif
#endregion    