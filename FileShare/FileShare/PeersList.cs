using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace HelloProtocol {

    /** *******************************************************************
     * This class represents the table of the peers inside the application.
     * A peer who is known by the application is contained inside this table,
     * which is a map of pairs id_of_the_peer - peer_informations.
     * The class is a Singleton, so that anyone who want to access it must
     * obtaiin the unique instance available through the static method 
     * getInstance().
     ** *******************************************************************/
    internal class PeersList {

        private ConcurrentDictionary<string, PeerEntry> map;

        /** 
         * SINGLETON CREATIONAL PATTERN
         * The unique istance of the PeersList class 
         */
        private static PeersList instance;
        /**
         * SINGLETON CREATIONAL PATTERN
         * The property which represents the unique instance of the class.
         */
        public static PeersList Instance {
            get {
                if (instance == null)
                    instance = new PeersList();
                return instance;
            }
        }

        /**
         * Delegate on which register callback to the event of insertion of a peer.
         */
        public delegate void PeerInsertedDel(Peer inserted);
        public event PeerInsertedDel PeerInserted;

        /**
         * Delegate on which register callback to the event of deletion of a peer.
         */
        public delegate void PeerRemovedDel(Peer removed);
        public event PeerRemovedDel PeerRemoved;


        /**
         * SINGLETON CREATIONAL PATTERN
         * Protected constructor 
         */
        protected PeersList() {
            map = new ConcurrentDictionary<string, PeerEntry>();
        }

        /**
         * Adds a peer inside the list. The id of the peer is used as key inside the 
         * dictionary, the peer is putted inside an instance of PeerEntry and the date
         * associated with the timestamp instance of this entry is "now".
         */
        public void put(Peer p) {
            //map.Add(p.Id, new PeerEntry(p, new DateTime()));
            map.TryAdd(p.Id, new PeerEntry(p, DateTime.Now));
            if (PeerInserted != null)
                PeerInserted(p);
        }

        /**
         * Returns the peer with the id coresponding to the defined key, null if no
         * peer with that id was found in the list.
         */
        public Peer get(string key) {
            PeerEntry p;
            if (map.TryGetValue(key, out p)) {
                return p.Peer;
            }
            return null;
        }

        /**
         * Deletes the entry related to the peer with the specified key from the table.
         */
         public void del(string key) {
            //Peer removed = map[key].Peer;
            PeerEntry removed = null;
            map.TryRemove(key, out removed);
            PeerRemoved?.Invoke(removed.Peer);
        }

        /**
         * Given a key and a DateTime object, updates the timestamp and the peer name of the
         * entry related to the peer associated to that key to the DateTime
         * argument.
         * Throws a PeerNotFoundEception if the peer was not present in the
         * table.
         */
        public void updatePeer(string key, DateTime newTime, string name) {
            PeerEntry p;
            if (map.TryGetValue(key, out p)) {
                p.Timestamp = newTime;
                p.Peer.Name = name;
            } else {
                throw new PeerNotFoundException();
            }
        }


        /**
         * Returns a list of all the peers in the table.
         */
        public List<Peer> Peers {
            get {
                List<Peer> list = new List<Peer>();
                foreach (string s in map.Keys) {
                    list.Add(get(s));
                }
                return list;
            }
        }

        /**
         * A property that represents a list of all the rows of the table
         */
        public List<PeerEntry> Rows {
            get {
                List<PeerEntry> list = new List<PeerEntry>();
                foreach (string s in map.Keys) {
                    PeerEntry p;
                    if (map.TryGetValue(s, out p)) {
                        list.Add(p);
                    }
                    else {
                        throw new PeerNotFoundException();
                    }
                }
                return list;
            }
        }

        /** *******************************************************************************
         * This class representes an entry inside the peer list, so an instance of this
         * class can be considered a row inside the table of the peers. A row contains all
         * the information related to the peer and a timestamp, which represents the time
         * at which the last keepalive packet was received from the peer linked to this 
         * entry.
         ** *******************************************************************************/
        public class PeerEntry {
            /**
             * The peer related to this entry.
             */
            private Peer peer;

            /**
             * Indicates last time a keepalive packet was received
             * from the peer related to this entry.
             */
            private DateTime timestamp;

            /**
             * Properties associated with the instance fields.
             * peer =>  Peer.
             *          The field must not be editable.
             * timestamp => Timestamp.
             *              The field can be edited, but only to
             *              update its value to "now".
             */
             public Peer Peer {
                get {
                    return peer;
                }
            }
            public DateTime Timestamp {
                get {
                    return timestamp;
                }
                set {
                    // The set operation can only set "now"
                    //  as new date.
                    //timestamp = new DateTime();   // DEBUG
                    timestamp = value;
                }
            }
            /**
             * Public constructor.
             */
            public PeerEntry(Peer peer, DateTime timestamp) {
                this.peer = peer;
                this.timestamp = timestamp;
            }
        }
    }
}