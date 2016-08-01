using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.Visitor;

namespace CPServiceTest.CPTree
{
    abstract class CPNode : ICPNode
    {
        LinkedList<ICPNode> childNodeList = new LinkedList<ICPNode>();

        public CPNode(string name, string fullName)
        {
            this.Name = name;
            this.FullName = fullName;
        }
        public string Name
        {
            get;
            internal set;
        }

        public string FullName
        {
            get;
            private set;
        }

        public ICPNode Parent
        {
            get;
            private set;
        }

        public void AddChild(ICPNode child)
        {
            if (!(child is CPNode))
            {
                return;
            }

            if (childNodeList.Contains(child))
            {
                return;
            }

            // add node onto tree
            childNodeList.AddLast(child);
            ((CPNode)child).Parent = this;

            // set sibling (next and previous)
            CPNode lastChild = childNodeList.Last.Value as CPNode;
            ((CPNode)child).PreviousSibling = lastChild;
            lastChild.NextSibling = child;
        }

        public ICPNode GetChild(string name)
        {
            throw new NotImplementedException();
        }

        public ICPNode FirstChild
        {
            get { throw new NotImplementedException(); }
        }

        public ICPNode LastChild
        {
            get { throw new NotImplementedException(); }
        }

        public ICPNode NextSibling
        {
            get;
            private set;
        }

        public ICPNode PreviousSibling
        {
            get;
            private set;
        }

        public List<ICPNode> ChildNodeList
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsLeafNode
        {
            get
            {
                return (this.childNodeList.Count == 0);
            }
        }

        public virtual int InstanceCount
        {
            get { throw new NotImplementedException(); }
        }

        public virtual void Accept(ICPVisitor cpVisitor, object context)
        {
            cpVisitor.VisitCPNode(this);

            foreach (var nextChild in this.childNodeList)
            {
                nextChild.Accept(cpVisitor, context);
            }
        }

        public object Tag { get; set; }
        
    }
}
