using BepInEx;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace RageQuitButton
{
    [BepInPlugin("com.Melon.RageQuitButton", "RageQuitButton", "1.0.0")]
    public class RageQuitButton : BaseUnityPlugin
    {
        public void Awake()
        {
            base.Logger.LogInfo("Plugin RageQuitButton is loaded!");
            Harmony harmony = new Harmony("com.Melon.RageQuitButton");
            MethodInfo methodInfo = AccessTools.Method(typeof(Player), "Kill", null, null);
            MethodInfo methodInfo2 = AccessTools.Method(typeof(RageQuitButton.Patches), "ShowRageQuitPopup", null, null);
            harmony.Patch(methodInfo, new HarmonyMethod(methodInfo2), null, null, null, null);
        }

        public class Patches
        {
            public static void ShowRageQuitPopup()
            {
                // Show popup
                ShowPopup("Rage quit?", "Do you want to rage quit?", () =>
                {
                    Application.Quit(); 
                });
            }

            private static void ShowPopup(string title, string text, System.Action onClose)
            {
                GameObject popupObject = new GameObject("Popup");
                Popup popup = popupObject.AddComponent<Popup>();
                popup.title = title;
                popup.text = text;
                popup.onClose = onClose;
            }
        }

        public class Popup : MonoBehaviour
        {
            public string title;
            public string text;
            public int width = 300;
            public int textOffset = 20;
            public Rect okbutton = new Rect(0, 0, 75, 20);
            public System.Action onClose;

            void OnGUI()
            {
                var textStyle = new GUIStyle
                {
                    alignment = TextAnchor.MiddleLeft
                };

                var height = (int)textStyle.CalcHeight(new GUIContent(text), width);
                var box = new Rect(Screen.width / 2 - width / 2, Screen.height / 2 - height / 2, width, height + textOffset + okbutton.height);
                var infotext = box;
                infotext.y += textOffset;
                infotext.height -= textOffset + okbutton.height;

                box.width += 40;
                box.x -= 20;

                okbutton.x = box.x + box.width - okbutton.width - 10;
                okbutton.y = box.y + box.height - okbutton.height - 10;

                var bgcolor = new Color(0, 1, 1); 
                bgcolor.a = 1;
                GUI.backgroundColor = bgcolor;

                GUI.Box(box, title);

                GUI.color = Color.white;
                GUI.TextField(infotext, text, textStyle);


                if (GUI.Button(okbutton, "OK"))
                {
                    Destroy(gameObject);
                    onClose?.Invoke(); 
                }
            }
        }
    }
}
