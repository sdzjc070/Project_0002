using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest
{
    class CircularQueue<T> : IQueue<T>
    {
        #region 成员变量
        private int maxsize; //循环顺序队列的容量
        private T[] data; //数组，用于存储循环顺序队列中的数据元素
        private int front; //指示循环顺序队列的队头
        private int rear; //指示循环顺序队列的队尾

        #endregion
        #region 索引器
        public T this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }
        #endregion
        #region 成员属性
        //容量属性
        public int Maxsize
        {
            get
            {
                return maxsize;
            }
            set
            {
                maxsize = value;
            }
        }
        //队头属性
        public int Front
        {
            get
            {
                return front;
            }
            set
            {
                front = value;
            }
        }
        //队尾属性
        public int Rear
        {
            get
            {
                return rear;
            }
            set
            {
                rear = value;
            }
        }
        #endregion
        #region 构造方法
        public CircularQueue(int size)
        {
            data = new T[size];
            maxsize = size;
            front = rear = -1;
        }
        #endregion
        #region 成员方法
        //求循环顺序队列的长度
        public int GetLength()
        {
            return (rear - front + maxsize) % maxsize;
        }
        //清空循环顺序队列：rear和front均等于-1。
        public void Clear()
        {
            front = rear = -1;
        }
        //判断循环顺序队列是否为空
        public bool IsEmpty()
        {
            if (front == rear)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //判断循环顺序队列是否为满:(rear + 1) % maxsize==front
        public bool IsFull()
        {
            if ((rear + 1) % maxsize == front)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //入队：先使循环顺序队列的rear加1，front不变，然后在rear指示的位置添加一个新元素。
        public void In(T item)
        {
            if (IsFull())
            {
                Console.WriteLine("Queue is full");
                return;
            }
            data[++rear] = item;
        }
        //出队：使队头指示器front加1，rear不变
        public T Out()
        {
            T tmp = default(T);
            //判断循环顺序队列是否为空
            if (IsEmpty())
            {
                Console.WriteLine("Queue is empty");
                return tmp;
            }
            tmp = data[++front];
            return tmp;
        }
        //获取队头数据元素
        public T GetFrontItem()
        {
            if (IsEmpty())
            {
                Console.WriteLine("Queue is empty!");
                return default(T);
            }
            return data[front + 1];
        }
        //遍历：
        public void showQueue()
        {
            while (front != rear)
            {
                Console.Write(data[front + 1] + "\t");
                front++;
            }
        }
        #endregion
    }
}
