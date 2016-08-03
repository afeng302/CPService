﻿using System;
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
    class BitFieldMaskSettingVisitor : ICPVisitor
    {
        bool disposed = false;
        public void VisitCPStruct(CPTree.ICPStruct cpStruct)
        {
            // do nothing
        }

        public void VisitCPField(CPTree.ICPField cpField)
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

        }

        public void Dispose()
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
