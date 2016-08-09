using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.CPTree;

namespace CPServiceTest.Visitor
{
    public class BitReverserContext
    {
        public BitReverserContext(byte[] image, bool isBigendian2LittleEndian)
        {
            this.Image = image;
            this.IsBigEndian2LittleEndian = IsBigEndian2LittleEndian;
        }

        public byte[] Image
        {
            get;
            private set;
        }

        public int StartOffset
        {
            get;
            set;
        }

        /// <summary>
        /// this option will be used for bit field. 
        /// the .cps file described the big endian layout.
        /// </summary>
        public bool IsBigEndian2LittleEndian
        {
            get;
            private set;
        }

    }

    /// <summary>
    /// CPE file will store the field as little-endian format, but the bit field will be stored as big-endian format.
    /// That's why we need to separate the byte reserver and bit reverse logic.
    /// </summary>
    class EndianReverseBitVisitor : AbsCPVisitor
    {
        bool disposed = false;

        public EndianReverseBitVisitor(BitReverserContext context)
            : base(context)
        {
        }

        public override bool TraverseMultiInstance
        {
            get
            {
                return true;
            }
        }

        public override void VisitCPStruct(CPTree.ICPStruct cpStruct)
        {
            // do nothing
        }

        public override void VisitCPField(CPTree.ICPField cpField)
        {
            if (cpField.FieldType != TetraCpFieldType.bit)
            {
                ((BitReverserContext)this.Context).StartOffset += (cpField.BitLen / 8);

                return;
            }

            // bit field
            // do the reverse logic when meet the first bit field in a structure
            if (cpField != cpField.Parent.FirstChild)
            {
                return;
            }

            //
            // reverse the whole bit field strcutre
            this.ReverseBitFieldStruct(cpField.Parent as ICPStruct, this.Context as BitReverserContext);

            //
            // increase the offset for whole bit field structure
            ((BitReverserContext)this.Context).StartOffset += ((ICPStruct)cpField.Parent).Size;
        }


        private void ReverseBitFieldStruct(ICPStruct cpStruct, BitReverserContext context)
        {
            // for the bit field reversion, byte order will not be changed,
            // only reverse the bit field order inside a byte

            //=================================================================
            // for a field, bitLen = 3, value = 3
            // Little-Endian:   0000 0(011)
            // Big-Endian:      (011)0 0000
            // 
            // step1. reverse bit in a byte
            //          (110)0 0000
            // step2. reverse bit within a field
            //          (011)0 0000
            // Note that, we need to reverse the bit within a field.
            //=================================================================

            //
            // #1 reverse the bit order for each byte
            for (int i = 0; i < cpStruct.Size; i++)
            {
                byte b = context.Image[context.StartOffset + i];
                context.Image[context.StartOffset + i] = this.ReverseByte(b);
            }

            //
            // #2 reverse the bit order within a field
            foreach (ICPField nextField in cpStruct.ChildNodeList)
            {
                // skip the field which has only 1 bit length
                if (nextField.BitLen == 1)
                {
                    continue;
                }
                int offset = nextField.Offset - cpStruct.Offset;
                // reverse bit ordre for the field which has 8 bits length
                if (nextField.BitLen == 8)
                {
                    byte b = context.Image[context.StartOffset + offset];
                    context.Image[context.StartOffset + offset] = this.ReverseByte(b);
                }
                // use mask to reverse the bit order for field
                byte mask = nextField.Mask;
                if (!context.IsBigEndian2LittleEndian)
                {
                    mask = ReverseByte(mask); // the field mask (.cps file) is for big-endian layout
                }
                // align to 0x01
                byte data = context.Image[context.StartOffset + offset];
                int counter = 0;
                byte tempMask = mask;
                while ((tempMask & 0x01) != 0x01)
                {
                    counter++;
                    tempMask >>= 1;
                    data >>= 1;
                }
                // push to stack
                Stack<int> binStack = new Stack<int>();
                for (int i = 0; i < nextField.BitLen; i++)
                {
                    if ((data & 0x01) == 0x01)
                    {
                        binStack.Push(1);
                    }
                    else
                    {
                        binStack.Push(0);
                    }
                    data >>= 1;
                }
                // pop from stack (align to 0x80)
                data = 0x00;
                while (binStack.Count > 0)
                {
                    data >>= 1;
                    if (binStack.Pop() == 1)
                    {
                        data |= 0x80;
                    }
                }
                // align to the original place
                for (int i = 0; i < 8 - counter - nextField.BitLen; i++)
                {
                    data >>= 1;
                }
                // replace original bits
                context.Image[context.StartOffset + offset] &= (byte)~mask;
                context.Image[context.StartOffset + offset] |= data;

            } // foreach (ICPField nextField in cpStruct.ChildNodeList)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpField"></param>
        /// <param name="context"></param>
        private void ReverseBitField(ICPField cpField, BitReverserContext context)
        {
            if (cpField.FieldType != TetraCpFieldType.bit)
            {
                return;
            }


            //=================================================================
            // for a field, bitLen = 3, value = 3
            // Little-Endian:   0000 0(011)
            // Big-Endian:      (011)0 0000
            // 
            // step1. reverse bit in a byte
            //          (110)0 0000
            // step2. reverse bit within a field
            //          (011)0 0000
            // Note that, we need to reverse the bit within a field.
            //=================================================================

        }

        private byte ReverseByte(byte inData)
        {
            byte outData = 0x00;

            Stack<int> binStack = new Stack<int>();
            for (int i = 0; i < 8; i++)
            {
                if ((inData & 0x01) == 0x01)
                {
                    binStack.Push(1);
                }
                else
                {
                    binStack.Push(0);
                }
                inData >>= 1;
            } // for (int i = 0; i < 8; i++)

            while (binStack.Count > 0)
            {
                outData >>= 1;
                if (binStack.Pop() == 1)
                {
                    outData |= 0x80;
                }
            }

            return outData;
        }

        public override void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            try
            {
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                disposed = true;
            }
        }

        ~EndianReverseBitVisitor()
        {
            Dispose(false);
        }
    }
}
