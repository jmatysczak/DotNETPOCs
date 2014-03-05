using System;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace AjaxMinAST {
	class Program {
		static void Main(string[] args) {
			var input = File.ReadAllText("input.js");

			Console.WriteLine("--- Raw ---");
			Console.WriteLine(input);
			Console.WriteLine("\r\n");

			Console.WriteLine("--- Minified ---");
			Console.WriteLine(new Minifier().MinifyJavaScript(input));
			Console.WriteLine("\r\n");

			Console.WriteLine("--- AST ---");
			var parser = new JSParser();
			parser.CompilerError += (_, ea) => Console.WriteLine(ea.Error);
			var block = parser.Parse(input);
			new Visitor().Visit(block);
		}
	}

	class Visitor : TreeVisitor {

	}
}
