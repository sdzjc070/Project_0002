using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTest
{
    interface IQueue<T>
    {
        int GetLength();//获取队列的长度
        bool IsEmpty();//是否为空队列
        void Clear();//清空
        void In(T item);//入队
        T Out();//出队
        T GetFrontItem();//取列头元素

    }
}
