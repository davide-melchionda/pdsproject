using System;

/**
 * Offers a way to access to a file on a storage through a managed interface that can 
 * introduce addictional functionalities such as compression, caching, pooling and so on.
 * This interface decuples the one who access to the file and the storage system. The 
 * purpose is to offer an abstraction that completely hides the storage system details
 * and in which can be built powerful facilieties focused on improving performance.
 * The abstraction is offered through the method getFileIterator(), which returns an
 * Iterator. The Iterator the object wrapping the access to the file, ad can be obtained 
 * only through the linked MemoryModule.
 */
public abstract class StorageModule {

    /**
     * Given a path to a file, returns a FileIterator object that allows 
     * to access to that file passing to the abstraction of this storage
     * module.
     */
    public abstract FileIterator getIterator(string path);

    /**
     * Defines the behaviour this StorageModule must assume when one of
     * the iterators he released is closed.
     * When closed correctly (see FileIterator.close() method), the FileIterator
     * will call autonomously this method.
     */
    protected abstract void onIteratorClosed(FileIterator iterator);

    /**
     * Represents the point of access to a specific file. To obtain a FileIterator which
     * points to a file, it's necessary to pass through the getIterator() method of the
     * StorageModule class.
     * The FileIterator class offers a method to register a callback to execute when the
     * Iterator is closed. The StorageModule registers a callback to manage the correct 
     * closing of the Iterator.
     */
    public abstract class FileIterator
    {
        /**
         * The storage module which is responsible for this FileIterator
         */
        private StorageModule storage;

        /**
         * Constructor.
         */
        protected FileIterator(StorageModule storage) {
            this.storage = storage;
        }

        /**
         * Puts in the buffer of byte given as parameter the next block
         * readed from the file. Returns the number of read byte.
         */
        public abstract int next(byte[] buffer);

        /**
         * Returns 'true' if there is still something to read, 'false' otherwise.
         */
        public abstract bool hasNext();

        /**
         * This method must be called when the owner of the file iterator
         * needs no more to use it.
         */
        public virtual void close() {
            storage.onIteratorClosed(this);
        }
    }

}