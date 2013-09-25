using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ClearCanvas.ImageViewer.Utilities.Media.View.WinForms
{
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class DevicesChangedMessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WndProMsgConst.WM_DEVICECHANGE)
            {
                DEV_BROADCAST_HDR lpdb = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                switch (m.WParam.ToInt32())
                {
                    case WndProMsgConst.DBT_DEVICEARRIVAL:
                        if (lpdb.dbch_devicetype == WndProMsgConst.DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME lpdbv = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                            int a = lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA;
                            if ((lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA) == WndProMsgConst.DBTF_MEDIA)
                            {
                                System.Windows.Forms.MessageBox.Show(DriveMaskToLetter(lpdbv.dbcv_unitmask).ToString());
                            }
                        }
                        break;
                    case WndProMsgConst.DBT_DEVICEREMOVECOMPLETE:
                        if (lpdb.dbch_devicetype == WndProMsgConst.DBT_DEVTYP_VOLUME)
                        {
                            DEV_BROADCAST_VOLUME lpdbv = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                            int a = lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA;
                            if ((lpdbv.dbcv_flags & WndProMsgConst.DBTF_MEDIA) == WndProMsgConst.DBTF_MEDIA)
                            {
                                System.Windows.Forms.MessageBox.Show(DriveMaskToLetter(lpdbv.dbcv_unitmask).ToString());
                            }
                        }
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets drive letter from a bit mask where bit 0 = A, bit 1 = B etc.
        /// There can actually be more than one drive in the mask but we 
        /// just use the last one in this case.
        /// </summary>       
        private static char DriveMaskToLetter(int mask)
        {
            char letter;
            string drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            // 1 = A, 2 = B, 4 = C...
            int cnt = 0;
            int pom = mask / 2;
            while (pom != 0)
            {
                // while there is any bit set in the mask, shift it to the right... 
                pom = pom / 2;
                cnt++;
            }
            if (cnt < drives.Length)
                letter = drives[cnt];
            else
                letter = '?';
            return letter;

        }

    }


}
