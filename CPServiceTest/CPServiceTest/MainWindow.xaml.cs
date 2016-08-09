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
                    string[] names = fullName.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
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
                            throw new Exception(string.Format("Invalid offset [{0}].", m.Groups["offset"].Value));
                        }
                        cpStruct.SetAttr(offset);
                    }
                    else
                    {
                        CPField cpField = new CPField(name, fullName);
                        cpNode = cpField;

                        TetraCpFieldType type;
                        if (!Enum.TryParse<TetraCpFieldType>(m.Groups["type"].Value.Replace(" ", "_"), true, out type))
                        {
                            type = TetraCpFieldType.None; // error log
                            throw new Exception(string.Format("Invalid type [{0}].", m.Groups["type"].Value));
                        }
                        int offset;
                        int bitLen;
                        int startBit = -1;
                        if (!int.TryParse(m.Groups["offset"].Value, out offset))
                        {
                            offset = 0; // error log
                            throw new Exception(string.Format("Invalid offset [{0}].", m.Groups["offset"].Value));
                        }
                        if (!int.TryParse(m.Groups["bit_len"].Value, out bitLen))
                        {
                            bitLen = 0; // error log
                            throw new Exception(string.Format("Invalid bit_len [{0}].", m.Groups["bit_len"].Value));
                        }
                        if (!string.IsNullOrEmpty(m.Groups["bit_start"].Value)
                            && !int.TryParse(m.Groups["bit_start"].Value, out startBit))
                        {
                            startBit = 0; // error log
                            throw new Exception(string.Format("Invalid bit_start [{0}].", m.Groups["bit_start"].Value));
                        }
                        cpField.SetAttr(type, offset, bitLen, startBit);
                    }

                    // dimension
                    string dim = m.Groups["dim"].Value;
                    string[] dims = dim.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    int counter = 1;
                    for (int i = 0; i < dims.Length; i++)
                    {
                        int d;
                        if (!int.TryParse(dims[i], out d))
                        {
                            // error log
                            throw new Exception(string.Format("Invalid dim [{0}].", m.Groups["dim"].Value));
                        }
                        counter *= d;
                    }
                    cpNode.SetInstanceCount(counter);

                    cpNode.Tag = line;

                    cpTree.AddNode(cpNode);

                } // while (!reader.EndOfStream)
            }

            sw.Stop();
            this.lstBoxOutput.Items.Insert(0, string.Format("Time elapsed (build tree): {0} ms", sw.ElapsedMilliseconds));
            sw.Restart();

            // set bit field mask
            using (BitFieldMaskSettingVisitor cpVisitor = new BitFieldMaskSettingVisitor())
            {
                cpTree.Root.Accept(cpVisitor, null);
            }

            sw.Stop();
            this.lstBoxOutput.Items.Insert(0, string.Format("Time elapsed (BitFieldMaskSettingVisitor): {0} ms", sw.ElapsedMilliseconds));
            sw.Restart();

            // verify the structure
            using (TestStructVisitor cpVisitor = new TestStructVisitor(@"C:\Users\a23126\Desktop\cpv-files\output-struct"))
            {
                cpTree.Root.Accept(cpVisitor, null);
            }

            sw.Stop();
            this.lstBoxOutput.Items.Insert(0, string.Format("Time elapsed (VerifyStrucVisitor): {0} ms", sw.ElapsedMilliseconds));
            sw.Restart();

            // verify node info
            using (TestNodeInfoVisitor cpVisitor = new TestNodeInfoVisitor(@"C:\Users\a23126\Desktop\cpv-files\output-node-info"))
            {
                cpTree.Root.Accept(cpVisitor, null);
            }
            sw.Stop();
            this.lstBoxOutput.Items.Insert(0, string.Format("Time elapsed (TestNodeInfoVisitor): {0} ms", sw.ElapsedMilliseconds));
            sw.Restart();

            // print struct size
            using (TestStructSizeVisitor cpVisitor = new TestStructSizeVisitor(@"C:\Users\a23126\Desktop\cpv-files\output-struct-size"))
            {
                cpTree.Root.Accept(cpVisitor, null);
            }
            sw.Stop();
            this.lstBoxOutput.Items.Insert(0, string.Format("Time elapsed (TestStructSizeVisitor): {0} ms", sw.ElapsedMilliseconds));
        }
    }
}
