using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil
{

    public class BitMaskAttribute : PropertyAttribute
    {
        public System.Type propType;
        public BitMaskAttribute(System.Type aType)
        {
            propType = aType;
        }
    }
}
