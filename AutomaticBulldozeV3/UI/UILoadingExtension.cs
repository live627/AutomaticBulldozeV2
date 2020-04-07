using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace AutomaticBulldozeV3.UI
{
    public class UILoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            UIComponent bulldozerBar = UIView.Find("BulldozerBar");
            UIAutoBulldozerPanel autoBulldozerPanel = bulldozerBar.AddUIComponent<UIAutoBulldozerPanel>();
            autoBulldozerPanel.relativePosition = new Vector3(10.0f, -10.0f);
        }
    }
}
