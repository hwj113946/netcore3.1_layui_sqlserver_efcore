$(function ($) {
    GetLoadNav();
});

function UseMenuTab(_html, n) {
    _html += '<dl class="layui-nav-child">';
    $.each(n, function (i) {
        var subrow = n[i];
        _html += '<dd  data-name="' + subrow.MenuName + '">';

        var childNodes = subrow.ChildNodes;
        if (childNodes.length > 0) {
            _html += '<a href="javascript:;" lay-tips="' + subrow.MenuName + '"><i class="layui-icon ' + subrow.MenuIcon + '"></i>' + subrow.MenuName + '</a>';

            _html = UseMenuTab(_html, childNodes);
        }
        else {
            _html += '<a  data-id="' + subrow.MenuId + '" lay-tips="' + subrow.MenuName + '" lay-href="' + subrow.MenuUrl + '" ><i class="layui-icon ' + subrow.MenuIcon + '" style="font-size: 16px;"></i>' + subrow.MenuName + '</a>';
        }
        _html += '</dd>';
    })
    _html += '</dl>';
    return _html;
};


function GetLoadNav() {
    var MenuData;
    var BtnData
    $.ajax({
        url: "/Main/GetMenuData",
        type: "get",
        dataType: "json",
        async: false,
        success: function (data) {
            MenuData = eval(data.UseMenuDatas);
        }
    });
    var _html = "";
    $.each(MenuData, function (i) {
        var row = MenuData[i];
        if (row.ParentMenuId == "0") {
            _html += '<li data-name="' + row.MenuName + '" class="layui-nav-item">';
            _html += '<a  href="javascript:;" lay-tips="' + row.MenuName + '" lay-direction="2"><i class="layui-icon ' + row.MenuIcon + '"></i><cite>' + row.MenuName + '</cite></a>';
            var childNodes = row.ChildNodes;
            if (childNodes.length > 0) {
                _html = UseMenuTab(_html, childNodes);
            }

            _html += '</li>';
        }
    });
    $("#LAY-system-side-menu").prepend(_html);
};