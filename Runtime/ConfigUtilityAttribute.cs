﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTJ.ConfigUtil
{
    public class ConfigUtilityAttribute : System.Attribute
    {
        public string filename { get; private set; }
        public bool visible { get; private set; }
        public string buildFuncName { get; private set; }

        public ConfigUtilityAttribute(string file,bool v = true)
        {
            this.filename = file;
            this.visible = v;
            this.buildFuncName = null;
        }
        public ConfigUtilityAttribute(string file,string buildFunc)
        {
            this.filename = file;
            this.visible = false;
            this.buildFuncName = buildFunc;
        }
    }
}