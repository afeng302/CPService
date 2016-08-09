using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.CPTree
{
    internal class TetraCPTree
    {
        Dictionary<string, ICPNode> nodeFullNameMap = new Dictionary<string, ICPNode>();

        public ICPNode Root
        {
            get;
            private set;
        }

        /// <summary>
        /// We assume the tree node will be always added by the order - parent before child.
        /// It can be assured by .cps file.
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(ICPNode node)
        {
            // node has been already added
            if (this.nodeFullNameMap.ContainsKey(node.FullName))
            {
                return;
            }

            string[] names = node.FullName.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            // root node
            if (names.Length == 1)
            {
                this.nodeFullNameMap[node.Name] = node;
                this.Root = node;
                return;
            }

            // search parent node
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < names.Length - 1; i++)
            {
                if (sb.Length == 0)
                {
                    sb.Append(names[i]);
                }
                else
                {
                    sb.Append(":" + names[i]);
                }
            }
            if (!this.nodeFullNameMap.ContainsKey(sb.ToString()))
            {
                throw new Exception("parent node has not been added.");
            }
            ICPNode parentNode = this.nodeFullNameMap[sb.ToString()];

            // add node onto tree
            parentNode.AddChild(node);
            this.nodeFullNameMap[node.FullName] = node;
        }

        public ICPNode GetNode(string name)
        {
            if (!this.nodeFullNameMap.ContainsKey(name))
            {
                return null;
            }

            return this.nodeFullNameMap[name];
        }
    }
}
