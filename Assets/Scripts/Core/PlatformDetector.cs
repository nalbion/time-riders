using UnityEngine;

/// <summary>
/// Utility class for detecting platform capabilities at runtime
/// </summary>
public static class PlatformDetector 
{
    private static bool? _isMobilePlatform = null;
    
    /// <summary>
    /// Detect if we're running on a mobile platform at runtime
    /// True for native Android/iOS OR WebGL with touch and tilt sensor support
    /// </summary>
    public static bool IsMobilePlatform 
    {
        get 
        {
            if (_isMobilePlatform == null) 
            {
                _isMobilePlatform = DetectMobilePlatform();
            }
            return _isMobilePlatform.Value;
        }
    }
    
    /// <summary>
    /// Force re-detection of platform (useful for testing)
    /// </summary>
    public static void RefreshPlatformDetection() 
    {
        _isMobilePlatform = null;
    }
    
    private static bool DetectMobilePlatform() 
    {
        // Check for native mobile platforms
        if (Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log($"Mobile platform detected: Native {Application.platform}");
            return true;
        }
        
        // Check for WebGL with mobile capabilities
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Check for touch support
            bool hasTouchSupport = Input.touchSupported;
            
            // Check for accelerometer/gyroscope (tilt sensors)
            bool hasTiltSensors = SystemInfo.supportsAccelerometer || SystemInfo.supportsGyroscope;
            
            if (hasTouchSupport && hasTiltSensors)
            {
                Debug.Log($"Mobile WebGL detected: Touch={hasTouchSupport}, TiltSensors={hasTiltSensors}");
                return true;
            }
            
            Debug.Log($"Desktop WebGL detected: Touch={hasTouchSupport}, TiltSensors={hasTiltSensors}");
            return false;
        }
        
        // Default to desktop for all other platforms
        Debug.Log($"Desktop platform detected: {Application.platform}");
        return false;
    }
    
    /// <summary>
    /// Get detailed platform information for debugging
    /// </summary>
    public static string GetPlatformInfo() 
    {
        return $"Platform: {Application.platform}\n" +
               $"Mobile: {IsMobilePlatform}\n" +
               $"Touch Supported: {Input.touchSupported}\n" +
               $"Accelerometer: {SystemInfo.supportsAccelerometer}\n" +
               $"Gyroscope: {SystemInfo.supportsGyroscope}\n" +
               $"Device Type: {SystemInfo.deviceType}";
    }
    
    /// <summary>
    /// Check if the current platform supports split-screen gaming
    /// </summary>
    public static bool SupportsSplitScreen => !IsMobilePlatform;
    
    /// <summary>
    /// Check if the current platform should use touch controls
    /// </summary>
    public static bool ShouldUseTouchControls => IsMobilePlatform || Input.touchSupported;
    
    /// <summary>
    /// Check if the current platform supports tilt controls
    /// </summary>
    public static bool SupportsTiltControls => SystemInfo.supportsAccelerometer || SystemInfo.supportsGyroscope;
}
