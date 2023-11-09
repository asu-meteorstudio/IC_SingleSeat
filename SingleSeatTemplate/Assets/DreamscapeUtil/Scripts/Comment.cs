using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamscapeUtil{

    public class Comment : MonoBehaviour
    {
        [TextArea(3, 100)]
        public string commentText = "";
    }
}
