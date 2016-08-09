using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.CPTree
{
    public interface ICPStruct : ICPNode
    {

        int Size { get; }

        void SetAttr(int offset);
    }
}
