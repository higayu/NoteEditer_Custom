using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadmanager : MonoBehaviour
{
    public static Loadmanager instance = null;
    public bool WAV_Lode_Flg = false;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }
}
