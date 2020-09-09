var clients = [];
$(function () {
    clients = $.clientsInit();
})
$.clientsInit = function () {
    var dataJson = {
        UseMenuData: [],
        UseBtnData: [],
    };
    var init = function () {
        $.ajax({
            url: "/Main/GetMenuData",
            type: "get",
            dataType: "json",
            async: false,
            success: function (data) {
                dataJson.UseMenuData = eval(data.UseMenuDatas);
                dataJson.UseBtnData = data.UseBtnDatas;
            }
        });
    }
    init();
    return dataJson;
}