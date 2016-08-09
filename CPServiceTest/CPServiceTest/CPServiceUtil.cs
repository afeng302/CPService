using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CPServiceTest.CPTree;
using CPServiceTest.Visitor;

namespace CPServiceTest
{
    static class CPServiceUtil
    {
        static Regex __reg_line = new Regex(@"(?<path>^[\w|:]+)\s+(?<offset>\d+):*(?<bit_start>\d*)\s+""(?<type>[\w|\s]+)""\s+{(?<dim>[\d|\s]+)}\s+(?<bit_len>\d+)");

        static TetraCPTree CreateCPTree(string cpsFile)
        {
            TetraCPTree cpTree = new TetraCPTree();

            using (StreamReader reader = new StreamReader(cpsFile))
            {
                //
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
            } // using (StreamReader reader = new StreamReader(cpsFile))

            //
            // set bit field mask
            using (BitFieldMaskSettingVisitor cpVisitor = new BitFieldMaskSettingVisitor())
            {
                cpTree.Root.Accept(cpVisitor, null);
            }

            return cpTree;
        }

        static byte[] GetCPValue(TetraCPTree cpTree, string fldName, byte[] cpImage)
        {
            // do not support the dimension parameter now

            ICPField cpField = cpTree.GetNode(fldName) as ICPField;
            if (cpField == null)
            {
                // error log
                return null;
            }

            if (cpField.FieldType == TetraCpFieldType.bit)
            {
                // handle it later
                return null;
            }

            byte[] data = new byte[cpField.BitLen / 8];
            Array.Copy(cpImage, cpField.Offset, data, 0, cpField.BitLen / 8);

            return data;
        }
    }
}
