using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;

public class GoogleAnalyticThread
{

    public String name;
    GaQueue threadGaRequestQueue;

    WebClient client;
    bool sendToGa = true;
    bool logToFile = true;
    string logFile = "C:\\Temp\\";

    private Logger logger = null;

    private DateTime date;

    public GoogleAnalyticThread(String name, String proxy,
        bool sendToGa, bool logToFile, string logFile)
    {
        this.name = name;
        this.threadGaRequestQueue = GaQueue.Instance();
        this.client = new WebClient();
        WebProxy wp = new WebProxy(proxy);
        client.Proxy = wp;
        client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36");

        this.sendToGa = sendToGa;
        this.logToFile = logToFile;
        this.logFile = logFile;

        date = System.DateTime.Now;

        if (logToFile)
        {
            logger = Logger.Instance(logFile, date);
            logger.writeToFile("creating : " + name);
        }
    }

    public void ThreadRun()
    {
        while (true)
        {
            if (this.threadGaRequestQueue.Count() > 0)
            {
                try
                {
                    GARequestObject requestObject = this.threadGaRequestQueue.Dequeue();
                                     
                    if (requestObject != null)
                    {
                        logger.writeToFile("ThreadRun " + requestObject);
                        if (sendToGa)
                        {
                            NameValueCollection collection = new NameValueCollection();
                            collection.Add("v", requestObject.v);
                            collection.Add("tid", requestObject.tid);
                            collection.Add("cid", requestObject.cid);
                            collection.Add("t", requestObject.t);
                            collection.Add("ec", requestObject.ec);
                            collection.Add("ea", requestObject.ea);
                            collection.Add("el", requestObject.el);
                            collection.Add("ev", requestObject.ev);
                            if (requestObject.referrer != null && (requestObject.referrer.Trim().Length > 0))
                                collection.Add("dr", requestObject.referrer);
                            
                            try
                            {
                                this.client.Headers.Add(HttpRequestHeader.UserAgent, requestObject.userAgent);
                                this.client.UploadValues(
                                    new Uri("http://www.google-analytics.com/collect"),
                                    collection
                                );
                                this.client.Dispose();
                            }
                            catch (System.Net.WebException e)
                            {
                                this.logger.writeToFile(e.Message);
                            }
                            collection.Clear();
                            collection = null;
                        }

                        if (logToFile)
                        {
                            this.logger.writeToFile(this.name, requestObject);
                        }
                    }
                }
                catch (Exception e) { }
            }
            else
            {
                Thread.Sleep(500);
            }

        }
    }

}