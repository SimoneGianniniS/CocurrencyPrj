using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CocorrenzaPrj
{
    class Program
    {
        private static Mutex mut = new Mutex();
        private const int numIterations = 2;
        private const int numThreads = 3;
        static void Main(string[] args)
        {

            //    var pippo = new ThreadExample();

            //    Thread newThread = new Thread(new ThreadStart(pippo.Producer));
            //    newThread.Start();
            //    //Thread newThread1 = new Thread(new ThreadStart(pippo.Producer));
            //    //newThread1.Start();
            //    //Thread newThread2 = new Thread(new ThreadStart(pippo.Producer));
            //    //newThread2.Start();
            //Thread newThread3 = new Thread(new ThreadStart(pippo.Consumer));
            //    newThread3.Start();
            //    //Thread newThread4 = new Thread(new ThreadStart(pippo.Consumer));
            //    //newThread4.Start();




            BufferMulti array = new BufferMulti();
            Buffer2thread array2 = new Buffer2thread();
            ProducerExamp prodM = new ProducerExamp(array, 10000);
            var producerThreadMulti = prodM.CreateProducerThreadMulti();
            producerThreadMulti.Name = "PRODUCERThreadMulti";
            var producerThreadMulti2 = prodM.CreateProducerThreadMulti();
            producerThreadMulti2.Name = "PRODUCERThreadMulti2";
            ConsumerExamp consM = new ConsumerExamp(array, 10000);
            var consumerThreadMulti = consM.CreateConsumerThreadMulti();
            var consumerThreadMulti2 = consM.CreateConsumerThreadMulti();
            consumerThreadMulti.Name = "consumerThreadMulti";
            consumerThreadMulti2.Name = "consumerThreadMulti2";

            ProducerExamp prodtwo = new ProducerExamp(array2, 10000);
            var producerThreadtwot = prodtwo.CreateProducerThreadMAX2thread();
            producerThreadtwot.Name = "PRODUCERThreadtwot";
            ConsumerExamp constwo = new ConsumerExamp(array2, 10000);
            var consumerThreadtwot = constwo.CreateConsumerThreadMAX2thread();
            consumerThreadtwot.Name = "consumerThreadtwot";

            producerThreadtwot.Start();
            consumerThreadtwot.Start();

            producerThreadMulti.Start();
            producerThreadMulti2.Start();
            consumerThreadMulti.Start();
            consumerThreadMulti2.Start();

            producerThreadtwot.Join();
            consumerThreadtwot.Join();

            producerThreadMulti.Join();
            producerThreadMulti2.Join();
            consumerThreadMulti.Join();
            consumerThreadMulti2.Join();
        }
    }

    public class BufferMulti
    {
        public int[] _array = new int[10];
        private int _iterations = 0;
        private static Mutex mut = new Mutex();

        public void WriteDataMulti(int val)
        {
            lock (this)
            {
                if (_array[_array.Length - 1] != 0)
                {
                    Console.WriteLine("{0} prepare for wait", Thread.CurrentThread.Name);
                    Monitor.Wait(this);
                }
                else
                {
                    _array[_iterations] = val;
                    Console.WriteLine("{0} insert position: {1}\tValue:  {2}\t thread status {3}", Thread.CurrentThread.Name, _iterations.ToString(), val.ToString(), Thread.CurrentThread.ThreadState);
                    _iterations++;
                    Monitor.Pulse(this);
                }

            }
        }

        public void ReadDataMulti(out int iterations, out int val)
        {

            lock (this)
            {
                if (_array.FirstOrDefault(x => x != 0) == 0)
                {
                    Console.WriteLine("{0} prepare for wait", Thread.CurrentThread.Name);
                    Monitor.Wait(this);
                    val = 0;
                    iterations = 0;
                }
                else
                {
                    _iterations--; //decrementa per leggere
                    //Console.WriteLine("{0} reading", Thread.CurrentThread.Name);
                    val = _array[_iterations];
                    _array[_iterations] = 0;
                    Console.WriteLine("{0} readed and deleted", Thread.CurrentThread.Name);
                    iterations = _iterations;
                    Monitor.Pulse(this);
                }
            }
        }
    }

    public class Buffer2thread
    {
        private int[] _array2 = new int[10];
        private int _iterations2 = 0;


        public void WriteData2thread(int val)
        {
            lock (this)
            {
                if (_array2[_array2.Length - 1] != 0)
                {
                    Console.WriteLine("***{0} prepare for wait", Thread.CurrentThread.Name);
                    Monitor.Wait(this);
                }

                _array2[_iterations2] = val;
                Console.WriteLine("***{0} insert position: {1}\tValue:  {2}\t thread status {3}", Thread.CurrentThread.Name, _iterations2.ToString(), val.ToString(), Thread.CurrentThread.ThreadState);
                _iterations2++;
                Monitor.Pulse(this);
            }
        }


        public void ReadData2thread(out int iterations, out int val)
        {

            lock (this)
            {
                if (_array2.FirstOrDefault(x => x != 0) == 0)
                {
                    Console.WriteLine("***{0} prepare for wait", Thread.CurrentThread.Name);
                    Monitor.Wait(this);
                }

                _iterations2--; //decrementa per leggere
                //Console.WriteLine("***{0} reading", Thread.CurrentThread.Name);
                val = _array2[_iterations2];
                _array2[_iterations2] = 0;
                Console.WriteLine("***{0} readed and deleted", Thread.CurrentThread.Name);
                iterations = _iterations2;
                Monitor.Pulse(this);
            }
        }
    }

    class ProducerExamp
    {
        private BufferMulti _array;
        private Buffer2thread _array2;
        private int TotalIters;

        Random rdm = new Random();

        public ProducerExamp(BufferMulti array, int iterations)
        {
            _array = array;
            TotalIters = iterations;
        }
        public ProducerExamp(Buffer2thread array, int iterations)
        {
            _array2 = array;
            TotalIters = iterations;
        }

        public Thread CreateProducerThreadMulti()
        {
            return new Thread(new ThreadStart(this.MultiInsert));
        }
        public Thread CreateProducerThreadMAX2thread()
        {
            return new Thread(new ThreadStart(this.twothreadInsert));
        }
        private void MultiInsert()
        {
            int iters = 0;
            do
            {
                _array.WriteDataMulti(rdm.Next(1, 100));
                iters++;
            } while (iters < TotalIters);
        }

        private void twothreadInsert()
        {
            int iters = 0;
            do
            {
                _array2.WriteData2thread(rdm.Next(1, 10));
                iters++;
            } while (iters < TotalIters);
        }
    }

    class ConsumerExamp
    {
        private BufferMulti _array;
        private Buffer2thread _array2;
        private int TotalIters;

        public ConsumerExamp(BufferMulti array, int iterations)
        {
            _array = array;
            TotalIters = iterations;
        }
        public ConsumerExamp(Buffer2thread array, int iterations)
        {
            _array2 = array;
            TotalIters = iterations;
        }

        public Thread CreateConsumerThreadMulti()
        {
            return new Thread(new ThreadStart(this.printValuesMulti));
        }
        public Thread CreateConsumerThreadMAX2thread()
        {
            return new Thread(new ThreadStart(this.printValues2thread));
        }

        private void printValuesMulti()
        {
            int position = new int();
            int value = new int();
            int iters = 0;
            do
            {
                _array.ReadDataMulti(out position, out value);
                if (value != 0)
                {
                    System.Console.WriteLine("{0} read position: {1}\tValue:  {2}", Thread.CurrentThread.Name, position.ToString(), value.ToString());
                }
                iters++;
            } while (iters < TotalIters);//da lavorare ancora su questa condizione, attualmente lascia i thread in sospeso
        }
        private void printValues2thread()
        {
            int position = new int();
            int value = new int();
            int iters = 0;
            do
            {
                _array2.ReadData2thread(out position, out value);
                if (value != 0)
                {
                    System.Console.WriteLine("***{0} read position: {1}\tValue:  {2}", Thread.CurrentThread.Name, position.ToString(), value.ToString());
                }
                iters++;
            } while (iters < TotalIters);
        }
    }































    //public class ThreadExample
    //{
    //    public static int BUFFER_SIZE = 10;
    //    public static int[] array = new int[BUFFER_SIZE];
    //    private Random random = new Random();
    //    private int itemCount;
    //    public void Producer()
    //    {
    //        while (true)
    //        {
    //            var item = produceItem();


    //            if (itemCount == BUFFER_SIZE)
    //            {
    //                Thread.Sleep(Timeout.Infinite);
    //                Console.WriteLine("Producer thread sleep");
    //            }

    //            putItemIntoBuffer(item);
    //            Console.WriteLine("Producer thread put {0}", item);
    //            itemCount++;


    //            if (itemCount == 1)
    //            {
    //                throw new ThreadInterruptedException();
    //            }
    //        }
    //    }

    //    public void putItemIntoBuffer(int item)
    //    {
    //        array[itemCount] = item;
    //    }

    //    private int produceItem()
    //    {
    //        return random.Next(BUFFER_SIZE);
    //    }

    //    public void Consumer()
    //    {
    //        while (true)
    //        {

    //            if (itemCount == 0)
    //            {
    //                Thread.Sleep(Timeout.Infinite);

    //            }

    //            var item = removeItemFromBuffer();
    //            itemCount--;


    //            if (itemCount == BUFFER_SIZE - 1)
    //            {
    //                throw new ThreadInterruptedException();
    //            }

    //            Console.WriteLine("Consumer thread take {0}", item);
    //        }
    //    }

    //    private int removeItemFromBuffer()
    //    {
    //        return array[itemCount];
    //    }
    //}
}
