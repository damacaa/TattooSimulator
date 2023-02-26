using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TattooEditorUI : MonoBehaviour
    {
        [SerializeField]
        Slider angleSlider;
        [SerializeField]
        Slider sizeSlider;

        public void SetSettings(float angle, float size)
        {
            angleSlider.value = angle;
            sizeSlider.value = size;
        }
    }
}
