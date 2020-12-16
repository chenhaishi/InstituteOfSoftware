//**************************************************专业相关操作的js文件****************************************************//

//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>函数区域>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


function DoAdd(courseFiled, successcallback, errorcallback) {

    console.log(courseFiled);
    Ajax("/CourseSyllabus/Course/DoOperation", courseFiled, "post", function (data) {

        successcallback(data);
    }, function (error) {
        errorcallback(error);
        });

}


//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<函数区域<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


layui.use(['table', 'layer','form'], function () {

    var table = layui.table;
    var layer = layui.layer;
    var form = layui.form;

    table.render({
        elem: '#courselist'
        , id: 'Courselist'
        , toolbar: '#topBar'
        , shade: 3
        , url: '/CourseSyllabus/Course/GetCourseData/'
        , cellMinWidth: 100 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
        , cols: [[
            { type: 'checkbox', fixed: 'left' }
            , { field: 'CourseName', title: '课程名称', sort: true }
            , { field: 'GrandName', title: '阶段', templet: '<div>{{d.Grand.GrandName}}</div>' }
            , {
                field: 'SpecialtyName', title: '专业', templet: function (res) {
                    
                    if (res.Major == null) {
                        return '无专业'
                    }
                    else {
                        return res.Major.SpecialtyName
                    }
                   
                    

                }
            }   
            , {
                field: 'IsDelete', title: '状态', templet: function (res) {

                    console.log(res.IsDelete);

                    if (res.IsDelete == false) {
                        return '正常';
                    }
                    else {
                        return '未激活';
                    }



                }
            }   
            , { field: 'TypeName', title: '课程类型', templet: '<div>{{d.CourseType.TypeName}}</div>' }   
            , { field: 'CourseCount', title: '课时', sort: true }
            , { field: 'right', title: '操作', toolbar: '#editBar', width: 250 }
           
        ]]
        , page: true
    });

    form.on('select(majorfilter)', function (data) {


        table.reload('Courselist', {

            url: '/CourseSyllabus/Course/SerachByMajor/'
            , where: { major: data.value} 
  
        });
      
    });  
    form.on('select(grandfilter)', function (data) {

        table.reload('Courselist', {

            url: '/CourseSyllabus/Course/SerachByGrand/'
            , where: { grand: data.value }

        });

    });

    $("#searchbtn").click(function () {

        var txt = $("#searchinput").val();

        table.reload('Courselist', {

            url: '/CourseSyllabus/Course/SerachByName'
            , where: { name: txt }

        });

    });

    //监听头部工具栏事件

    table.on('toolbar(courselist_filter)', function (obj) {

        var checkStatus = table.checkStatus(obj.config.id);

        switch (obj.event) {
            case 'Add':
                //新增课程区域

                layer.open({
                    type: 2,
                    area: ["900px", "520px"],
                    skin: "demo-class2",
                    shade: [0],
                    title: "新增课程",
                    content: '/CourseSyllabus/Course/OperationView/',
                    end: function () {
                        table.reload('Courselist', {

                        });
                    }

                });

                break;
            case 'TypeBtton':
                layer.open({
                    type: 2,
                    area: ["900px", "420px"],
                    skin: "demo-class2",
                    shade: [0.8],
                    title: "查询课程类型",
                    content: '/CourseSyllabus/Course/CourseTypeIndex'
                });
                break;

            case 'isUsing':

                console.log(checkStatus);
                if (checkStatus.data.count == 0) {

                    layer.msg("请选择课程");

                    return;
                }

                var loadindex = layer.load(1, { shade: false }); //0代表加载的风格，支持0-2

                var ids = "";

                for (var i = 0; i < checkStatus.data.length; i++) {
                    ids += checkStatus.data[i].CurriculumID + ",";
                }

                $.post("/CourseSyllabus/Course/UsingOrProhibitCourse", { courseIds: ids }, function (result) {

                    layer.close(loadindex);

                    if (result.ErrorCode == 200) {

                        layer.msg("成功!", { icon: 1 }, function () {

                            table.reload('Courselist', {

                            });

                        });
                    }
                    else {
                        layer.msg("失败!", { icon: 2}, function () {

                           

                        });
                    }

                });

                break;


        };
    });


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>新增课程区域>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    //监听表单提交
    form.on('submit(operation)', function (data) {
       // console.log(data.elem) //被执行事件的元素DOM对象，一般为button对象
       // console.log(data.form) //被执行提交的form对象，一般在存在form标签时才会返回
       // console.log(data.field) //当前容器的全部表单字段，名值对形式：{name: value}

        if (data.field.MajorID.length == 0) {
            data.field.MajorID = 0;
        }

        if (data.field.IsEndCurr == "0") {
            data.field.IsEndCurr = false;
        }
        else {
            data.field.IsEndCurr = true;
        }

        console.log(data.field);
        //调用添加方法
        DoAdd(data.field, function (data) {

           

            if (data.ErrorCode == 200) {

                layer.msg("操作成功", { time: 1000 }, function () {

                    //当你在iframe页面关闭自身时
                    var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
                    parent.layer.close(index); //再执行关闭
                });

                table.reload('Courselist', {

                });

            }

        }, function (error) {

            layer.msg("请检查网络！......");

            });


        return false; //阻止表单跳转。如果需要表单跳转，去掉这段即可。
    });


//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<新增课程区域<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>监听行工具区域>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


    //监听行工具事件
    table.on('tool(courselist_filter)', function (obj) {
        var data = obj.data;
        var id = obj.data.CurriculumID;
        var CourseName = obj.data.CourseName;
        
        if (obj.event === 'edit') {
            layer.open({
                title: "编辑课程 (" + CourseName + ")",
                skin: "demo-class",
                type: 2,
                area: ["900px", "500px"],
                content: '/CourseSyllabus/Course/OperationView/' + id,
                end: function () {
                    table.reload('Courselist', {

                    });


                }

            });
        } else if (obj.event === 'detail') {

            layer.open({
                title: "详细课程 (" + CourseName + ")",
                skin: "demo-class",
                type: 2,
                area: ["900px", "420px"],
                content: '/CourseSyllabus/Course/DetailView/' + id,

            });


        }

    });


//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<监听行工具区域<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


    $(".detailclose").click(function () {
        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        parent.layer.close(index); //再执行关闭   
    });

    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<自定义验证<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    //自定义验证规则
    form.verify({
        valueMax: function (value) {
            if (value < 0 ) {
                return '值不能为负数！！！';
            }
        }        
    });


});

function Close_My() {
    layui.use('layer', function () {
        var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        parent.layer.close(index); //再执行关闭 
    });
      
}