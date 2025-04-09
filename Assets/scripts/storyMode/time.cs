using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class time : MonoBehaviour
{
    int[] timeProgress =
    {
        60, // seconds in minute
        15, // minutes in day
        5, // days in month
    };
    int secods = 0;
    float line = 0;

    int month;
    int days;
    int hours;

    private void Start()
    {
        line = _Save.LoadFloat("lineT");
        int datePart = Mathf.FloorToInt(line);

        month = datePart / 100;
        days = datePart % 100;
        hours = Mathf.FloorToInt((line - datePart) * 100);

        StartCoroutine(dwd());
    }

    IEnumerator dwd()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            secods++;
            if (secods == timeProgress[0]) { hours++; secods = 0; }
            if (hours == timeProgress[1]) { days++; hours = 0; }
            if (days == timeProgress[2]) { month++; days = 0; }

            _Save.Save("lineT", month * 100 + days + hours / 100f);
        }
    }
}