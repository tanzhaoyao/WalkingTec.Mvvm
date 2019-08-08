using System;
using System.Collections.Generic;
using System.Linq;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// ���νṹModel����չ����
    /// </summary>
    public static class ITreeDataExtension
    {

        /// <summary>
        /// ��ȡһ�����ڵ��µ������ӽڵ㣬�����ӽڵ���ӽڵ�
        /// </summary>
        /// <typeparam name="T">���νṹ��</typeparam>
        /// <param name="self">���νṹʵ��</param>
        /// <param name="order">�����ֶΣ���Ϊ��</param>
        /// <returns>���νṹ�б����������ӽڵ�</returns>
        public static List<T> GetAllChildren<T>(this T self, Func<T, object> order = null)
            where T : TopBasePoco, ITreeData<T>
        {
            List<T> rv = new List<T>();
            var children = self.Children;
            if(order != null && children != null)
            {
                children = children.OrderBy(order).ToList();
            }
            if (children != null && children.Count() > 0)
            {
                var dictinct = children.Where(x => x.ID != self.ID).ToList();
                foreach (var item in dictinct)
                {
                    rv.Add(item);
                    //�ݹ�����ӽڵ���ӽڵ�
                    rv.AddRange(item.GetAllChildren(order));
                }
            }
            return rv;
        }

        public static int GetLevel<T>(this T self)
            where T : TopBasePoco, ITreeData<T>
        {
            int level = 0;
            while (self.Parent != null)
            {
                level++;
                self = self.Parent;
            }
            return level;
        }

        /// <summary>
        /// ��ѯ���ݿ⣬����ĳ���ڵ�ID�ݹ��ȡ�������м�����ӽڵ�ID
        /// </summary>
        /// <typeparam name="T">���νṹ��</typeparam>
        /// <param name="self">���νṹʵ��</param>
        /// <param name="dc">dc</param>
        /// <param name="subids">�ӽڵ�ID�б�</param>
        /// <returns>���м����ӽڵ�ID</returns>
        public static List<Guid> GetAllChildrenIDs<T>(this T self
            , IDataContext dc
            , List<Guid> subids = null)
            where T : TopBasePoco, ITreeData<T>
        {
            List<Guid> rv = new List<Guid>();
            List<Guid> ids = null;
            if (subids == null)
            {
                ids = dc.Set<T>().Where(x => x.ParentId == self.ID).Select(x => x.ID).ToList();
            }
            else
            {
                ids = dc.Set<T>().Where(x => subids.Contains(x.ParentId.Value)).Select(x => x.ID).ToList();
            }
            if (ids != null && ids.Count > 0)
            {
                rv.AddRange(ids);
                rv.AddRange(self.GetAllChildrenIDs(dc, ids));
            }
            return rv;
        }

        /// <summary>
        /// �����νṹ�б�ת��Ϊ��׼�б�
        /// </summary>
        /// <typeparam name="T">���νṹ��</typeparam>
        /// <param name="self">���νṹʵ��</param>
        /// <param name="order">�����ֶΣ�����Ϊ��</param>
        /// <returns>���ر�׼�б����нڵ㶼��ͬһ����</returns>
        public static List<T> FlatTree<T>(this List<T> self, Func<T,object> order = null)
            where T : TopBasePoco, ITreeData<T>
        {
            List<T> rv = new List<T>();
            if(order != null)
            {
                self = self.OrderBy(order).ToList();
            }
            foreach (var item in self)
            {
                rv.Add(item);
                rv.AddRange(item.GetAllChildren(order));
            }
            return rv;
        }
    }
}