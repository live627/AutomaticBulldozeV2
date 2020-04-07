using AutomaticBulldozeV3.UI.Localization;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace AutomaticBulldozeV3.UI
{
    public class UIAutoBulldozerPanel : UIPanel
    {
        private UIButton demolishAbandonedButton;
        private UIButton demolishBurnedButton;

        public static readonly SavedBool DemolishAbandoned = new SavedBool("ModDemolishAbandoned", Settings.gameSettingsFile, true, true);
        public static readonly SavedBool DemolishBurned = new SavedBool("ModDemolishBurned", Settings.gameSettingsFile, true, true);

        private static void InitButton(UIButton button)
        {
            var sprite = "SubBarButtonBase";
            var spriteHov = sprite + "Hovered";
            button.normalBgSprite = spriteHov;
            button.disabledBgSprite = spriteHov;
            button.hoveredBgSprite = spriteHov;
            button.focusedBgSprite = spriteHov;
            button.pressedBgSprite = sprite + "Pressed";
            button.textColor = new Color32(255, 255, 255, 255);
        }

        private void UpdateCheckButton(UIButton button, bool isActive)
        {
            var inactiveColor = new Color32(64, 64, 64, 255);
            var activeColor = new Color32(64, 255, 64, 255);
            var textColor = new Color32(255, 255, 255, 255);
            var textColorDis = new Color32(128, 128, 128, 255);

            if (isActive)
            {
                button.color = activeColor;
                button.focusedColor = activeColor;
                button.hoveredColor = activeColor;
                button.pressedColor = activeColor;
                button.textColor = textColor;
            }
            else
            {
                button.color = inactiveColor;
                button.focusedColor = inactiveColor;
                button.hoveredColor = inactiveColor;
                button.pressedColor = inactiveColor;
                button.textColor = textColorDis;
            }

            button.Unfocus();
        }

        private void SetLocales()
        {
            var bWidth = LocalizationManager.GetButtonWidth();
            demolishAbandonedButton.text = "Switch.DemolishAbandoned".Translate();
            demolishAbandonedButton.width = bWidth;
            demolishBurnedButton.text = "Switch.DemolishBurned".Translate();
            demolishBurnedButton.width = bWidth;
        }

        public override void Start()
        {
            // configure panel
            height = 50;
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Horizontal;
            autoLayoutPadding = new RectOffset(0, 10, 0, 0);
            autoLayoutStart = LayoutStart.TopLeft;

            demolishAbandonedButton = AddUIComponent<UIButton>();
            demolishAbandonedButton.width = 200;
            demolishAbandonedButton.height = 50;
            InitButton(demolishAbandonedButton);
            demolishAbandonedButton.eventClick += (component, param) =>
            {
                DemolishAbandoned.value = !DemolishAbandoned.value;
                UpdateCheckButton(demolishAbandonedButton, DemolishAbandoned.value);
            };

            demolishBurnedButton = AddUIComponent<UIButton>();
            
            demolishBurnedButton.width = 200;
            demolishBurnedButton.height = 50;
            InitButton(demolishBurnedButton);
            demolishBurnedButton.eventClick += (component, param) =>
            {
                DemolishBurned.value = !DemolishBurned.value;
                UpdateCheckButton(demolishBurnedButton, DemolishBurned.value);
            };

            UpdateCheckButton(demolishAbandonedButton, DemolishAbandoned.value);
            UpdateCheckButton(demolishBurnedButton, DemolishBurned.value);

            SetLocales();
            LocalizationManager.Instance.eventLocaleChanged += language => SetLocales();
            
            base.Start();
        }
    }
}
