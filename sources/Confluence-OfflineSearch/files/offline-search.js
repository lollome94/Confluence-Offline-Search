var index = elasticlunr(function () {
	this.addField('Title');
	this.addField('Body');
	this.addField('Page');
	this.setRef('ID');

});

$(function() {
	init_gui();
	//documentsFiller();
});


function init_gui()
{
	var html = "<div id='of-wrapper'>";
	
	html+='<input type="text" id="of-filter"> ';
	html+='<button onclick="btnSearch_Click();">Search</button>';
	html +="</div>";

	$("body").append(html);
}


function btnSearch_Click()
{
	var result = index.search($("#of-filter").val());
	console.log(result);
}
