using SerializableHttps;
using SerializableHttps.AuthenticationMethods;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace CargoWiseXUBExtractor
{
	public static class XUBExtractor
	{
		public static async Task<bool> FetchBatchRange(int fromID, int toID, string countryCode, Options opts)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			var done = 0;
			for (int i = fromID; i <= toID; i++)
			{
				if (done == 0)
					Console.WriteLine($"\t\tGetting batch 1 out of {toID - fromID + 1}");
				else
					Console.WriteLine($"\t\tGetting batch {done} out of {toID - fromID + 1} (Spend: {stopWatch.Elapsed.ToString("h'h 'm'm 's's'")}, Est: {((stopWatch.Elapsed / done) * (toID - fromID + 1 - done)).ToString("h'h 'm'm 's's'")})");
				await FetchBatch(i, countryCode, opts);
				done++;
			}
			return true;
		}

		public static async Task<bool> FetchBatch(int batchID, string countryCode, Options opts)
		{
			XNamespace ns = "http://www.cargowise.com/Schemas/Universal/2011/11";
			XElement xml = new XElement(ns + "UniversalTransactionBatchRequest",
				new XElement(ns + "TransactionBatchRequest",
					new XElement(ns + "DataContext",
						new XElement(ns + "DataTargetCollection",
							new XElement(ns + "DataTarget",
								new XElement(ns + "Type", "BatchNumber"),
								new XElement(ns + "Key", batchID)
							)
						),
						new XElement(ns + "Company",
							new XElement(ns + "Code", countryCode)
						),
						new XElement(ns + "EnterpriseID", opts.EnterpriseID),
						new XElement(ns + "ServerID", opts.ServerID)
					),
					new XElement(ns + "ActionType",
						new XElement(ns + "Code", "EXP")
					)
				)
			);

			SerializableHttpsClient _httpClient = new SerializableHttpsClient();
			var authorization = new AuthenticationHeaderValue(
				"Basic",
				Convert.ToBase64String(Encoding.GetEncoding("iso-8859-1").GetBytes(string.Format("{0}:{1}",
				opts.UserName,
				opts.Password
				))));
			_httpClient.SetAuthentication(new ManualAuthenticationMethod(authorization));
			_httpClient.TimeOut = TimeSpan.FromSeconds(60);
			XElement? response = null;
			while (response == null)
			{
				try
				{
					response = await _httpClient.PostAsync<string, XElement>(XMLHelper.ConvertXElementToString(xml), opts.Endpoint);
				}
				catch
				{
					Console.WriteLine("\t\teAdapter did not respond with OK. Retrying...");
				}
			}
			var text = XMLHelper.ConvertXElementToString(response);
			if (text.Contains("| Nothing to export.</ProcessingLog>"))
			{
				Console.WriteLine("\t\tNothing more to export!");
				return false;
			}

			File.WriteAllText(Path.Combine(opts.OutputPath, countryCode, $"{countryCode} - Batch {batchID}.xml"), text);
			return true;
		}
	}
}
