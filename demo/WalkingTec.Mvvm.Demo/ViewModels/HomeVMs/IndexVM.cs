using System.Collections.Generic;
using System.Linq;
using System.Web;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Demo.ViewModels.HomeVMs
{
    public class IndexVM : BaseVM
    {
        public List<FrameworkMenu> AllMenu { get; set; }

        public List<TreeSelectListItem> Menu
        {
            get
            {
                return GetUserMenu();
            }
        }

        public List<TreeSelectListItem> GetUserMenu()
        {
            if (ConfigInfo.IsQuickDebug == true)
            {
                return AllMenu.AsQueryable().GetTreeSelectListItems(null, null, x => x.PageName, null, x => x.IConId.ToString(), x => x.Url, SortByName: false); ;
            }
            else
            {
                var rv = AllMenu.Where(x =>x.ShowOnMenu == true).AsQueryable().GetTreeSelectListItems(null, null, x => x.PageName, null, x => x.IConId.ToString(), x => x.IsInside==true ? x.Url : "/_framework/outside?url="+x.Url, SortByName: false);
                RemoveUnAccessableMenu(rv, LoginUserInfo);
                RemoveEmptyMenu(rv);
                return rv;
            }
        }

        /// <summary>
        /// �Ƴ�û��Ȩ�޷��ʵĲ˵�
        /// </summary>
        /// <param name="menus">�˵��б�</param>
        /// <param name="info">�û���Ϣ</param>
        private void RemoveUnAccessableMenu(List<TreeSelectListItem> menus, LoginUserInfo info)
        {
            if (menus == null)
            {
                return;
            }

            List<TreeSelectListItem> toRemove = new List<TreeSelectListItem>();
            //���û��ָ���û���Ϣ�����õ�ǰ�û��ĵ�¼��Ϣ
            if (info == null)
            {
                info = LoginUserInfo;
            }
            //ѭ�����в˵���
            foreach (var menu in menus)
            {
                //�ж��Ƿ���Ȩ�ޣ����û�У�����ӵ���Ҫ�Ƴ����б���
                var url = menu.Url;
                if (!string.IsNullOrEmpty(url) && url.StartsWith("/_framework/outside?url="))
                {
                    url = url.Replace("/_framework/outside?url=", "");
                }
                if (!string.IsNullOrEmpty(url) && info.IsAccessable(HttpUtility.UrlDecode(url)) == false)
                {
                    toRemove.Add(menu);
                }
                //�����Ȩ�ޣ���ݹ���ñ���������Ӳ˵�
                else
                {
                    RemoveUnAccessableMenu(menu.Children, info);
                }
            }
            //ɾ��û��Ȩ�޷��ʵĲ˵�
            foreach (var remove in toRemove)
            {
                menus.Remove(remove);
            }
        }

        private void RemoveEmptyMenu(List<TreeSelectListItem> menus)
        {
            for(int i = 0; i < menus.Count; i++)
            {
                if ((menus[i].Children == null || menus[i].Children.Count == 0) && (menus[i].Url == null))
                {
                    menus.RemoveAt(i);
                    i--;
                }
            }

        }

    }
}
