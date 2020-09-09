//  菜单显示异常修改tinymce/skins/ui/oxide/skin.min.css:96 .tox-silver-sink的z-index值
//  http://tinymce.ax-z.cn/   中文文档

layui.define(['jquery'],function (exports) {
    var $ = layui.$

    var modFile = layui.cache.modules['tinymce'];

    var modPath = modFile.substr(0, modFile.lastIndexOf('.'))

    var setter = layui.setter || {}//兼容layuiadmin

    var response = setter.response || {}//兼容layuiadmin

    //  ----------------  以上代码无需修改  ----------------

    var settings = {
        base_url: modPath
        , images_upload_url: '/News/ImageFileUpload'//图片上传接口，可在option传入，也可在这里修改，option的值优先
        , images_upload_base_path:'/News'
        , language: 'zh_CN'//语言，可在option传入，也可在这里修改，option的值优先
        , response: {//后台返回数据格式设置
            statusName: response.statusName || 'code'//返回状态字段
            , msgName: response.msgName || 'msg'//返回消息字段
            , dataName: response.dataName || 'data'//返回的数据
            , statusCode: response.statusCode || {
                ok: 0//数据正常
            }
        }
        , success: function (res, succFun, failFun) {//图片上传完成回调 根据自己需要修改
            if (res.code == 200) {
                succFun(res.location);
            } else {
                failFun(res[this.response.msgName]);
            }
            //if (res[this.response.statusName] == this.response.statusCode.ok) {
            //    succFun(res[this.response.dataName]);
            //} else {
            //    failFun(res[this.response.msgName]);
            //}
        }
    };

    //  ----------------  以下代码无需修改  ----------------

    var t = {};

    //初始化
    t.render = function (option,callback) {

        var admin = layui.admin || {}

        option.base_url = isset(option.base_url) ? option.base_url : settings.base_url

        option.language = isset(option.language) ? option.language : settings.language

        option.selector = isset(option.selector) ? option.selector : option.elem

        option.quickbars_selection_toolbar = isset(option.quickbars_selection_toolbar) ? option.quickbars_selection_toolbar : 'cut copy | bold italic underline strikethrough '

        option.plugins = isset(option.plugins) ? option.plugins : 'quickbars print preview searchreplace autolink fullscreen image link media codesample table charmap hr advlist lists wordcount imagetools indent2em';

        option.toolbar = isset(option.toolbar) ? option.toolbar : 'undo redo | forecolor backcolor bold italic underline strikethrough | indent2em alignleft aligncenter alignright alignjustify outdent indent | link bullist numlist image table codesample | formatselect fontselect fontsizeselect';

        option.resize = isset(option.resize) ? option.resize : false;

        option.elementpath = isset(option.elementpath) ? option.elementpath : false;

        option.branding = isset(option.branding) ? option.branding : false;

        option.contextmenu_never_use_native = isset(option.contextmenu_never_use_native) ? option.contextmenu_never_use_native : true;

        option.menubar = isset(option.menubar) ? option.menubar : 'file edit insert format table';

        option.images_upload_url = isset(option.images_upload_url) ? option.images_upload_url : settings.images_upload_url;

        option.images_upload_handler = isset(option.images_upload_handler) ? option.images_upload_handler : function (blobInfo, succFun, failFun) {

            var formData = new FormData();

            formData.append('target', 'edit');

            formData.append('edit', blobInfo.blob());

            var ajaxOpt = {

                url: option.images_upload_url,

                dataType: 'json',

                type: 'POST',

                data: formData,

                processData: false,

                contentType: false,

                success: function (res) {

                    settings.success(res, succFun, failFun)

                },
                error: function (res) {

                    failFun("网络错误：" + res.status);

                }
            };

            if (typeof admin.req == 'function') {

                admin.req(ajaxOpt);

            } else {

                $.ajax(ajaxOpt);

            }
        }, option.file_picker_callback= function (callback, value, meta) {
            //文件分类
            var filetype = '.pdf, .txt, .zip, .rar, .7z, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .mp3, .mp4';
            //后端接收上传文件的地址
            var upurl = '/News/FileUpload';
            //为不同插件指定文件类型及后端地址
            switch (meta.filetype) {
                case 'image':
                    filetype = '.jpg, .jpeg, .png, .gif';
                    //upurl = 'upimg.php';
                    break;
                case 'media':
                    filetype = '.mp3, .mp4';
                    //upurl = 'upfile.php';
                    break;
                case 'file':
                default:
            }
            //模拟出一个input用于添加本地文件
            var input = document.createElement('input');
            input.setAttribute('type', 'file');
            input.setAttribute('accept', filetype);
            input.click();
            input.onchange = function () {
                var file = this.files[0];

                var xhr, formData;
                //console.log(file.name);
                xhr = new XMLHttpRequest();
                xhr.withCredentials = false;
                xhr.open('POST', upurl);
                xhr.onload = function () {
                    var json;
                    if (xhr.status != 200) {
                        failure('网络错误: ' + xhr.status);
                        return;
                    }
                    json = JSON.parse(xhr.responseText);
                    if (!json || typeof json.location != 'string') {
                        failure('无效的JSON: ' + xhr.responseText);
                        return;
                    }
                    callback(json.location);
                };
                formData = new FormData();
                formData.append('file', file, file.name);
                xhr.send(formData);
            };
        }

        option.menu = isset(option.menu) ? option.menu : {
            file: {title: '文件', items: 'newdocument | print preview fullscreen | wordcount'},
            edit: {title: '编辑', items: 'undo redo | cut copy paste pastetext selectall | searchreplace'},
            format: {
                title: '格式',
                items: 'bold italic underline strikethrough superscript subscript | formats | forecolor backcolor | removeformat'
            },
            table: {title: '表格', items: 'inserttable tableprops deletetable | cell row column'},
        };
        if(typeof tinymce == 'undefined'){

            $.ajax({//获取插件
                url: option.base_url + '/tinymce.js',

                dataType: 'script',

                cache: true,

                async: false,
            });

        }

        layui.sessionData('layui-tinymce',{

            key:option.selector,

            value:option

        })

        tinymce.init(option);

        if(typeof callback == 'function'){

            callback.call(option)

        }

        return tinymce.activeEditor;
    };

    t.init = t.render

    // 获取ID对应的编辑器对象
    t.get = function (elem) {

        if(elem && /^#|\./.test(elem)){

            var id = elem.substr(1)

            var edit = tinymce.editors[id];

            if(!edit){

                return console.error("编辑器未加载")

            }

            return edit

        } else {

            return console.error("elem错误")

        }
    }

    //重载
    t.reload = function (option,callback) {
        option = option || {}

        var edit = t.get(option.elem);

        var optionCache = layui.sessionData('layui-tinymce')[option.elem]

        edit.destroy()

        $.extend(optionCache,option)

        tinymce.init(optionCache)

        if(typeof callback == 'function'){

            callback.call(optionCache)

        }

        return tinymce.activeEditor;
    }

    function isset(value){
        return typeof value != 'undefined' && value != null
    }

    exports('tinymce', t);
});
