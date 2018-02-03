using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Represents a thread whom life-cycle is managed in a hierarchical way: each
/// thread is started by a parent, which is responsible of its life-cycle. If the
/// parent dies, he must pass its childs to its parent.
/// The class offers a mechanism to close consecutively all the threads in the
/// hierarchy. Calling the StopThread() method makes the current thread execute
/// the StopThread() method on all of its childs and then wait for their termination.
/// It's a blocking call.
/// </summary>
public abstract class ExecutableThread {

    /// <summary>
    /// The thread associated to this ExecutableThread
    /// </summary>
    private Thread thread;

    private object synch_obj = new object();

    /// <summary>
    /// Is the thread still running?
    /// </summary>
    private bool alive;
    /// <summary>
    /// Property: is the thread still running?
    /// </summary>
    public bool Alive {
        get {
            return alive;
        }
    }

    /// <summary>
    /// The list of the childs of this ExecutableThread.
    /// </summary>
    private List<ExecutableThread> childs = new List<ExecutableThread>();
    /// <summary>
    /// A read-only property for childs field.
    /// </summary>
    public List<ExecutableThread> Childs {
        get {
            return childs;
        }
    }

    /// <summary>
    /// True if the thread must stop itself
    /// </summary>
    private bool stop = false;
    /// <summary>
    /// A protected property for stop field
    /// </summary>
    protected bool Stop {
        get {
            return stop;
        }
        set {
            stop = value;
        }
    }
    /// <summary>
    /// The only way to set the stop field to true.
    /// Now the thread should stop.
    /// It's not possible to turn stop to false again.
    /// </summary>
    public void StopThread() {

        lock (synch_obj) {
            if (Alive) {
                // first of all we must call the PrepareStop so that if the 
                // thread is executing any blocking operation it can manage this
                // (e.g. an ExecutableThread could close the socket on which it's
                // working in order to exit from the blocking Read() with an exception
                PrepareStop();

                // We require to each of our child to stop its execution
                foreach (ExecutableThread c in childs) {
                    // Then we can call the StopThread(). Please note that the thread could 
                    // have been already ended. In this case this operation is useless, but 
                    // we must do it because we don't have any information.
                    c.StopThread();
                 }

                // However the child is not forced to stop immediately,
                // so we must call join in order to wait for its termination.
                // We do this after having asked to all of our child to stop
                for (int i = childs.Count - 1; i >= 0; i--) {// loopping again on our childs
                    childs[i].Join();   // This is our wrapper function
                    childs.Remove(childs[i]); // Only now we can remove it from the list
                }

                // All childs are gone. We can call the relative callback.
                ChildsGone?.Invoke();

                // Now we can set our stop field to 'true' and return to our parent (if any)
                stop = true;
            }
        }
    }

    /// <summary>
    /// The delagate associated to the event of terminating the tread.
    /// </summary>
    public delegate void OnTerminating();
    public event OnTerminating Terminate;

    /// <summary>
    /// The delagate associated to the event of the end of all the childs.
    /// The thread can do here any operation the liveness of its childs 
    /// made impossible to do.
    /// </summary>
    public delegate void OnChildsGone();
    public event OnChildsGone ChildsGone;

    /// <summary>
    /// This method is necessary to make the parent aware of the existence of
    /// a new thread created in its execution flow. If a child is registered 
    /// to this parent it will enter in the execution cycle managed by this class.
    /// We must register a specific callback on Terminate event: if this child
    /// dies we will hinerit all of its childs. In this way, even if a parent 
    /// dies, all of its childs will be passed to its parent ExecutableThread 
    /// so that the hierarchy will not be lost.
    /// </summary>
    /// <param name="child"></param>
    public void RegisterChild(ExecutableThread child) {
        // we must register a child to
        childs.Add(child);

        // NOTE: We must not remove the child from the list in the
        // following Terminate callback: if we do this, we will not find 
        // the child in the childs list anymore.

        // We register a callback on terminating: we must pass receive 
        // all the childs of the registered child if he will die.
        child.Terminate += () => {
            foreach (ExecutableThread c in child.Childs)
                childs.Add(c);
        };
    }

    /// <summary>
    /// Executes the 'execute()' method on a new thread, manages it's 
    /// life-cycle coherently with the logic of this ExecutableThread 
    /// class.
    /// </summary>
    public void run() {
        thread = new Thread(() => {
            lock (synch_obj) {
                alive = true;
            }

            execute();              // execute the logic of the thread
            Terminate?.Invoke();    // execute the operation associated to the end of this thread

            lock (synch_obj) {
                alive = false;          // thread no more alive
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }
    
    /// <summary>
    /// In this method the thread can implement the execution logic.
    /// </summary>
    protected abstract void execute();

    /// <summary>
    /// Is called from the parent in order to require the thread to
    /// stop. It's an emergency mechanism to allow threads involved in 
    /// blocking operation to terminate. It's called on the parent
    /// thread, but it can access to specific fields in the child. Its
    /// logic is defined by the child.
    /// In this way the child can execute any task needed to unlock 
    /// from me locking state.
    /// </summary>
    protected abstract void PrepareStop();

    /// <summary>
    /// Wrapper for the Join() method of the Thread class.
    /// </summary>
    public void Join() {
        if (thread != null /*&& thread.IsAlive*/)
            thread.Join();
    }
}
