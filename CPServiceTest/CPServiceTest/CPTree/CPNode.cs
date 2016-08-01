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
            // type check
            CPNode cpNodeChild = child as CPNode;
            if (cpNodeChild == null)
            {
                return; // error log
            }

            if (childNodeList.Contains(cpNodeChild))
            {
                return;
            }

            // add node onto tree
            LinkedListNode<ICPNode> linkedListNode = childNodeList.AddLast(cpNodeChild);
            cpNodeChild.AttachedLinkedListNode = linkedListNode;
            cpNodeChild.Parent = this;
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
            get
            {
                return this.AttachedLinkedListNode.Next == null ? null : this.AttachedLinkedListNode.Next.Value;
            }
        }

        public ICPNode PreviousSibling
        {
            get
            {
                return this.AttachedLinkedListNode.Previous == null ? null : this.AttachedLinkedListNode.Previous.Value;
            }
        }

        public List<ICPNode> ChildNodeList
        {
            get { throw new NotImplementedException(); }
        }

        public LinkedListNode<ICPNode> AttachedLinkedListNode
        {
            get;
            private set;
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
