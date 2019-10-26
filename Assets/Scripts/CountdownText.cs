using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for text

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{
    public delegate void CountdownDelegate();
    public static event CountdownDelegate OnCountdownFinished;
    public static event CountdownDelegate OnCountdownStarted;

    Text countdown;

    void OnEnable() // called when page is set to active
    {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        //OnCountdownStarted();
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        
        int count = 3;
        for (int i = 0; i < count; i++) // loops 3 times
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }

        OnCountdownFinished();
    }
}