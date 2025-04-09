using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Windows;
using System;
using System.IO;
using UnityEngine;

// -----------------!!! NOT USED IN THIS VERSION OF THE GAME !!!-----------------

public class _SystemInteractions : MonoBehaviour // used to interact with the player computer
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    private const int SPI_SETDESKWALLPAPER = 20;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    public static void SetWallpaper(string imgName) // sets the player wallpaper
    {
        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, Application.dataPath+"/sprites/"+imgName, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
    }

    public static void createTXT(string name, string content, string filePath) // creates txt file
    {
        string fileName = name+".txt";
        File.WriteAllText(filePath, content);
    }
}