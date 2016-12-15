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
using System.Net;
using System.Text.RegularExpressions;

namespace Cliver
{
    class HttpService
    {
        static HttpService()
        {
        }

        static public void Start()
        {
            if (t != null)
                return;
            t = ThreadRoutines.StartTry(() => { run(Properties.Settings.Default.ServicePort); });
        }
        static Thread t = null;

        static void run(int port)
        {
            try
            {
                if (listener != null)
                {
                    listener.Stop();
                    listener.Close();
                }
                listener = new HttpListener();
                //listener.Prefixes.Add("http://localhost:" + port + "/");
                listener.Prefixes.Add("http://*:" + port + "/");
                listener.Start();
                while (listener.IsListening)
                {
                    HttpListenerContext context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem((o) => { request_handler(context); });
                }
            }
            catch (ThreadAbortException)
            { }
            catch (Exception e)
            {
                Message.Error("Http Service broken: " + e.Message);
                Application.Exit();
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                    listener.Close();
                    listener = null;
                }
            }
        }
        static HttpListener listener = null;

        static public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
                listener.Close();
                listener = null;
            }
            if (t != null && t.IsAlive)
                t.Abort();
            t = null;
        }

        static private void request_handler(object _context)
        {
            HttpListenerContext context = (HttpListenerContext)_context;
            HttpListenerResponse response = context.Response;
            try
            {
                if (!Regex.IsMatch(context.Request.ContentType, @"", RegexOptions.IgnoreCase | RegexOptions.Singleline))
                {
                    response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                    return;
                }
                response.StatusCode = 200;
                string data = CiscoHttp.ProcessRequest(context.Request);
                byte[] buffer = response.ContentEncoding.GetBytes(data);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            finally
            {
                response.Close();
            }
        }
    }
}