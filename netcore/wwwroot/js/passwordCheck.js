function Check(password) {
    var one = "^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[._~!@#$^&*])[A-Za-z0-9._~!@#$^&*]{8,20}$";
    if (password.match(one)) {
        return true;
    } else {
        return false;
    }
}

function NeedChange(layer,need,url) {
    if (need == 'True') {
        layer.msg("密码不符合要求或重置密码后需要更改密码，请修改密码！", { icon: 5 }, function () {
            window.top.location.href =url;
        });
    }
}