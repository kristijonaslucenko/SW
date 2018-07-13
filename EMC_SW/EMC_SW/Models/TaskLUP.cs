using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMC_SW.Models {

    public class TaskLUP
    {
        public byte[] AddedTask { get; set; }
        public int id { get; set; }
        public int Repetition { get; set; }
        public bool IsContinuous { get; set; }
    }

    public class TaskQueue
    {
        //private Task task = new Task();
   
       ConcurrentQueue<TaskLUP> q = new ConcurrentQueue<TaskLUP>();
        public List<TaskLUP> Q
        {
            get
            {
                return q.ToList();
            }
            set
            {
                q = new ConcurrentQueue<TaskLUP>(value);
            }
        }

        public int Limit { get; set; }

        public TaskQueue()
        {
        }

        public void Enqueue(TaskLUP obj)
        {
            q.Enqueue(obj);
            lock (this)
            {
                TaskLUP overflow;
                while (q.Count > Limit && q.TryDequeue(out overflow)) ;
            }
        }
        
        public TaskLUP Get(int index)
        {
            return q.ElementAtOrDefault(index);
        }
        public int CountEntries()
        {
            return q.Count();
        }
    }
}
