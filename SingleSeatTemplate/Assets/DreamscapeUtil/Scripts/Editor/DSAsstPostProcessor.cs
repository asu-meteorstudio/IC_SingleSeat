using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DreamscapeUtil
{
    public class DSAssetPostProcessor : AssetPostprocessor
    {
        
        public void OnPreprocessTexture()
        {
            string p = assetPath.ToLower();
           if(p.EndsWith(".jpg") || p.EndsWith(".jpeg") || p.EndsWith(".psd") || p.EndsWith(".tif") || p.EndsWith(".tiff"))
           {
                int i = p.IndexOf('.');
                string extension = p.Substring(i);

                Debug.LogWarningFormat("Texture file extension is not allowed: {0}", extension);
           }
        }
    }
}
