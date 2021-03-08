using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rito.Demo
{
    public class Test_WRandomPick : MonoBehaviour
    {
        public bool _flag;
        private void OnValidate()
        {
            if (_flag)
            {
                _flag = false;
                Test();
            }
        }

        void Test()
        {
            var wrPicker = new Rito.WeightedRandomPicker<string>();

            // 아이템 및 가중치 목록 전달
            wrPicker.Add(

                ("냥냥이A", 332),
                ("냥냥이B", 332),
                ("냥냥이C", 332),

                ("멍멍이A", 1200),
                ("멍멍이B", 1200),
                ("멍멍이C", 1200),
                ("멍멍이D", 1200),
                ("멍멍이E", 1200),

                ("삐약이A", 1502),
                ("삐약이B", 1502)
            );

            for (int i = 0; i < 10000; i++)
            {
                Debug.Log(wrPicker.GetRandomPick());
            }

            Debug.Log("");
            foreach (var item in wrPicker.GetItemDictReadonly())
            {
                Debug.Log(item);
            }

            Debug.Log("");
            foreach (var item in wrPicker.GetNormalizedItemDictReadonly())
            {
                Debug.Log(item);
            }
        }
    }
}