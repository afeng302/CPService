using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.Visitor;

namespace CPServiceTest.CPTree
{
    class CPField : CPNode, ICPField
    {
        public CPField(string name, string fullName) 
            : base (name, fullName)
        {
        }
        public TetraCpFieldType FieldType { get; private set; }

        public int Offset { get; private set; }

        public int BitLen { get; private set; }

        public void SetAttr(TetraCpFieldType type, int offset, int bitLen)
        {
            this.FieldType = type;
            this.Offset = offset;
            this.BitLen = bitLen;
        }

        public override void Accept(ICPVisitor cpVisitor, object context)
        {
            cpVisitor.VisitCPField(this);
        }
    }
}
