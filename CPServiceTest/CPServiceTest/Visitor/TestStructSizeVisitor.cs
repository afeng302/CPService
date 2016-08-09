using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CPServiceTest.CPTree;

namespace CPServiceTest.Visitor
{
    class TestStructSizeVisitor : AbsCPVisitor
    {
        StreamWriter sw = null;
        bool disposed = false;

        public TestStructSizeVisitor(string outputFile)
        {
            sw = new StreamWriter(outputFile, false);
        }

        ICPNode rootNode = null;
        int blkSize = 0;
        public override void VisitCPStruct(CPTree.ICPStruct cpStruct)
        {
            sw.WriteLine(@"{0} - size[{1}] instance[{2}]", cpStruct.FullName, cpStruct.Size, cpStruct.InstanceCount);

            if (cpStruct.Parent == null)
            {
                rootNode = cpStruct;
            }

            if (cpStruct.Parent == rootNode)
            {
                blkSize += cpStruct.Size;
            }

            if ((cpStruct.Parent == rootNode) && (cpStruct.NextSibling == null))
            {
                sw.WriteLine("---- this is last block. Total size[{0}] ----", blkSize);
            }
        }

        public override void VisitCPField(CPTree.ICPField cpField)
        {
            // do nothing
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

        ~TestStructSizeVisitor()
        {
            Dispose(false);
        }
    }
}
