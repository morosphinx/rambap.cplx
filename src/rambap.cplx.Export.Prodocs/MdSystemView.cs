using rambap.cplx.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rambap.cplx.Export.Prodocs
{
    public class MdSystemView : IInstruction
    {
        private Pinstance DocumentedPart { get; init; }
        public MdSystemView(Pinstance Target)
        {
            DocumentedPart = Target;
        }

        public void Do(string path)
        {
            using (var file = File.OpenWrite(path))
            using (var stream = new StreamWriter(file))
            {
                stream.WriteLine(CommonSections.CommonHeader(DocumentedPart));
            }
        }
    }
}
