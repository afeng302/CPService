﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.Visitor
{
    class TestStructVisitor : AbsCPVisitor
    {
        StreamWriter sw = null;
        bool disposed = false;
        public TestStructVisitor(string outputFile)
        {
            sw = new StreamWriter(outputFile, false);
        }

        public override void VisitCPStruct(CPTree.ICPStruct cpStruct)
        {
            sw.WriteLine(cpStruct.Tag.ToString());
        }

        public override void VisitCPField(CPTree.ICPField cpField)
        {
            sw.WriteLine(cpField.Tag.ToString());
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
                sw.Close();
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

        ~TestStructVisitor()
        {
            Dispose(false);
        }
    }
}
