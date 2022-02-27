using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://blog.csdn.net/JKerving/article/details/83088832#_27
// 1：一个完全二叉树
//      元素k的父节点所在数组中的位置为[k/2]
//      元素k的子节点所在数组中的位置为2k和2k+1
//      假设堆有n个节点，树的高度为h=floor(logn),因为是完全二叉树
// 下沉： 对于最大堆来说，最大元素位于根节点，那么删除操作就是交换根节点与堆的最后一个节点，然后将交换后的最后一个节点(交换前为根节点，value为最大值)移除并返回元素。此时新根节点需要下沉到适合的位置；
// 上浮： 当插入新元素的时候，将新元素添加至二叉堆的最后一个节点后，此时新节点需要根据value进行上浮操作，直到找到适合的位置。
public class Heap<T> where T : IPriority {
    private IList<T> list = new List<T>();

    private StringBuilder sb = new StringBuilder();
    private bool max = true;

    public int Count {
        get { return list.Count; }
    }

    public Heap(bool max = true) {
        this.Reset();
        this.max = max;
    }

    public Heap<T> Reset() {
        this.max = true;
        this.sb.Clear();
        this.list.Clear();
        return this;
    }

    // 下沉
    private void Down(int index) {
        while (HasLeftChild(index)) {
            int subIndex = index * 2 + 1;
            int leftIndex = subIndex;
            int rightIndex = leftIndex + 1;
            if (max) {
                // 左右节点优先级比较，获取优先级较大者
                if (HasRightChild(index) && list[leftIndex].Priority() < list[rightIndex].Priority()) {
                    ++subIndex;
                }

                if (list[index].Priority() > list[subIndex].Priority()) {
                    break;
                }
            }
            else {
                if (HasRightChild(index) && list[leftIndex].Priority() > list[rightIndex].Priority()) {
                    ++subIndex;
                }

                if (list[index].Priority() < list[subIndex].Priority()) {
                    break;
                }
            }

            Swap(index, subIndex);
            index = subIndex;
        }
    }

    // 上浮
    private void Up(int index) {
        int parentIndex = (index - 1) / 2;
        if (max) {
            while (index > 0 && list[index].Priority() > list[parentIndex].Priority()) {
                parentIndex = (index - 1) / 2;
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }
        else {
            while (index > 0 && list[index].Priority() < list[parentIndex].Priority()) {
                parentIndex = (index - 1) / 2;
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }
    }

    private bool HasChild(int index) {
        return HasLeftChild(index) || HasRightChild(index);
    }

    private bool HasLeftChild(int index) {
        return index * 2 + 1 < Count;
    }

    private bool HasRightChild(int index) {
        return index * 2 + 2 < Count;
    }

    private void Swap(int leftIndex, int rightIndex) {
        (list[leftIndex], list[rightIndex]) = (list[rightIndex], list[leftIndex]);
    }

    public void Push(T item) {
        list.Add(item);
        if (Count >= 2) {
            Up(Count - 1);
        }
    }

    public bool Pop(out T retValue) {
        bool ret = false;
        retValue = default(T);
        int count = Count;
        if (count > 0) {
            ret = true;
            if (count > 1) {
                // 多于一个元素
                Swap(0, count - 1);
                retValue = list[count - 1];
                list.RemoveAt(count - 1);
                Down(0);
            }
            else {
                // 只有一个元素
                retValue = list[0];
                list.Clear();
            }
        }

        return ret;
    }

    public bool Top(out T retValue) {
        bool ret = false;
        retValue = default(T);
        if (Count > 0) {
            ret = true;
            retValue = list[0];
        }

        return ret;
    }

    public override string ToString() {
        sb.Clear();
        foreach (T t in list) {
            sb.AppendFormat("Item:{0} Priority:{1}\n", t, t.Priority());
        }

        sb.AppendLine();
        return sb.ToString();
    }
}
