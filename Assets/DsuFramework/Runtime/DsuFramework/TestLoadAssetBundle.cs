using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class TestLoadAssetBundle : MonoBehaviour
{
    //string url = "<here the URL for asset bundle>";
    public string bundleUrl = "http://localhost/assetbundles/bundle01";
    public string assetName = "Capsule";
    public int version = 1;

    //// Start is called before the first frame update
    //IEnumerator Start()
    //{
    //    using (WWW web = WWW.LoadFromCacheOrDownload(bundleUrl, version))
    //    {
    //        yield return web;
    //        AssetBundle remoteAssetBundle = web.assetBundle;
    //        if (remoteAssetBundle == null)
    //        {
    //            Debug.LogError("Failed to download AssetBundle!");
    //            yield break;
    //        }
    //        Instantiate(remoteAssetBundle.LoadAsset(assetName));
    //        remoteAssetBundle.Unload(false);
    //    }
    //}

    IEnumerator DownloadAsset()
    {
        /*
         * directly use UnityWebRequestAssetBundle.GetAssetBundle
         * instead of "manually" configure and attach the download handler etc
         */
        using (var uwr = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl, 36, 0))
        {
            var operation = uwr.SendWebRequest();

            /* 
             * this should be done only once actually 
             */
            //progressBar.color = Color.red;

            while (!operation.isDone)
            {
                /* 
                 * as BugFinder metnioned in the comments
                 * what you want to track is uwr.downloadProgress
                 */
                //downloadDataProgress = uwr.downloadProgress * 100;

                /*
                 * use a float division here 
                 * I don't know what type downloadDataProgress is
                 * but if it is an int than you will always get 
                 * an int division <somethingSmallerThan100>/100 = 0
                 */
                //progressBar.fillAmount = downloadDataProgress / 100.0f;
                //print("Download: " + downloadDataProgress);
                yield return null;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
            {
                print("Get asset from bundle...");
            }

            /* 
             * You do not have to Dispose uwr since the using block does this automatically 
             */
            //uwr.Dispose();

            //Load scene
            print("ready to Load scene from asset...");
            //StartCoroutine(LoadSceneProgress("Example"));
            bundle.Unload(false);
        }
    }
}
