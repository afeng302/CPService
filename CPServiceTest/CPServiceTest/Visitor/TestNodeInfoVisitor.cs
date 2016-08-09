using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.Visitor
{
    class TestNodeInfoVisitor : AbsCPVisitor
    {
        StreamWriter sw = null;
        bool disposed = false;

        public TestNodeInfoVisitor(string outputFile)
        {
            sw = new StreamWriter(outputFile, false);
        }
        public override void VisitCPStruct(CPTree.ICPStruct cpStruct)
        {
            sw.WriteLine(@"{0} {1} ""struct"" {{{2}}} ", cpStruct.FullName, cpStruct.Offset, cpStruct.InstanceCount);
        }

        public override void VisitCPField(CPTree.ICPField cpField)
        {
            sw.WriteLine(@"{0} {1} ""{2}"" {{{3}}} mask[{4:X2}]", cpField.FullName, cpField.Offset, 
                cpField.FieldType, cpField.InstanceCount, cpField.Mask);
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

        ~TestNodeInfoVisitor()
        {
            Dispose(false);
        }
    }
}
