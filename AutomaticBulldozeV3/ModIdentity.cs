using ICities;

namespace AutomaticBulldozeV3
{
    public class ModIdentity : IUserMod
    {
        private static System.Reflection.AssemblyName Asm => System.Reflection.Assembly.GetExecutingAssembly().GetName();

        public string Name => $"{Asm.Name}(v{Asm.Version})";

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
