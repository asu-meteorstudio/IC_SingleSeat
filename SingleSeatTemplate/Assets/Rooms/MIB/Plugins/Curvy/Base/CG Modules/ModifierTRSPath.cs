// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Modifier/TRS Path", ModuleName="TRS Path", Description = "Transform,Rotate,Scale a Path")]
    [HelpURL(CurvySpline.DOCLINK + "cgtrspath")]
    public class ModifierTRSPath : TRSModuleBase, IOnRequestPath
    {
        [HideInInspector]
        [InputSlotInfo(typeof(CGPath), Name = "Path A", ModifiesData = true)]
        public CGModuleInputSlot InPath = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGPath))]
        public CGModuleOutputSlot OutPath = new CGModuleOutputSlot();

       

        #region ### Public Properties ###

        public float PathLength
        {
            get
            {
                return (IsConfigured) ? InPath.SourceSlot().OnRequestPathModule.PathLength : 0;
            }
        }

        public bool PathIsClosed
        {
            get
            {
                return (IsConfigured) ? InPath.SourceSlot().OnRequestPathModule.PathIsClosed : false;
            }
        }

        #endregion

       
        #region ### IOnRequestProcessing ###

        public CGData[] OnSlotDataRequest(CGModuleInputSlot requestedBy, CGModuleOutputSlot requestedSlot, params CGDataRequestParameter[] requests)
        {
            if (requestedSlot == OutPath)
            {
                var Data = InPath.GetData<CGPath>(requests);

                Matrix4x4 mat = Matrix;
                Matrix4x4 scaleLessMatrix = Matrix4x4.TRS(Transpose, Quaternion.Euler(Rotation), Vector3.one);
                for (int i = 0; i < Data.Count; i++)
                {
                    Data.Position[i] = mat.MultiplyPoint3x4(Data.Position[i]);
                    Data.Normal[i] = scaleLessMatrix.MultiplyVector(Data.Normal[i]);
                }

                Data.Recalculate();
                return new CGData[1] { Data };

            }
            return null;
        }
    }

        #endregion
  


        
}
