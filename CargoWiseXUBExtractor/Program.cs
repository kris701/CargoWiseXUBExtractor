using CommandLine;
using CommandLine.Text;
using System;

namespace CargoWiseXUBExtractor
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var parser = new Parser(with => with.HelpWriter = null);
			var parserResult = parser.ParseArguments<Options>(args);
			parserResult.WithNotParsed(errs => DisplayHelp(parserResult, errs));
			await parserResult.WithParsedAsync(Run);
		}

		public static async Task Run(Options opts)
		{
			if (Directory.Exists(opts.OutputPath))
			{
				Console.WriteLine("Purging output path...");
				Directory.Delete(opts.OutputPath, true);
			}
			Directory.CreateDirectory(opts.OutputPath);

			Console.WriteLine("Begining XUB extraction...");
			var targetCompanies = opts.TargetExpression.Split(';', StringSplitOptions.RemoveEmptyEntries);
			Console.WriteLine($"\tA total of {targetCompanies.Length} companies to extract from.");
			foreach (var targetCompany in targetCompanies)
			{
				var split = targetCompany.Split(':', StringSplitOptions.RemoveEmptyEntries);
				var companyName = split[0];
				var selectionExpressions = split[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
				Directory.CreateDirectory(Path.Combine(opts.OutputPath, companyName));
				Console.WriteLine($"\tBegining Extraction for company {companyName}.");
				foreach (var expression in selectionExpressions)
				{
					if (expression.Contains('-'))
					{
						var expSplit = expression.Split('-', StringSplitOptions.RemoveEmptyEntries);
						var fromID = Int32.Parse(expSplit[0]);
						var toID = Int32.Parse(expSplit[1]);
						Console.WriteLine($"\t\tExtracting batch '{fromID}' to '{toID}' for company '{companyName}'");
						await XUBExtractor.FetchBatchRange(fromID, toID, companyName, opts);
					}
					else
					{
						var targetID = Int32.Parse(expression);
						Console.WriteLine($"\t\tExtracting batch '{targetID}' for company '{companyName}'");
						await XUBExtractor.FetchBatch(targetID, companyName, opts);
					}
				}
			}
		}

		private static void HandleParseError(IEnumerable<Error> errs)
		{
			var sentenceBuilder = SentenceBuilder.Create();
			foreach (var error in errs)
				if (error is not HelpRequestedError)
					Console.WriteLine(sentenceBuilder.FormatError(error));
		}

		private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
		{
			var helpText = HelpText.AutoBuild(result, h =>
			{
				h.AddEnumValuesToHelpText = true;
				return h;
			}, e => e, verbsIndex: true);
			Console.WriteLine(helpText);
			HandleParseError(errs);
		}
	}
}