using ICities;

namespace AutomaticBulldozeV3
{
    public class ModIdentity : IUserMod
    {
        public string Name => $"Automatic Bulldoze (v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version})";

        public string Description
        {
            get
            {
                return "Automatically destroys abandoned and burned buildings"
#if DEBUG
                        + " DEBUG version"
#endif
                    ;
            }
        }
    }
}
