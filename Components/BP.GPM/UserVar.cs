using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.GPM
{
    /// <summary>
    /// �û�����
    /// </summary>
    public class UserVarAttr : EntityNoNameAttr
    {
        /// <summary>
        /// Val
        /// </summary>
        public const string Val = "Val";
        /// <summary>
        /// Note
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// GroupKey
        /// </summary>
        public const string GroupKey = "GroupKey";
    }
    /// <summary>
    /// �û�����
    /// </summary>
    public class UserVar : EntityNoName
    {
        #region ����
        public object ValOfObject
        {
            get
            {
                return this.GetValByKey(UserVarAttr.Val);
            }
            set
            {
                this.SetValByKey(UserVarAttr.Val, value);
            }
        }
        public string Val
        {
            get
            {
                return this.GetValStringByKey(UserVarAttr.Val);
            }
            set
            {
                this.SetValByKey(UserVarAttr.Val, value);
            }
        }
        public float ValOfFloat
        {
            get
            {
                return this.GetValFloatByKey(UserVarAttr.Val);
            }
            set
            {
                this.SetValByKey(UserVarAttr.Val, value);
            }
        }
        public int ValOfInt
        {
            get
            {
                return this.GetValIntByKey(UserVarAttr.Val);
            }
            set
            {
                this.SetValByKey(UserVarAttr.Val, value);
            }
        }
        public decimal ValOfDecimal
        {
            get
            {
                return this.GetValDecimalByKey(UserVarAttr.Val);
            }
            set
            {
                this.SetValByKey(UserVarAttr.Val, value);
            }
        }
        public bool ValOfBoolen
        {
            get
            {
                return this.GetValBooleanByKey(UserVarAttr.Val);
            }
            set
            {
                this.SetValByKey(UserVarAttr.Val, value);
            }
        }	
        /// <summary>
        /// note
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStringByKey(UserVarAttr.Note);
            }
            set
            {
                this.SetValByKey(UserVarAttr.Note, value);
            }
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// �û�����
        /// </summary>
        public UserVar()
        {
        }
        /// <summary>
        /// �û�����
        /// </summary>
        /// <param name="mypk"></param>
        public UserVar(string no)
        {
            this.No = no;
            this.Retrieve();
        }
         /// <summary>
		/// ��ֵ
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="isNullAsVal"></param> 
        public UserVar(string key, object isNullAsVal)
		{
			try
			{
				this.No=key;
				this.Retrieve(); 
			}
			catch
			{				
				if (this.RetrieveFromDBSources()==0)
				{
					this.Val = isNullAsVal.ToString();
					this.Insert();
				}
			}
		}
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_UserVar");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "�û�����";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(UserVarAttr.No, null, "��", true, false, 1, 30, 20);
                map.AddTBString(UserVarAttr.Name, null, "����", true, false, 0, 120, 20);
                map.AddTBString(UserVarAttr.Val, null, "ֵ", true, false, 0, 120, 20,true);
                map.AddTBString(UserVarAttr.GroupKey, null, "����ֵ", true, false, 0, 120, 20, true);
                map.AddTBStringDoc(UserVarAttr.Note, null, "˵��", true, false,true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// �û�����s
    /// </summary>
    public class UserVars : EntitiesNoName
    {
        #region get value by key
        /// <summary>
        /// ���������ļ�
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="val">val</param>
        public static int SetValByKey(string key, object val)
        {
            UserVar en = new UserVar(key, val);
            en.ValOfObject = val;
            return en.Update();
        }
        /// <summary>
        /// ��ȡhtml����
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValByKeyHtml(string key)
        {
            return DataType.ParseText2Html(UserVars.GetValByKey(key));
        }
        public static string GetValByKey(string key)
        {
            foreach (UserVar cfg in UserVars.MyUserVars)
            {
                if (cfg.No == key)
                    return cfg.Val;
            }

            throw new Exception("error key=" + key);
        }
        /// <summary>
        /// �õ���һ��key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValByKey(string key, string isNullAs)
        {
            foreach (UserVar cfg in UserVars.MyUserVars)
            {
                if (cfg.No == key)
                    return cfg.Val;
            }

            UserVar en = new UserVar(key, isNullAs);
            //UserVar en = new UserVar(key);
            return en.Val;
        }
        public static int GetValByKeyInt(string key, int isNullAs)
        {
            foreach (UserVar cfg in UserVars.MyUserVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfInt;
            }

            UserVar en = new UserVar(key, isNullAs);
            //UserVar en = new UserVar(key);
            return en.ValOfInt;
        }
        public static int GetValByKeyDecimal(string key, int isNullAs)
        {
            foreach (UserVar cfg in UserVars.MyUserVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfInt;
            }

            UserVar en = new UserVar(key, isNullAs);
            //UserVar en = new UserVar(key);
            return en.ValOfInt;
        }
        public static bool GetValByKeyBoolen(string key, bool isNullAs)
        {

            foreach (UserVar cfg in UserVars.MyUserVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfBoolen;
            }


            int val = 0;
            if (isNullAs)
                val = 1;

            UserVar en = new UserVar(key, val);

            return en.ValOfBoolen;
        }
        public static float GetValByKeyFloat(string key, float isNullAs)
        {
            foreach (UserVar cfg in UserVars.MyUserVars)
            {
                if (cfg.No == key)
                    return cfg.ValOfFloat;
            }

            UserVar en = new UserVar(key, isNullAs);
            return en.ValOfFloat;
        }
        private static UserVars _MyUserVars = null;
        public static UserVars MyUserVars
        {
            get
            {
                if (_MyUserVars == null)
                {
                    _MyUserVars = new UserVars();
                    _MyUserVars.RetrieveAll();
                }
                return _MyUserVars;
            }
        }
        public static void ReSetVal()
        {
            _MyUserVars = null;
        }
        #endregion

        #region ����
        /// <summary>
        /// �û�����s
        /// </summary>
        public UserVars()
        {
        }
        /// <summary>
        /// �û�����s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public UserVars(string fk_mapdata)
        {
            
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// �õ����� Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new UserVar();
            }
        }
        #endregion

        #region Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
        /// <summary>
        /// ת���� java list,C#���ܵ���.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<UserVar> ToJavaList()
        {
            return (System.Collections.Generic.IList<UserVar>)this;
        }
        /// <summary>
        /// ת����list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<UserVar> Tolist()
        {
            System.Collections.Generic.List<UserVar> list = new System.Collections.Generic.List<UserVar>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((UserVar)this[i]);
            }
            return list;
        }
        #endregion Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
    }
}