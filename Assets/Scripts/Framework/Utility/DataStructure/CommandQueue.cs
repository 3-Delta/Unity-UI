using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Command : IPriority {
    private event Action execAction = delegate { };
    private event Func<Command, bool> canExec = delegate { return true; };

    public Command(Func<Command, bool> canExec, Action execAction) {
        this.canExec = canExec ?? this.canExec;
        this.execAction = execAction ?? this.execAction;
    }

    public bool TryExec(Command lastCommand) {
        bool can = canExec.Invoke(lastCommand);
        if (can) {
            execAction?.Invoke();
        }

        return can;
    }

    public abstract int Priority();
}

// 参考UIScheduler的设计
// 用于处理接收到新任务但是不能立即执行需要等待到合适的时机比如回到主程的时候才去执行
public class CommandQueue {
    private List<Command> queue = new List<Command>();
    private int max = 1;
    private Command lastCommand = null;

    public CommandQueue(int max) {
        this.max = max;
    }

    public bool Enqueue(Command command) {
        return true;
    }

    public bool Dequeue() {
        return true;
    }

    public Command Top() {
        return null;
    }

    public void Exec() {
        if (queue.Count > 0) {
            var top = Top();
            if (top != null && top.TryExec(lastCommand)) {
                lastCommand = top;
                Dequeue();
            }
        }
        else {
            lastCommand = null;
        }
    }
}
