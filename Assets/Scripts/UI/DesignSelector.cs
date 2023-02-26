using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DesignSelector : MonoBehaviour
    {
        public static DesignSelector Instance;

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        Transform content;
        [SerializeField]
        GameObject buttonPrefab;

        List<GameObject> buttons = new List<GameObject>();

        public void AddDesignButton(Sprite sprite, string name)
        {
            GameObject g = GameObject.Instantiate(buttonPrefab, content);
            g.GetComponent<DesignSelectionButton>().SetSprite(sprite, name);
            g.transform.SetSiblingIndex(g.transform.GetSiblingIndex() - 1);
            buttons.Add(g);
        }

        public void DeleteButtons()
        {
            foreach (GameObject b in buttons)
            {
                Destroy(b);
            }
            buttons.Clear();
        }
    }
}