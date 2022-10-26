using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;
using UnityEngine.UI;

namespace Aokoro.UI.ControlsDiplaySystem.UI
{
    public class KeyboardAxis : ControlIcon
    {
        [SerializeField]
        TextMeshProUGUI[] texts;
        [SerializeField]
        Image[] images;

        [SerializeField]
        Sprite upArrow;
        [SerializeField]
        Sprite downArrow;
        [SerializeField]
        Sprite rightArrow;
        [SerializeField]
        Sprite leftArrow;

        public override void SetupIcon(CD_InputControl control)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                string controlPath = control.GetPathAtIndex(i);
                switch (controlPath.ToLower().Trim())
                {
                    case "uparrow":
                        SetArrowIcon(i, upArrow);
                        break;
                    case "downarrow":
                        SetArrowIcon(i, downArrow);
                        break;
                    case "rightarrow":
                        SetArrowIcon(i, rightArrow);
                        break;
                    case "leftarrow":
                        SetArrowIcon(i, leftArrow);
                        break;
                    default:
                        string name = control.GetDisplayNameAtIndex(i);

                        texts[i].SetText(name switch {
                            "W" => "Z",
                            "A" => "Q",
                            _ => name
                        });
                        images[i].gameObject.SetActive(false);
                        break;
                }


            }

            void SetArrowIcon(int i, Sprite sprite)
            {
                images[i].gameObject.SetActive(true);
                texts[i].gameObject.SetActive(false);
                images[i].sprite = sprite;
            }
        }
    }
}