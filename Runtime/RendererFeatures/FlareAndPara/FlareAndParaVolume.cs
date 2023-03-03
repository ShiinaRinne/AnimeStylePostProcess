using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("AnimeStylePostProcess/FlareAndPara")]
public class FlareAndParaVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter flareInstensity = new(0.3f,0,1);
    public ClampedFloatParameter flareInteration = new(3.5f,1,5);
    public ColorParameter innerColor = new(new Color(0.6f,0.8f,0.5f,1));
    public ColorParameter outerColor = new(new Color(0.7f,0.3f,0.5f,1));
    public ClampedFloatParameter colorMixedMidPoint = new(-1.2f,-2f,2f);
    public ClampedFloatParameter colorMixedSoftness = new(0.8f,0,2);
    
    public ClampedFloatParameter flareRange = new(1.5f, 0f, 2f);
    public BoolParameter rotateWithMainLight = new(true);
    public ClampedFloatParameter extraRotation = new(0,0,360);
    public Vector2Parameter controlPoint = new(new Vector2(0.5f, 0.5f));
    public Vector2Parameter startPosition = new(new Vector2(0.5f, 0.5f));
    public Vector2Parameter endPosition = new(new Vector2(0.5f, 0.5f));
    
    
    public ClampedFloatParameter paraIntensity = new(1,0,2);
    public ClampedFloatParameter paraRange = new(5,0,5);
    public ClampedFloatParameter paraRotation = new(0,0,360);
    
    // public ObjectParameter<ClampedFloatParameter> mainLightObject = new ObjectParameter<ClampedFloatParameter>(new(0,0,10));
    
    public Light targetLight = null;
    
    
    public bool IsActive() => flareInstensity.value > 0 || paraIntensity.value > 0;
    
    public bool IsTileCompatible() => false;
}
