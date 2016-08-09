using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.Visitor
{
    abstract class AbsCPVisitor : ICPVisitor
    {
        public AbsCPVisitor()
        {
        }
        public AbsCPVisitor(object context)
        {
            this.Context = context;
        }
        public virtual bool TraverseMultiInstance
        {
            get { return false; }
        }

        public abstract void VisitCPStruct(CPTree.ICPStruct cpStruct);

        public abstract void VisitCPField(CPTree.ICPField cpField);

        public abstract void Dispose();

        public object Context { get; private set; }
    }
}
