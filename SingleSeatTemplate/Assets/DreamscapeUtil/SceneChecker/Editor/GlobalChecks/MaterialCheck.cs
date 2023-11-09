using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamscapeUtil
{
    [Description("Checks for missing or broken materials")]
    public class MaterialCheck : ISceneCheckerGlobalSceneCheck
    {
        public int CheckForErrors(SceneCheckerContext context)
        {
            int numErrors = 0;

            SceneCheckerIterator it = SceneCheckerIterator.IterContext(context);
            while (it.MoveNext())
            {
                if (it.Current.TryGetComponent<Renderer>(out Renderer rend))
                {
                    if(rend.sharedMaterials.Length == 0)
                    {
                        numErrors++;
                        Debug.LogWarningFormat("Renderer has no materials - {0}", rend.name);
                    }
                    if (rend is ParticleSystemRenderer)
                    {
                        ParticleSystemRenderer psRend = rend as ParticleSystemRenderer;

                        Material mat = psRend.sharedMaterial;
                        //for particle system renderers, it's normal for trail renderer to be null as long as trails module
                        //is not enabled, so need to special case this
                        if (mat == null)
                        {
                            numErrors++;
                            Debug.LogWarningFormat(rend, "Material is null - {0}", rend.name);
                        }
                        else
                        {
                            if (mat.shader == null)
                            {
                                numErrors++;
                                Debug.LogWarningFormat(rend, "Shader on material '{0}' is null - {1}", mat.name, rend.name);
                            }
                            else
                            {
                                if (!mat.shader.isSupported)
                                {
                                    numErrors++;
                                    Debug.LogWarningFormat(rend, "Shader on material '{0}' is not supported - {1}", mat.name, rend.name);
                                }
                            }
                        }

                        if(psRend.trailMaterial == null)
                        {
                            ParticleSystem ps = psRend.GetComponent<ParticleSystem>();
                            if (ps.trails.enabled)
                            {
                                numErrors++;
                                Debug.LogWarningFormat(rend, "Trail material is null - {0}", rend.name);
                            }
                        }
                        else
                        {
                            Material trailMat = psRend.trailMaterial;
                            if (trailMat.shader == null)
                            {
                                numErrors++;
                                Debug.LogWarningFormat(rend, "Shader on material '{0}' is null - {1}", trailMat.name, rend.name);
                            }
                            else
                            {
                                if (!trailMat.shader.isSupported)
                                {
                                    numErrors++;
                                    Debug.LogWarningFormat(rend, "Shader on material '{0}' is not supported - {1}", trailMat.name, rend.name);
                                }
                            }
                        }
                    }
                    else if(rend is BillboardRenderer)
                    {
                        BillboardRenderer bill = (BillboardRenderer)rend;
                        if(bill.billboard == null)
                        {
                            numErrors++;
                            Debug.LogWarningFormat(bill, "Billboard is null - {0}", bill.name);
                        }
                    }
                    else
                    {
                        foreach (Material m in rend.sharedMaterials)
                        {
                            if (m == null)
                            {
                                numErrors++;
                                Debug.LogWarningFormat(rend, "Material is null - {0}", rend.name);
                            }
                            else
                            {
                                if (m.shader == null)
                                {
                                    numErrors++;
                                    Debug.LogWarningFormat(rend, "Shader on material '{0}' is null - {1}", m.name, rend.name);
                                }
                                else
                                {
                                    if (!m.shader.isSupported)
                                    {
                                        numErrors++;
                                        Debug.LogWarningFormat(rend, "Shader on material '{0}' is not supported - {1}", m.name, rend.name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return numErrors;
        }    
    }
}
