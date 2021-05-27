using System;
using System.Data;
using System.Threading.Tasks;
using Empire.DriverLog.Data;
using Empire.Shared.Utilities;

namespace Empire.DriverLog.Business
{
    public class RunFactory
    {
        public async Task Update(Run oRun)
        {
            var oRunRepository = new RunRepository();

            try
            {

                string sNote = String.IsNullOrWhiteSpace(oRun.Note) ? String.Empty : oRun.Note.Trim();

                byte[] aSignatureBytes = (oRun.Signature == null) ? null : oRun.Signature.ToPngBytes();
                await oRunRepository.Update(oRun.Number, sNote, aSignatureBytes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Run> Instance(int iRunNo)
        {
            var oRunRepository = new RunRepository();

            IDataReader oDataReader = null;
            try
            {
                oDataReader = await oRunRepository.Get(iRunNo);
            }
            catch (Exception)
            {
                throw;
            }

            if(!oDataReader.Read())
            {
                return null;
            }

            return new Run(oDataReader);
        }
    }
}
