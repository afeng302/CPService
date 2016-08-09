using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.Visitor;

namespace CPServiceTest.CPTree
{

    public interface ICPNode
    {
        string Name { get; }

        string FullName { get; }

        ICPNode Parent { get; }

        void AddChild(ICPNode child);

        ICPNode GetChild(string name);

        ICPNode FirstChild { get; }

        ICPNode LastChild { get; }

        ICPNode NextSibling { get; }

        ICPNode PreviousSibling { get; }

        List<ICPNode> ChildNodeList { get; }

        LinkedListNode<ICPNode> AttachedLinkedListNode { get; }

        int Offset { get; }

        bool IsLeafNode { get; }

        int InstanceCount { get; }

        void SetInstanceCount(int count);

        void Accept(ICPVisitor cpVisitor, object context);

        object Tag { get; set; }

    }
}
