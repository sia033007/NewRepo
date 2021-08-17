using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewScript : MonoBehaviour
{
    public string serverUrl;
    List<float> position = new List<float>();
    public InputField username;
    public InputField password;
    public int score;
    public static NewScript instance;
    // Start is called before the first frame update
    private void Awake() {
        instance = this;
    }
    void Start()
    {
        position.Add(this.transform.position.x);
        position.Add(this.transform.position.y);
        position.Add(this.transform.position.z);

        Human human1 = new Human(15,"Ali","IRAN");
        Human human2 = new Human(22,"Jack","ENGLAND");
        Human human3 = new Human(50,"Paulo","BRAZIL");

        human1.Person();
        human2.Person();
        human3.Person();
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(sendPos(serverUrl));
        float midScreen = Screen.width/2;
        if(Input.touchCount>0)
        {
            Touch t = Input.GetTouch(0);
            if(t.phase == TouchPhase.Began && t.position.x<midScreen)
            {
                transform.Translate(0,2,0);
            }
            if(t.phase == TouchPhase.Stationary && t.position.x>midScreen)
            {
                transform.Translate(2,0,0);
            }
        }
    }
     IEnumerator AssetBundleCreate()
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle("");
            yield return request.SendWebRequest();

            if(request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
                var prefab = bundle.LoadAsset("Player");
                GameObject player = Instantiate(prefab,transform.position,Quaternion.identity)as GameObject;
            }
        }
        IEnumerator Login()
        {
            WWWForm form = new WWWForm();
            form.AddField("Username",username.text);
            form.AddField("Password",password.text);
            UnityWebRequest request = UnityWebRequest.Post("",form);
            yield return request.SendWebRequest();

            if(request.downloadHandler.text[0]=='0')
            {
                Debug.Log("Successfully logged in!");
                score = int.Parse(request.downloadHandler.text.Split('\t')[1]);

            }
        }
        IEnumerator sendPos(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if(request.isHttpError || request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                var posJSON = JsonUtility.ToJson(position);
            }

        }      
}
public class Human{
    public int age;
    public string name;
    public string country;

    public Human(int myage, string myname, string mycountry)
    {
        age = myage;
        name = myname;
        country = mycountry;
    }
    public void Person()
    {
        Debug.Log($"{name} is {age} years old from {country}");
    }
}
