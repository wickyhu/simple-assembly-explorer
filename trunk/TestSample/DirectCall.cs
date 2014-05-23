using System;
using System.Collections.Generic;
using System.Text;

namespace TestSample.DirectCallTest
{    
    public class DirectCallTestClass
    {
        public class DirectCallSubTestClass
        {
            public static void StaticCall()
            {
            }

            public void InstanceCall()
            {
            }
        }

        public static void StaticCall()
        {
        }

        public void InstanceCall()
        {
        }

        public void DirectCall01()
        {
            StaticCall();
        }

        public void DirectCall02()
        {
            InstanceCall();
        }

        public void DirectCall10()
        {
            DirectCallSubTestClass.StaticCall();
        }

        public void DirectCall11()
        {
            new DirectCallSubTestClass().InstanceCall();
        }

        public void TestMethod()
        {
            DirectCall01();
            DirectCall02();
            
            DirectCall10();
            DirectCall11();
        }

        public void TestMethodCompare()
        {
            StaticCall();
            InstanceCall();

            DirectCallSubTestClass.StaticCall();
            DirectCall11();
        }

    }
}