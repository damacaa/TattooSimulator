using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DesignSelectionButton : MonoBehaviour
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
            TattooSpawner.Instance.SetTexture(spriteName);
            UIManager.instance.CloseTattooSelector();
        }

        public void Delete()
        {
            print($"Deleting design {spriteName}");
            DesignManager.Instance.DeleteDesign(spriteName);
            Destroy(gameObject);
        }
    }
}
