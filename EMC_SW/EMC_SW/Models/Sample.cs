using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMC_SW.Models {
    public class Sample
    {

       ConcurrentQueue<byte[]> q = new ConcurrentQueue<byte[]>();
        public List<byte[]> Q
        {
            get
            {
                return q.ToList();
            }
            set
            {
                q = new ConcurrentQueue<byte[]>(value);
            }
        }

        public int Limit { get; set; }

        public Sample()
        {
        }

        public void Enqueue(byte[] obj)
        {
            q.Enqueue(obj);
            lock (this)
            {
                byte[] overflow;
                while (q.Count > Limit && q.TryDequeue(out overflow)) ;
            }
        }

       
        public byte[] GetMax()
        {
            return q.Max();
        }
        public byte[] GetMin()
        {
            return q.Min();
        }
        public byte[] Get(int index)
        {
            return q.ElementAtOrDefault(index);
        }
        public int CountEntries()
        {
            return q.Count();
        }
    }
}
