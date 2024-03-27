using System.Runtime.InteropServices;
using UnityEngine;
public class SiteLock : MonoBehaviour
{
    public string[] domains = new string[] {
       "https://www.coolmathgames.com",
       "www.coolmathgames.com",
       "https://edit.coolmathgames.com",
       "edit.coolmathgames.com",
       "www.stage.coolmathgames.com",
       "stage-edit.coolmathgames.com",
       "https://dev.coolmathgames.com",
       "https://www.stage.coolmathgames.com",
       "https://stage-edit.coolmathgames.com",
       "https://dev.coolmathgames.com",
       "https://dev-edit.coolmathgames.com"
   };
    [DllImport("__Internal")]
    private static extern void RedirectTo(string url);
    // Check right away if the domain is valid
    private void Start()
    {
        CheckDomains();
    }
    private void CheckDomains()
    {
        if (!IsValidHost(domains))
        {
            RedirectTo("www.coolmathgames.com");
        }
    }
    private bool IsValidHost(string[] hosts)
    {
        foreach (string host in hosts)
            if (Application.absoluteURL.IndexOf(host) == 0)
                return true;
        return false;
    }
}

