 using System.Collections;
using System.Collections.Generic;
 using System.Text;
 using TMPro;
 using Unity.Serialization.Json;
 using UnityEngine.UI;
 using Unity.Profiling;
using UnityEngine;

 

public class UIScript : MonoBehaviour
{
    public TextMeshProUGUI fpstext;//displays fps
    public TextMeshProUGUI spawntext;//displays number of spawned entities
    public TextMeshProUGUI gctext;//displays graphics card schematics
    public TextMeshProUGUI suText;//displays system memory usage

    public GameObject s1;//spawner_1
    public GameObject s2;//spawner_2
    public GameObject s3;//spawner_3

    
    
    
    public SpawnManager spawn;

    private bool turret;
    
    private int numSmooth = 10;
    private float[] times;
    private int curTime;

    private float rotationCounter;
    private float rotMax = 2.0f;

    private ProfilerRecorder systemMemoryRecorder;//the system memory overall output
    private ProfilerRecorder gcMemoryRecorder;
    private ProfilerRecorder mainThreadTimeRecorder;//how long does it take to execute main thread
    void Start()
    {
 
        times = new float[numSmooth];

        s1.SetActive(false);
        s2.SetActive(false);
        s3.SetActive(false);
        

        turret = false;
    }

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;

        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;


            return r;
        }
    }
    
    void OnEnable()
    {
        systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
    }

    void OnDisable()
    {
        systemMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();
    }
    
    // Update is called once per frame
    void Update()
    {
        //This will update the fps. 
        times[curTime] = Time.deltaTime;
        curTime = (curTime + 1)% numSmooth;//provides a modular answer where every time
        //curTime reaches 10 it goes back to zero.
        float avTime = 0.0f;
        foreach(float t in times)
        {
            avTime = +t;
            
        }
        avTime /= times.Length;

        float fps = 0.1f/ avTime;//gives the current fps value.
        int fpsInt = (int)(fps + 0.5f);
        
        //var sb = new StringBuilder(500);
        //sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
        //sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
        //sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
        //string statsText = sb.ToString();
        
        fpstext.text = $"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms";
        spawntext.text = "Count: " + spawn.Count;
        gctext.text = $"GPU: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB";
        suText.text = $"System: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB";

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            s1.SetActive(false);
            s2.SetActive(false);
            s3.SetActive(false);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            s1.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            s2.SetActive(true);
            
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            s3.SetActive(true);
            turret = true;

        }




        if (turret == true)
        {
            rotationCounter++;
            if (rotationCounter <= 110)
            {
                s3.transform.rotation = Quaternion.Euler(0f, 0f, -0.28f * rotationCounter);
            }
            else if (rotationCounter > 110 && rotationCounter <= 240)
            {
                s3.transform.rotation = Quaternion.Euler(0f, 0f, 0.28f * rotationCounter);
            }
            else
            {
                rotationCounter = 0;
            }
        }
        //Debug.Log(rotationCounter);
    }
}