using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;

// 날짜 : 2021-04-24 PM 6:02:18
// 작성자 : Rito

namespace Rito.Plugins
{
    public static class ColorDebug
    {
        private const string UnityEditorOnly = "UNITY_EDITOR";
        private static readonly Dictionary<string, string> ColorDIct = new Dictionary<string, string>()
        {
            { "<R>", "<color=Red>" },
            { "<G>", "<color=Green>" },
            { "<B>", "<color=Blue>" },
            { "<W>", "<color=White>" },
            { "<K>", "<color=Black>" },
            { "<C>", "<color=Cyan>" },
            { "<M>", "<color=Magenta>" },
        };
        private const string Close = "</>";
        private const string CloseTag = "</color>";

        [Conditional(UnityEditorOnly)]
        public static void Log(object msg)
        {
            string str = msg.ToString();

            foreach (var pair in ColorDIct)
            {
                str = Regex.Replace(str, pair.Key, pair.Value);
            }

            str = Regex.Replace(str, Close, CloseTag);

            UnityEngine.Debug.Log(str);
        }
    }
}