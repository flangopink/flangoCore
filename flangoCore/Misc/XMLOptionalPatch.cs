using Verse;
using System;
using System.Xml;
using System.Linq;
using System.Reflection;

namespace flangoCore
{
	public class XMLOptionalPatch : PatchOperation
	{
        public string settingsPath;

		public string key;

		public PatchOperation trueOperation;

		public PatchOperation falseOperation;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            FieldInfo settings = null;
            FieldInfo field = null;

            var mods = typeof(Mod).AllSubclasses();

            string[] pathSplit = settingsPath.Split('.'); // flangoCore.Controller.settings
            string classPath = string.Join(".", pathSplit.Take(pathSplit.Length - 1)); // flangoCore.Controller
            string settingsClass = pathSplit.Last(); // settings

            Log.Message(classPath + " " + settingsClass);

            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].ToString() == classPath)
                {
                    settings = mods[i].GetField(settingsClass);

                    if (settings != null)
                    {
                        field = settings.FieldType.GetField(key);
                        break;
                    }
                }
            }

            if (field != null && field.FieldType == typeof(bool))
            {
                bool b = (bool)field.GetValue(settings.GetValue(null));

                Log.Message(b.ToString());

                if (b && trueOperation != null)
                    return trueOperation.Apply(xml);

                else if (!b && falseOperation != null)
                    return falseOperation.Apply(xml);

                if (trueOperation == null) 
                    return falseOperation != null;
            }
            return true;
        }
    }
}
