﻿//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        16 October 2007
//Copyright: (C) 2006-2007, Sergey Stoyan
//********************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using Bonjour;

namespace Cliver.CisteraNotification
{
    class Info : Notification
    {
        internal Info(string title, string text, string image_url, string action_name, Action action) : base(title, text, image_url, action_name, action)
        {
        }

        internal override void Show()
        {
            InfoWindow w = InfoWindow.Create(Title, Text, ImageUrl, ActionName, Action);
        }
        InfoWindow w = null;

        internal override void Deleting()
        {
            w?.Close();
            w = null;
        }
    }
}