using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPriority {
    int Priority();
}

public class PriorityQueue<T> where T : IPriority {
    private Heap<T> heap = new Heap<T>(true);

    public PriorityQueue() {
        Reset();
    }

    public void Reset() {
        heap.Reset();
    }

    public void Enqueue(T item) {
        heap.Push(item);
    }

    public bool Dequeue(out T retValue) {
        return heap.Pop(out retValue);
    }

    public bool Top(out T retValue) {
        return heap.Top(out retValue);
    }

    public int Count {
        get { return heap.Count; }
    }

    public override string ToString() {
        return heap.ToString();
    }
}
