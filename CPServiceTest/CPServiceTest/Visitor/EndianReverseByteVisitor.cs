using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.CPTree;

namespace CPServiceTest.Visitor
{
    public class ByteReverserContext
    {
        public ByteReverserContext(byte[] image)
        {
            this.Image = image;
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
    }
    class EndianReverseByteVisitor : AbsCPVisitor
    {
        bool disposed = false;

        public EndianReverseByteVisitor(ByteReverserContext context)
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
            this.ReverseByteOrder(cpField, this.Context as ByteReverserContext);
        }

        private void ReverseByteOrder(ICPField cpField, ByteReverserContext context)
        {
            switch (cpField.FieldType)
            {
                case TetraCpFieldType.None:
                case TetraCpFieldType.Char:
                case TetraCpFieldType.unsigned_char:
                case TetraCpFieldType.signed_char:
                case TetraCpFieldType.UINT8:
                case TetraCpFieldType.BYTE:
                    {
                        // single byte type
                        context.StartOffset += 1;
                        break;
                    }
                case TetraCpFieldType.bit:
                    {
                        // for bit field, we only increase the offset on last child
                        if (cpField == cpField.Parent.LastChild)
                        {
                            context.StartOffset += ((CPStruct)cpField.Parent).Size;
                        }
                        break;
                    }
                case TetraCpFieldType.unsigned_long:
                case TetraCpFieldType.signed_long:
                case TetraCpFieldType.UINT32:
                    {
                        // 4 bytes type
                        for (int i = 0; i < cpField.InstanceCount; i++)
                        {
                            // swap [0] and [3]
                            byte b = context.Image[context.StartOffset + 0];
                            context.Image[context.StartOffset + 0] = context.Image[context.StartOffset + 3];
                            context.Image[context.StartOffset + 3] = b;
                            // swap [1] and [2]
                            b = context.Image[context.StartOffset + 1];
                            context.Image[context.StartOffset + 1] = context.Image[context.StartOffset + 2];
                            context.Image[context.StartOffset + 2] = b;

                        }
                        context.StartOffset += 4;

                        break;
                    }
                case TetraCpFieldType.UINT16:
                case TetraCpFieldType.wchar_t:
                case TetraCpFieldType.signed_short:
                case TetraCpFieldType.unsigned_short:
                    {
                        // 2 bytes type
                        for (int i = 0; i < cpField.InstanceCount; i++)
                        {
                            // swap [0] and [1]
                            byte b = context.Image[context.StartOffset + 0];
                            context.Image[context.StartOffset + 0] = context.Image[context.StartOffset + 1];
                            context.Image[context.StartOffset + 1] = b;
                        }
                        context.StartOffset += 2;

                        break;
                    }
                default:
                    break;
            }

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

        ~EndianReverseByteVisitor()
        {
            Dispose(false);
        }
    }
}
