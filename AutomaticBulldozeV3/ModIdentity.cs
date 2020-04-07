using System.Reflection;
using ICities;

namespace AutomaticBulldozeV3
{
    public class ModIdentity : IUserMod
    {
        public string Name => "Automatic Bulldoze v3";

        public string Description
        {
            get
            {
                return $"Automatically destroys abandoned and burned buildings ({Assembly.GetExecutingAssembly().GetName().Version})"
#if DEBUG
                        + " DEBUG version"
#endif
                    ;
            }
        }
    }
}
