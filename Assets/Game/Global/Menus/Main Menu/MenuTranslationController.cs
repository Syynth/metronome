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

        public Transform Camera;

        public Vector3 NewPanelLocation = new Vector3(-1920, 0);
        public Vector3 LoadPanelLocation = new Vector3(1920, 0);
        public Vector3 SettingsPanelLocation = new Vector3(0, 1080);
        public Vector3 MainPanelLocation = Vector3.zero;

        public CanvasGroup NewPanel;
        public CanvasGroup LoadPanel;
        public CanvasGroup SettingsPanel;
        public CanvasGroup MainPanel;

        public float menuTransitionTime = 4f;
        public float cameraTranslationScale = 75f;
        public float time = 0;

        public void GoToPanel(string panel)
        {
            CurrentPanel = panel;
            time = menuTransitionTime;
        }

        void Update()
        {
            time = Mathf.Max(0, time - Time.deltaTime);
            var timeScale = (menuTransitionTime - time) / menuTransitionTime;
            var inverseTimeScale = 1 - timeScale;
            switch (CurrentPanel.ToLower())
            {
                case "main":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, MainPanelLocation, Curve.Evaluate(timeScale));
                    NewPanel.alpha = inverseTimeScale / menuTransitionTime;
                    LoadPanel.alpha = inverseTimeScale / menuTransitionTime;
                    MainPanel.alpha = timeScale;
                    SettingsPanel.alpha = inverseTimeScale / menuTransitionTime;
                    break;
                case "load":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, LoadPanelLocation, Curve.Evaluate(timeScale));
                    NewPanel.alpha = inverseTimeScale / menuTransitionTime;
                    LoadPanel.alpha = timeScale;
                    MainPanel.alpha = inverseTimeScale / menuTransitionTime;
                    SettingsPanel.alpha = inverseTimeScale / menuTransitionTime;
                    break;
                case "new":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, NewPanelLocation, Curve.Evaluate(timeScale));
                    NewPanel.alpha = timeScale;
                    LoadPanel.alpha = inverseTimeScale / menuTransitionTime;
                    MainPanel.alpha = inverseTimeScale / menuTransitionTime;
                    SettingsPanel.alpha = inverseTimeScale / menuTransitionTime;
                    break;
                case "settings":
                    transform.localPosition = Vector3.Lerp(transform.localPosition, SettingsPanelLocation, Curve.Evaluate(timeScale));
                    NewPanel.alpha = inverseTimeScale / menuTransitionTime;
                    LoadPanel.alpha = inverseTimeScale / menuTransitionTime;
                    MainPanel.alpha = inverseTimeScale / menuTransitionTime;
                    SettingsPanel.alpha = timeScale;
                    break;
            }
            Camera.transform.position = transform.localPosition / -cameraTranslationScale;
        }

    }


}
