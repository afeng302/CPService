using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.CPTree;

namespace CPServiceTest.Visitor
{
    interface ICPVisitor : IDisposable
    {
        void VisitCPNode(ICPNode node);

        void VisitCPStruct(ICPStrcut cpStruct);

        void VisitCPField(ICPField cpField);
    }
}
