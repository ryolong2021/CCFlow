if ($.fn.pagination){
	$.fn.pagination.defaults.beforePageText = '第';
	$.fn.pagination.defaults.afterPageText = '共有ページ';
	$.fn.pagination.defaults.displayMsg = '{from}から{to}までを表示し、合計{total}記録を表示します。';
}
if ($.fn.datagrid){
	$.fn.datagrid.defaults.loadMsg = '処理中ですので、少々お待ちください。';
}
if ($.fn.treegrid && $.fn.datagrid){
	$.fn.treegrid.defaults.loadMsg = $.fn.datagrid.defaults.loadMsg;
}
if ($.messager){
	$.messager.defaults.ok = 'を選択します';
	$.messager.defaults.cancel = 'キャンセル';
}
if ($.fn.validatebox){
	$.fn.validatebox.defaults.missingMessage = 'この入力項目は必須項目です。';
	$.fn.validatebox.defaults.rules.email.message = '有効なメールアドレスを入力してください。';
	$.fn.validatebox.defaults.rules.url.message = '有効なURLアドレスを入力してください。';
	$.fn.validatebox.defaults.rules.length.message = '输入内容长度必须介于{0}和{1}之间';
	$.fn.validatebox.defaults.rules.remote.message = 'このフィールドを修正してください';
}
if ($.fn.numberbox){
	$.fn.numberbox.defaults.missingMessage = 'この入力項目は必須項目です。';
}
if ($.fn.combobox){
	$.fn.combobox.defaults.missingMessage = 'この入力項目は必須項目です。';
}
if ($.fn.combotree){
	$.fn.combotree.defaults.missingMessage = 'この入力項目は必須項目です。';
}
if ($.fn.combogrid){
	$.fn.combogrid.defaults.missingMessage = 'この入力項目は必須項目です。';
}
if ($.fn.calendar){
	$.fn.calendar.defaults.weeks = ['日','月','火','水','木','金','土'];
	$.fn.calendar.defaults.months = ['1月','2月','3月','4月','5月','6月','7月','8月','9月','10月','11月','12月'];
}
if ($.fn.datebox){
	$.fn.datebox.defaults.currentText = '今日';
	$.fn.datebox.defaults.closeText = '閉じる';
	$.fn.datebox.defaults.okText = 'を選択します';
	$.fn.datebox.defaults.missingMessage = 'この入力項目は必須項目です。';
	$.fn.datebox.defaults.formatter = function(date){
		var y = date.getFullYear();
		var m = date.getMonth()+1;
		var d = date.getDate();
		return y+'-'+(m<10?('0'+m):m)+'-'+(d<10?('0'+d):d);
	};
	$.fn.datebox.defaults.parser = function(s){
		if (!s) return new Date();
		var ss = s.split('-');
		var y = parseInt(ss[0],10);
		var m = parseInt(ss[1],10);
		var d = parseInt(ss[2],10);
		if (!isNaN(y) && !isNaN(m) && !isNaN(d)){
			return new Date(y,m-1,d);
		} else {
			return new Date();
		}
	};
}
if ($.fn.datetimebox && $.fn.datebox){
	$.extend($.fn.datetimebox.defaults,{
		currentText: $.fn.datebox.defaults.currentText,
		closeText: $.fn.datebox.defaults.closeText,
		okText: $.fn.datebox.defaults.okText,
		missingMessage: $.fn.datebox.defaults.missingMessage
	});
}
if ($.fn.datetimespinner){
	$.fn.datetimespinner.defaults.selections = [[0,4],[5,7],[8,10],[11,13],[14,16],[17,19]]
}