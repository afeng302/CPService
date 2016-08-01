﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.CPTree
{
    class CPStruct : CPNode, ICPStrcut
    {
        public CPStruct(string name, string fullName)
            : base(name, fullName)
        {
        }

        public int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Offset
        {
            get;
            private set;
        }

        public void SetAttr(int offset)
        {
            this.Offset = offset;
        }

        public override void Accept(Visitor.ICPVisitor cpVisitor, object context)
        {
            cpVisitor.VisitCPStruct(this);

            foreach (var nextChild in this.ChildNodeList)
            {
                nextChild.Accept(cpVisitor, context);
            }
        }
    }
}
