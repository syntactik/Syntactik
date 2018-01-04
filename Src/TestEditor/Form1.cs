#region license
// Copyright © 2017 Maxim O. Trushin (trushin@gmail.com)
//
// This file is part of Syntactik.
// Syntactik is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Syntactik is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Syntactik.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Syntactik;
using Syntactik.Compiler;
using Syntactik.Compiler.IO;
using Syntactik.Compiler.Pipelines;
using Syntactik.DOM;
using Syntactik.IO;
using Module = Syntactik.DOM.Mapped.Module;

namespace TestEditor
{
    public partial class Form1 : Form
    {
        class ErrorListener : IErrorListener
        {
            public List<string> Errors { get; } = new List<string>();

            public void OnError(int code, Interval interval, params object[] args)
            {
                Errors.Add(ParsingErrors.Format(code, args) + $" ({interval.Begin.Line}:{interval.Begin.Column})-({interval.End.Line}:{interval.End.Column})");
            }
        }

        public Form1()
        {
            InitializeComponent();
        }


        private void SourceTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sourceTextBox.Text != string.Empty) Clipboard.SetText(sourceTextBox.Text);

            if (modeStripDropDownButton.Text == @"PAIR")
            {
                var module = new Module {Name = "Module", Value = null, FileName = ""};
                var parser = new Parser(new InputStream(sourceTextBox.Text), new PairFactory(), module);
                //var parser = new Parser(new InputStream(((RichTextBox) sender).Text));
                var errorListener = new ErrorListener();
                parser.ErrorListeners.Add(errorListener);

                try
                {
                    var printer = new DomPrinter();
                    printer.Visit((Pair) parser.ParseModule());
                    domTextBox.Text = printer.Text;
                    errorsTextBox.Text = string.Join(Environment.NewLine, errorListener.Errors.ToArray());
                }
                catch (Exception exception)
                {
                    errorsTextBox.Text = exception.ToString();
                }
            }
            else
            {
                var outputName = modeStripDropDownButton.Text == @"XML" ? "main.s4x" : "main.s4j";
                var compilerParameters = CreateCompilerParameters(outputName);
                var compiler = new SyntactikCompiler(compilerParameters);
                var context = compiler.Run();
                
                if (context.InMemoryOutputObjects.Count >= 1)
                {
                    var sb = new StringBuilder();
                    foreach (var item in context.InMemoryOutputObjects)
                    {
                        sb.Append(item.Key).AppendLine(":").AppendLine(item.Value.ToString());
                    }
                    domTextBox.Text = sb.ToString();
                }
                else
                {
                    domTextBox.Text = "";
                }
                errorsTextBox.Text = SerializeErrors(context.Errors);
            }
        }

        private CompilerParameters CreateCompilerParameters(string name)
        {
            var compilerParameters = new CompilerParameters { Pipeline = new CompileToMemory(true) };
            compilerParameters.Input.Add(new StringInput(name, sourceTextBox.Text));

            return compilerParameters;
        }


        private static string SerializeErrors(IEnumerable<CompilerError> errors)
        {
            var sb = new StringBuilder();
            foreach (var error in errors)
            {
                sb.Append(error.Code + " " + error.LexicalInfo + ": ");
                sb.AppendLine(error.Message);
                if (error.InnerException != null)
                    sb.AppendLine(error.InnerException.StackTrace);
            }

            return sb.ToString();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText("1.txt", sourceTextBox.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sourceTextBox.SelectionTabs = new[] { 20, 40, 60, 80, 100, 120, 140 };
            if (File.Exists("1.txt")) sourceTextBox.Text = File.ReadAllText("1.txt");
            
        }

        private void sourceTextBox_SelectionChanged(object sender, EventArgs e)
        {
            int index = ((RichTextBox) sender).SelectionStart;
            int line = ((RichTextBox) sender).GetLineFromCharIndex(index);

            // Get the column.
            int firstChar = ((RichTextBox)sender).GetFirstCharIndexFromLine(line);
            int column = index - firstChar;

            lineLabel.Text = (line + 1).ToString();
            columnLabel.Text = (column + 1).ToString();
        }

        private void jsonToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void xmlStripMenuItem_Click(object sender, EventArgs e)
        {
            modeStripDropDownButton.Text = @"XML";
        }

        private void pairStripMenuItem_Click(object sender, EventArgs e)
        {
            modeStripDropDownButton.Text = @"PAIR";
        }

        private void jsonStripMenuItem_Click(object sender, EventArgs e)
        {
            modeStripDropDownButton.Text = @"JSON";
        }
    }
}
