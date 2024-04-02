using System;
using UnityEngine;

namespace T2FToolKit
{
    
    internal static class DeviceIDUtil
    {
        
        public readonly static string DeviceID;

        static DeviceIDUtil()
        {
            DeviceID = $"{GetMACAddress()}_{GetAndroidID()}_{GetDeviceID()}_{SystemInfo.deviceUniqueIdentifier}";
        }
        
        /// <summary>
        /// Android设备的MAC地址
        /// 从Android 6.0 (API 级别 23)开始，android.permission.ACCESS_WIFI_STATE 权限并不能保证你能成功获取MAC地址，因为系统会返回一个固定的值
        /// <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
        /// </summary>
        /// <returns></returns>
        private static string GetMACAddress()
        {
            var macAddress = "Unavailable";
            try
            {
#if UNITY_ANDROID
                AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject wifiManager = context.Call<AndroidJavaObject>("getSystemService", "wifi");
                AndroidJavaObject wifiInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo");
                macAddress = wifiInfo.Call<string>("getMacAddress");
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to get MAC_ID: " + e.Message);
            }
            return macAddress;
        }
        
        private static string GetAndroidID()
        {
            var androidID = "Unavailable";
            
            try
            {
#if UNITY_ANDROID
                using (AndroidJavaClass settingsSecure = new AndroidJavaClass("android.provider.Settings$Secure"))
                {
                    using (AndroidJavaObject activityContext = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        androidID = settingsSecure.CallStatic<string>("getString", activityContext.Call<AndroidJavaObject>("getContentResolver"), "android_id");
                    }
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to get ANDROID_ID: " + e.Message);
            }

            return androidID;
        }
        
        
        /// <summary>
        /// 对于DEVICE_ID，从Android 10开始，不再允许访问非重置的设备标识符，如IMEI和MEID。如果你的应用目标是Android 10或更高版本，建议使用其他设备标识符，如ANDROID_ID。
        /// 如果你的应用仍需访问DEVICE_ID且面向的是Android 9（API 级别 28）或更低版本的设备，
        /// 可以使用TelephonyManager.getDeviceId()方法，但这需要READ_PHONE_STATE权限，并且不推荐用于新应用或更新中。
        /// </summary>
        /// <returns></returns>
        private static string GetDeviceID()
        {
            var deviceID = "Unavailable";
            try
            {
#if UNITY_ANDROID 
                using (AndroidJavaObject telephonyManager = new AndroidJavaClass("android.telephony.TelephonyManager"))
                {
                    deviceID = telephonyManager.Call<string>("getDeviceId");
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to get DEVICE_ID: " + e.Message);
            }
            return deviceID;
        }
        
    }
}