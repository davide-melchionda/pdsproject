using System.Threading;

abstract class ExecutableThread {

    public void run() {
        Thread t = new Thread(execute);
        t.Start();
    }

    protected abstract void execute();

}
