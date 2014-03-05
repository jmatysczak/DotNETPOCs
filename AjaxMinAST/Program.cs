using Microsoft.Ajax.Utilities;
using System;
using System.IO;

namespace AjaxMinAST {
	class Program {
		static void Main(string[] args) {
			var js = File.ReadAllText("input.js");

			Console.WriteLine("--- Raw ---");
			Console.WriteLine(js);

			Console.WriteLine("--- Minified ---");
			Console.WriteLine(new Minifier().MinifyJavaScript(js));
		}
	}
}
