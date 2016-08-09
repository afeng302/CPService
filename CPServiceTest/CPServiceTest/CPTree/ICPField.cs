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

    public interface ICPField : ICPNode
    {
        TetraCpFieldType FieldType { get; }

        int BitLen { get; }

        int StartBit { get; }

        byte Mask { get; }

        void SetMask(byte mask);

        void SetAttr(TetraCpFieldType type, int offset, int bitLen, int startBit);
    }
}
