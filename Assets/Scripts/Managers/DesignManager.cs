using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static NativeGallery;
using SimpleFileBrowser;

public class DesignManager : MonoBehaviour
{
    static public DesignManager instance;

    private void Awake()
    {
        instance = this;
    }


    [SerializeField]
    Transform content;
    [SerializeField]
    List<Sprite> sprites;
    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    bool loadFromFolder = true;

    public Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
    List<GameObject> buttons = new List<GameObject>();

    string imageFolderPath;



    public void LoadDesigns()
    {
        print(buttons.Count + " buttons deleted");
        foreach (GameObject b in buttons)
        {
            Destroy(b);
        }
        buttons.Clear();

        textures = new Dictionary<string, Texture>();

#if !UNITY_WEBGL || UNITY_EDITOR
        imageFolderPath = FileManager.ImageFolderPath;

        Resources.UnloadUnusedAssets();

        /*for (int i = 0; i < sprites.Count; i++)
        {
            AddButton(sprites[i].texture, sprites[i].texture.name);
        }
        sprites.Clear();*/

        if (loadFromFolder)
        {
            string[] files;
            files = System.IO.Directory.GetFiles(imageFolderPath, "*.png");

            for (int i = 0; i < files.Length; i++)
            {
                Texture2D tex = LoadPNG(files[i]);
                string name = Path.GetFileNameWithoutExtension(files[i]);
                AddDesign(tex, name);
            }
            //Debug.Log("Loading done");
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
            tex = new Texture2D(1, 1);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            //tex.alphaIsTransparency = true;
            //tex.anisoLevel = 8;
        }
        return tex;
    }

    void AddDesign(Texture2D tex, string name)
    {
        tex.name = name;
        textures[name] = tex;
        Sprite s = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

        GameObject g = GameObject.Instantiate(buttonPrefab, content);
        g.GetComponent<TattooSelectionButton>().SetSprite(s);
        g.transform.SetSiblingIndex(g.transform.GetSiblingIndex() - 1);
        buttons.Add(g);

#if !UNITY_WEBGL
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(imageFolderPath + "/" + name + ".png", bytes);
        //Resources.UnloadUnusedAssets();
#endif
    }

    public void RequestImage()
    {
#if UNITY_WEBGL
        GetImage.GetImageFromUserAsync(gameObject.name, "ReceiveImage");
        //Alternative:
        //https://forum.unity.com/threads/how-do-i-let-the-user-load-an-image-from-their-harddrive-into-a-webgl-app.380985/
#elif UNITY_STANDALONE
        PickImageStandalone();
#elif UNITY_ANDROID || UNITY_IOS
        PickImageMobile(0);
#endif
    }


#if UNITY_WEBGL
    static string s_dataUrlPrefix = "data:image/png;base64,";
    public void ReceiveImage(string dataUrl)
    {
        if (dataUrl.StartsWith(s_dataUrlPrefix))
        {
            byte[] pngData = System.Convert.FromBase64String(dataUrl.Substring(s_dataUrlPrefix.Length));

            // Create a new Texture (or use some old one?)
            Texture2D tex = new Texture2D(1, 1); // does the size matter? hehe
            if (tex.LoadImage(pngData))
            {
                AddDesign(tex, "a");
                print(dataUrl);
            }
            else
            {
                Debug.LogError("could not decode image");
            }
        }
        else
        {
            Debug.LogError("Error getting image:" + dataUrl);
        }
    }
#endif

    private void PickImageMobile(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);//Check all paramenters later
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                string name = Path.GetFileNameWithoutExtension(path);
                AddDesign(texture, name);

                //byte[] bytes = texture.EncodeToPNG();
                //File.WriteAllBytes(imageFolderPath + "/" + name + ".png", bytes);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                //Destroy(texture, 5f);
            }
        });

        Debug.Log("Permission result: " + permission);
    }

    void PickImageStandalone()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));
        FileBrowser.SetDefaultFilter(".png");
        //FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Load your designs", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            for (int i = 0; i < FileBrowser.Result.Length; i++)
            {
                Debug.Log(FileBrowser.Result[i]);

                // Read the bytes of the first file via FileBrowserHelpers
                // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
                byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[i]);

                // Create a new Texture (or use some old one?)
                Texture2D tex = new Texture2D(1, 1); // does the size matter? hehe
                if (tex.LoadImage(bytes))
                {
                    string name = Path.GetFileNameWithoutExtension(FileBrowser.Result[i]);
                    AddDesign(tex, name);
                    //FileBrowserHelpers.CopyFile(FileBrowser.Result[i], FileManager.ImageFolderPath + "/" + name);
                }
            }
        }
    }
}
