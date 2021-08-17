using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSample : MonoBehaviour
{
    public int score;
    public List<float> playerpos = new List<float>();
    // Start is called before the first frame update
    void Start()
    {
        SampleCommand.updatescore += UpdateScore;
        playerpos.Add(this.transform.position.x);
        playerpos.Add(this.transform.position.y);
        playerpos.Add(this.transform.position.z);
        
    }
    private void Update() {
        var posToJson = JsonUtility.ToJson(playerpos);
    }
    public void UpdateScore()
    {
        score +=1;
    }
}
