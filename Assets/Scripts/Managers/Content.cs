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
        //string path = @"C:\Users\Public\Pictures\Sample Pictures\";
        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            GameObject g = GameObject.Instantiate(buttonPrefab, content);
            g.GetComponent<TattooSelectionButton>().SetSprite(sprites[i]);
            Texture t = sprites[i].texture;
            textures[t.name] = t;
        }
        sprites.Clear();
        yield return null;

#if UNITY_STANDALONE || UNITY_EDITOR
        if (loadFromFolder)
        {
            string path = Directory.GetParent(Application.dataPath).ToString() + "\\Images";

            //string pathPreFix = @"file://";

            string[] files;
            files = System.IO.Directory.GetFiles(path, "*.png");
            yield return null;


            for (int i = 0; i < files.Length; i++)
            {
                Texture2D tex = LoadPNG(files[i]);
                string name = Path.GetFileName(files[i]);
                name = Path.GetFileNameWithoutExtension(name);
                tex.name = name;
                textures[name] = tex;
                yield return null;
                Sprite s = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                yield return null;

                GameObject g = GameObject.Instantiate(buttonPrefab, content);
                g.GetComponent<TattooSelectionButton>().SetSprite(s);
                yield return null;
            }
            Debug.Log("Loading done");
        }

        yield return null;
        TattooManager.instance.LoadData();
        yield return null;
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
