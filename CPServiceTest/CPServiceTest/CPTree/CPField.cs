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
            : base(name, fullName)
        {
            this.Mask = 0xFF;
        }
        public TetraCpFieldType FieldType { get; private set; }

        public int BitLen { get; private set; }
        
        public int StartBit { get; private set; }

        public byte Mask { get; private set; }

        public void SetAttr(TetraCpFieldType type, int offset, int bitLen, int startBit)
        {
            this.FieldType = type;
            this.Offset = offset;
            this.BitLen = bitLen;
            this.StartBit = startBit;
        }

        public override void Accept(ICPVisitor cpVisitor, object context)
        {
            cpVisitor.VisitCPField(this);
        }
    }
}
