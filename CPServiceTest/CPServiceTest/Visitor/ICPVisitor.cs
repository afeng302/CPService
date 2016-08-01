using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.CPTree;

namespace CPServiceTest.Visitor
{
    interface ICPVisitor
    {
        void VisitCPNode(ICPNode node);

        //void VisitCPSStruc(ICPStrcut cpStruct);
    }
}
