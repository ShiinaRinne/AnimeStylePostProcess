using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("AnimeStylePostProcess/FlareAndPara")]
public class FlareAndParaVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter flareInstensity = new(0f,0,1);
    public ClampedFloatParameter flareIteration = new(3.5f,1,5);
    public ColorParameter innerColor = new(new Color(1,1,1,1));
    public ColorParameter outerColor = new(new Color(1,1,1,1));
    public ClampedFloatParameter colorMixedMidPoint = new(0f,-3,3);
    public ClampedFloatParameter colorMixedSoftness = new(1,0,2);
    
    public ClampedFloatParameter flareRange = new(0, 0, 2);
    public ClampedFloatParameter desaturate = new(0, 0, 1);
    public BoolParameter rotateWithMainLight = new(true);
    public ClampedFloatParameter extraRotation = new(0,0,360);
    
    public ClampedFloatParameter paraIntensity = new(0,0,2);
    public ClampedFloatParameter paraRange = new(5,0,5);
    public ClampedFloatParameter paraRotation = new(0,0,360);
    
    // public ObjectParameter<ClampedFloatParameter> mainLightObject = new ObjectParameter<ClampedFloatParameter>(new(0,0,10));
    
    public Light targetLight = null;
    
    
    public bool IsActive() => flareInstensity.value > 0 || paraIntensity.value > 0;
    
    public bool IsTileCompatible() => false;
}
