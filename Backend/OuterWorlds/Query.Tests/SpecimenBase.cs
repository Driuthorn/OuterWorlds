using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OuterWorlds.Infrastructure.Data.Query.Tests
{
    public class SpecimenBase<T> where T : class
    {
        protected SpecimenBase()
        { }

        public static T Create(string path)
        {
            StringBuilder jsonBuilder = null;

            using (var reader = new StreamReader(path))
            {
                jsonBuilder = new StringBuilder(reader.ReadToEnd());
            }

            return JsonConvert.DeserializeObject<T>(jsonBuilder.ToString());
        }

        public static List<T> CreateList(string path)
        {
            StringBuilder jsonBuilder = null;

            using (var reader = new StreamReader(path))
            {
                jsonBuilder = new StringBuilder(reader.ReadToEnd());
            }

            return JsonConvert.DeserializeObject<List<T>>(jsonBuilder.ToString());
        }
    }
}
