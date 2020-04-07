using ICities;

namespace AutomaticBulldozeV3.UI.Localization
{
    public class LocalizationMonitor : ThreadingExtensionBase
    {
        public override void OnBeforeSimulationTick()
        {
            LocalizationManager.Instance.CheckAndUpdateLocales();

            base.OnBeforeSimulationTick();
        }
    }
}