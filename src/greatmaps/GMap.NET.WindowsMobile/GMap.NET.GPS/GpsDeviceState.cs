//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this sample source code is subject to the terms of the Microsoft
// license agreement under which you licensed this sample source code. If
// you did not accept the terms of the license agreement, you are not
// authorized to use this sample source code. For the terms of the license,
// please see the license agreement between you and Microsoft or, if applicable,
// see the LICENSE.RTF on your install media or the root of your tools installation.
// THE SAMPLE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//
#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace GMap.NET.GPS
{
   public enum GpsServiceState : int
   {
      Off=0,
      On=1,
      StartingUp=2,
      ShuttingDown=3,
      Unloading=4,
      Uninitialized=5,
      Unknown=-1
   }

   [StructLayout(LayoutKind.Sequential)]
   internal struct FileTime
   {
      int dwLowDateTime;
      int dwHighDateTime;
   }

   /// <summary>
   /// GpsDeviceState holds the state of the gps device and the friendly name if the 
   /// gps supports them.
   /// </summary>
   [StructLayout(LayoutKind.Sequential)]
   public class GpsDeviceState
   {
      public static int GpsMaxFriendlyName = 64;
      public static int GpsDeviceStructureSize = 216;

      int serviceState = 0;
      /// <summary>
      /// State of the GPS Intermediate Driver service
      /// </summary>
      public GpsServiceState ServiceState
      {
         get
         {
            return (GpsServiceState) serviceState;
         }
      }

      int deviceState = 0;
      /// <summary>
      /// Status of the actual GPS device driver.
      /// </summary>
      public GpsServiceState DeviceState
      {
         get
         {
            return (GpsServiceState) deviceState;
         }
      }

      string friendlyName = string.Empty;
      /// <summary>
      /// Friendly name of the real GPS device we are currently using.
      /// </summary>
      public string FriendlyName
      {
         get
         {
            return friendlyName;
         }
      }

      /// <summary>
      /// Constructor of GpsDeviceState.  It copies values from the native pointer 
      /// passed in. 
      /// </summary>
      /// <param name="pGpsDevice">Native pointer to memory that contains
      /// the GPS_DEVICE data</param>
      public GpsDeviceState(IntPtr pGpsDevice)
      {
         // make sure our pointer is valid
         if(pGpsDevice == IntPtr.Zero)
         {
            throw new ArgumentException();
         }

         // read in the service state which starts at offset 8
         serviceState = Marshal.ReadInt32(pGpsDevice, 8);
         // read in the device state which starts at offset 12
         deviceState = Marshal.ReadInt32(pGpsDevice, 12);

         // the friendly name starts at offset 88
         IntPtr pFriendlyName = (IntPtr) (pGpsDevice.ToInt32() + 88);
         // marshal the native string into our gpsFriendlyName
         friendlyName = Marshal.PtrToStringUni(pFriendlyName);
      }
   }
}
