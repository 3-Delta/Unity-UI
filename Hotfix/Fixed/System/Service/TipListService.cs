using System;
using System.Collections.Generic;

namespace Logic.Hotfix.Fixed
{
    // 登录游戏之后，某些需要在主界面上展示的队列
    // 需要考虑登录之后进入的是战斗，或者cutscene的情况
    public class TipListService : SysBase<TipListService>
    {
        [Serializable]
        public class Tip : IPriority
        {
            public uint id;
            public int priority;

            public Tip(uint id, int priority)
            {
                this.id = id;
                this.priority = priority;
            }

            public int Priority()
            {
                return priority;
            }

            // 点击前往的操作
            public virtual void OnGoTo()
            {

            }
        }

        public readonly PriorityQueue<Tip> queue = new PriorityQueue<Tip>();

        public IList<Tip> Tips
        {
            get
            {
                return queue.heap.list;
            }
        }
    }
}
