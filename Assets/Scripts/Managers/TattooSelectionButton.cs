using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TattooSelectionButton : MonoBehaviour
{
    [SerializeField]
    Image tattooImage;
    [SerializeField]
    Sprite sprite;

    public void SetSprite(Sprite sp)
    {
        sprite = sp;
        tattooImage.sprite = sprite;

    }

    public void SelectTattoo()
    {
        TattooManager.instance.SetTexture(tattooImage.mainTexture);
    }
}
