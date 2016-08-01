using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPServiceTest.Visitor
{
    class VerifyStructVisitor : ICPVisitor
    {
        StreamWriter sw = null;
        bool disposed = false;
        public VerifyStructVisitor(string outputFile)
        {
            sw = new StreamWriter(outputFile, false);
        }
        public void VisitCPNode(CPTree.ICPNode node)
        {
            sw.WriteLine(node.Tag.ToString());
        }

        public void VisitCPStruct(CPTree.ICPStrcut cpStruct)
        {
            throw new NotImplementedException();
        }

        public void VisitCPField(CPTree.ICPField cpField)
        {
            throw new NotImplementedException();
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

        ~VerifyStructVisitor()
        {
            Dispose(false);
        }
    }
}
