using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.CPTree;

namespace CPServiceTest.Visitor
{

/*
            typedef struct
            {
               uint32_t freq_sim_dup      :1;
               uint32_t sin_multi_slot    :1;
               uint32_t conc_multicar_op  :1;
               uint32_t voice             :1;
               uint32_t e2e_encryption    :1;
               uint32_t circuit_mode_data :1;
               uint32_t TETRA_packet_data :1;
               uint32_t fast_switching    :1;
               uint32_t DCK_air_if_encryption :1;
               uint32_t CLCH_carrier_chng :1;
               uint32_t concurrent_chnls  :1;
               uint32_t advanced_link     :1;
               uint32_t minimum_mode      :1;
               uint32_t carr_spec_channel :1;
               uint32_t authentication    :1;
               uint32_t SCK_air_if_encryption :1;
               uint32_t tetra_version_num :3;
               uint32_t common_SCCH       :1;
               uint32_t reserved1         :1;
               uint32_t mac_d_blck        :1;
               uint32_t extended_al       :1;
               uint32_t d8psk             :1;
               uint32_t UNUSED_BITS       :8;
   
            }  class_of_ms_t;
*/


    /// <summary>
    /// The bit field offset and mask setting.
    /// 
    /// http://mjfrazer.org/mjfrazer/bitfields/ 
    /// https://msdn.microsoft.com/en-us/library/yszfawxh.aspx
    /// 
    /// </summary>
    class BitFieldMaskSettingVisitor : AbsCPVisitor
    {
        bool disposed = false;
        public override void VisitCPStruct(CPTree.ICPStruct cpStruct)
        {
            // do nothing
        }

        public override void VisitCPField(CPTree.ICPField cpField)
        {
            if (cpField.FieldType != CPTree.TetraCpFieldType.bit)
            {
                return; // only focus on bit field
            }

            if (((ICPStruct)cpField.Parent).Size != 4) // sizeof (uint32_t)
            {
                // error log
                // we only support the bit field definition at header of this file
                // for other case, we cannot get enough information from .cps file
                throw new Exception("bit field mask setting error");
            }

            //
            // CP Service will aussume the codeplug binary is always big-endian.
            // We will calculate the mask and offset base on big-endian schema.
            //

            //
            // update offset and startbit
            int offset = cpField.Offset;
            offset += (cpField.StartBit / 8);
            int startbit = (cpField.StartBit % 8);
            cpField.SetAttr(cpField.FieldType, offset, cpField.BitLen, startbit);

            //
            // update mask
            cpField.SetMask(MaskMatrix[cpField.BitLen - 1, cpField.StartBit]);
        }


        /// <summary>
        /// http://mjfrazer.org/mjfrazer/bitfields/
        /// 
        /// "Again, the bits are pack from most significant on a big endian machine and least significant 
        /// on a little endian machine. Interpreted as a short, the bitfield 'a' adds 0x0001 to 'value' on 
        /// a little endian machine and 0x8000 on a big endian machine. The unused bit is left to the end 
        /// of interpreting the struct, so it is the MSB on a little endian machine and the LSB on a big 
        /// endian machine."
        /// 
        /// This matrix works for big endian machine (CP Service will handle big endian codeplug stream).  
        /// [bitLen - 1, startBit]
        /// </summary>
        private static byte[,] MaskMatrix = new byte[,] 
            { 
    // startBit=0,    1,    2,    3,    4,    5,    6,    7
              { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01}, // bitLen=1
              { 0xC0, 0x60, 0x30, 0x18, 0x0C, 0x06, 0x03, 0   }, // bitLen=2
              { 0xE0, 0x70, 0x38, 0x1C, 0x0E, 0x07, 0,    0   }, // bitLen=3
              { 0xF0, 0x78, 0x3C, 0x1E, 0x0F, 0,    0,    0   }, // bitLen=4
              { 0xF8, 0x7C, 0x3E, 0x1F, 0,    0,    0,    0   }, // bitLen=5
              { 0xFC, 0x7E, 0x3F, 0,    0,    0,    0,    0   }, // bitLen=6
              { 0xFE, 0x7F, 0,    0,    0,    0,    0,    0   }, // bitLen=7
              { 0xFF, 0,    0,    0,    0,    0,    0,    0   }}; // bitLen=8

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

        ~BitFieldMaskSettingVisitor()
        {
            Dispose(false);
        }
    }
}
