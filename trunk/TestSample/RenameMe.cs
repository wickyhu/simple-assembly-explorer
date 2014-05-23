using System;
using System.Collections.Generic;
using System.Text;

namespace TestSample.RenameMeTest
{
    public interface RenameMeInterface1
    {
        string RenameMeProperty { get; set; }
        string RenameMeMethod();
    }

    public interface RenameMeInterface2
    {
        string RenameMeProperty { get; set; }
        string RenameMeMethod();
    }

    public interface RenameMeInterface3
    {
        string RenameMeProperty { get; set; }
        string RenameMeMethod();
    }

    public abstract class RenameMeAbstract1<RenameMeT>
    {
        protected RenameMeT RenameMeField;
        public RenameMeT RenameMeProperty
        {
            get { return RenameMeField; }
        }
        public abstract RenameMeT RenameMeMethod();
        public abstract RenameMeT RenameMeMethod(RenameMeT ga);
    }

    public class RenameMeClass1 : RenameMeInterface1, RenameMeInterface2, RenameMeInterface3, IDisposable
    {
        public static RenameMeClass1 RenameMeInstance = new RenameMeClass1();


        #region IDisposable Members

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region RenameMeInterface1 Members

        public string RenameMeProperty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual string RenameMeMethod()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region RenameMeInterface2 Members, share with RenameMeInterface1

        //string RenameMeInterface2.RenameMeProperty
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //string RenameMeInterface2.RenameMeMethod()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        #region RenameMeInterface3 Members

        string RenameMeInterface3.RenameMeProperty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string RenameMeInterface3.RenameMeMethod()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RenameMeClass2 : RenameMeClass1
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        public override string RenameMeMethod()
        {
            return base.RenameMeMethod();
        }
    }

    public class RenameMeClass3 : RenameMeClass1
    {
        public override void Dispose()
        {
            base.Dispose();
        }

        public override string RenameMeMethod()
        {
            return base.RenameMeMethod();
        }
    }

    public class RenameMeClass4<RenameMeT2> : RenameMeAbstract1<RenameMeClass1>
    {
        public RenameMeT2 RenameMeField2;

        public override RenameMeClass1 RenameMeMethod()
        {
            return this.RenameMeField;
        }

        public override RenameMeClass1 RenameMeMethod(RenameMeClass1 ga)
        {
            return ga;
        }

        public RenameMeT2 RenameMeMethod(int i)
        {
            if (i > 0) return RenameMeField2;
            return default(RenameMeT2);
        }
    }

    public class RenameMeAttribute : Attribute
    {
        private Type RenameMeField;

        public RenameMeAttribute(Type type)
        {
            RenameMeField = type;
        }
    }

    [RenameMe(typeof(RenameMeClass1))]
    public class RenameMeGenericClass1<T> where T : RenameMeClass1
    {
        private RenameMeClass1 RenameMeField;

        public RenameMeClass1 RenameMeProperty
        {
            get { return RenameMeField; }
            set { RenameMeField = value; }
        }

        public RenameMeClass1 RenameMeMethod<T2>(RenameMeClass1 rmcParam)
        {
            RenameMeClass1 rmcVar;
            rmcVar = rmcParam;
            return rmcVar; 
        }        
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false)]
    public sealed class RenameMeAttribute2 : Attribute
    {
        public object RenameMe1;
        public object RenameMe2 { get; set; }
        public object RenameMe3;

        public RenameMeAttribute2() { }
    }

    [RenameMeAttribute2(RenameMe1 = 0.0, RenameMe2 = 100.0, RenameMe3 = 97.0)]
    public class RenameMeAttribute2Class
    {
        [RenameMeAttribute2(RenameMe1 = 0.0, RenameMe2 = 200.0, RenameMe3 = 88.0)]
        public double RenameMeDouble { get; set; }

        [RenameMeAttribute2(RenameMe1 = 0)]
        public uint RenameMeUInt { get; set; }
    }


    public class TestGenericMethod
    {
        public void RenameMeMethod1()
        {
            RenameMeGenericClass1<RenameMeClass2> c = new RenameMeGenericClass1<RenameMeClass2>();
            c.RenameMeMethod<RenameMeClass2>(new RenameMeClass1());
        }

        public void RenameMeMethod2()
        {
            RenameMeGenericClass1<RenameMeClass3> c = new RenameMeGenericClass1<RenameMeClass3>();
            c.RenameMeMethod<RenameMeClass3>(new RenameMeClass1());
        }
    }

}
