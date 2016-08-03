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
            get { return this.childNodeList.First == null ? null : this.childNodeList.First.Value; }
        }

        public ICPNode LastChild
        {
            get { return this.childNodeList.Last == null ? null : this.childNodeList.Last.Value; }
        }

        public ICPNode NextSibling
        {
            get
            {
                if ((this.AttachedLinkedListNode == null) || (this.AttachedLinkedListNode.Next == null))
                {
                    return null;
                }
                return this.AttachedLinkedListNode.Next.Value;
            }
        }

        public ICPNode PreviousSibling
        {
            get
            {
                if ((this.AttachedLinkedListNode == null) || (this.AttachedLinkedListNode.Previous == null))
                {
                    return null;
                }
                return this.AttachedLinkedListNode.Previous.Value;
            }
        }

        public List<ICPNode> ChildNodeList
        {
            get { return this.childNodeList.ToList(); }
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

        public int Offset
        {
            get;
            internal set;
        }

        public int InstanceCount
        {
            get;
            private set;
        }

        public void SetInstanceCount(int count)
        {
            this.InstanceCount = count;
        }
        public abstract void Accept(ICPVisitor cpVisitor, object context);

        public object Tag { get; set; }

    }
}
