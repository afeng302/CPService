using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CPServiceTest.CPTree;
using CPServiceTest.Visitor;

namespace CPServiceTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static Regex __reg_line = new Regex(@"(?<path>^[\w|:]+)\s+(?<offset>\d+):*(?<bit_start>\d*)\s+""(?<type>[\w|\s]+)""\s+{(?<dim>[\d|\s]+)}\s+(?<bit_len>\d+)");
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();

            TetraCPTree cpTree = new TetraCPTree();

            using (StreamReader reader = new StreamReader(@"C:\Users\a23126\Desktop\cpv-files\cpu20717ngp.cps"))
            {
                sw.Start();

                // build cp tree
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    Match m = __reg_line.Match(line);
                    if (!m.Success)
                    {
                        continue;
                    }

                    string fullName = m.Groups["path"].Value;
                    string[] names = fullName.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    string name = names[names.Length - 1]; // the last section

                    if (m.Groups["type"].Value == "union")
                    {
                        // assume there is no "union" type now
                        throw new Exception("Not support union type.");
                    }

                    ICPNode cpNode = null;
                    if (m.Groups["type"].Value == "struct")
                    {
                        CPStruct cpStruct = new CPStruct(name, fullName);
                        cpNode = cpStruct;

                        int offset;
                        if (!int.TryParse(m.Groups["offset"].Value, out offset))
                        {
                            offset = 0; // error log
                        }
                        cpStruct.SetAttr(offset);
                    }
                    else
                    {
                        CPField cpField = new CPField(name, fullName);
                        cpNode = cpField;

                        TetraCpFieldType type;
                        if (Enum.TryParse<TetraCpFieldType>(m.Groups["type"].Value.Replace(" ", "_"), true, out type))
                        {
                            type = TetraCpFieldType.None; // error log
                        }
                        int offset;
                        int bitLen;
                        if (!int.TryParse(m.Groups["offset"].Value, out offset))
                        {
                            offset = 0; // error log
                        }
                        if (!int.TryParse(m.Groups["bit_len"].Value, out bitLen))
                        {
                            bitLen = 0; // error log
                        }
                        cpField.SetAttr(type, offset, bitLen);
                    }

                    cpNode.Tag = line;

                    cpTree.AddNode(cpNode);

                } // while (!reader.EndOfStream)
            }

            sw.Stop();
            this.lstBoxOutput.Items.Insert(0, string.Format("Time elapsed (build tree): {0} ms", sw.ElapsedMilliseconds));
            sw.Restart();

            // verify the structure
            using (VerifyStrucVisitor cpVisitor = new VerifyStrucVisitor(@"C:\Users\a23126\Desktop\cpv-files\output-struct"))
            {
                cpTree.Root.Accept(cpVisitor, null);
            }

            sw.Stop();
            this.lstBoxOutput.Items.Insert(0, string.Format("Time elapsed (VerifyStrucVisitor): {0} ms", sw.ElapsedMilliseconds));
            
        }
    }
}
