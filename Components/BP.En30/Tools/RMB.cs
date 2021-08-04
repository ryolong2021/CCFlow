
using System;

namespace BP.Tools
{ 
	/// 
	/// 功能：字符串处理函数集 
	/// 
	public class DealString
	{ 
		#region 私有成员 
		/// 
		/// 输入字符串 
		/// 
		private string inputString=null; 
		/// 
		/// 输出字符串 
		/// 
		private string outString=null; 
		/// 
		/// 提示信息 
		/// 
		private string noteMessage=null; 
		#endregion 

		#region 公共属性 
		/// 
		/// 输入字符串 
		/// 
		public string InputString 
		{ 
			get{return inputString;} 
			set{inputString=value;} 
		} 
		/// 
		/// 输出字符串 
		/// 
		public string OutString 
		{ 
			get{return outString;} 
			set{outString=value;} 
		} 
		/// 
		/// 提示信息 
		/// 
		public string NoteMessage 
		{ 
			get{return noteMessage;} 
			set{noteMessage=value;} 
		} 
		#endregion 

		#region 构造函数 
        public DealString() 
		{ 
			// 
			// TODO: 在此处添加构造函数逻辑 
			// 
		} 
		#endregion 

		#region 公共方法 
		public void ConvertToChineseNum() 
		{ 
			string numList="ゼロワンツースリーフォーウールーチーバジウ"; 
			string rmbList = "セント1000万1000万1000万1000万"; 
			double number=0; 
			string tempOutString=null; 

			try 
			{ 
				number=double.Parse(this.inputString); 
			} 
			catch 
			{ 
				this.noteMessage="入力パラメータは数値ではありません"; 
				return; 
			} 

			if(number>9999999999999.99) 
				this.noteMessage="RMB値が範囲外です"; 

			//将小数转化为整数字符串 
			string tempNumberString=Convert.ToInt64(number*100).ToString(); 
			int tempNmberLength=tempNumberString.Length; 
			int i=0; 
			while( i<tempNmberLength ) 
			{ 
				int oneNumber=Int32.Parse(tempNumberString.Substring(i,1)); 
				string oneNumberChar=numList.Substring(oneNumber,1); 
				string oneNumberUnit=rmbList.Substring(tempNmberLength-i-1,1); 
				if(oneNumberChar!="ゼロ") 
					tempOutString+=oneNumberChar+oneNumberUnit; 
				else 
				{ 
					if(oneNumberUnit=="億"||oneNumberUnit=="万"||oneNumberUnit=="元"||oneNumberUnit=="ゼロ") 
					{ 
						while (tempOutString.EndsWith("ゼロ")) 
						{ 
							tempOutString=tempOutString.Substring(0,tempOutString.Length-1); 
						} 

					}
                    if (oneNumberUnit == "億" || (oneNumberUnit == "万" && !tempOutString.EndsWith("億")) || oneNumberUnit == "元")
                    {
                        tempOutString += oneNumberUnit;
                    }
                    else
                    {
                        bool tempEnd = tempOutString.EndsWith("億");
                        bool zeroEnd = tempOutString.EndsWith("ゼロ");
                        if (tempOutString.Length > 1)
                        {
                            bool zeroStart = tempOutString.Substring(tempOutString.Length - 2, 2).StartsWith("ゼロ");
                            if (!zeroEnd && (zeroStart || !tempEnd))
                                tempOutString += oneNumberChar;
                        }
                        else
                        {
                            if (!zeroEnd && !tempEnd)
                                tempOutString += oneNumberChar;
                        }
                    } 
				} 
				i+=1; 
			} 

			while (tempOutString.EndsWith("ゼロ")) 
			{ 
				tempOutString=tempOutString.Substring(0,tempOutString.Length-1); 
			} 

			while(tempOutString.EndsWith("元")) 
			{ 
				tempOutString=tempOutString+"整える"; 
			} 

			this.outString=tempOutString; 


		} 
		#endregion 
	} 
} 
