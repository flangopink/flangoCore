using Verse;
using System.Collections.Generic;
using System.Xml;

namespace flangoCore
{
    public class XMLRandomPatch : PatchOperation
    {
        public List<PatchOperation> operations;
        public bool enabled = true;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (enabled && !operations.NullOrEmpty())
                return operations.RandomElement().Apply(xml);
            return true;
        }
    }
}
