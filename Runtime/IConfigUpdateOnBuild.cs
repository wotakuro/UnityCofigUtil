using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.ConfigUtil
{
    public interface IConfigUpdateOnBuild
    {
        void OnPreprocessBuild();
    }
}
