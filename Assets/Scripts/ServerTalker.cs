using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class ServerTalker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetData("http://localhost:8000/user/"));
        //make a web request to get info from server
        //this will be a text response
        
    }

    void processServerResponse(string rawResponse)
    {
        JSONNode node = JSON.Parse(rawResponse);
        //that text is actually a JSON, so we need to pars that into something
        //we can navigate output to the console
        Debug.Log("Username : " + node["username"]);

    }
    IEnumerator GetData(string address)
    {
        UnityWebRequest request = UnityWebRequest.Get(address);
        yield return request.SendWebRequest();

        if(request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            processServerResponse(request.downloadHandler.text);

        }
        
    }
    
}
