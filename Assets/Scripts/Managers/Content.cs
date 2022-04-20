using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Content : MonoBehaviour
{
    static public Content instance;


    [SerializeField]
    Transform content;
    [SerializeField]
    List<Sprite> sprites;
    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    bool loadFromFolder = true;

    public Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
    void Awake()
    {
        instance = this;

#if UNITY_WEBGL || UNITY_EDITOR
        for (int i = 0; i < sprites.Count; i++)
        {
            GameObject g = GameObject.Instantiate(buttonPrefab, content);
            g.GetComponent<TattooSelectionButton>().SetSprite(sprites[i]);
            Texture t = sprites[i].texture;
            textures[t.name] = t;
        }
        sprites.Clear();
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        if (loadFromFolder)
        {
            string path = Directory.GetParent(Application.dataPath).ToString() + "\\Images";

            //string pathPreFix = @"file://";

            string[] files;
            files = System.IO.Directory.GetFiles(path, "*.png");



            for (int i = 0; i < files.Length; i++)
            {
                Texture2D tex = LoadPNG(files[i]);
                string name = Path.GetFileName(files[i]);
                name = Path.GetFileNameWithoutExtension(name);
                tex.name = name;
                textures[name] = tex;

                Sprite s = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);


                GameObject g = GameObject.Instantiate(buttonPrefab, content);
                g.GetComponent<TattooSelectionButton>().SetSprite(s);

            }
            Debug.Log("Loading done");
        }

        //TattooManager.instance.LoadData();

#endif
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            //tex.alphaIsTransparency = true;
            //tex.anisoLevel = 8;
        }
        return tex;
    }
}
