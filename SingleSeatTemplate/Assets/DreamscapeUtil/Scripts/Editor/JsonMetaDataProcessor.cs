using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

class JsonMetaDataProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public class MetadataJson
    {
        public string guid { get; set; }
        public DateTime build_ended_at { get; set; }
        public string build_machine { get; set; }
        public string unity_version { get; set; }
        public bool is_dev_version { get; set; }
        public List<string> xr_sdk { get; set; }
        //public ulong total_size { get; set; }

    };

    public void OnPostprocessBuild(BuildReport report)
    {
        List<string> sdks = new List<string>();

        /*foreach (XRLoader loader in XRGeneralSettings.Instance.Manager.activeLoaders)
        {
            string[] loader_name = loader.GetType().ToString().Split('.');
            sdks.Add(loader_name[2]);
        }*/

        var metadataJson = new MetadataJson
        {
            guid = report.summary.guid.ToString(),
            build_ended_at = DateTime.SpecifyKind(report.summary.buildEndedAt, DateTimeKind.Utc).ToLocalTime(),
            build_machine = Environment.MachineName,
            unity_version = Application.unityVersion, 
            xr_sdk = sdks,
            //total_size = report.summary.totalSize,
            is_dev_version = false
        };

        if (report.summary.options.HasFlag(BuildOptions.Development))
        {
            metadataJson.is_dev_version = true;
        }

        string jsonString = JsonConvert.SerializeObject(metadataJson);
        string[] output = { jsonString };

        string filePath = report.summary.outputPath;

        filePath = Path.GetDirectoryName(filePath);
        filePath = Path.GetDirectoryName(filePath);

        string jsonFilePath = filePath + "/metadata.json";

        File.WriteAllLines(jsonFilePath, output);

    }
}

