﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;

namespace ClearCanvas.ImageViewer.Utilities.Media.View.WinForms
{
   public class ProcessEx
    {
       private static Hashtable processWnd = new Hashtable();
       public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(uint dwErrCode);

        public static IntPtr GetCurrentWindowHandle(uint proid)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            uint uiPid = proid;
            object objWnd = processWnd[uiPid];

            if (objWnd != null)
            {
                ptrWnd = (IntPtr)objWnd;
                if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd))  // 从缓存中获取句柄
                {
                    return ptrWnd;
                }
                else
                {
                    ptrWnd = IntPtr.Zero;
                }
            }

            bool bResult = EnumWindows(new WNDENUMPROC(EnumWindowsProc), uiPid);
            // 枚举窗口返回 false 并且没有错误号时表明获取成功
            if (!bResult && Marshal.GetLastWin32Error() == 0)
            {
                objWnd = processWnd[uiPid];
                if (objWnd != null)
                {
                    ptrWnd = (IntPtr)objWnd;
                }
            }

            return ptrWnd;
        }

        private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
        {
            uint uiPid = 0;

            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref uiPid);
                if (uiPid == lParam)    // 找到进程对应的主窗口句柄
                {
                    processWnd.Add(uiPid, hwnd);   // 把句柄缓存起来
                    SetLastError(0);    // 设置无错误
                    return false;   // 返回 false 以终止枚举窗口
                }
            }

            return true;
        }

       //IntPtr hwnd = IntPtr.Zero;
       //     Process[] pros = Process.GetProcesses();
       //     foreach (Process pro in pros)
       //     {
       //         if(pro.ProcessName.Equals("NRCServer"))
       //         {
                                           
       //             hwnd = GetCurrentWindowHandle(Convert.ToUInt32(pro.Id));
                     
       //         }
 
       //     }
    }
}
