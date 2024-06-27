using ScrollableMessageBoxLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ScrollableMessageBoxLib.I18N
{
    public sealed class CultureInfoEnumerator
    {
        private IEnumerable<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> result = new List<CultureInfo>();

            ResourceManager rm = new ResourceManager(typeof(Resources));

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture)) continue; //do not use "==", won't work

                    ResourceSet rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                        result.Add(culture);
                }
                catch (CultureNotFoundException)
                {
                    //NOP
                }
            }
            return result;
        }

        public List<string> GetAvailableLanguages()
        {
            List<string> result = new List<string>();
            var cultures = GetAvailableCultures();
            foreach (CultureInfo culture in cultures)
                //result.Add(culture.NativeName + " (" + culture.EnglishName + " [" + culture.TwoLetterISOLanguageName + "])");
                result.Add(culture.IetfLanguageTag);
            return result;
        }
    }
}
