using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CargoWiseXUBExtractor
{
	public class Options
	{
		[Option('o', "output", Required = false, HelpText = "Where to output the batches to.", Default = "out")]
		public string OutputPath { get; set; } = "out";

		[Option('n', "enterpriseID", Required = true, HelpText = "What enterprise ID to use.")]
		public string EnterpriseID { get; set; } = "";
		[Option('s', "serverID", Required = true, HelpText = "What server ID to use.")]
		public string ServerID { get; set; } = "";

		[Option('e', "endpoint", Required = true, HelpText = "The endpoint for the eAdapter inbound.")]
		public string Endpoint { get; set; } = "";
		[Option('u', "username", Required = true, HelpText = "The username to use for the inbound connection.")]
		public string UserName { get; set; } = "";
		[Option('p', "password", Required = true, HelpText = "The password to use for the inbound connection.")]
		public string Password { get; set; } = "";

		[Option('t', "targetExpression", Required = true, HelpText = "Target expression. The format is '{companyName}:{targetBatch};' for a single batch. '{companyName}:{fromBatch}-{toBatch};' for a range of batches. You can combine multiple expression, seperated by a ;. Target badge and ranges are INCLUSIVE!")]
		public string TargetExpression { get; set; } = "";

	}
}
