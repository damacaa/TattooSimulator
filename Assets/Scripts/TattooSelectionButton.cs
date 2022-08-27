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
    string spriteName;

    public void SetSprite(Sprite sp, string spriteName)
    {
        sprite = sp;
        tattooImage.sprite = sprite;
        this.spriteName = spriteName;
    }

    public void SelectTattoo()
    {
        TattooManager.instance.SetTexture(spriteName);
        UIManager.instance.CloseTattooSelector();
    }

    public void Delete()
    {
        print($"Deleting design {spriteName}");
        DesignManager.instance.DeleteDesign(spriteName);
        Destroy(gameObject);
    }
}
