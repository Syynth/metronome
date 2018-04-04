using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Menu
{

    public class MenuTranslationController : MonoBehaviour
    {

        public string CurrentPanel = "main";

        public AnimationCurve Curve;

        public Vector3 NewPanelLocation = new Vector3(-1920, 0);
        public Vector3 LoadPanelLocation = new Vector3(1920, 0);
        public Vector3 SettingsPanelLocation = new Vector3(0, 1080);
        public Vector3 MainPanelLocation = Vector3.zero;

        public CanvasGroup NewPanel;
        public CanvasGroup LoadPanel;
        public CanvasGroup SettingsPanel;
        public CanvasGroup MainPanel;

        public float time = 0;

        public void GoToPanel(string panel)
        {
            CurrentPanel = panel;
            time = 2;
        }

        void Update()
        {
            time = Mathf.Max(0, time - Time.deltaTime);
            switch (CurrentPanel.ToLower())
            {
                case "main":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, MainPanelLocation, Curve.Evaluate(2 - time));
                    NewPanel.alpha = time;
                    LoadPanel.alpha = time;
                    MainPanel.alpha = 2 - time;
                    SettingsPanel.alpha = time;
                    break;
                case "load":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, LoadPanelLocation, Curve.Evaluate(2 - time));
                    NewPanel.alpha = time;
                    LoadPanel.alpha = 2 - time;
                    MainPanel.alpha = time;
                    SettingsPanel.alpha = time;
                    break;
                case "new":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, NewPanelLocation, Curve.Evaluate(2 - time));
                    NewPanel.alpha = 2 - time;
                    LoadPanel.alpha = time;
                    MainPanel.alpha = time;
                    SettingsPanel.alpha = time;
                    break;
                case "settings":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, SettingsPanelLocation, Curve.Evaluate(2 - time));
                    NewPanel.alpha = time;
                    LoadPanel.alpha = time;
                    MainPanel.alpha = time;
                    SettingsPanel.alpha = 2 - time;
                    break;
            }
        }

    }


}
