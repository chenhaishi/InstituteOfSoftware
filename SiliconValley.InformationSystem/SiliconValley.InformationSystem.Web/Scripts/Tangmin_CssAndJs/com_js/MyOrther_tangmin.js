//转换时间的方法
function TimeChange(newtime) {
    if (newtime == null)
        return "";
 
    var date = new Date(parseInt(newtime.slice(6)));
    var year = date.getFullYear();
    var month = parseInt(date.getMonth()) + 1;
    if (parseInt(month) < 10) {
        month = "0" + month;
    }
    var days = date.getDate();
    if (days < 10) {
        days = "0" + days;
    }
    var result = year + '-' + month + '-' + days;
    return result;
}
//判断星期几
function judgeDate(date) {
    // 标准时间 Wed Jul 31 2019 00:00:00 GMT+0800 (中国标准时间)
    var _date = new Date(date);
    // getDay() 返回表示星期的某一天
    var num = _date.getDay(_date);
    var week;
    switch (num) {
        case 0:
            week = "星期日";
            break;
        case 1:
            week = "星期一";
            break;
        case 2:
            week = "星期二"
            break;
        case 3:
            week = "星期三"
            break;
        case 4:
            week = "星期三"
            break;
        case 5:
            week = "星期五"
            break;
        case 6:
            week = "星期六"
            break;
        default:
            break;
    };
    return week;
}
//关闭弹出层
function MycloseEject(layer,index) {
    parent.layer.close(index); //再执行关闭   
}
 

//转换日期格式
function TimeFormt(newtime) {
    var date = new Date(newtime);
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();

    return year + "年" + month +"月"+ day +"日"
}

 