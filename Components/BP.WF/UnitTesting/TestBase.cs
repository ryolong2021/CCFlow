using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web.Controls;
using BP.Web;

namespace BP.UnitTesting
{
    public enum EditState
    {
        /// <summary>
        /// ÒÑ¾­ÍEÉ
        /// </summary>
        Passed,
        /// <summary>
        /// ±à¼­ÖĞ
        /// </summary>
        Editing,
        /// <summary>
        /// Î´ÍEÉ
        /// </summary>
        UnOK
    }
	/// <summary>
	/// ²âÊÔ»ùÀE
	/// </summary>
    abstract public class TestBase
    {
        public EditState EditState = EditState.Editing;
        /// <summary>
        /// Ö´ĞĞ²½ÖèĞÅÏ¢
        /// </summary>
        public int TestStep = 0;
        public string Note = "";
        /// <summary>
        /// Ôö¼Ó²âÊÔÄÚÈİ.
        /// </summary>
        /// <param name="note">²âÊÔÄÚÈİµÄÏE¸ÃèÊE</param>
        public void AddNote(string note)
        {
            TestStep++;
            if (Note == "")
            {
                Note += "\t\n ½øĞĞ:" + TestStep + "ÏûÎâÊÔ";
                Note += "\t\n" + note;
            }
            else
            {
                Note += "\t\n²âÊÔÍ¨¹ı.";
                Note += "\t\n ½øĞĞ:" + TestStep + "ÏûÎâÊÔ";
                Note += "\t\n" + note;
            }
        }
        public string sql = "";
        public DataTable dt = null;
        /// <summary>
        /// ÈÃ×ÓÀàÖØĞ´
        /// </summary>
        public virtual void Do()
        {
        }

        #region »ù±¾ÊôĞÔ.
        /// <summary>
        /// ±EE
        /// </summary>
        public string Title = "Î´ÃEûµÄµ¥Ôª²âÊÔ";
        public string DescIt = "ÃèÊE";
        /// <summary>
        /// ´úêóĞÅÏ¢
        /// </summary>
        public string ErrInfo = "";
        #endregion
        /// <summary>
        /// ²âÊÔ»ùÀE
        /// </summary>
        public TestBase() { }
    }

}
