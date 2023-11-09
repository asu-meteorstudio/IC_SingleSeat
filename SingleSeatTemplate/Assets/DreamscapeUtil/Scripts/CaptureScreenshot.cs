using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{
    public class CaptureScreenshot : MonoBehaviour
    {
        [Tooltip("Folder to Place Screenshots In(relative to root of project folder)")]
        public string folder = "Screenshots";
        [Tooltip("Size of screenshots as a multiple of size of the game window")]
        public int sizeMultiplier = 1;


        [EasyButtons.Button("Capture Screenshot")]
        public void Capture()
        {
            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }

            int i = 0;
            string filePath;
            do
            {
                filePath = string.Format("{0}/capture_{1:D04}.png", folder, i);
                i++;
            } while (System.IO.File.Exists(filePath));


            ScreenCapture.CaptureScreenshot(filePath, sizeMultiplier);
        }
    }
}
