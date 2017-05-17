using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace DupeTracker
{
    public partial class DupeTracker : Form
    {
        string CurrentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString();

        public DupeTracker()
        {
            InitializeComponent();
            label1.Text = CurrentPath + "...";
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(textBox3));
            Console.WriteLine("Welcome to DupeTracker." + "\n" + "Click Doppelgänger to find duplicate entries in clinical forms located in the selected directory." + "\n");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                CurrentPath = folderBrowserDialog1.SelectedPath;
                label1.Text = CurrentPath + "...";
                Console.WriteLine("Path has changed to " + label1.Text + "\n");
            }
        }

        private void textBox1_FocusEnter(object sender, EventArgs e)
        {
            if (textBox1.Text == "^\\s*<item code=\"(?<code>\\d*)\">(?<label>.*)<\\/item>$")
            {
                textBox1.Text = "";
                textBox1.ForeColor = SystemColors.ActiveCaptionText;
            }
        }

        private void textBox1_FocusLeave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "^\\s*<item code=\"(?<code>\\d*)\">(?<label>.*)<\\/item>$";
                textBox1.ForeColor = SystemColors.ControlDarkDark;
            }
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Help;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int fCount = Directory.GetFiles(CurrentPath, "*.xml", SearchOption.TopDirectoryOnly).Length;
            if (fCount > 0)
            {
                string CurrentLine;
                string CompareLine;

                string RegEx = textBox1.Text;

                foreach (string file in Directory.EnumerateFiles(CurrentPath, "*.xml"))
                {
                    Console.WriteLine(file);
                    var lines = File.ReadLines(file);
                    CurrentLine = lines.First();

                    for (int i = 1; i <= lines.Count(); i++)
                    {
                        Match iMatch = Regex.Match(CurrentLine, RegEx);
                        if (iMatch.Groups[1].Value.Length >= 0)
                        {
                            for (int j = i + 1; j <= lines.Count(); j++)
                            {
                                CompareLine = lines.Skip(j - 1).First();
                                Match jMatch = Regex.Match(CompareLine, RegEx);
                                if (jMatch.Groups[1].Value.Length > 0)
                                    if (iMatch.Groups[1].Value == jMatch.Groups[1].Value)
                                        if (!textBox3.Text.Contains(jMatch.Groups[2].Value))
                                        Console.WriteLine("Code " + iMatch.Groups[1] + " with the label \"" + iMatch.Groups[2] + "\" on line " + i + " is duplicated by code " + jMatch.Groups[1] + " with the label \"" + jMatch.Groups[2] + "\" on line " + j + ".");
                                if (i != lines.Count())
                                    CurrentLine = lines.Skip(i).First();
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
            else
                Console.WriteLine("No XML files found in the selected directory!" + "\n");
        }
    }
}