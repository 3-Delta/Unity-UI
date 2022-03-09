using System.Collections;
using System.Collections.Generic;
using System;

public class NWQueue
{
    private Queue<NWPackage> packageQueue = new Queue<NWPackage>();

    public int Count => packageQueue.Count;

    public NWQueue() { Clear(); }
    public void Clear() { packageQueue.Clear(); }
    public void Enqueue(NWPackage package)
    {
        lock (packageQueue) { packageQueue.Enqueue(package); }
    }
    public bool Dequeue(ref NWPackage package)
    {
        lock (packageQueue)
        {
            if (packageQueue.Count > 0)
            {
                package = packageQueue.Dequeue();
                return true;
            }
            else { return false; }
        }
    }
}
