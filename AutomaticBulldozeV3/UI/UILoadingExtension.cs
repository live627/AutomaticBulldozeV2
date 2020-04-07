using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace AutomaticBulldozeV3.UI
{
    public class UILoadingExtension : LoadingExtensionBase
    {
        private LoadMode mode;
        private UIAutoBulldozerPanel autoBulldozerPanel;

        private void InitWindows()
        {
            var bulldozerBar = UIView.Find("BulldozerBar");
            autoBulldozerPanel = bulldozerBar.AddUIComponent<UIAutoBulldozerPanel>();
            autoBulldozerPanel.relativePosition = new Vector3(10.0f, -20.0f);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;
            mode = mode;

            InitWindows();
        }

        public override void OnLevelUnloading()
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            if (autoBulldozerPanel != null)
                Object.Destroy(autoBulldozerPanel);
        }
    }
}
