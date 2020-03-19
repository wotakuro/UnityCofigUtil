# UnityCofigUtil
Simple Configuration Tool

## enviroment
2019.3 ( exclude WebGL.because this depends on StreamingAssets....)

## define config data
```
using UTJ.ConfigUtil;

/** arg0 : Config name*/
[ConfigUtility("server")]
public class ConfigServer
{
    public string url;
    public ClassData innerClass;
}

[System.Serializable]
public class ClassData
{
    public string name;
    public int val;
}
```
## edit config window
Select Tools->Config

![ConfigWindow](Documentation~/img/ConfigWindow.png)
<br />
1.Select config type<br/>
2.Also you can save the data as preset.<br />
3.Edit data area<br />
4.Save to StreamingAssetsPath. <br/>


## How to load
```
var config = UTJ.ConfigUtil.ConfigLoader.LoadData<ConfigServer>();
```


## Auto Generate Config on preprocess build

If you implements "IConfigUpdateOnBuild" to the config class, config data will be generated before build.


### Example (git info)

```
using UTJ.ConfigUtil;

/** arg0 : Config name
  * arg1 : Visible flag from config window
  */
[ConfigUtility("server", false)]
public class BuildInfo : IConfigUpdateOnBuild
{
    public string buildDate;
    public string branch;
    public string hash;
    public string shortHash;
    public string lastUpdate;

    public void OnPreprocessBuild()
    {
        this.buildDate = System.DateTime.Now.ToString();
        GetGitInfo(out this.branch, "rev-parse --abbrev-ref @");
        string lastCommitInfo = null;
        if (GetGitInfo(out lastCommitInfo, "log -n 1 --format=%H,%h,%cd --date=format:%Y%m%d%H%M%S"))
        {
            var results = lastCommitInfo.Split(',');
            if (results.Length > 2)
            {
                this.hash = results[0];
                this.shortHash = results[1];
                this.lastUpdate = results[2];
            }
        }
    }

    private static bool GetGitInfo(out string res, string arg)
    {
        System.Diagnostics.Process pro = new System.Diagnostics.Process();
        pro.StartInfo.FileName = "git";
        pro.StartInfo.Arguments = arg;
        pro.StartInfo.CreateNoWindow = true;
        pro.StartInfo.UseShellExecute = false;
        pro.StartInfo.RedirectStandardOutput = true;
        pro.StartInfo.RedirectStandardError = true;
        pro.StartInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
        pro.Start();
        string err = pro.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(err))
        {
            res = err;
            return false;
        }
        res = pro.StandardOutput.ReadToEnd().Trim();
        return true;
    }
}
```