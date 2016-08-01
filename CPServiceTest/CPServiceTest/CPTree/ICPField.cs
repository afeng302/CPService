using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.CPTree
{
    public enum TetraCpFieldType
    {
        None = 0,
        Char,
        unsigned_long,
        unsigned_char,
        UINT16,
        signed_char,
        //CFT_char,
        wchar_t,
        signed_long,
        signed_short,
        UINT8,
        bit,
        unsigned_short,
        BYTE,
        UINT32,
    }

    interface ICPField : ICPNode
    {
        TetraCpFieldType FieldType { get; }

        int Offset { get; }

        int BitLen { get; }

        void SetAttr(TetraCpFieldType type, int offset, int bitLen);
    }
}
