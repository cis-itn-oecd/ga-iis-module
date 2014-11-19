using System;
using System.Collections;
using System.Linq;
using System.Text;


public sealed class GaQueue
{
    static GaQueue instance = null;
    static readonly object padlock = new Object();
    private Queue gaRequestQueue;

    GaQueue()
    {
        this.gaRequestQueue = new Queue();
    }

    public static GaQueue Instance()
    {
        lock (padlock)
        {
            if (instance == null)
            {
                instance = new GaQueue();
            }
            return instance;
        }
    }

    public int Count()
    {
        return this.gaRequestQueue.Count;
    }

    public void Enqueue(GARequestObject requestObject)
    {
        lock (this.gaRequestQueue.SyncRoot)
        {
            lock (padlock)
            {
                this.gaRequestQueue.Enqueue(requestObject);
            }
        }
    }

    public GARequestObject Dequeue() {
        GARequestObject requestObject = null;
        lock (this.gaRequestQueue.SyncRoot)
        {
            lock (padlock)
            {
                if (this.gaRequestQueue.Count > 0)
                {
                    requestObject = (GARequestObject)this.gaRequestQueue.Dequeue();
                    requestObject.requestCount = this.gaRequestQueue.Count;
                }
            }
        }
        return requestObject;
    }
}

