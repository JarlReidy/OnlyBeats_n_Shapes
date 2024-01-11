using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;

public class ScreenScript : MonoBehaviour
{
    public TextMeshProUGUI test;//flickering text
    public GameObject logo;//only geometry and tunes logo

    private float timer;

    public Button ecsbutton;//this will open the ecs test level


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer = timer + Time.deltaTime;//updating timer

        Vector3 scaler = new Vector3(Mathf.Lerp(0.0001f,0.0001f,0.0001f), 0.1f, 0.1f);

        if (timer>=0.5)
        {
            test.enabled = false;
          
        }
        if(timer>=1)
        {
            test.enabled = true;
         
            timer = 0;
        }
    }


    public void ECS_Level()
    {
      //  yield return new WaitForSeconds(2.55f);//seconds delay before loading new ecs_scene
        SceneManager.LoadScene(1);//index which we have used.

    }

    public void Norm_Level()
    {
        SceneManager.LoadScene(2);
    }    
}
