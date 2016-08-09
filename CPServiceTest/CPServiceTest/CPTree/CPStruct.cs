using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.CPTree
{
    class CPStruct : CPNode, ICPStruct
    {
        public CPStruct(string name, string fullName)
            : base(name, fullName)
        {
        }

        public int Size
        {
            get
            {
                // next sibling offset - my offset
                if (this.NextSibling != null)
                {
                    return this.NextSibling.Offset - this.Offset;
                }

                // last leaf child offset + last leaf child length - my offset
                ICPNode lastChild = this.LastChild;
                ICPNode parent = lastChild;
                while (parent.LastChild != null)
                {
                    lastChild = parent.LastChild;
                    parent = lastChild;
                }
                ICPField lastField = lastChild as ICPField;
                if (lastField == null)
                {
                    // error log
                    throw new Exception("last child is null");
                }
                if (lastField.FieldType == TetraCpFieldType.bit)
                {
                    // error log
                    throw new Exception("cannot get the size of bit field structure");
                }

                return lastField.Offset + lastField.BitLen / 8 - this.Offset;
            }
        }

        public void SetAttr(int offset)
        {
            this.Offset = offset;
        }

        public override void Accept(Visitor.ICPVisitor cpVisitor, object context)
        {
            int count = 1;

            if (cpVisitor.TraverseMultiInstance)
            {
                count = this.InstanceCount;
            }

            for (int i = 0; i < count; i++)
            {
                cpVisitor.VisitCPStruct(this);

                foreach (var nextChild in this.ChildNodeList)
                {
                    nextChild.Accept(cpVisitor, context);
                }
            } // for (int i = 0; i < this.InstanceCount; i++)
        }
    }
}
