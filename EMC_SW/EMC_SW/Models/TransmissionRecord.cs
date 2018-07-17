using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMC_SW.Models {

    public class Record
    {
        public byte[] AddedTask { get; set; }
        public int Id { get; set; }
        public int Repetition { get; set; }
        public bool IsContinuous { get; set; }
        public int Transmitted { get; set; }
        public int Received { get; set; }
        public int Missing { get; set; }
        public int Errors { get; set; }
    }

    public class TransmissionRecord
    {

       ConcurrentQueue<Record> q = new ConcurrentQueue<Record>();
        public List<Record> Q
        {
            get
            {
                return q.ToList();
            }
            set
            {
                q = new ConcurrentQueue<Record>(value);
            }
        }

        public int Limit { get; set; }

        public TransmissionRecord()
        {
        }

        public void Enqueue(Record obj)
        {
            q.Enqueue(obj);
            lock (this)
            {
                Record overflow;
                while (q.Count > Limit && q.TryDequeue(out overflow)) ;
            }
        }

        public void Dequeue()
        {
            Record overflow;
            q.TryDequeue(out overflow);
        }

        public bool Peek(out Record PeekedTask)
        {
            return q.TryPeek(out PeekedTask);
        }

        public Record Get(int index)
        {
            return q.ElementAtOrDefault(index);
        }
        public int CountEntries()
        {
            return q.Count();
        }
    }
}
