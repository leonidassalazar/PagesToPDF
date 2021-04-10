using System.Linq;

namespace Core.Utils
{
    public class EntitiesUtil
    {
        public static void CopyPropertiesSameType<T>(T source, T destiny)
        {
            var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
            var destProps = typeof(T).GetProperties()
                                            .Where(x => x.CanWrite)
                                            .ToList();

            foreach (var sourceProp in sourceProps)
            {
                if (destProps.Any(x => x.Name == sourceProp.Name))
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    { // check if the property can be set or no.
                        p.SetValue(destiny, sourceProp.GetValue(source, null), null);
                    }
                }

            }
        }
    }
}
